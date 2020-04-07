using LeopardToolKit.Office;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LeopardToolKit.Test
{
    [TestClass]
    public class NpoiOfficeTest
    {
        [TestMethod]
        public void TestNpoiExcelExport()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddNpoiOffice();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IOffice office = serviceProvider.GetRequiredService<IOffice>();
            string fileName = Guid.NewGuid().ToString();
            office.ExportToExcel(GetDemoDatas(), $"{fileName}.xlsx",new ExportOption() { SheetName="sheet3" });
            Assert.IsTrue(File.Exists($"{fileName}.xlsx"));
        }

        [TestMethod]
        public void TestNpoiExcelImport()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddNpoiOffice();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IOffice office = serviceProvider.GetRequiredService<IOffice>();
            string fileName = Guid.NewGuid().ToString();
            office.ExportToExcel(GetDemoDatas(), $"{fileName}.xlsx", new ExportOption() { SheetName = "sheet3" });
            var result =office.ImportFromExcel<DemoData>($"{fileName}.xlsx");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        private List<DemoData> GetDemoDatas()
        {
            List<DemoData> demoDatas = new List<DemoData>();
            demoDatas.Add(new DemoData() { Name = "Foo", Age = 20 });
            demoDatas.Add(new DemoData() { Name = "Bar", Age = 21 });
            return demoDatas;
        }
    }

    public class DemoData
    {
        public string Name { get; set; }

        [ExcelHeader("年龄")]
        public int? Age { get; set; }
    }
}
