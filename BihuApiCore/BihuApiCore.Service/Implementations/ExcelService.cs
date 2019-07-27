using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Implementations
{
    public class ExcelService:IExcelService
    {
        #region 正常写入文件

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

            HSSFWorkbook  hssfworkbook=await GetWorkbook(list,"导出通话记录");
            //把这个HSSFWorkbook实例写入文件
            FileStream file = new FileStream(fullPath, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();


            return BaseResponse.Ok();
        }

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

        #endregion
        
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
             
                for (int cellIndex = 0; cellIndex < fieldCount; cellIndex++)
                {
                    var property = properties[cellIndex];
                    var data = list[cellIndex];
                    var a =property.GetValue(data);
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

        #region 写入流的方法返回

        

        #endregion




    }
    public class ExcelTestClass
    {
        public static List<ExcelTestClass> GetList()
        {
            return new List<ExcelTestClass>
            {
                new ExcelTestClass{Id = 1,Name = "xxx"},
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
    }
}
