﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using HXCPcClient.CommonClass;
using System.Windows.Forms;
using Utility.Common;
using SYSModel;

namespace HXCPcClient.Chooser
{
    /// <summary>
    /// 应收导入
    /// </summary>
    public class frmReceivable : frmBalanceDocuments
    {
        public frmReceivable(string custCode) :
            base(custCode)
        {

        }

        public frmReceivable()
            : base()
        { }
        protected override void BindData()
        {
            if (custCode.Length == 0)
            {
                return;
            }
            dgvData.RowCount = 0;
            DataTable dt;//数据
            StringBuilder sbWhere = new StringBuilder();
            sbWhere.AppendFormat("cust_id='{0}'", custCode);
            sbWhere.AppendFormat(" and wait_money>0");
            sbWhere.AppendFormat(" and isnull(is_occupy_finance,0)='{0}'", (int)DataSources.EnumImportStaus.OPEN);
            if (!string.IsNullOrEmpty(dtiDate.StartDate))
            {
                sbWhere.AppendFormat(" and order_date>{0}", Common.LocalDateTimeToUtcLong(Convert.ToDateTime(dtiDate.StartDate).Date));
            }
            if (!string.IsNullOrEmpty(dtiDate.EndDate))
            {
                sbWhere.AppendFormat(" and order_date <{0}", Common.LocalDateTimeToUtcLong(Convert.ToDateTime(dtiDate.EndDate).Date.AddDays(1)));
            }
            if (!string.IsNullOrEmpty(dtiBillDate.StartDate))
            {
                sbWhere.AppendFormat(" and receivables_date>{0}", Common.LocalDateTimeToUtcLong(Convert.ToDateTime(dtiBillDate.StartDate).Date));
            }
            if (!string.IsNullOrEmpty(dtiBillDate.EndDate))
            {
                sbWhere.AppendFormat(" and receivables_date<{0}", Common.LocalDateTimeToUtcLong(Convert.ToDateTime(dtiBillDate.EndDate).Date.AddDays(1)));
            }
            string orderNum = txtOrderNum.Caption.Trim();//单据编号
            if (orderNum.Length > 0)
            {
                sbWhere.AppendFormat(" and order_num like '%{0}%'", orderNum);
            }
            string checkNum = txtCheckNumber.Caption.Trim();//发票号
            if (checkNum.Length > 0)
            {
                sbWhere.AppendFormat(" and receipt_no like '%{0}%'", checkNum);
            }
            dt = DBHelper.GetTable("", "v_parts_sale_billing_receivable", "*", sbWhere.ToString(), "", "order by order_date");
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }
            foreach (DataRow dr in dt.Rows)
            {
                DataGridViewRow dgvr = dgvData.Rows[dgvData.Rows.Add()];
                dgvr.Cells[colID.Name].Value = dr["sale_billing_id"];//ID
                dgvr.Cells[colBillsName.Name].Value = "销售开单";//单据名称
                dgvr.Cells[colBillsType.Name].Value = DataSources.GetDescription(typeof(DataSources.EnumSaleOrderType), dr["order_type"]);//单据类型
                dgvr.Cells[colBillsCode.Name].Value = dr["order_num"];//单据编码
                dgvr.Cells[colTotalMoney.Name].Value = dr["current_collect"];//总金额
                dgvr.Cells[colBalanceMoney.Name].Value = dr["money"];//已结算
                dgvr.Cells[colWaitMoney.Name].Value = dr["wait_money"];//未结算
                dgvr.Cells[colReceivablesDate.Name].Value = Common.UtcLongToLocalDateTime(dr["receivables_date"], "yyyy-MM-dd");//收款日期
                dgvr.Cells[colReceiptNO.Name].Value = dr["receipt_no"];//发票号
                dgvr.Cells[colOrderDate.Name].Value = Common.UtcLongToLocalDateTime(dr["order_date"], "yyyy-MM-dd");//单据日期
            }
        }

        protected override bool LockDocument(string ids)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("is_occupy_finance", ((int)DataSources.EnumImportStaus.OCCUPY).ToString());
            return DBHelper.BatchUpdateDataByIn("锁定销售开单", "tb_parts_sale_billing", dic, "sale_billing_id", ids.Split(','));
        }
    }
}
