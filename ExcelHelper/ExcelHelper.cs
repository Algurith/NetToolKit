using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    }
}
