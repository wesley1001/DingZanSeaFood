using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace BreezeShop.Core.FileFactory
{
    public class ExcelTool
    {
        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <returns>返回的DataTable</returns>
        public static DataTable ExcelToDataTable(MemoryStream stream)
        {
            var workbook = new HSSFWorkbook(stream);

            //获取excel的第一个sheet
            var sheet = workbook.GetSheetAt(0);

            var table = new DataTable();
            //获取sheet的首行
            var headerRow = sheet.GetRow(0);

            //一行最后一个方格的编号 即总的列数
            int cellCount = headerRow.LastCellNum;

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }

            //最后一列的标号  即总的行数
            int rowCount = sheet.LastRowNum;

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                table.Rows.Add(dataRow);
            }

            workbook = null;
            sheet = null;

            return table;
        }

        public static MemoryStream ExportListToExcel<T>(IList<T> list, IList<String> headerList, IList<String> sortList)
        {
            try
            {
                //文件流对象
                //FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                var stream = new MemoryStream();

                //打开Excel对象
                var workbook = new HSSFWorkbook();

                //Excel的Sheet对象
                var sheet = workbook.CreateSheet("sheet1");

                //set date format
                var cellStyleDate = workbook.CreateCellStyle();
                var format = workbook.CreateDataFormat();
                cellStyleDate.DataFormat = format.GetFormat("yyyy年m月d日");

                //使用NPOI操作Excel表
                var row = sheet.CreateRow(0);
                int count = 0;

                var properties = TypeDescriptor.GetProperties(typeof (T));

                //if (headerList != null && properties.Count != headerList.Count)
                //    throw new Exception("集合的属性个数和标题行List的个数不一致");

                //如果没有自定义的行首,那么采用反射集合的属性名做行首
                if (headerList == null)
                {
                    for (int i = 0; i < properties.Count; i++) //生成sheet第一行列名 
                    {
                        var cell = row.CreateCell(count++);
                        cell.SetCellValue(String.IsNullOrEmpty(properties[i].DisplayName)
                            ? properties[i].Name
                            : properties[i].DisplayName);
                    }
                }
                else
                {
                    foreach (var t in headerList)
                    {
                        var cell = row.CreateCell(count++);
                        cell.SetCellValue(t);
                    }
                }

                //将数据导入到excel表中
                for (var i = 0; i < list.Count; i++)
                {
                    var rows = sheet.CreateRow(i + 1);
                    count = 0;

                    object value = null;
                    //如果自定义导出属性及排序字段为空,那么走反射序号的方式
                    if (sortList == null)
                    {
                        for (var j = 0; j < properties.Count; j++)
                        {

                            var cell = rows.CreateCell(count++);
                            value = properties[j].GetValue(list[i]);
                            cell.SetCellValue(value == null ? String.Empty : value.ToString());
                        }
                    }
                    else
                    {
                        foreach (var t in sortList)
                        {
                            var cell = rows.CreateCell(count++);
                            value = properties[t].GetValue(list[i]);
                            cell.SetCellValue(value == null ? String.Empty : value.ToString());
                        }
                    }
                }


                //保存excel文档
                sheet.ForceFormulaRecalculation = true;

                workbook.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);

                return stream;
            }
            catch
            {
                return new MemoryStream();
            }
        }

        /// <summary>
        /// 将DataSet数据集转换HSSFworkbook对象，并保存为Stream流
        /// </summary>
        /// <param name="ds"></param>
        /// <returns>返回数据流Stream对象</returns>
        public static MemoryStream ExportDatasetToExcel(DataSet ds)
        {
            try
            {
                //文件流对象
                //FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                var stream = new MemoryStream();

                //打开Excel对象
                var workbook = new HSSFWorkbook();

                //Excel的Sheet对象
                var sheet = workbook.CreateSheet("sheet1");


                var cellFont = workbook.CreateFont();
                var cellStyle = workbook.CreateCellStyle();
                //- 加粗，白色前景色
                cellFont.Boldweight = (short) NPOI.SS.UserModel.FontBoldWeight.Bold;
                //- 这个是填充的模式，可以是网格、花式等。如果需要填充单色，请使用：SOLID_FOREGROUND
                //cellStyle.FillPattern = NPOI.SS.UserModel.FillPatternType.SOLID_FOREGROUND;
                //- 设置这个样式的字体，如果没有设置，将与所有单元格拥有共同字体！
                cellStyle.SetFont(cellFont);
                cellStyle.Alignment = HorizontalAlignment.Center;


                //set date format
                var cellStyleDate = workbook.CreateCellStyle();
                var format = workbook.CreateDataFormat();
                cellStyleDate.DataFormat = format.GetFormat("yyyy年m月d日");

                //使用NPOI操作Excel表
                var row = sheet.CreateRow(0);
                int count = 0;
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++) //生成sheet第一行列名 
                {
                    var cell = row.CreateCell(count++);
                    cell.SetCellValue(ds.Tables[0].Columns[i].Caption);
                    cell.CellStyle = cellStyle;
                }
                //将数据导入到excel表中
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var rows = sheet.CreateRow(i + 1);
                    count = 0;
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        var cell = rows.CreateCell(count++);
                        Type type = ds.Tables[0].Rows[i][j].GetType();
                        if (type == typeof (int) || type == typeof (Int16)
                            || type == typeof (Int32) || type == typeof (Int64))
                        {
                            cell.SetCellValue((int) ds.Tables[0].Rows[i][j]);
                        }
                        else
                        {
                            if (type == typeof (float) || type == typeof (double) || type == typeof (Double))
                            {
                                cell.SetCellValue((Double) ds.Tables[0].Rows[i][j]);
                            }
                            else
                            {
                                if (type == typeof (DateTime))
                                {
                                    cell.SetCellValue(((DateTime) ds.Tables[0].Rows[i][j]).ToString("yyyy-MM-dd HH:mm"));
                                }
                                else
                                {
                                    if (type == typeof (bool) || type == typeof (Boolean))
                                    {
                                        cell.SetCellValue((bool) ds.Tables[0].Rows[i][j]);
                                    }
                                    else
                                    {
                                        cell.SetCellValue(ds.Tables[0].Rows[i][j].ToString());
                                    }
                                }
                            }
                        }
                    }
                }

                //保存excel文档
                sheet.ForceFormulaRecalculation = true;

                workbook.Write(stream);

                return stream;
            }
            catch
            {
                return new MemoryStream();
            }
        }
    }
}
