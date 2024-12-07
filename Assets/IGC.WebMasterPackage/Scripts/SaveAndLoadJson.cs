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
                writer.Write(json);  // ファイルに内容を書き込む
                Debug.Log($"{applicationPath}にデータを書き込みました");
            }
        }
        catch (Exception ex)
        {
        }
    }
    public string LoadJson(string path)
    {
        // 念のためファイルの存在チェック
        if (!File.Exists(path)) return "";

        // JSONデータとしてデータを読み込む
        return File.ReadAllText(path);
    }
}
