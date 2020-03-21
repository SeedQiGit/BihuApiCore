using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BihuApiCore.Infrastructure.Extensions;
using NPOI;
using NPOI.XSSF.UserModel;

namespace BihuApiCore.Service.Implementations
{
    public class ExcelService:IExcelService
    {

        public async Task<BaseResponse> Xlsx()
        {
            List<ExcelTestClass> list=ExcelTestClass.GetList();
            //BaseDirectory后面有\所以exel前面就不加\了
            var storePath = AppDomain.CurrentDomain.BaseDirectory + "Excel";
            if (!Directory.Exists(storePath))
            {
                Directory.CreateDirectory(storePath);
            }

            string fileNam = "导出数据-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string fullPath = storePath +"\\"+ fileNam;

            XSSFWorkbook workbook = new XSSFWorkbook();

            POIXMLProperties props = workbook.GetProperties();
            props.CoreProperties.Creator = "北京易天正诚信息技术有限公司";
            props.CoreProperties.Subject = "壁虎科技";
            props.CoreProperties.Title = "壁虎科技";
            props.CoreProperties.Created = DateTime.Now;
            props.CustomProperties.AddProperty("壁虎科技", "壁虎科技");


            XSSFCellStyle headStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            XSSFFont font = (XSSFFont)workbook.CreateFont();
            font.FontHeightInPoints = 12;
            font.Boldweight = 700;
            headStyle.SetFont(font);

            var sheet =workbook.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);
           
            var headCell = headerRow.CreateCell(0);
            headCell.SetCellValue("111");


            FileStream file = new FileStream(fullPath, FileMode.Create);
            workbook.Write(file);
            file.Close();

            return BaseResponse.Ok(fullPath);
        }

