﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BihuApiCore.Infrastructure.Helper;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace BihuApiCore.Infrastructure.Extensions
{
    public static class ListToExcelExtention
    {
       public static HSSFWorkbook ListWorkbookExchange<T>(List<T> list,string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "北京易天正诚信息技术有限公司";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "壁虎车险"; //填加xls文件作者信息
                si.LastAuthor = "壁虎车险"; //填加xls文件最后保存者信息
                si.Comments = "壁虎车险"; //填加xls文件作者信息
                si.Subject = sheetName;//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            ListToSheet(workbook, list, sheetName);

            return workbook;
        }

        /// <summary>
        /// 获取表头样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public static HSSFCellStyle GetHeadStyle(this HSSFWorkbook workbook)
        {
            HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            HSSFFont font = (HSSFFont)workbook.CreateFont();
            font.FontHeightInPoints = 12;
            font.Boldweight = 700;
            headStyle.SetFont(font);
            return headStyle;
        }

        /// <summary>
        /// 统一设置列宽度
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="fieldCount"></param>
        public static void SetColumnWidth(this HSSFSheet sheet,int fieldCount)
        {
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
        }

        public static void ListToSheet<T>(HSSFWorkbook workbook, List<T> list, string sheetName)
        {
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(sheetName);
            HSSFCellStyle headStyle=workbook.GetHeadStyle();

            //值类型直接返回第一列
            Type tp = typeof(T);
            //属性列表
            PropertyInfo[] properties = tp.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            //property.Name是属性的英文名，怎么转换成中文？使用DescriptionAttribute特性
            List<string> fieldStringArray = new List<string>();
            foreach (var property in properties)
            {
                fieldStringArray.Add(property.GetEnumDescription());
            }
  
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

            #region 单元格样式

            ICellStyle styleCell = workbook.CreateCellStyle();
            styleCell.Alignment = HorizontalAlignment.Center;//居中 
            styleCell.VerticalAlignment = VerticalAlignment.Center;//垂直居中 

            #endregion

            for (int i = 0; i < count; i++)
            {
                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(i + 1);
                var data = list[i];
                for (int cellIndex = 0; cellIndex < fieldCount; cellIndex++)
                {
                    var property = properties[cellIndex];
                    HSSFCell newCell = (HSSFCell)dataRow.CreateCell(cellIndex, CellType.String);
                    newCell.SetCellValue(property.GetValue(data).ToString());
                    newCell.CellStyle = styleCell;
                }
            }

            //统一设置列宽度
            sheet.SetColumnWidth(fieldCount);
        }
    }


    
    
}