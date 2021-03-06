﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HXCPcClient.Chooser;
using HXCPcClient.CommonClass;
using SYSModel;
using Utility.Common;
using ServiceStationClient.ComponentUI;
using HXCPcClient.UCForm.RepairBusiness.RepairCallback;
using ServiceStationClient.ComponentUI.TextBox;

namespace HXCPcClient.UCForm.RepairBusiness.OldPartsManagement.OldPartsReceipt
{
    /// <summary>
    /// 旧件管理-旧件收货单新增编辑
    /// Author：JC
    /// AddTime：2014.10.31
    /// </summary>
    public partial class UCOldPartsReceiptAddOrEdit : UCBase
    {
        #region 属性设置
        /// <summary>
        /// 父窗体
        /// </summary>
        public UCOldPartsReceiptManager uc;
        /// <summary>
        /// 窗体状态
        /// </summary>
        public WindowStatus wStatus;
        /// <summary>
        /// 救援单ID
        /// </summary>
        public string strId = string.Empty;
        /// <summary>
        /// 操作名称
        /// </summary>
        string opName = string.Empty;
        /// <summary>
        /// 前置单据Id
        /// </summary>
        public string strBefore_orderId = string.Empty;
        /// <summary>
        /// 业务单号-导入数据时用
        /// </summary>
        string strYWOrders = string.Empty;
        /// <summary>
        /// 车牌号-导入数据时用
        /// </summary>
        string strVehicleNo = string.Empty;
        /// <summary>
        /// 单据状态
        /// </summary>
        string strStatus = string.Empty;
        /// <summary>
        /// 用于判断用料是否重复添加
        /// </summary>
        List<string> listMater = new List<string>();
        /// <summary>
        /// 是否自动关闭
        /// </summary>
        bool isAutoClose = false;
        #endregion

        #region 初始化窗体
        public UCOldPartsReceiptAddOrEdit()
        {
            InitializeComponent();
            CommonFuncCall.BindDepartment(cboOrgId, GlobalStaticObj.CurrUserCom_Id, "请选择");
            GetRepairNo();
            SetDgvAnchor();
            base.SaveEvent += new ClickHandler(UCOldPartsReceiptAddOrEdit_SaveEvent);
            base.SubmitEvent += new ClickHandler(UCOldPartsReceiptAddOrEdit_SubmitEvent);
            base.ImportEvent += new ClickHandler(UCOldPartsReceiptAddOrEdit_ImportEvent);
            base.CancelEvent += new ClickHandler(UCOldPartsReceiptAddOrEdit_CancelEvent);
            SetQuick();
        }
        #endregion

