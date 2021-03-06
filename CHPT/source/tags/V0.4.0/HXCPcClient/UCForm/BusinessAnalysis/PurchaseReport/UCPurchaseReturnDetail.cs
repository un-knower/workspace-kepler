﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HXCPcClient.Chooser;
using HXCPcClient.CommonClass;

namespace HXCPcClient.UCForm.BusinessAnalysis.PurchaseReport
{
    public partial class UCPurchaseReturnDetail : UCReport
    {
        public UCPurchaseReturnDetail()
            : base("v_purchase_return_detail_report","采购退换货汇总表")
        {
            InitializeComponent();
            dgvReport.ColumnHeadersHeight = 40;
            dgvReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }
        //窗体加载
        private void UCPurchaseReturnDetail_Load(object sender, EventArgs e)
        {
            //添加需要把负数显示成红色的字段
            listNegative = new List<string>();
            listNegative.Add("退回数量");
            listNegative.Add("退回货款");
            listNegative.Add("退回税额");
            listNegative.Add("退回金额");
            //供应商类别
            CommonFuncCall.BindComBoxDataSource(cbosup_type, "sys_supplier_category", "全部");
            //仓库
            CommonFuncCall.BindWarehouse(cbowh_code, "全部");
            //公司
            CommonFuncCall.BindCompany(cboCompany, "全部");
            dicreate_time.StartDate = DateTime.Now.ToString("yyyy-MM-01");
            dicreate_time.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            //添加合并列
            dgvReport.MergeColumnNames.Add("退回配件");
            dgvReport.AddSpanHeader(6, 6, "退回配件");
            dgvReport.MergeColumnNames.Add("换出配件");
            dgvReport.AddSpanHeader(12, 6, "换出配件");
            //添加打印报表合并
            List<string> listTuiHui = new List<string>();
            listTuiHui.Add("退回单位");
            listTuiHui.Add("退回数量");
            listTuiHui.Add("退回单价");
            listTuiHui.Add("退回货款");
            listTuiHui.Add("退回税额");
            listTuiHui.Add("退回金额");
            AddSpanRows("退回配件", listTuiHui);

            List<string> listHuanChu = new List<string>();
            listHuanChu.Add("换出单位");
            listHuanChu.Add("换出数量");
            listHuanChu.Add("换出单价");
            listHuanChu.Add("换出货款");
            listHuanChu.Add("换出税额");
            listHuanChu.Add("换出金额");
            AddSpanRows("换出配件", listHuanChu);
            BindData();
        }
        //选择公司，绑定公司所有部门
        private void cboCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCompany.SelectedValue == null)
            {
                return;
            }
            CommonFuncCall.BindDepartment(cboorg_id, cboCompany.SelectedValue.ToString(), "全部");
        }
        //供应商选择
        private void txtcsup_code_ChooserClick(object sender, EventArgs e)
        {
            frmSupplier frmSup = new frmSupplier();
            if (frmSup.ShowDialog() == DialogResult.OK)
            {
                txtcsup_code.Text = frmSup.supperCode;
                txtsup_name.Caption = frmSup.supperName;
            }
        }
        //配件选择
        private void txtcparts_code_ChooserClick(object sender, EventArgs e)
        {
            frmParts parts = new frmParts();
            if (parts.ShowDialog() == DialogResult.OK)
            {
                txtcparts_code.Text = parts.PartsCode;
                txtPartsName.Caption = parts.PartsName;
            }
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        void BindData()
        {
            string files = @"配件编码,配件名称,图号,配件品牌,配件类别,厂商编码,退回单位,退回单价,换出单位,换出单价,
                        sum(isnull(退回数量,0)) 退回数量,sum(isnull(退回货款,0)) 退回货款,sum(isnull(退回税额,0)) 退回税额,sum(isnull(退回金额,0)) 退回金额,
                        sum(isnull(换出数量,0)) 换出数量,sum(isnull(换出货款,0)) 换出货款,sum(isnull(换出税额,0)) 换出税额,sum(isnull(换出金额,0)) 换出金额";
            string groupBy = "group by 配件编码,配件名称,图号,配件品牌,配件类别,厂商编码,退回单位,退回单价,换出单位,换出单价";
            dt = DBHelper.GetTable("", "v_purchase_return_detail_report", files, GetWhere(), "", groupBy);
            //单据不做合计
            List<string> listNot = new List<string>();
            listNot.Add("退回单价");
            listNot.Add("换出单价");
            dt.DataTableSum(listNot);
            dgvReport.DataSource = dt;
        }
        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        private void dgvReport_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenDocument();
        }
        /// <summary>
        /// 打开采购开单明细表
        /// </summary>
        void OpenDocument()
        {
            if (dgvReport.CurrentRow == null)
            {
                return;
            }
            string partsName = CommonCtrl.IsNullToString(dgvReport.CurrentRow.Cells[colPartsName.Name].Value);
            if (partsName.Length == 0)
            {
                return;
            }
            UCPurchaseBillingDetail detail = new UCPurchaseBillingDetail();
            detail.supCode = txtcsup_code.Text;
            detail.supName = txtsup_name.Caption;
            detail.supType = CommonCtrl.IsNullToString(cbosup_type.SelectedValue);
            detail.whCode = CommonCtrl.IsNullToString(cbowh_code.SelectedValue);
            detail.partsCode = CommonCtrl.IsNullToString(dgvReport.CurrentRow.Cells[colPartsCode.Name].Value);
            detail.partsName = partsName;
            detail.drawingNum = txtdrawing_num.Caption;
            detail.partsBrand = txtparts_brand.Caption;
            detail.stratDate = dicreate_time.StartDate;
            detail.endDate = dicreate_time.EndDate;
            detail.commpany = CommonCtrl.IsNullToString(cboCompany.SelectedValue);
            detail.orgID = CommonCtrl.IsNullToString(cboorg_id.SelectedValue);
            base.addUserControl(detail, "采购开单明细表", "UCPurchaseBillingDetail", this.Tag.ToString(), this.Name);
        }
    }
}
