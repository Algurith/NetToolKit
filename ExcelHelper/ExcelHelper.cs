using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CellType = NPOI.SS.UserModel.CellType;

namespace ExcelHelper
{
    public class ExcelHelper
    {
        /// <summary>
        /// Stream转DataTable
        /// <para>读取上传文件</para>
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static DataTable LoadToDataTable(Stream fileStream) =>
            SheetToDataTable(new HSSFWorkbook(fileStream).GetSheetAt(0));

        public static DataTable LoadToDataTable(string file)
        {
            try
            {
                IFormulaEvaluator evaluator;
                return SheetToDataTable(InitializeWorkbook(file, out evaluator).GetSheetAt(0));
            }
            catch (Exception)
            {
                return LoadFromCsv(file, true,
                    x => Regex.Replace(string.IsNullOrEmpty(x) ? string.Empty : x, @"\t", ",").Split(','));
            }
        }

        /// <summary>
        /// Sheet转DataTable
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static DataTable SheetToDataTable(ISheet sheet) 
        {
            var dt = new DataTable();
            //表头
            var header = sheet.GetRow(sheet.FirstRowNum);
            var columns = new List<int>();
            for (var i = 0; i < header.LastCellNum; i++) 
            {
                var obj = GetValueType(header.GetCell(i));
                if (obj == null || obj.ToString() == string.Empty)
                    dt.Columns.Add(new DataColumn("Columns" + i));
                else
                    dt.Columns.Add(new DataColumn(obj.ToString()));

                columns.Add(i);
            }
            //数据
            for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) 
            {
                var dr = dt.NewRow();
                var hasValue = false;
                foreach (var j in columns) 
                {
                    try
                    {
                        var tmpRow = sheet.GetRow(i);
                        var tmpCell = tmpRow?.GetCell(j);
                        if (tmpCell == null)
                            continue;
                        dr[j] = GetValueType(tmpCell);
                        if (dr[j] != null && Convert.ToString(dr[j]) != string.Empty)
                            hasValue = true;
                    }
                    catch
                    {
                        throw;
                    }
                }
                if (hasValue)
                    dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>   
        /// 获取单元格类型(xls) .注意:公式会自动计算返回结果
        /// </summary>   
        /// <param name="cell">单元格</param>   
        /// <returns>单元格值</returns>   
        private static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:   
                    return null;
                case CellType.Boolean: //BOOLEAN:   
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:   
                    if (DateUtil.IsCellDateFormatted(cell)) //日期类型
                    {
                        return cell.DateCellValue;
                    }
                    //其他数字类型
                    return cell.NumericCellValue;
                case CellType.String: //STRING:   
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:   
                    return cell.ErrorCellValue;
                case CellType.Formula:
                    switch (cell.CachedFormulaResultType)
                    {
                        case CellType.String:
                            var strFormula = cell.StringCellValue;
                            return !string.IsNullOrEmpty(strFormula) ? strFormula : "";
                        case CellType.Numeric:
                            return Convert.ToString(cell.NumericCellValue);
                        case CellType.Boolean:
                            return Convert.ToString(cell.BooleanCellValue);
                        case CellType.Error:
                            return ErrorEval.GetText(cell.ErrorCellValue);
                        default:
                            return "";
                    }
                default:
                    //   return "=" + cell.CellFormula;
                    return "";
            }
        }

        private static IWorkbook InitializeWorkbook(string excelFile, out IFormulaEvaluator evaluator)
        {
            using (var file = new FileStream(excelFile, FileMode.Open, FileAccess.Read))
            {
                if (string.Equals(file.Name.Split('.').Last(), "xlsx", StringComparison.CurrentCultureIgnoreCase))
                {
                    var xssWorkbook = new XSSFWorkbook(file);
                    evaluator = new XSSFFormulaEvaluator(xssWorkbook);
                    return xssWorkbook;
                }
                var hssWorkbook = new HSSFWorkbook(file);
                evaluator = new HSSFFormulaEvaluator(hssWorkbook);
                return hssWorkbook;
            }
        }

        private static DataTable LoadFromCsv(string file, bool isRowOneHeader = true, Func<string, string[]> p1 = null)
        {

            var csvDataTable = new DataTable();

            //no try/catch - add these in yourselfs or let exception happen
            //String[] csvData = File.ReadAllLines(HttpContext.Current.Server.MapPath(file));
            var csvData = File.ReadAllLines(file, Encoding.UTF8);

            //if no data in file ‘manually' throw an exception
            if (csvData.Length == 0)
            {
                throw new Exception("CSV File Appears to be Empty");
            }
            if (p1 == null)
            {
                p1 = x => x.Split(',');
            }
            var headings = p1(csvData[0]);
            var index = 0; //will be zero or one depending on isRowOneHeader

            if (isRowOneHeader) //if first record lists headers
            {
                index = 1; //so we won't take headings as data

                //for each heading
                for (var i = 0; i < headings.Length; i++)
                {
                    //replace spaces with underscores for column names
                    headings[i] = headings[i].Replace(" ", "_");

                    //add a column for each heading
                    csvDataTable.Columns.Add(headings[i], typeof(string));
                }
            }
            else //if no headers just go for col1, col2 etc.
            {
                for (var i = 0; i < headings.Length; i++)
                {
                    //create arbitary column names
                    csvDataTable.Columns.Add("col" + (i + 1), typeof(string));
                }
            }

            //populate the DataTable
            for (var i = index; i < csvData.Length; i++)
            {
                //create new rows
                var row = csvDataTable.NewRow();

                for (var j = 0; j < headings.Length; j++)
                {
                    //fill them
                    row[j] = p1(csvData[i])[j];
                }

                //add rows to over DataTable
                csvDataTable.Rows.Add(row);
            }

            //return the CSV DataTable
            return csvDataTable;

        }
    }
}
