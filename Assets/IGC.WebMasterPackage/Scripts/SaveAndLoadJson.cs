using System;
using System.IO;
using UnityEngine;

public class SaveAndLoadJson 
{
    public void SaveJson(string json, string applicationPath)
    {
        applicationPath += "/Resources";
        if (!Directory.Exists(applicationPath))
        {
            Directory.CreateDirectory(applicationPath);
        }
        applicationPath += "/IGC.WebMasterPackage";
        if (!Directory.Exists(applicationPath))
        {
            Directory.CreateDirectory(applicationPath);
        }
        applicationPath = applicationPath + "/MasterDataJson.json";
        if (!Directory.Exists(applicationPath))
        {
            string fileContent = "";
            File.WriteAllText(applicationPath, fileContent);
        }
        try
        {
            using (StreamWriter writer = new StreamWriter(applicationPath, false))
            {
                writer.Write(json);  // �t�@�C���ɓ��e����������
                Debug.Log($"{applicationPath}�Ƀf�[�^���������݂܂���");
            }
        }
        catch (Exception ex)
        {
        }
    }
    public string LoadJson(string path)
    {
        // �O�̂��߃t�@�C���̑��݃`�F�b�N
        if (!File.Exists(path)) return "";

        // JSON�f�[�^�Ƃ��ăf�[�^��ǂݍ���
        return File.ReadAllText(path);
    }
}