        #region 正常写入文件

        
        public async Task<BaseResponse> ListToExcelFileXlsx()
        {
            List<ExcelTestClass> list=ExcelTestClass.GetList();
            //BaseDirectory后面有\所以exel前面就不加\了
            var storePath = AppDomain.CurrentDomain.BaseDirectory + "Excel";
            if (!Directory.Exists(storePath))
            {
                Directory.CreateDirectory(storePath);
            }

            string fileNam = "导出数据-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string fullPath = storePath +"\\"+ fileNam;

            //HSSFWorkbook  hssfworkbook=await GetWorkbook(list,"导出通话记录");
            XSSFWorkbook  hssfworkbook=ListToExcelExtention.ListToXlsx(list,"导出通话记录");
            //把这个HSSFWorkbook实例写入文件
            FileStream file = new FileStream(fullPath, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();

            return BaseResponse.Ok(fullPath);
        }


        public async Task<BaseResponse> ListToExcelFile()
        {
            List<ExcelTestClass> list=ExcelTestClass.GetList();
            //BaseDirectory后面有\所以exel前面就不加\了
            var storePath = AppDomain.CurrentDomain.BaseDirectory + "Excel";
            if (!Directory.Exists(storePath))
            {
                Directory.CreateDirectory(storePath);
            }

            string fileNam = "导出数据-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            string fullPath = storePath +"\\"+ fileNam;

            //HSSFWorkbook  hssfworkbook=await GetWorkbook(list,"导出通话记录");
            HSSFWorkbook  hssfworkbook=ListToExcelExtention.ListWorkbookExchange(list,"导出通话记录");
            //把这个HSSFWorkbook实例写入文件
            FileStream file = new FileStream(fullPath, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();

            return BaseResponse.Ok(fullPath);
        }

        #endregion

        #region 测试代码

        public async Task<HSSFWorkbook> GetWorkbook(List<ExcelTestClass> list,string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "耐心的雪球有限公司";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "耐心"; //填加xls文件作者信息
                si.LastAuthor = "耐心"; //填加xls文件最后保存者信息
                si.Comments = "耐心"; //填加xls文件作者信息
                si.Subject = "导出文件记录";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion
           
            HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            HSSFFont font = (HSSFFont)workbook.CreateFont();
            font.FontHeightInPoints = 12;
            font.Boldweight = 700;
            headStyle.SetFont(font);
            await ListToSheet(workbook, list, headStyle, sheetName);
            return workbook;
        }
        public async Task ListToSheet<T>(HSSFWorkbook workbook, List<T> list, HSSFCellStyle headStyle, string sheetName)
        {
            //值类型直接返回第一列
            Type tp = typeof(T);
            //属性列表
            PropertyInfo[] properties = tp.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            //property.Name是属性的英文名，怎么转换成中文？使用DescriptionAttribute特性
            List<string> fieldStringArray=new List<string>();
            foreach (var property in properties)
            {
                fieldStringArray.Add(property.GetEnumDescription());
            }
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(sheetName);
           
            int fieldCount = fieldStringArray.Count;
            HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
            headerRow.HeightInPoints = 20;
            for (int i = 0; i < fieldCount; i++)
            {
                #region 表头及样式
                headerRow.CreateCell(i).SetCellValue(fieldStringArray[i]);
                headerRow.GetCell(i).CellStyle = headStyle;
                sheet.AutoSizeColumn(i);
                #endregion
            }

             var count = list.Count();
            for (int i = 0; i < count; i++)
            {
                #region 单元格样式

                ICellStyle styleCell = workbook.CreateCellStyle();
                styleCell.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//居中 
                styleCell.VerticalAlignment = VerticalAlignment.Center;//垂直居中 

                #endregion

                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(i + 1);
                var data = list[i];
                for (int cellIndex = 0; cellIndex < fieldCount; cellIndex++)
                {
                    var property = properties[cellIndex];
                    HSSFCell newCell = (HSSFCell)dataRow.CreateCell(cellIndex, CellType.String);
                    newCell.SetCellValue(  property.GetValue(data).ToString());
                    newCell.CellStyle = styleCell;
                }
            }

            #region  统一设置列宽度

            for (int columnNum = 0; columnNum <= fieldCount; columnNum++)
            {
                int columnWidth = sheet.GetColumnWidth(columnNum) / 256;//获取当前列宽度  
                for (int rowNum = 1; rowNum <= sheet.LastRowNum; rowNum++)//在这一列上循环行  
                {
                    IRow currentRow = sheet.GetRow(rowNum);
                    ICell currentCell = currentRow.GetCell(columnNum);

                    int length = currentCell != null ? Encoding.Default.GetBytes(currentCell.ToString()).Count() : 0;//获取当前单元格的内容宽度  
                    if (columnWidth < length + 1)
                    {
                        columnWidth = length + 1;
                    }//若当前单元格内容宽度大于列宽，则调整列宽为当前单元格宽度，后面的+1是我人为的将宽度增加一个字符  
                }

                sheet.SetColumnWidth(columnNum, (((columnWidth > 50 ? columnWidth / 4 : columnWidth) + 3) * 256));

            }

            #endregion

        }

        #endregion 

        #region 写入流的方法返回

        public async Task<MemoryStream> ListToExcelStream()
        {
            List<ExcelTestClass> list=ExcelTestClass.GetList();
            string fileNam = "导出数据-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            //HSSFWorkbook  hssfworkbook=await GetWorkbook(list,"导出通话记录");
            HSSFWorkbook  hssfworkbook=ListToExcelExtention.ListWorkbookExchange(list,"导出通话记录");
            using (MemoryStream ms = new MemoryStream())
            {
                hssfworkbook.Write(ms);
                await ms.FlushAsync();
                ms.Position = 0;
                hssfworkbook.Close();
                var buf = ms.ToArray();
                return ms;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> ListToExcelByte()
        {
            List<ExcelTestClass> list=ExcelTestClass.GetList();
            string fileNam = "导出数据-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            //HSSFWorkbook  hssfworkbook=await GetWorkbook(list,"导出通话记录");
            HSSFWorkbook  hssfworkbook=ListToExcelExtention.ListWorkbookExchange(list,"导出通话记录");
            using (MemoryStream ms = new MemoryStream())
            {
                hssfworkbook.Write(ms);
                await ms.FlushAsync();
                ms.Position = 0;
                hssfworkbook.Close();
                byte[] buf = ms.ToArray();
                return buf;
            }
        }

        #endregion
    }
    public class ExcelTestClass
    {
        public static List<ExcelTestClass> GetList()
        {
            return new List<ExcelTestClass>
            {
                new ExcelTestClass{Id = 1,Name = "xxXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXx"},
                new ExcelTestClass{Id = 2,Name = "uyy"},
            };

        }
        /// <summary>
        /// 编号
        /// </summary>
        [Description("编号")]
        public long Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Description("姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [Time]
        [Description("通话时长")]
        public int Second { get; set; }=1000;
    }

 
}
