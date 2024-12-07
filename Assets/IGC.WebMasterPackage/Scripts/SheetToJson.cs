using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace IGC.WebMasterPackage
{
    public class SheetToJson
    {
        public string Main(List<SheetData> sheets)
        {
            // シートデータをJSONにエクスポート
            return ExportSheetAsJson(sheets);
        }

        /// <summary>
        /// スプレッドシートのデータをJSONとして返す
        /// </summary>
        private string ExportSheetAsJson(List<SheetData> sheets)
        {
            var data = new Dictionary<string, object>();

            foreach (var sheet in sheets)
            {
                string sheetName = sheet.SheetName;
                var rows = sheet.Data;

                // シート名が#から始まる場合は無視する
                if (sheetName.StartsWith("#"))
                {
                    continue;
                }

                // パラメータシートの処理
                if (sheetName.StartsWith("-"))
                {
                    data[sheetName.Substring(1)] = GenerateParameterSheetAsJson(rows);
                }
                else
                {
                    data[sheetName] = GenerateDataSheetAsJson(sheet.Keys, rows);
                }
            }

            // JSON形式にシリアライズ
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            return json;
        }

        /// <summary>
        /// データシートをJSON形式で生成
        /// </summary>
        private List<Dictionary<string, string>> GenerateDataSheetAsJson(List<string> keys, List<List<string>> rows)
        {
            var sheetData = new List<Dictionary<string, string>>();
            for (int i = 1; i < rows[0].Count; i++) // 2行目以降がデータ
            {
                var row = rows.Select(v => v[i]).ToList();
                var rowData = new Dictionary<string, string>();

                for (int j = 0; j < row.Count; j++)
                {
                    string key = keys[j];
                    string value = row[j];

                    // 変数名が#から始まる場合は無視する
                    if (key.StartsWith("#"))
                    {
                        continue;
                    }
                    rowData[key] = value; // Unityで扱いやすい形式
                }
                sheetData.Add(rowData);
            }
            return sheetData;
        }

        /// <summary>
        /// パラメータシートをJSON形式で生成
        /// </summary>
        private Dictionary<string, string> GenerateParameterSheetAsJson(List<List<string>> rows)
        {
            var sheetData = new Dictionary<string, string>();

            for (int i = 1; i < rows.Count; i++) // 2行目以降
            {
                string keyName = rows[i][0];  // 1列目がキー
                string value = rows[i][2];    // 3列目が値

                sheetData[keyName] = value;
            }

            return sheetData;
        }
    }
}