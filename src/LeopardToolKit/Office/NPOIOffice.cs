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
        public void ExportToExcel<T>(IEnumerable<T> data, string fullPath, ExportOption exportOption = null)
        {
            data.ThrowIfNull(nameof(data));
            fullPath.ThrowIfNull(nameof(fullPath));

            exportOption = exportOption ?? new ExportOption();

            List<T> list = data.ToList();

            #region create workbook
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
            #endregion

            #region cell style
            Dictionary<Type, ICellStyle> cellStyles = new Dictionary<Type, ICellStyle>();
            IDataFormat dataFormat = workbook.CreateDataFormat();

            ICellStyle dateTimeCellStyle = workbook.CreateCellStyle();
            dateTimeCellStyle.DataFormat = dataFormat.GetFormat(exportOption.DateFormat ?? "yyyy-MM-dd HH:mm:ss");
            cellStyles[typeof(DateTime)] = dateTimeCellStyle;

            if (!exportOption.NumberFormat.IsEmpty())
            {
                ICellStyle doubleCellStyle = workbook.CreateCellStyle();
                doubleCellStyle.DataFormat = dataFormat.GetFormat(exportOption.NumberFormat);
                cellStyles[typeof(double)] = doubleCellStyle;
            }

            #endregion

            #region create sheet
            ISheet exportSheet;
            string sheetName = exportOption.SheetName.IsEmpty() ? "sheet1" : exportOption.SheetName;
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
            #endregion

            #region create header
            IRow row = exportSheet.CreateRow(0);
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                ICell cell = row.CreateCell(i);
                var prop = propertyInfos[i];
                var headerAttr = prop.GetCustomAttribute<ExcelHeaderAttribute>();
                cell.SetCellValue(headerAttr?.Name ?? prop.Name);
            }
            #endregion

            #region export data
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
                        SetCellValue(cell, cellValue,cellStyles);
                    }
                }
            }
            #endregion

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

        public List<T> ImportFromExcel<T>(string fullPath, ImportOption importOption = null) where T : new()
        {
            fullPath.ThrowIfNull(nameof(fullPath));

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"The {fullPath} is not found", fullPath);
            }

            importOption = importOption ?? new ImportOption();

            #region create workbook
            IWorkbook workbook;
            if (fullPath.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase))
            {
                workbook = new XSSFWorkbook(new FileStream(fullPath, FileMode.Open));
            }
            else
            {
                workbook = new HSSFWorkbook(new FileStream(fullPath, FileMode.Open));
            }
            #endregion

            #region create sheet
            ISheet importSheet;
            if (importOption.SheetName.IsEmpty())
            {
                importSheet = workbook.GetSheetAt(importOption.SheetIndex);
                if (importSheet == null)
                {
                    throw new Exception($"Not found sheet with index '0'");
                }
            }
            else
            {
                importSheet = workbook.GetSheet(importOption.SheetName);
                if (importSheet == null)
                {
                    throw new Exception($"Not found sheet with name '{importOption.SheetName}'");
                }
            }
            #endregion
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<PropertyInfo, int> propertyMap = new Dictionary<PropertyInfo, int>();
            IRow headerRow = importSheet.GetRow(0);
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                ICell cell = headerRow.GetCell(i);
                string columnName = cell.StringCellValue;
                columnName.ThrowIfNull(nameof(columnName));
                foreach (var prop in propertyInfos)
                {
                    var headerAttr = prop.GetCustomAttribute<ExcelHeaderAttribute>();
                    if(columnName == prop.Name || columnName == headerAttr?.Name)
                    {
                        propertyMap[prop] = i;
                        break;
                    }
                }
            }

            List<T> result = new List<T>();
            for (int i = (importSheet.FirstRowNum + 1); i <= importSheet.LastRowNum; i++)
            {
                IRow row = importSheet.GetRow(i);
                T t = new T();

                foreach (var prop in propertyInfos)
                {
                    ICell cell = row.GetCell(propertyMap[prop]);
                    var value = GetCellValue(cell, prop.PropertyType);
                    if(value != null)
                    {
                        prop.SetValue(t, value);
                    }
                }

                result.Add(t);
            }
            workbook?.Close();
            return result;
        }

        private object GetCellValue(ICell cell, Type type)
        {
            if (type.Name == typeof(Nullable<>).Name)
            {
                type = type.GenericTypeArguments[0];
            }


            if (cell.CellType == CellType.String)
            {
                return GetCellValue(cell.StringCellValue, type);
            }
            else if(cell.CellType == CellType.Numeric)
            {
                if(type == typeof(DateTime))
                {
                    return cell.DateCellValue;
                }
                else if (type == typeof(DateTimeOffset))
                {
                    return (DateTimeOffset)cell.DateCellValue;
                }
                else
                {
                    return GetCellValue(cell.NumericCellValue.ToString(), type);
                }
            }
            else if (cell.CellType == CellType.Boolean)
            {
                return cell.BooleanCellValue;
                
            }
            else
            {
                return GetCellValue(cell.StringCellValue, type);
            }
        }

        private object GetCellValue(string cellStringValue, Type type)
        {
            if (cellStringValue == null)
            {
                return null;
            }
            if (type.Name == typeof(Nullable<>).Name)
            {
                type = type.GenericTypeArguments[0];
            }
            if (type == typeof(string))
            {
                return cellStringValue;
            }
            if (type == typeof(DateTime))
            {
                DateTime.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(DateTimeOffset))
            {
                DateTimeOffset.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(TimeSpan))
            {
                TimeSpan.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(Guid))
            {
                Guid.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(long))
            {
                long.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(ulong))
            {
                ulong.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(int))
            {
                int.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(uint))
            {
                uint.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(short))
            {
                short.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(ushort))
            {
                ushort.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(byte))
            {
                byte.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(double))
            {
                double.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(float))
            {
                float.TryParse(cellStringValue, out var value);
                return value;
            }
            if (type == typeof(decimal))
            {
                decimal.TryParse(cellStringValue, out var value);
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// https://blog.csdn.net/oYuHuaChen/article/details/82109773
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        /// <param name="cellStyles"></param>
        private void SetCellValue(ICell cell, object value, Dictionary<Type, ICellStyle> cellStyles)
        {
            if (value.GetType() == typeof(double))
            {
                if (cellStyles.TryGetValue(typeof(double), out var style))
                {
                    cell.CellStyle = style;
                }
                cell.SetCellValue((double)value);
            }
            else if (value.GetType() == typeof(int))
            {
                cell.SetCellValue(double.Parse(value.ToString()));
            }
            else if(value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTimeOffset))
            {
                if (cellStyles.TryGetValue(typeof(DateTime), out var style))
                {
                    cell.CellStyle = style;
                }
                cell.SetCellValue(((DateTimeOffset)value).DateTime);
            }
            else if (value.GetType() == typeof(bool))
            {
                cell.SetCellValue((bool)value);
            }
            else
            {
                cell.SetCellValue(value.ToString());
            }
        }
    }
}
