using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace IGC.WebMasterPackage
{
    public class GenerateCSharpClasses
    {
        public void GenerateCSharpClasse(List<SheetData> sheetDataList)
        {
            StringBuilder classCode = new StringBuilder();
            List<string> classNames = new List<string>(); // classNamesに実際のクラス名を設定
            List<string> classNameTypes = new List<string>(); // classNameTypesに型を設定
            classCode.AppendLine("using System;");
            classCode.AppendLine("using UnityEngine;");
            classCode.AppendLine();

            foreach (var sheetData in sheetDataList)
            {
                string sheetName = sheetData.SheetName;
                List<string> keys = sheetData.Keys;
                List<string> types = sheetData.Types;

                // シート名が#から始まる場合は無視する
                if (sheetName.StartsWith("#"))
                {
                    continue;
                }
                if (sheetName.StartsWith("-"))
                {
                    string name = sheetName.Substring(1);
                    classCode.AppendLine(GenerateParameterClassCode(name, sheetData.Data));
                    classNames.Add(name);
                    classNameTypes.Add(name);
                }
                else
                {
                    classCode.AppendLine(GenerateClassCode(sheetName, keys, types));
                    classNames.Add(sheetName);
                    classNameTypes.Add(sheetName + "[]");
                }
            }

            classCode.AppendLine(GenerateClassCode("MasterData", classNames, classNameTypes));

            SaveTextToUserFolder("MasterData.cs", classCode.ToString());
        }

        /// <summary>
        /// クラスコード生成
        /// </summary>
        private string GenerateClassCode(string className, List<string> keys, List<string> types)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("[Serializable]");
            code.AppendLine($"public class {className} {{");

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                string type = types[i];

                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                // 変数名が#から始まる場合は無視する
                if (key.StartsWith("#"))
                {
                    continue;
                }

                code.AppendLine($"  public {type} {key};");
            }

            code.AppendLine("}");
            return code.ToString();
        }

        /// <summary>
        /// パラメータークラスコード生成
        /// </summary>
        private string GenerateParameterClassCode(string className, List<List<string>> data)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("[Serializable]");
            code.AppendLine($"public class {className} {{");

            for (int i = 1; i < data.Count; i++)
            {
                string key = data[i][0];
                string type = data[i][1];

                code.AppendLine($"  public {type} {key};");
            }

            code.AppendLine("}");
            return code.ToString();
        }

        /// <summary>
        /// 作成したMasterDataを上書き
        /// </summary>
        private void SaveTextToUserFolder(string fileName, string code)
        {
            string assetsPath = Application.dataPath;
            string filePath = Path.Combine(Application.dataPath, "Resources/IGC.WebMasterPackage/", $"{fileName}");
            if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources")))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));
            }
            if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources/IGC.WebMasterPackage")))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources/IGC.WebMasterPackage"));
            }
            if (!Directory.Exists(filePath))
            {
                string fileContent = "";
                File.WriteAllText(filePath, fileContent);
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    writer.Write(code);  // ファイルに内容を書き込む
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while writing to file: {ex.Message}");
            }
        }
    }
}