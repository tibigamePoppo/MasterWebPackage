using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace IGC.WebMasterPackage
{
    public class SheetToJson
    {
        public string Main(List<SheetData> sheets)
        {
            // �V�[�g�f�[�^��JSON�ɃG�N�X�|�[�g
            return ExportSheetAsJson(sheets);
        }

        /// <summary>
        /// �X�v���b�h�V�[�g�̃f�[�^��JSON�Ƃ��ĕԂ�
        /// </summary>
        private string ExportSheetAsJson(List<SheetData> sheets)
        {
            var data = new Dictionary<string, object>();

            foreach (var sheet in sheets)
            {
                string sheetName = sheet.SheetName;
                var rows = sheet.Data;

                // �V�[�g����#����n�܂�ꍇ�͖�������
                if (sheetName.StartsWith("#"))
                {
                    continue;
                }

                // �p�����[�^�V�[�g�̏���
                if (sheetName.StartsWith("-"))
                {
                    data[sheetName.Substring(1)] = GenerateParameterSheetAsJson(rows);
                }
                else
                {
                    data[sheetName] = GenerateDataSheetAsJson(sheet.Keys, rows);
                }
            }

            // JSON�`���ɃV���A���C�Y
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            return json;
        }

        /// <summary>
        /// �f�[�^�V�[�g��JSON�`���Ő���
        /// </summary>
        private List<Dictionary<string, string>> GenerateDataSheetAsJson(List<string> keys, List<List<string>> rows)
        {
            var sheetData = new List<Dictionary<string, string>>();
            for (int i = 1; i < rows[0].Count; i++) // 2�s�ڈȍ~���f�[�^
            {
                var row = rows.Select(v => v[i]).ToList();
                var rowData = new Dictionary<string, string>();

                for (int j = 0; j < row.Count; j++)
                {
                    string key = keys[j];
                    string value = row[j];

                    // �ϐ�����#����n�܂�ꍇ�͖�������
                    if (key.StartsWith("#"))
                    {
                        continue;
                    }
                    rowData[key] = value; // Unity�ň����₷���`��
                }
                sheetData.Add(rowData);
            }
            return sheetData;
        }

        /// <summary>
        /// �p�����[�^�V�[�g��JSON�`���Ő���
        /// </summary>
        private Dictionary<string, string> GenerateParameterSheetAsJson(List<List<string>> rows)
        {
            var sheetData = new Dictionary<string, string>();

            for (int i = 1; i < rows.Count; i++) // 2�s�ڈȍ~
            {
                string keyName = rows[i][0];  // 1��ڂ��L�[
                string value = rows[i][2];    // 3��ڂ��l

                sheetData[keyName] = value;
            }

            return sheetData;
        }
    }
}