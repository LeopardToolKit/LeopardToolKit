using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Office
{
    public interface IOffice
    {
        void ExportToExcel<T>(IEnumerable<T> data, string fullPath, ExportOption exportOption = null);

        List<T> ImportFromExcel<T>(string fullPath, ImportOption importOption = null) where T : new();
    }
}