        #region 设置速查功能
        /// <summary>
        /// 设置速查功能
        /// </summary>
        private void SetQuick()
        {
            //设置客户编码速查
            txtCustomNO.SetBindTable("tb_customer", "cust_code");
            txtCustomNO.GetDataSourced += new ServiceStationClient.ComponentUI.TextBox.TextChooser.GetDataSourceHandler(tc_GetDataSourced);
            txtCustomNO.DataBacked += new TextChooser.DataBackHandler(txtCustomNO_DataBacked);

        }
        /// <summary>
        /// 客户编码速查连带信息
        /// </summary>
        /// <param name="dr"></param>
        void txtCustomNO_DataBacked(DataRow dr)
        {
            try
            {
                txtCustomNO.Text = CommonCtrl.IsNullToString(dr["cust_code"]);
                txtCustomNO.Tag = CommonCtrl.IsNullToString(dr["cust_id"]);
                txtCustomName.Caption = CommonCtrl.IsNullToString(dr["cust_name"]);
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }

        /// <summary>
        /// 设置速查
        /// </summary>
        /// <param name="sqlString"></param>
        void tc_GetDataSourced(TextChooser tc, string sqlString)
        {
            try
            {
                DataTable dvt = CommonFuncCall.GetDataSource(sqlString);
                tc.SetDataSource(dvt);
                if (dvt != null)
                {
                    tc.Search();
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }

        #endregion

        #region 取消功能
        void UCOldPartsReceiptAddOrEdit_CancelEvent(object sender, EventArgs e)
        {
            try
            {
                if (MessageBoxEx.Show("确认要取消吗?", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(strBefore_orderId))
                {
                    List<SQLObj> listSql = new List<SQLObj>();
                    UpdateMaintainInfo(listSql, strBefore_orderId, "0");
                    DBHelper.BatchExeSQLMultiByTrans("更新前置单据导入状态", listSql);
                }
                isAutoClose = true;
                deleteMenuByTag(this.Tag.ToString(), uc.Name);
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 顶部button显示设置
        /// <summary>
        ///  顶部button显示设置
        /// </summary>
        private void SetTopbuttonShow()
        {
            base.btnAdd.Visible = false;
            base.btnCopy.Visible = false;
            base.btnDelete.Visible = false;
            base.btnStatus.Visible = false;
            base.btnVerify.Visible = false;
            base.btnView.Visible = false;
            base.btnEdit.Visible = false;
            base.btnSync.Visible = false;
            base.btnConfirm.Visible = false;
            base.btnActivation.Visible = false;
        }
        #endregion

        #region 导入事件
        void UCOldPartsReceiptAddOrEdit_ImportEvent(object sender, EventArgs e)
        {
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            ShowContextMenuStrip(x, y);
        }
        /// <summary> 点击导入按钮，显示导入方式的下拉选项
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        private void ShowContextMenuStrip(int X, int Y)
        {
            contextMenuM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            contextMenuM.Show();
            contextMenuM.Location = new System.Drawing.Point(X, Y);
        }
        #endregion

        #region 提交事件
        void UCOldPartsReceiptAddOrEdit_SubmitEvent(object sender, EventArgs e)
        {
            opName = "提交领料退货单信息";
            SaveOrSubmitMethod("提交", DataSources.EnumAuditStatus.SUBMIT);
        }
        #endregion

        #region 保存事件
        void UCOldPartsReceiptAddOrEdit_SaveEvent(object sender, EventArgs e)
        {
            opName = "保存旧件收货单信息";
            SaveOrSubmitMethod("保存", DataSources.EnumAuditStatus.DRAFT);
        }
        #endregion

        #region 验证必填项
        private Boolean CheckControlValue()
        {
            try
            {
                errorProvider1.Clear();
                if (string.IsNullOrEmpty(CommonCtrl.IsNullToString(dtpReceiptTime.Value)))
                {
                    Validator.SetError(errorProvider1, dtpReceiptTime, "收货日期不能为空!");
                    return false;
                }
                if (string.IsNullOrEmpty(txtCustomNO.Text.Trim()))
                {
                    Validator.SetError(errorProvider1, txtCustomNO, "客户编码不能为空!");
                    return false;
                }
                if (string.IsNullOrEmpty(txtCustomName.Caption.Trim()))//客户名称不能为空
                {
                    Validator.SetError(errorProvider1, txtCustomName, "客户名称不能为空!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
                return false;
            }
            return true;
        }
        #endregion

        #region 保存、提交方法
        /// <summary>
        /// 保存、提交方法
        /// </summary>
        /// <param name="strMessage">提示信息</param>
        /// <param name="Estatus">单据操作状态</param>
        private void SaveOrSubmitMethod(string strMessage, DataSources.EnumAuditStatus Estatus)
        {
            try
            {
                #region 必要的判断   
                if (!CheckControlValue()) return;
                int intPartCount = 0;
                foreach (DataGridViewRow dgvr in dgvMaterials.Rows)
                {
                    string strPname = CommonCtrl.IsNullToString(dgvr.Cells["parts_name"].Value);
                    string strPCode = CommonCtrl.IsNullToString(dgvr.Cells["parts_code"].Value);
                    if (strPname.Length > 0 && strPCode.Length > 0)
                    {
                        intPartCount++;
                    }
                }
                if (intPartCount == 0)
                {
                    MessageBoxEx.Show("配件信息不能为空，请添加配件信息!", "提示", MessageBoxButtons.OK, MessageBoxIcon.None);
                    return;
                }
                #endregion
                if (MessageBoxEx.Show("确认要" + strMessage + "吗?", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                List<SQLObj> listSql = new List<SQLObj>();
                SaveOrderInfo(listSql, Estatus);
                SaveMaterialsData(listSql, strId);
                if (DBHelper.BatchExeSQLMultiByTrans(opName, listSql))
                {
                    MessageBoxEx.Show("" + strMessage + "成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.None);
                    uc.BindPageData();
                    isAutoClose = true;
                    deleteMenuByTag(this.Tag.ToString(), this.uc.Name);
                }
                else
                {
                    MessageBoxEx.Show("" + strMessage + "失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
                MessageBoxEx.Show("" + strMessage + "失败！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region 领料退货单基本信息保存
        private void SaveOrderInfo(List<SQLObj> listSql, DataSources.EnumAuditStatus status)
        {
            try
            {
                SQLObj obj = new SQLObj();
                obj.cmdType = CommandType.Text;
                Dictionary<string, ParamObj> dicParam = new Dictionary<string, ParamObj>();
                #region 基本信息
                dicParam.Add("receipt_time", new ParamObj("receipt_time", Common.LocalDateTimeToUtcLong(dtpReceiptTime.Value).ToString(), SysDbType.BigInt));//制单时间            
                dicParam.Add("customer_code", new ParamObj("customer_code", !string.IsNullOrEmpty(CommonCtrl.IsNullToString(txtCustomNO.Text)) ? txtCustomNO.Text.Trim() : null, SysDbType.VarChar, 40));//客户编码
                dicParam.Add("customer_id", new ParamObj("customer_id", !string.IsNullOrEmpty(CommonCtrl.IsNullToString(txtCustomNO.Tag)) ? txtCustomNO.Tag.ToString() : null, SysDbType.VarChar, 40));//客户Id
                dicParam.Add("customer_name", new ParamObj("customer_name", txtCustomName.Caption.Trim(), SysDbType.VarChar, 20));//客户名称
                dicParam.Add("linkman", new ParamObj("linkman", txtContact.Caption.Trim(), SysDbType.VarChar, 20));//联系人
                if (!string.IsNullOrEmpty(txtContactPhone.Caption.Trim()))//联系人手机
                {
                    dicParam.Add("link_man_mobile", new ParamObj("link_man_mobile", txtContactPhone.Caption.Trim(), SysDbType.VarChar, 15));//联系人手机 
                }
                else
                {
                    dicParam.Add("link_man_mobile", new ParamObj("link_man_mobile", null, SysDbType.VarChar, 15));//联系人手机 
                }
                dicParam.Add("remarks", new ParamObj("remarks", txtRemark.Caption.Trim(), SysDbType.VarChar, 200));//备注
                dicParam.Add("Import_status", new ParamObj("Import_status", Convert.ToInt32(DataSources.EnumImportStaus.OPEN).ToString(), SysDbType.VarChar, 1));  
                #endregion

                //经办人id
                dicParam.Add("responsible_opid", new ParamObj("responsible_opid", !string.IsNullOrEmpty(CommonCtrl.IsNullToString(cobYHandle.SelectedValue)) ? cobYHandle.SelectedValue.ToString() : null, SysDbType.VarChar, 40));
                //经办人
                dicParam.Add("responsible_name", new ParamObj("responsible_name", !string.IsNullOrEmpty(CommonCtrl.IsNullToString(cobYHandle.SelectedValue)) ? cobYHandle.SelectedValue : null, SysDbType.VarChar, 40));
                //部门
                dicParam.Add("org_name", new ParamObj("org_name", !string.IsNullOrEmpty(CommonCtrl.IsNullToString(cboOrgId.SelectedValue)) ? cboOrgId.SelectedValue : null, SysDbType.VarChar, 40));
                dicParam.Add("enable_flag", new ParamObj("enable_flag", Convert.ToInt32(DataSources.EnumEnableFlag.USING).ToString(), SysDbType.VarChar, 1));//信息状态（1|激活；2|作废；0|删除）           
                //单据类型
                dicParam.Add("receipt_type", new ParamObj("receipt_type", "1", SysDbType.VarChar, 2));
                if (status == DataSources.EnumAuditStatus.SUBMIT)//提交操作时生成单号
                {
                    dicParam.Add("receipts_no", new ParamObj("receipts_no", labReceiptNoS.Text.Trim(), SysDbType.VarChar, 40));//收货单号 
                    //单据状态
                    dicParam.Add("info_status", new ParamObj("info_status", Convert.ToInt32(status).ToString(), SysDbType.VarChar, 40));
                    //更新前置单据导入状态
                    if (!string.IsNullOrEmpty(strBefore_orderId))
                    {
                        UpdateMaintainInfo(listSql, strBefore_orderId, "2");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(strStatus) && strStatus == Convert.ToInt32(DataSources.EnumAuditStatus.NOTAUDIT).ToString())
                    {
                        dicParam.Add("receipts_no", new ParamObj("receipts_no", labReceiptNoS.Text.Trim(), SysDbType.VarChar, 40));//领料退货单号 
                        //单据状态
                        dicParam.Add("info_status", new ParamObj("info_status", Convert.ToInt32(status).ToString(), SysDbType.VarChar, 40));
                    }
                    else
                    {
                        //单据状态
                        dicParam.Add("info_status", new ParamObj("info_status", Convert.ToInt32(status).ToString(), SysDbType.VarChar, 40));
                        dicParam.Add("receipts_no", new ParamObj("receipts_no", null, SysDbType.VarChar, 40));//旧件收货单号
                    }
                    //更新前置单据导入状态
                    if (!string.IsNullOrEmpty(strBefore_orderId))
                    {
                        UpdateMaintainInfo(listSql, strBefore_orderId, "0");
                    }
                }
                if (wStatus == WindowStatus.Add || wStatus == WindowStatus.Copy)
                {
                    strId = Guid.NewGuid().ToString();
                    dicParam.Add("oldpart_id", new ParamObj("oldpart_id", strId, SysDbType.VarChar, 40));//Id
                    dicParam.Add("create_by", new ParamObj("create_by", HXCPcClient.GlobalStaticObj.UserID, SysDbType.VarChar, 40));//创建人id（制单人） 
                    dicParam.Add("create_name", new ParamObj("create_name", HXCPcClient.GlobalStaticObj.UserName, SysDbType.VarChar, 40));//创建人
                    dicParam.Add("create_time", new ParamObj("create_time", Common.LocalDateTimeToUtcLong(HXCPcClient.GlobalStaticObj.CurrentDateTime).ToString(), SysDbType.BigInt));
                    obj.sqlString = @"insert into tb_maintain_oldpart_receiv_send (receipt_time,customer_code,customer_id,customer_name,linkman,link_man_mobile,remarks,Import_status,responsible_opid
                 ,responsible_name,org_name,enable_flag,info_status,receipt_type,receipts_no,oldpart_id,create_by,create_name,create_time)
                 values (@receipt_time,@customer_code,@customer_id,@customer_name,@linkman,@link_man_mobile,@remarks,@Import_status,@responsible_opid
                 ,@responsible_name,@org_name,@enable_flag,@info_status,@receipt_type,@receipts_no,@oldpart_id,@create_by,@create_name,@create_time);";
                }
                else if (wStatus == WindowStatus.Edit)
                {
                    dicParam.Add("oldpart_id", new ParamObj("oldpart_id", strId, SysDbType.VarChar, 40));//Id
                    dicParam.Add("update_by", new ParamObj("update_by", HXCPcClient.GlobalStaticObj.UserID, SysDbType.VarChar, 40));//修改人Id
                    dicParam.Add("update_name", new ParamObj("update_name", HXCPcClient.GlobalStaticObj.UserName, SysDbType.VarChar, 40));//修改人姓名
                    dicParam.Add("update_time", new ParamObj("update_time", Common.LocalDateTimeToUtcLong(HXCPcClient.GlobalStaticObj.CurrentDateTime).ToString(), SysDbType.BigInt));//修改时间               
                    obj.sqlString = @"update tb_maintain_oldpart_receiv_send set receipt_time=@receipt_time,customer_code=@customer_code,customer_id=@customer_id,customer_name=@customer_name,linkman=@linkman,link_man_mobile=@link_man_mobile,remarks=@remarks,Import_status=@Import_status,responsible_opid=@responsible_opid
                 ,responsible_name=@responsible_name,org_name=@org_name,enable_flag=@enable_flag,info_status=@info_status,receipt_type=@receipt_type,receipts_no=@receipts_no,update_by=@update_by,update_name=@update_name,update_time=@update_time
                where oldpart_id=@oldpart_id";
                }
                obj.Param = dicParam;
                listSql.Add(obj);
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 配件信息保存
        private void SaveMaterialsData(List<SQLObj> listSql, string partID)
        {
            try
            {
                foreach (DataGridViewRow dgvr in dgvMaterials.Rows)
                {
                    string strPname = CommonCtrl.IsNullToString(dgvr.Cells["parts_name"].Value);
                    string strPCmoney = CommonCtrl.IsNullToString(dgvr.Cells["parts_code"].Value);
                    if (strPname.Length > 0 && strPCmoney.Length > 0)
                    {
                        SQLObj obj = new SQLObj();
                        obj.cmdType = CommandType.Text;
                        Dictionary<string, ParamObj> dicParam = new Dictionary<string, ParamObj>();
                        dicParam.Add("maintain_id", new ParamObj("maintain_id", partID, SysDbType.VarChar, 40));
                        dicParam.Add("parts_code", new ParamObj("parts_code", dgvr.Cells["parts_code"].Value, SysDbType.VarChar, 40));
                        dicParam.Add("parts_name", new ParamObj("parts_name", dgvr.Cells["parts_name"].Value, SysDbType.VarChar, 40));
                        dicParam.Add("orders_no", new ParamObj("orders_no", dgvr.Cells["orders_no"].Value, SysDbType.VarChar, 40));
                        dicParam.Add("vehicle_no", new ParamObj("vehicle_no", dgvr.Cells["vehicle_no"].Value, SysDbType.VarChar, 40));
                        dicParam.Add("unit", new ParamObj("unit", dgvr.Cells["unit"].Value, SysDbType.VarChar, 20));
                        dicParam.Add("drawn_no", new ParamObj("drawn_no", dgvr.Cells["drawn_no"].Value, SysDbType.VarChar, 40));
                        dicParam.Add("quantity", new ParamObj("quantity", !string.IsNullOrEmpty(CommonCtrl.IsNullToString(dgvr.Cells["quantity"].Value)) ? dgvr.Cells["quantity"].Value : null, SysDbType.Decimal, 15));
                        dicParam.Add("unit_price", new ParamObj("unit_price", !string.IsNullOrEmpty(CommonCtrl.IsNullToString(dgvr.Cells["unit_price"].Value)) ? dgvr.Cells["unit_price"].Value : null, SysDbType.Decimal, 15));
                        dicParam.Add("sum_money", new ParamObj("sum_money", !string.IsNullOrEmpty(CommonCtrl.IsNullToString(dgvr.Cells["sum_money"].Value)) ? dgvr.Cells["sum_money"].Value : null, SysDbType.Decimal, 15));
                        string strIsimport = CommonCtrl.IsNullToString(dgvr.Cells["whether_imported"].Value);
                        if (!string.IsNullOrEmpty(strIsimport))
                        {
                            dicParam.Add("whether_imported", new ParamObj("whether_imported", strIsimport == "是" ? "1" : "0", SysDbType.VarChar, 1));
                        }
                        else
                        {
                            dicParam.Add("whether_imported", new ParamObj("whether_imported", null, SysDbType.VarChar, 1));
                        }
                        dicParam.Add("vehicle_brand", new ParamObj("vehicle_brand", dgvr.Cells["vehicle_brand"].Value, SysDbType.VarChar, 40));
                        dicParam.Add("remarks", new ParamObj("remarks", dgvr.Cells["remarks"].Value, SysDbType.VarChar, 200));
                        dicParam.Add("enable_flag", new ParamObj("enable_flag", "1", SysDbType.VarChar, 1));
                        string strPID = CommonCtrl.IsNullToString(dgvr.Cells["parts_id"].Value);
                        if (strPID.Length == 0)
                        {
                            opName = "新增旧件收货单";
                            strPID = Guid.NewGuid().ToString();
                            dicParam.Add("parts_id", new ParamObj("parts_id", strPID, SysDbType.VarChar, 40));
                            obj.sqlString = @"insert into tb_maintain_oldpart_material_detail (maintain_id,parts_code,parts_name,orders_no,vehicle_no,unit,drawn_no,quantity,unit_price
                         ,sum_money,whether_imported,vehicle_brand,remarks,enable_flag,parts_id)
                         values (@maintain_id,@parts_code,@parts_name,@orders_no,@vehicle_no,@unit,@drawn_no,@quantity,@unit_price
                         ,@sum_money,@whether_imported,@vehicle_brand,@remarks,@enable_flag,@parts_id);";
                        }
                        else
                        {
                            dicParam.Add("parts_id", new ParamObj("parts_id", dgvr.Cells["parts_id"].Value, SysDbType.VarChar, 40));
                            opName = "更新领料退货单";
                            obj.sqlString = @"update tb_maintain_oldpart_material_detail set maintain_id=@maintain_id,parts_code=@parts_code,parts_name=@parts_name,orders_no=@orders_no,vehicle_no=@vehicle_no,unit=@unit,drawn_no=@drawn_no,quantity=@quantity,unit_price=@unit_price
                         ,sum_money=@sum_money,whether_imported=@whether_imported,vehicle_brand=@vehicle_brand,remarks=@remarks,enable_flag=@enable_flag
                        where parts_id=@parts_id";
                        }
                        obj.Param = dicParam;
                        listSql.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 客户编码选择器事件
        /// <summary>
        /// 客户编码选择器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCustomNO_ChooserClick(object sender, EventArgs e)
        {
            try
            {
                frmCustomerInfo frmCInfo = new frmCustomerInfo();
                DialogResult result = frmCInfo.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtCustomNO.Text = frmCInfo.strCustomerNo;
                    txtCustomNO.Tag = frmCInfo.strCustomerId;
                    txtCustomName.Caption = frmCInfo.strCustomerName;
                    txtContact.Caption = frmCInfo.strLegalPerson;
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 生成旧件收货单号创建人姓名
        /// <summary>
        /// 生成旧件收货单号创建人姓名
        /// </summary>
        private void GetRepairNo()
        {
            labReceiptNoS.Text = CommonUtility.GetNewNo(SYSModel.DataSources.EnumProjectType.PartsReceipt);
            labCreatePersonS.Text = HXCPcClient.GlobalStaticObj.UserName;
        }
        #endregion

        #region 根据部门的选择绑定经办人
        /// <summary>
        /// 根据部门的选择绑定经办人
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboOrgId_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboOrgId.SelectedValue == null)
                {
                    return;
                }
                CommonFuncCall.BindHandle(cobYHandle, cboOrgId.SelectedValue.ToString(), "请选择");
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 窗体Load事件
        private void UCOldPartsReceiptAddOrEdit_Load(object sender, EventArgs e)
        {

            SetTopbuttonShow();
            dtpReceiptTime.Value = DateTime.Now;
            // base.SetBtnStatus(wStatus);
            if (wStatus == WindowStatus.Edit || wStatus == WindowStatus.Copy)
            {
                BindData();
            }
            else if (wStatus == WindowStatus.Add)
            {
                labReceiptNoS.Visible = false;
            }
        }
        #endregion

        #region 根据旧件收货单ID获取信息编辑用
        /// <summary>
        /// 根据旧件收货单ID获取信息，编辑用
        /// </summary>
        private void BindData()
        {
            try
            {
                #region 基础信息
                string strWhere = string.Format("oldpart_id='{0}'", strId);
                DataTable dt = DBHelper.GetTable("查询旧件收货货单", "tb_maintain_oldpart_receiv_send ", "*", strWhere, "", "");
                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }
                DataRow dr = dt.Rows[0];

                string strCtime = CommonCtrl.IsNullToString(dr["receipt_time"]);
                if (!string.IsNullOrEmpty(strCtime))
                {
                    long Rticks = Convert.ToInt64(strCtime);
                    dtpReceiptTime.Value = Common.UtcLongToLocalDateTime(Rticks);//收货日期
                }

                #region 客户信息
                txtCustomNO.Text = CommonCtrl.IsNullToString(dr["customer_code"]);//客户编码
                txtCustomName.Caption = CommonCtrl.IsNullToString(dr["customer_name"]);//客户名称  
                txtCustomNO.Tag = CommonCtrl.IsNullToString(dr["customer_id"]);//客户关联id  
                txtContact.Caption = CommonCtrl.IsNullToString(dr["linkman"]);//联系人
                txtContactPhone.Caption = CommonCtrl.IsNullToString(dr["link_man_mobile"]);//联系人手机
                txtRemark.Caption = CommonCtrl.IsNullToString(dr["remarks"]);//备注
                #endregion
                #endregion

                #region 配件信息
                DataTable dmt = DBHelper.GetTable("旧件收货明细数据", "tb_maintain_oldpart_material_detail", "*", string.Format(" maintain_id='{0}' and enable_flag='" + Convert.ToInt32(DataSources.EnumEnableFlag.USING).ToString() + "'", strId), "", "");
                if (dmt.Rows.Count > 0)
                {
                    if (dmt.Rows.Count >= dgvMaterials.Rows.Count)
                    {
                        dgvMaterials.Rows.Add(dmt.Rows.Count - dgvMaterials.Rows.Count + 2);
                    }
                    else if ((dgvMaterials.Rows.Count - dmt.Rows.Count) == 1)
                    {
                        dgvMaterials.Rows.Add(1);
                    }
                    for (int i = 0; i < dmt.Rows.Count; i++)
                    {
                        DataRow dmr = dmt.Rows[i];
                        dgvMaterials.Rows[i].Cells["parts_id"].Value = CommonCtrl.IsNullToString(dmr["parts_id"]);
                        dgvMaterials.Rows[i].Cells["parts_code"].Value = CommonCtrl.IsNullToString(dmr["parts_code"]);
                        dgvMaterials.Rows[i].Cells["parts_name"].Value = CommonCtrl.IsNullToString(dmr["parts_name"]);
                        dgvMaterials.Rows[i].Cells["orders_no"].Value = CommonCtrl.IsNullToString(dmr["orders_no"]);
                        dgvMaterials.Rows[i].Cells["vehicle_no"].Value = CommonCtrl.IsNullToString(dmr["vehicle_no"]);
                        dgvMaterials.Rows[i].Cells["unit"].Value = CommonCtrl.IsNullToString(dmr["unit"]);
                        dgvMaterials.Rows[i].Cells["drawn_no"].Value = CommonCtrl.IsNullToString(dmr["drawn_no"]);
                        dgvMaterials.Rows[i].Cells["quantity"].Value = CommonCtrl.IsNullToString(dmr["quantity"]);
                        dgvMaterials.Rows[i].Cells["unit_price"].Value = CommonCtrl.IsNullToString(dmr["unit_price"]);
                        dgvMaterials.Rows[i].Cells["sum_money"].Value = CommonCtrl.IsNullToString(dmr["sum_money"]);
                        dgvMaterials.Rows[i].Cells["whether_imported"].Value = CommonCtrl.IsNullToString(dmr["whether_imported"]) == "1" ? "是" : "否";
                        dgvMaterials.Rows[i].Cells["vehicle_brand"].Value = CommonCtrl.IsNullToString(dmr["vehicle_brand"]);
                        dgvMaterials.Rows[i].Cells["remarks"].Value = CommonCtrl.IsNullToString(dmr["remarks"]);
                        listMater.Add(CommonCtrl.IsNullToString(dmr["parts_code"]));

                    }
                }
                #endregion

                #region 顶部按钮设置
                if (wStatus == WindowStatus.Edit)
                {
                    if (!string.IsNullOrEmpty(CommonCtrl.IsNullToString(dr["receipts_no"])))
                    {
                        labReceiptNoS.Text = CommonCtrl.IsNullToString(dr["receipts_no"]);//旧件收货单号
                    }
                    else
                    {
                        labReceiptNoS.Visible = false;
                    }
                    strStatus = CommonCtrl.IsNullToString(dr["info_status"]);//单据状态
                    if (strStatus == Convert.ToInt32(DataSources.EnumAuditStatus.SUBMIT).ToString())
                    {
                        //已提交状态屏蔽提交按钮
                        base.btnSubmit.Enabled = false;
                    }
                    else if (strStatus == Convert.ToInt32(DataSources.EnumAuditStatus.AUDIT).ToString())
                    {
                        //已审核时屏蔽提交、审核按钮
                        base.btnSubmit.Enabled = false;
                        base.btnVerify.Enabled = false;
                    }
                    //else if (strStatus == Convert.ToInt32(DataSources.EnumAuditStatus.NOTAUDIT).ToString())
                    //{
                    //    //审核没通过时屏蔽提交按钮
                    //    base.btnSubmit.Enabled = false;
                    //}
                }
                else
                {
                    labReceiptNoS.Visible = false;
                }
                #endregion
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 重写是否进口、车型的值
        private void dgvMaterials_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.Value == null)
                {
                    return;
                }
                string fieldNmae = dgvMaterials.Columns[e.ColumnIndex].DataPropertyName;
                if (fieldNmae.Equals("whether_imported"))
                {
                    e.Value = !string.IsNullOrEmpty(CommonCtrl.IsNullToString(e.Value)) ? e.Value.ToString() == "1" ? "是" : "否" : "";
                }
                if (fieldNmae.Equals("vehicle_brand"))
                {
                    e.Value = !string.IsNullOrEmpty(CommonCtrl.IsNullToString(e.Value)) ? GetDicName(e.Value.ToString()) : "";
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }

        }
        #endregion

        #region 根据码表ID获取其对应的名称
        /// <summary>
        /// 根据码表ID获取其对应的名称
        /// </summary>
        /// <param name="strId">码表Id值</param>
        private string GetDicName(string strId)
        {
            return DBHelper.GetSingleValue("获取码表值", "sys_dictionaries", "dic_name", "dic_id='" + strId + "'", "");
        }
        #endregion

        #region 设置底部DataGridView的Anchor属性
        /// <summary>
        /// 设置底部DataGridView的Anchor属性
        /// </summary>
        private void SetDgvAnchor()
        {
            #region 配件设置
            dgvMaterials.Dock = DockStyle.Fill;
            //dgvMaterials.Columns["MCheck"].HeaderText = "选择";
            dgvMaterials.ReadOnly = false;
            dgvMaterials.Rows.Add(3);
            dgvMaterials.AllowUserToAddRows = true;
            dgvMaterials.Columns["parts_code"].ReadOnly = true;
            dgvMaterials.Columns["parts_name"].ReadOnly = true;
            dgvMaterials.Columns["orders_no"].ReadOnly = true;
            dgvMaterials.Columns["vehicle_no"].ReadOnly = true;
            dgvMaterials.Columns["drawn_no"].ReadOnly = true;
            dgvMaterials.Columns["unit"].ReadOnly = true;
            dgvMaterials.Columns["sum_money"].ReadOnly = true;
            dgvMaterials.Columns["whether_imported"].ReadOnly = true;
            dgvMaterials.Columns["vehicle_brand"].ReadOnly = true;
            dgvMaterials.Columns["parts_id"].ReadOnly = true;
            ControlsConfig.NumberLimitdgv(dgvMaterials, new List<string>() { "quantity", "unit_price" });

            string[] strMArrary = { "quantity", "sum_money" };
            ControlsConfig.DatagGridViewTotalConfig(dgvMaterials, strMArrary);
            #endregion
        }
        #endregion

        #region 维修单导入功能
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(strBefore_orderId))
                {
                    List<SQLObj> listSql = new List<SQLObj>();
                    UpdateMaintainInfo(listSql, strBefore_orderId, "0");
                    DBHelper.BatchExeSQLMultiByTrans("更新前置单据导入状态", listSql);
                }
                UCRepairCallbackImport CallbackImport = new UCRepairCallbackImport();
                CallbackImport.PatrsRuc = this;
                CallbackImport.strTag = "2";
                CallbackImport.ShowDialog();
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 维修返修导入-根据前置单据绑定维修信息
        /// <summary>
        /// 维修返修导入-根据前置单据绑定维修信息
        /// </summary>
        public void BindSetmentData()
        {
            try
            {
                DataTable dt = DBHelper.GetTable("查询维修单单", "tb_maintain_info", "*", " maintain_id ='" + strBefore_orderId + "'", "", "");
                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }
                DataRow dr = dt.Rows[0];
                txtCustomNO.Text = CommonCtrl.IsNullToString(dr["customer_code"]);//客户编码
                txtCustomName.Caption = CommonCtrl.IsNullToString(dr["customer_name"]);//客户名称  
                txtCustomNO.Tag = CommonCtrl.IsNullToString(dr["customer_id"]);//客户关联id
                txtContact.Caption = CommonCtrl.IsNullToString(dr["linkman"]);//联系人
                txtContactPhone.Caption = CommonCtrl.IsNullToString(dr["link_man_mobile"]);//联系人手机   

                strYWOrders = CommonCtrl.IsNullToString(dr["maintain_no"]);//单号  
                strVehicleNo = CommonCtrl.IsNullToString(dr["vehicle_no"]);//车牌号  
                BindMData(strBefore_orderId);
                List<SQLObj> listSql = new List<SQLObj>();
                UpdateMaintainInfo(listSql, strBefore_orderId, "1");
                DBHelper.BatchExeSQLMultiByTrans("更新前置单据导入状态", listSql);
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 配件数据
        private void BindMData(string strOrderId)
        {
            try
            {
                //配件数据
                DataTable dmt = DBHelper.GetTable("配件数据", "tb_maintain_material_detail", "*", string.Format(" maintain_id='{0}'", strOrderId), "", "");
                if (dmt.Rows.Count > 0)
                {
                    if (dmt.Rows.Count > dgvMaterials.Rows.Count)
                    {
                        dgvMaterials.Rows.Add(dmt.Rows.Count - dgvMaterials.Rows.Count + 1);
                    }
                    for (int i = 0; i < dmt.Rows.Count; i++)
                    {
                        DataRow dmr = dmt.Rows[i];
                        dgvMaterials.Rows[i].Cells["parts_code"].Value = CommonCtrl.IsNullToString(dmr["parts_code"]);
                        dgvMaterials.Rows[i].Cells["parts_name"].Value = CommonCtrl.IsNullToString(dmr["parts_name"]);
                        dgvMaterials.Rows[i].Cells["orders_no"].Value = strYWOrders;
                        dgvMaterials.Rows[i].Cells["vehicle_no"].Value = strVehicleNo;
                        dgvMaterials.Rows[i].Cells["unit"].Value = CommonCtrl.IsNullToString(dmr["unit"]);
                        dgvMaterials.Rows[i].Cells["drawn_no"].Value = CommonCtrl.IsNullToString(dmr["drawn_no"]);
                        dgvMaterials.Rows[i].Cells["quantity"].Value = CommonCtrl.IsNullToString(dmr["quantity"]);
                        dgvMaterials.Rows[i].Cells["unit_price"].Value = CommonCtrl.IsNullToString(dmr["unit_price"]);
                        dgvMaterials.Rows[i].Cells["sum_money"].Value = CommonCtrl.IsNullToString(dmr["sum_money"]);
                        dgvMaterials.Rows[i].Cells["whether_imported"].Value = CommonCtrl.IsNullToString(dmr["whether_imported"]) == "1" ? "是" : "否";
                        dgvMaterials.Rows[i].Cells["vehicle_brand"].Value = CommonCtrl.IsNullToString(dmr["vehicle_brand"]);
                        dgvMaterials.Rows[i].Cells["remarks"].Value = CommonCtrl.IsNullToString(dmr["remarks"]);
                    }
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 导入后更新维修单导入状态
        /// <summary>
        /// 导入后更新维修单导入状态
        /// </summary>
        /// <param name="strReservId">维修单Id</param>
        /// <param name="status">操作状体，0保存开放、1导入占用、2提交审核锁定</param>
        private void UpdateMaintainInfo(List<SQLObj> listSql, string strReservId, string status)
        {
            try
            {
                SQLObj obj = new SQLObj();
                obj.cmdType = CommandType.Text;
                Dictionary<string, ParamObj> dicParam = new Dictionary<string, ParamObj>();

                dicParam.Add("maintain_id", new ParamObj("maintain_id", strReservId, SysDbType.VarChar, 40));
                if (status == "0")
                {
                    //保存时，前置单据被释放               
                    dicParam.Add("Import_status", new ParamObj("Import_status", "0", SysDbType.VarChar, 40));//开放
                    obj.sqlString = "update tb_maintain_info set Import_status=@Import_status where maintain_id=@maintain_id";

                }
                else if (status == "1")
                {
                    //导入时，前置单据被占用 
                    dicParam.Add("Import_status", new ParamObj("Import_status", "1", SysDbType.VarChar, 40));//占用
                    obj.sqlString = "update tb_maintain_info set Import_status=@Import_status where maintain_id=@maintain_id";
                }
                else if (status == "2")
                {
                    //审核提交时，前置单据被锁定
                    dicParam.Add("Import_status", new ParamObj("Import_status", "2", SysDbType.VarChar, 40));//锁定
                    obj.sqlString = "update tb_maintain_info set Import_status=@Import_status where maintain_id=@maintain_id";

                }
                obj.Param = dicParam;
                listSql.Add(obj);
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 单元格双击读取配件信息
        private void dgvMaterials_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int intMrows = e.RowIndex;//当前行的索引
                frmParts frmPart = new frmParts();
                DialogResult result = frmPart.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string strPId = frmPart.PartsID;
                    DataTable dpt = DBHelper.GetTable("", "v_parts_chooser", "*", " parts_id='" + strPId + "'", "", "");
                    if (dpt.Rows.Count > 0)
                    {
                        DataRow dpr = dpt.Rows[0];
                        if (listMater.Contains(CommonCtrl.IsNullToString(dpr["ser_parts_code"])))
                        {
                            if (MessageBoxEx.Show("已添加此配件,是否继续添加?", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                            {
                                return;
                            }
                        }
                        for (int i = 0; i <= intMrows; i++)
                        {
                            if (string.IsNullOrEmpty(CommonCtrl.IsNullToString(dgvMaterials.Rows[i].Cells["parts_code"].Value)))
                            {
                                dgvMaterials.Rows[i].Cells["parts_code"].Value = CommonCtrl.IsNullToString(dpr["ser_parts_code"]);
                                dgvMaterials.Rows[i].Cells["parts_name"].Value = CommonCtrl.IsNullToString(dpr["parts_name"]);
                                dgvMaterials.Rows[i].Cells["unit"].Value = CommonCtrl.IsNullToString(dpr["unit"]);
                                dgvMaterials.Rows[i].Cells["drawn_no"].Value = CommonCtrl.IsNullToString(dpr["drawing_num"]);
                                dgvMaterials.Rows[i].Cells["quantity"].Value = "0";
                                dgvMaterials.Rows[i].Cells["unit_price"].Value =!string.IsNullOrEmpty(CommonCtrl.IsNullToString(dpr["ref_out_price"]))?CommonCtrl.IsNullToString(dpr["ref_out_price"]):"0";
                                dgvMaterials.Rows[i].Cells["whether_imported"].Value = CommonCtrl.IsNullToString(dpr["is_import"]) == "1" ? "是" : "否";
                                dgvMaterials.Rows[i].Cells["vehicle_brand"].Value = CommonFuncCall.GetCarTypeForMa((CommonCtrl.IsNullToString(dpr["ser_parts_code"])));
                                dgvMaterials.Rows.Add(1);
                                break;
                            }
                        }
                    }
                }
                foreach (DataGridViewRow dgvr in dgvMaterials.Rows)
                {
                    string strPCode = CommonCtrl.IsNullToString(dgvr.Cells["parts_code"].Value);
                    if (!string.IsNullOrEmpty(strPCode)&&!listMater.Contains(strPCode))
                    {
                        listMater.Add(strPCode);
                    }
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }

        private void dgvMaterials_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(CommonCtrl.IsNullToString(dgvMaterials.Rows[e.RowIndex].Cells["parts_code"].Value)) && !string.IsNullOrEmpty(CommonCtrl.IsNullToString(dgvMaterials.Rows[e.RowIndex].Cells["parts_name"].Value)))
                {
                    if (e.ColumnIndex == 8 || e.ColumnIndex == 7)
                    {
                        ControlsConfig.SetCellsValue(dgvMaterials, e.RowIndex, "quantity,unit_price");
                        string strNum = !string.IsNullOrEmpty(CommonCtrl.IsNullToString(dgvMaterials.Rows[e.RowIndex].Cells["quantity"].Value)) ? dgvMaterials.Rows[e.RowIndex].Cells["quantity"].Value.ToString() : "0";
                        string strUnitPrice = !string.IsNullOrEmpty(CommonCtrl.IsNullToString(dgvMaterials.Rows[e.RowIndex].Cells["unit_price"].Value)) ? dgvMaterials.Rows[e.RowIndex].Cells["unit_price"].Value.ToString() : "0";
                        dgvMaterials.Rows[e.RowIndex].Cells["sum_money"].Value = Convert.ToString(Convert.ToDecimal(strNum = strNum == "" ? "0" : strNum) * Convert.ToDecimal(Convert.ToDecimal(strUnitPrice = strUnitPrice == "" ? "0" : strUnitPrice)));
                    }
                    dgvMaterials.Rows[e.RowIndex].Cells["quantity"].Value = ControlsConfig.SetNewValue(dgvMaterials.Rows[e.RowIndex].Cells["quantity"].Value, 1);
                    dgvMaterials.Rows[e.RowIndex].Cells["unit_price"].Value = ControlsConfig.SetNewValue(dgvMaterials.Rows[e.RowIndex].Cells["unit_price"].Value, 2);
                    dgvMaterials.Rows[e.RowIndex].Cells["sum_money"].Value = ControlsConfig.SetNewValue(dgvMaterials.Rows[e.RowIndex].Cells["sum_money"].Value, 2);
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }

        }
        #endregion

        #region 右键添加配件
        private void addParts_Click(object sender, EventArgs e)
        {
            try
            {
                int intMrows = dgvMaterials.CurrentRow.Index;//当前行的索引
                frmParts frmPart = new frmParts();
                DialogResult result = frmPart.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string strPId = frmPart.PartsID;
                    DataTable dpt = DBHelper.GetTable("", "v_parts_chooser", "*", " parts_id='" + strPId + "'", "", "");
                    if (dpt.Rows.Count > 0)
                    {
                        DataRow dpr = dpt.Rows[0];
                        if (listMater.Contains(CommonCtrl.IsNullToString(dpr["ser_parts_code"])))
                        {
                            if (MessageBoxEx.Show("已添加此配件,是否继续添加?", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                            {
                                return;
                            }
                        }
                        for (int i = 0; i <= intMrows; i++)
                        {
                            if (string.IsNullOrEmpty(CommonCtrl.IsNullToString(dgvMaterials.Rows[i].Cells["parts_code"].Value)))
                            {
                                dgvMaterials.Rows[i].Cells["parts_code"].Value = CommonCtrl.IsNullToString(dpr["ser_parts_code"]);
                                dgvMaterials.Rows[i].Cells["parts_name"].Value = CommonCtrl.IsNullToString(dpr["parts_name"]);
                                dgvMaterials.Rows[i].Cells["unit"].Value = CommonCtrl.IsNullToString(dpr["unit"]);
                                dgvMaterials.Rows[i].Cells["drawn_no"].Value = CommonCtrl.IsNullToString(dpr["drawing_num"]);
                                dgvMaterials.Rows[i].Cells["quantity"].Value = "0";
                                dgvMaterials.Rows[i].Cells["unit_price"].Value =!string.IsNullOrEmpty(CommonCtrl.IsNullToString(dpr["ref_out_price"]))?CommonCtrl.IsNullToString(dpr["ref_out_price"]):"0";
                                dgvMaterials.Rows[i].Cells["whether_imported"].Value = CommonCtrl.IsNullToString(dpr["is_import"]) == "1" ? "是" : "否";
                                dgvMaterials.Rows[i].Cells["vehicle_brand"].Value = CommonFuncCall.GetCarTypeForMa((CommonCtrl.IsNullToString(dpr["ser_parts_code"])));
                                dgvMaterials.Rows.Add(1);
                                break;
                            }
                        }
                    }
                }
                foreach (DataGridViewRow dgvr in dgvMaterials.Rows)
                {
                    string strPCode = CommonCtrl.IsNullToString(dgvr.Cells["parts_code"].Value);
                    if (!string.IsNullOrEmpty(strPCode) && !listMater.Contains(strPCode))
                    {
                        listMater.Add(strPCode);
                    }
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 右键删除配件
        private void deleteParts_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < dgvMaterials.RowCount; i++)
                {
                    DataGridViewRow dgvr = dgvMaterials.Rows[i];
                    object isCheck = dgvr.Cells["Mcheck"].Value;
                    //将选中的删除
                    if (isCheck != null && (bool)isCheck)
                    {
                        string strItemId = CommonCtrl.IsNullToString(dgvMaterials.Rows[i].Cells["material_id"].Value);
                        string strPartCode = CommonCtrl.IsNullToString(dgvMaterials.Rows[i].Cells["parts_code"].Value);
                        if (!string.IsNullOrEmpty(strItemId))
                        {
                            List<string> listField = new List<string>();
                            Dictionary<string, string> comField = new Dictionary<string, string>();
                            listField.Add(strItemId);
                            comField.Add("enable_flag", Convert.ToInt32(DataSources.EnumEnableFlag.DELETED).ToString());
                            DBHelper.BatchUpdateDataByIn("删除维修用料", "tb_maintain_material_detail", comField, "material_id", listField.ToArray());
                            dgvMaterials.Rows.RemoveAt(i--);
                        }
                        else
                        {
                            dgvMaterials.Rows.RemoveAt(i--);
                        }
                        listMater.Remove(strPartCode);
                    }
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion

        #region 限制手机号码只能数据数字
        private void txtContactPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            ValidationRegex.KeyPressInt(sender, e);
        }
        #endregion

        #region 点击页签上的叉号提示是否关闭当前窗体
        /// <summary>
        /// 点击页签上的叉号提示是否关闭当前窗体
        /// </summary>
        /// <returns></returns>
        public override bool CloseMenu()
        {
            if (isAutoClose)
            {
                return true;
            }
            if (MessageBoxEx.Show("确定要关闭当前窗体吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(strBefore_orderId))
                {
                    List<SQLObj> listSql = new List<SQLObj>();
                    UpdateMaintainInfo(listSql, strBefore_orderId, "0");
                    DBHelper.BatchExeSQLMultiByTrans("更新前置单据状体", listSql);
                }
            }
            return true;
        }
        #endregion

        #region 右键编辑配件
        private void eidtParts_Click(object sender, EventArgs e)
        {
            try
            {
                int intMrows = dgvMaterials.CurrentRow.Index;
                if (intMrows >= 0)
                {
                    string strPcode = CommonCtrl.IsNullToString(dgvMaterials.Rows[intMrows].Cells["parts_code"].Value);
                    if (!string.IsNullOrEmpty(strPcode))
                    {
                        frmParts frmPart = new frmParts();
                        DialogResult result = frmPart.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            string strPId = frmPart.PartsID;
                            DataTable dpt = DBHelper.GetTable("", "v_parts_chooser", "*", " parts_id='" + strPId + "'", "", "");
                            if (dpt.Rows.Count > 0)
                            {
                                DataRow dpr = dpt.Rows[0];
                                if (listMater.Contains(CommonCtrl.IsNullToString(dpr["ser_parts_code"])))
                                {
                                    if (MessageBoxEx.Show("已添加此配件,是否继续添加?", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                                    {
                                        return;
                                    }
                                }
                                dgvMaterials.Rows[intMrows].Cells["parts_code"].Value = CommonCtrl.IsNullToString(dpr["ser_parts_code"]);
                                dgvMaterials.Rows[intMrows].Cells["parts_name"].Value = CommonCtrl.IsNullToString(dpr["parts_name"]);
                                dgvMaterials.Rows[intMrows].Cells["unit"].Value = CommonCtrl.IsNullToString(dpr["unit"]);
                                dgvMaterials.Rows[intMrows].Cells["drawn_no"].Value = CommonCtrl.IsNullToString(dpr["drawing_num"]);
                                dgvMaterials.Rows[intMrows].Cells["quantity"].Value = "0";
                                dgvMaterials.Rows[intMrows].Cells["unit_price"].Value = !string.IsNullOrEmpty(CommonCtrl.IsNullToString(dpr["ref_out_price"])) ? CommonCtrl.IsNullToString(dpr["ref_out_price"]) : "0";
                                dgvMaterials.Rows[intMrows].Cells["whether_imported"].Value = CommonCtrl.IsNullToString(dpr["is_import"]) == "1" ? "是" : "否";
                                dgvMaterials.Rows[intMrows].Cells["vehicle_brand"].Value = CommonFuncCall.GetCarTypeForMa((CommonCtrl.IsNullToString(dpr["ser_parts_code"])));
                                listMater.Remove(strPcode);
                                foreach (DataGridViewRow dgvr in dgvMaterials.Rows)
                                {
                                    string strPCode = CommonCtrl.IsNullToString(dgvr.Cells["parts_code"].Value);
                                    if (!string.IsNullOrEmpty(strPCode) && !listMater.Contains(strPCode))
                                    {
                                        listMater.Add(strPCode);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HXCPcClient.GlobalStaticObj.GlobalLogService.WriteLog(ex);
            }
        }
        #endregion
    }
}
