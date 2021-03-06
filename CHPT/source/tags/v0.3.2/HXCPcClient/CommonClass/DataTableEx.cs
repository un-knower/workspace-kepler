﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Utility.Common;

namespace HXCPcClient.CommonClass
{
    /// <summary>
    /// DataTable扩展
    /// </summary>
    public static class DataTableEx
    {
        /// <summary>
        /// DataTable分组
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="groupBy">分组列</param>
        /// <param name="listNot">不进行合计的列</param>
        public static void DataTableGroup(this DataTable dt, string groupBy,string groupByColumn,List<string> listNot)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }
            var querySum = from t in dt.AsEnumerable()
                           group t by t.Field<string>(groupBy);
            var dtAs = dt.AsEnumerable();
            DataRow drSum = DataTableSum(dt, dt.AsEnumerable(), listNot);
            int rowIndex=0;
            foreach (var query in querySum)
            {
                DataRow drTitle = dt.NewRow();
                drTitle[groupByColumn] = query.Key;
                dt.Rows.InsertAt(drTitle, rowIndex);
                rowIndex++;
                DataRow dr = dt.NewRow();
                if (dt.Columns[0].DataType.Name == "String")
                {
                    dr[0] = "小计";
                }
                foreach (DataColumn dc in dt.Columns)
                {
                    if (listNot != null && listNot.Contains(dc.ColumnName))
                    {
                        continue;
                    }
                    switch (dc.DataType.Name)
                    {
                        case "Int":
                            dr[dc.ColumnName] = query.Sum(t => t.Field<int>(dc.ColumnName));
                            break;
                        case "Int64":
                            dr[dc.ColumnName] = query.Sum(t => t.Field<Int64>(dc.ColumnName));
                            break;
                        case "Double":
                            dr[dc.ColumnName] = query.Sum(t => t.Field<double>(dc.ColumnName));
                            break;
                        case "Decimal":
                            dr[dc.ColumnName] = query.Sum(t => t.Field<decimal>(dc.ColumnName));
                            break;
                    }
                }
                rowIndex += query.Count();
                dt.Rows.InsertAt(dr, rowIndex);
                rowIndex++;
            }
            dt.Rows.Add(drSum);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="groupBy1">分组字段1</param>
        /// <param name="groupByColumn1">分组存放列</param>
        /// <param name="groupText1">分组显示文字</param>
        /// <param name="groupBy2"></param>
        /// <param name="groupByColumn2"></param>
        /// <param name="groupText2"></param>
        /// <param name="listNot">不合计的字段</param>
        public static void DataTableGroup(this DataTable dt, string groupBy1, string groupByColumn1,
           string groupText1 ,string groupBy2, string groupByColumn2,string groupText2, List<string> listNot)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }
            var querySum = from t in dt.AsEnumerable()
                           group t by new { t1 = t.Field<string>(groupBy1),t2=t.Field<string>(groupBy2) };
            var dtAs = dt.AsEnumerable();
            DataRow drSum = DataTableSum(dt, dt.AsEnumerable(), listNot);
            int rowIndex = 0;
            foreach (var query in querySum)
            {
                DataRow drTitle = dt.NewRow();
                drTitle[groupByColumn1] = groupText1 + query.Key.t1;
                drTitle[groupByColumn2] = groupText2 + query.Key.t2;
                dt.Rows.InsertAt(drTitle, rowIndex);
                rowIndex++;
                DataRow dr = dt.NewRow();
                if (dt.Columns[0].DataType.Name == "String")
                {
                    dr[0] = "小计";
                }
                foreach (DataColumn dc in dt.Columns)
                {
                    if (listNot != null && listNot.Contains(dc.ColumnName))
                    {
                        continue;
                    }
                    switch (dc.DataType.Name)
                    {
                        case "Int":
                            dr[dc.ColumnName] = query.Sum(t => t.Field<int>(dc.ColumnName));
                            break;
                        case "Int64":
                            dr[dc.ColumnName] = query.Sum(t => t.Field<Int64>(dc.ColumnName));
                            break;
                        case "Double":
                            dr[dc.ColumnName] = query.Sum(t => t.Field<double>(dc.ColumnName));
                            break;
                        case "Decimal":
                            dr[dc.ColumnName] = query.Sum(t => t.Field<decimal>(dc.ColumnName));
                            break;
                    }
                }
                rowIndex += query.Count();
                dt.Rows.InsertAt(dr, rowIndex);
                rowIndex++;
            }
            dt.Rows.Add(drSum);
        }
        /// <summary>
        /// 合计DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="listNot">不进行和计的列</param>
        public static void DataTableSum(this DataTable dt,List<string> listNot)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }
            var dtAs = dt.AsEnumerable();
            DataRow dr= DataTableSum(dt, dt.AsEnumerable(), listNot);
            dt.Rows.Add(dr);
        }

        public static DataRow DataTableSum(DataTable dt, EnumerableRowCollection<DataRow> dtAs, List<string> listNot)
        {
            DataRow dr = dt.NewRow();
            if (dt.Columns[0].DataType.Name == "String")
            {
                dr[0] = "合计";
            }
            foreach (DataColumn dc in dt.Columns)
            {
                if (listNot != null && listNot.Contains(dc.ColumnName))
                {
                    continue;
                }
                switch (dc.DataType.Name)
                {
                    case "Int":
                        dr[dc.ColumnName] = dtAs.Sum(t => t[dc.ColumnName] == DBNull.Value ? 0 : Convert.ToInt32(t[dc.ColumnName]));
                        break;
                    case "Int64":
                        dr[dc.ColumnName] = dtAs.Sum(t => t[dc.ColumnName] == DBNull.Value ? 0 : Convert.ToInt64(t[dc.ColumnName]));
                        break;
                    case "Double":
                        dr[dc.ColumnName] = dtAs.Sum(t => t[dc.ColumnName] == DBNull.Value ? 0 : Convert.ToDouble(t[dc.ColumnName]));
                        break;
                    case "Decimal":
                        dr[dc.ColumnName] = dtAs.Sum(t => t[dc.ColumnName] == DBNull.Value ? 0 : Convert.ToDecimal(t[dc.ColumnName]));
                        break;
                }
            }
            return dr;
        }

        /// <summary>
        /// 转换日期列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnDate">要转换的列名</param>
        public static void DataTableToDate(this DataTable dt, string columnDate)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }
            string columnDate1 = columnDate + "1";
            dt.Columns.Add(columnDate1);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[columnDate] != null && dr[columnDate] != DBNull.Value)
                {
                    long date = (long)dr[columnDate];
                    dr[columnDate1] = Common.UtcLongToLocalDateTime(date).ToString("yyyy-MM-dd");
                }
            }
            dt.Columns.Remove(columnDate);
            dt.Columns[columnDate1].ColumnName = columnDate;
        }

        public static void DataTableToDate(this DataTable dt, List<string> listDate)
        {
            if(listDate==null || listDate.Count==0)
            {
                return;
            }
            foreach (string columnDate in listDate)
            {
                dt.DataTableToDate(columnDate);
            }
        }
        public static DataTable ConvertToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            // column names    
            PropertyInfo[] oProps = null;
            if (varlist == null) return dtReturn;
            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow    
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;
                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
    }
}
