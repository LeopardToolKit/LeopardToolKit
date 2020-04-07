using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.IO;

namespace LeopardToolKit.Office
{
    public class NPOIOffice : IOffice
    {
        public void ExportToExcel<T>(IEnumerable<T> data, string fullPath, ExcelOption excelOption = null)
        {
            data.ThrowIfNull(nameof(data));
            fullPath.ThrowIfNull(nameof(fullPath));

            excelOption = excelOption ?? new ExcelOption();

            List<T> list = data.ToList();

            IWorkbook workbook;
            if (fullPath.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase))
            {
                if (File.Exists(fullPath))
                {
                    workbook = new XSSFWorkbook(new FileStream(fullPath, FileMode.OpenOrCreate));
                }
                else
                {
                    workbook = new XSSFWorkbook();
                }
                
            }
            else
            {
                if (File.Exists(fullPath))
                {
                    workbook = new HSSFWorkbook(new FileStream(fullPath, FileMode.OpenOrCreate));
                }
                else
                {
                    workbook = new HSSFWorkbook();
                }
            }
            ISheet exportSheet;
            string sheetName = excelOption.SheetName.IsEmpty() ? "sheet1" : excelOption.SheetName;
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);
                if(sheet.SheetName == sheetName)
                {
                    workbook.RemoveSheetAt(i);
                    break;
                }
            }
            exportSheet = workbook.CreateSheet(sheetName);
            
            
            //Header
            IRow row = exportSheet.CreateRow(0);

            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                ICell cell = row.CreateCell(i);
                var prop = propertyInfos[i];
                var headerAttr = prop.GetCustomAttribute<ExcelHeaderAttribute>();
                cell.SetCellValue(headerAttr?.Name ?? prop.Name);
            }

            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                IRow sheetRow = exportSheet.CreateRow(i+1);
                for (int j = 0; j < propertyInfos.Length; j++)
                {
                    object cellValue = propertyInfos[j].GetValue(t);
                    ICell cell = sheetRow.CreateCell(j);
                    if (cellValue == null)
                    {
                        cell.SetCellValue("");
                    }
                    else
                    {
                        cell.SetCellValue(cellValue.ToString());
                    }
                }
            }

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                exportSheet.AutoSizeColumn(i);
            }
            using (FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                workbook.Write(fileStream);
            }
            workbook?.Close();
        }

        public List<T> ImportFromExcel<T>(string fullPath)
        {
            throw new NotImplementedException();
        }
    }
}
