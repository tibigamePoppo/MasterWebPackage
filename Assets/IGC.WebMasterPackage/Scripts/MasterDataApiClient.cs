using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace IGC.WebMasterPackage
{
    public enum MasterType
    {
        Web,
        Resource,
        Text,
    }
    public class MasterDataApiClient : MonoBehaviour
    {
        private const string configSheetName = "SheetConfig"; 
        [SerializeField, Tooltip("�X�v���b�h�V�[�g��ID�����")] private string spreadsheetId = "18DLe0x6q_o3C1m-bi7LxTCk06qeekzHl7xofULShkNo";  // �X�v���b�h�V�[�g��ID
        [SerializeField, Tooltip("�摜��ǂݍ���")] private bool _isImageResourceRequest = false;
        [SerializeField, Tooltip("�}�X�^�[�f�[�^�̃^�C�v")] private MasterType _masterType = MasterType.Web;
        [SerializeField, Tooltip("MasterData"),TextArea] private string _masterDataString;
        private const string ImageResourceUrlSheet = "-CharacterImage";//�摜URL���L�ڂ���Ă���p�����[�^�V�[�g��
        private List<SheetData> _sheetDatas;
        private List<bool> _waiting;
        private const string ResourcePath = "Resources/IGC.WebMasterPackage/";

        public static MasterData masterData;
        public static List<Sprite> sprites;
        public static bool loadIsEnd = false;

        void Start()
        {
            Init();
            if(_masterType == MasterType.Web)
            {
                StartCoroutine(GetSpreadsheetMetadata());
            }
            else if (_masterType == MasterType.Resource)
            {
                SaveAndLoadJson saveAndLoadJson = new SaveAndLoadJson();
                string json = saveAndLoadJson.LoadJson(Path.Combine(Application.dataPath, $"{ResourcePath}", $"MasterDataJson.json"));
                masterData = JsonUtility.FromJson<MasterData>(json);
                GetImageResourcesNotUseWeb();
                loadIsEnd = true;
            }
            else
            {
                masterData = JsonUtility.FromJson<MasterData>(_masterDataString);
                GetImageResourcesNotUseWeb();
                loadIsEnd = true;
            }
        }

        private void Init()
        {
            _sheetDatas = new List<SheetData>();
            _waiting = new List<bool>();
            masterData = null;
            sprites = new List<Sprite>();
        }

        /// <summary>
        /// �X�v���b�h�V�[�g�̃��^�f�[�^���擾����
        /// </summary>
        IEnumerator GetSpreadsheetMetadata()
        {
            Debug.Log("MasterDataApiClient.LoadStart!");

            string url = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/gviz/tq?tqx=out:csv&sheet={configSheetName}";

            _waiting.Add(false);
            // HTTP���N�G�X�g�𑗂�
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var sheetConfig = ConvertToArrayListFrom(request.downloadHandler.text);
                    var sheetNames = sheetConfig.Select(v => v[0]).ToArray();

                    // �V�[�g���̃��X�g���擾
                    foreach (var sheet in sheetNames)
                    {
                        if (sheet.StartsWith("#")) continue; // config�͓ǂݗ��Ȃ�
                        _waiting.Add(false);
                        // �e�V�[�g�̃f�[�^���擾
                        StartCoroutine(GetSheetData(sheet));
                    }
                }
                else
                {
                    Debug.LogError("Request failed: " + request.error);
                }
            }

            _waiting.RemoveAt(0);
            yield return new WaitUntil(() => _waiting.Count == 0);
            GenerateCSharpClasses generateCSharpClasses = new GenerateCSharpClasses();
            generateCSharpClasses.GenerateCSharpClasse(_sheetDatas);
            SheetToJson sheetToJson = new SheetToJson();
            var json = sheetToJson.Main(_sheetDatas);
            masterData = JsonUtility.FromJson<MasterData>(json);
            SaveAndLoadJson saveAndLoadJson = new SaveAndLoadJson();
            saveAndLoadJson.SaveJson(json, Application.dataPath);
            if (_isImageResourceRequest)
            {
                _waiting.Add(false);
                StartCoroutine(ImageResourcesRequest(_sheetDatas.Where(d => d.SheetName == ImageResourceUrlSheet).First()));
            }
            yield return new WaitUntil(() => _waiting.Count == 0);
            loadIsEnd = true;
            Debug.Log("MasterDataApiClient.LoadFinish!");
        }

        /// <summary>
        /// �e�V�[�g�̃f�[�^���擾����
        /// </summary>
        IEnumerator GetSheetData(string sheetName)
        {
            string url = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/gviz/tq?tqx=out:csv&sheet={sheetName}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var convertData = ConvertToArrayListFrom(request.downloadHandler.text);
                    convertData = sheetName.StartsWith("-") ? convertData : Transpose(convertData); // �p�����[�^�f�[�^�ł͂Ȃ��ꍇ�͓]�u���s��
                    SheetData data = new SheetData();
                    data.SheetName = sheetName;
                    data.Keys = convertData.Select(v => v[0]).ToList();
                    data.Types = convertData.Select(v => v[1]).ToList();
                    data.Data = convertData.ToList();
                    _sheetDatas.Add(data);
                }
                else
                {
                    Debug.LogError($"Request failed for sheet {sheetName}: " + request.error);
                }
                _waiting.RemoveAt(0);
            }
        }

        private void GetImageResourcesNotUseWeb()
        {
            Type type = typeof(MasterData);
            FieldInfo[] fields = type.GetFields();
            FieldInfo imageSource = fields.Where(f => f.Name == "CharacterImage").First();

            Type characterImageType = imageSource.GetValue(masterData).GetType();
            FieldInfo[] sampleField = characterImageType.GetFields();
            var imageNames = sampleField.Select(f => f.Name).ToArray();
            if (imageNames.Length == 0) return;

            foreach (var name in imageNames)
            {
                string filePath = Path.Combine(Application.dataPath, $"{ResourcePath}", $"{name}.png");
                if (File.Exists(filePath))
                {
                    SetSpriteData(filePath, name, imageSource);
                    continue;
                }
            }
        }

        private IEnumerator ImageResourcesRequest(SheetData sheetData)
        {
            string assetsPath = Application.dataPath;
            string[] resourceUrls = sheetData.Data.Skip(1).Select(d2 => d2[2]).ToArray();


            Type type = typeof(MasterData);
            FieldInfo[] fields = type.GetFields();
            FieldInfo imageSource = fields.Where(f => f.Name == ImageResourceUrlSheet.Substring(1)).First();
            if (imageSource == null) yield break;


            foreach (string resourceUrl in resourceUrls)
            {
                _waiting.Add(false);
                string spriteName = sheetData.Data.Skip(1).Where(d2 => d2[2] == resourceUrl).Select(d => d[0]).First();
                string filePath = Path.Combine(assetsPath, $"{ResourcePath}", $"{spriteName}.png");

                if (File.Exists(filePath))
                {
                    SetSpriteData(filePath, spriteName, imageSource);
                    _waiting.RemoveAt(0);
                    continue;
                }

                UnityWebRequest request = UnityWebRequestTexture.GetTexture(resourceUrl);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        File.WriteAllBytes(filePath, request.downloadHandler.data);
                        SetSpriteData(filePath, spriteName, imageSource);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                }
                else
                {
                    Debug.Log(request.error);
                }
                _waiting.RemoveAt(0);
            }
            _waiting.RemoveAt(0);
        }

        private void SetSpriteData(string filePath, string spriteName, FieldInfo imageSource)
        {
            Sprite sprite = LoadSprite(filePath);
            sprite.name = spriteName;
            sprites.Add(sprite);

            object characterImageObject = imageSource.GetValue(masterData);

            if (characterImageObject == null)
            {
                Debug.LogError("CharacterImage object is null.");
                return;
            }

            Type characterImageType = imageSource.GetValue(masterData).GetType();
            FieldInfo sampleField = characterImageType.GetField(spriteName);

            if (sampleField == null)
            {
                Debug.LogError($"Field '{spriteName}' not found in CharacterImage.");
                return;
            }

            sampleField.SetValue(characterImageObject, sprite);
            Debug.Log($"Load data {spriteName}");
        }

        /// <summary>
        /// �ǂݍ���csv�f�[�^�������₷���`�ɕϊ�
        /// </summary>
        List<List<string>> ConvertToArrayListFrom(string _text)
        {
            List<List<string>> characterDataArrayList = new List<List<string>>();
            StringReader reader = new StringReader(_text);
            reader.ReadLine();  // 1�s�ڂ̓��x���Ȃ̂ŊO��
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();        // ��s���ǂݍ���
                string[] elements = line.Split(',');    // �s�̃Z����,�ŋ�؂���
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i] == "\"\"")
                    {
                        continue;                       // �󔒂͏���
                    }
                    elements[i] = elements[i].TrimStart('"').TrimEnd('"');
                }
                characterDataArrayList.Add(elements.ToList());
            }
            return characterDataArrayList;
        }

        /// <summary>
        /// �f�[�^�̓]�u�p�̊֐�
        /// </summary>
        public static List<List<string>> Transpose(List<List<string>> matrix)
        {
            // �񐔂ƍs�����擾
            int rowCount = matrix.Count;
            int colCount = matrix[0].Count;

            // �V�����]�u���List<List<string>>���쐬
            List<List<string>> result = new List<List<string>>();

            // �񂲂ƂɐV�����s���쐬
            for (int i = 0; i < colCount; i++)
            {
                List<string> newRow = new List<string>();

                for (int j = 0; j < rowCount; j++)
                {
                    newRow.Add(matrix[j][i]);
                }

                result.Add(newRow);
            }

            return result;
        }

        /// <summary>
        /// sprite�̓ǂݍ���
        /// </summary>
        Sprite LoadSprite(string path)
        {
            if (File.Exists(path))
            {
                // �t�@�C������o�C�g�f�[�^��ǂݍ���
                byte[] fileData = File.ReadAllBytes(path);

                // �o�C�g�f�[�^����e�N�X�`�����쐬
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(fileData)) // �摜�f�[�^��ǂݍ���
                {
                    // �e�N�X�`������X�v���C�g���쐬
                    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.LogError("Failed to load texture from the file.");
                    return null;
                }
            }
            else
            {
                Debug.LogError("File does not exist at path: " + path);
                return null;
            }
        }
    }

    [Serializable]
    public class SheetData
    {
        public string SheetName;
        public List<string> Keys;
        public List<string> Types;
        public List<List<string>> Data; // �p�����[�^�[�f�[�^�i�����̃L�[�E�^�C�v�j
    }
}