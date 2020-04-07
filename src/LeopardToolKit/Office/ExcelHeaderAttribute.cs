using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Office
{
    public class ExcelHeaderAttribute : Attribute
    {
        public string Name { get; set; }
        public ExcelHeaderAttribute(string name)
        {
            this.Name = name;
        }
    }
}
