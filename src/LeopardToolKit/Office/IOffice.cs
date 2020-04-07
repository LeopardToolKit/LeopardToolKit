using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Office
{
    public interface IOffice
    {
        void ExportToExcel<T>(IEnumerable<T> data, string fullPath, ExcelOption excelOption = null);

        List<T> ImportFromExcel<T>(string fullPath);
    }
}
