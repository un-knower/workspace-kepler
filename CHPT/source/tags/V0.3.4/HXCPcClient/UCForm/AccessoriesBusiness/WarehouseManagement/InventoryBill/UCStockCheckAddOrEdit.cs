﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SYSModel;
using Model;
using HXCPcClient.CommonClass;
using ServiceStationClient.ComponentUI;
using HXCPcClient.Chooser;
using Utility.Common;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
namespace HXCPcClient.UCForm.AccessoriesBusiness.WarehouseManagement.InventoryBill
{
    public partial class UCStockCheckAddOrEdit : UCBase
    {

        #region 全局变量
        WindowStatus status;
        UCStockCheckManager UCCheckBM;
        private string StockCheckId = string.Empty;
        private string submit = DataSources.GetDescription(DataSources.EnumOperateType.submit, true);//提交
        private string save = DataSources.GetDescription(DataSources.EnumOperateType.save, true);//保存
        int Initialindex = -1;//GridView初始行索引
        private string CheckBillID = "stock_check_id";
        private string CheckBillTable = "tb_parts_stock_check";
        private string CheckPartTable = "tb_parts_stock_check_p";

        private string CheckBillLogMsg = "查询盘点单表信息";
        private string CheckPartLogMsg = "查询盘点单配件表信息";
        private string ImportTitle = "导入盘点单配件信息";

        //配件单据列名
        private const string PID = "ID";
        private const string PNum = "partsnum";
        private const string PNam = "partname";
        private const string PSpc = "PartSpec";
        private const string PDrw = "drawingnum";
        private const string PUnt = "unitname";
        private const string PBrd = "partbrand";
        private const string PCfc = "CarFactoryCode";
        private const string PBrc = "BarCode";
        private const string PPcnt = "Papcounts";
        private const string PFcnt = "FmOffCount";
        private const string PLcnt = "ProLossCount";
        private const string PBsp = "BusinesPrice";
        private const string PCmy = "Calcmoney";
        private const string PRmk = "remarks";


        /// <summary>
        /// 获取盘点单表头表尾信息实体
        /// </summary>
        tb_parts_stock_check CheckBillEntity = new tb_parts_stock_check();
        #endregion
        public UCStockCheckAddOrEdit(WindowStatus state, string CheckId, UCStockCheckManager UCCheckManager)
        {
            InitializeComponent();
            DTPickorder_date.Value = DateTime.Now.ToShortDateString();//获取当前系统时间
            this.UCCheckBM = UCCheckManager;//获取其它收货单管理类
            this.status = state;//获取操作状态
            this.StockCheckId = CheckId;//其它收货单ID
            base.SaveEvent += new ClickHandler(UCStockCheckAddOrEdit_SaveEvent);//保存
            base.SubmitEvent += new ClickHandler(UCStockCheckAddOrEdit_SubmitEvent);//提交
            base.ImportEvent += new ClickHandler(UCStockCheckAddOrEdit_ImportEvent);//导入
            
            //设置列表的可编辑状态
            gvPartsMsgList.ReadOnly = false;
            foreach (DataGridViewColumn dgCol in gvPartsMsgList.Columns)
            {
                if (dgCol.Name != colCheck.Name && dgCol.Name != FmOffCount.Name && dgCol.Name != remarks.Name) dgCol.ReadOnly = true;

            }
            base.btnExport.Visible = false;
            base.btnConfirm.Visible = false;
            base.btnEdit.Visible = false;
            base.btnBalance.Visible = false;
            base.btnPrint.Visible = false;
            gvPartsMsgList.HeadCheckChanged += new DataGridViewEx.DelegateOnClick(gvPartsMsgList_HeadCheckChanged); //复选框标题显示为复选框状态  
        }
        //复选框选择状态
        private void gvPartsMsgList_HeadCheckChanged()
        {
            IsDataGridViewCheckBox();
        }
        /// <summary>
        /// 窗体初始加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCStockReceiptAddOrEdit_Load(object sender, EventArgs e)
        {
            try
            {

                //获取仓库名称
                CommonFuncCall.BindWarehouse(Combwh_name, "请选择");
                //公司ID
                string com_id = GlobalStaticObj.CurrUserCom_Id;
                CommonFuncCall.BindDepartment(Comborg_name, com_id, "请选择");//选择部门名称
                CommonFuncCall.BindHandle(Combhandle_name, "", "请选择");//选择经手人

                if (status == WindowStatus.Edit || status == WindowStatus.Copy)
                {
                    GetBillHeadEndMessage(StockCheckId);//获取单据头尾信息
                    GetBillPartsMsg(StockCheckId);//获取单据配件信息

                }
                else if (status == WindowStatus.Add || status == WindowStatus.Copy)
                {
                    txtorder_status_name.Caption = DataSources.GetDescription(DataSources.EnumAuditStatus.DRAFT, true);//获取单据状态
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }
        /// <summary>
        /// 调价单保存
        /// </summary>
        /// <param name="send"></param>
        /// <param name="e"></param>
        private void UCStockCheckAddOrEdit_SaveEvent(object send, EventArgs e)
        {
            SaveOrSubmitMethod(save);
        }
        /// <summary>
        /// 调价单提交
        /// </summary>
        /// <param name="send"></param>
        /// <param name="e"></param>
        private void UCStockCheckAddOrEdit_SubmitEvent(object send, EventArgs e)
        {
            SaveOrSubmitMethod(submit);
        }
        /// <summary>
        /// 调价单导入单据
        /// </summary>
        /// <param name="send"></param>
        /// <param name="e"></param>
        private void UCStockCheckAddOrEdit_ImportEvent(object send, EventArgs e)
        {
            try
            {
                BtnImportMenu.Show(btnImport, 0, btnImport.Height);//指定导入菜单在导入按钮位置显示
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        /// <summary>
        /// 根据输入实盘数量计算盈亏数量和金额
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvPartsMsgList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                for (int i = 0; i < gvPartsMsgList.Rows.Count; i++)
                {
                    if (gvPartsMsgList.Rows[i].Cells["FmOffCount"].Value==null)
                        {
                            MessageBoxEx.Show("您输入的实盘数量不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                else if (!IsNumber(gvPartsMsgList.Rows[i].Cells["FmOffCount"].Value.ToString()))
                    {
                        MessageBoxEx.Show("请您输入数字类型的数值！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        gvPartsMsgList.Rows[i].Cells["FmOffCount"].Value = string.Empty;
                        return;
                    }
                    else
                    {
                        int PaperCount = Convert.ToInt32(gvPartsMsgList.Rows[i].Cells["Papcounts"].Value.ToString());//账面数量
                        int FirmCount = Convert.ToInt32(gvPartsMsgList.Rows[i].Cells["FmOffCount"].Value.ToString());//实盘数量 
                        decimal ProfitCount = decimal.Zero;
                        decimal BusnesPrice = Convert.ToDecimal(gvPartsMsgList.Rows[i].Cells["BusinesPrice"].Value.ToString());//业务单价

                        if (FirmCount <= 0)
                        {
                            MessageBoxEx.Show("您输入的实盘数量不能小于等于零！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        else
                        {
                            gvPartsMsgList.Rows[i].Cells["ProLossCount"].Value = FirmCount - PaperCount;//计算盈亏数量
                            ProfitCount = Convert.ToDecimal(gvPartsMsgList.Rows[i].Cells["ProLossCount"].Value.ToString());//盈亏数量
                            if (ProfitCount < decimal.Zero)
                            {
                                gvPartsMsgList.Rows[i].Cells["ProLossCount"].Style.ForeColor = Color.Red;
                            }
                            gvPartsMsgList.Rows[i].Cells["Calcmoney"].Value = ProfitCount * BusnesPrice;//计算总金额
                            if (Convert.ToDecimal(gvPartsMsgList.Rows[i].Cells["Calcmoney"].Value.ToString()) < decimal.Zero)
                            {
                                gvPartsMsgList.Rows[i].Cells["Calcmoney"].Style.ForeColor = Color.Red;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }
        /// <summary>
        /// 正则表达式判断输入字符串是否为数字
        /// </summary>
        /// <param name="strValue">要判断的字符串</param>
        /// <returns></returns>
        private static bool IsNumber(string strValue)
        {
            string RegPattern = "^[0-9]*$";
            Regex RegNum = new Regex(RegPattern);
            if (RegNum.IsMatch(strValue.Trim())) return true;
            RegPattern=@"^\d+\.\d{1,3}?$";//判断一到三位小数数值
            RegNum =new Regex(RegPattern);
            if (RegNum.IsMatch(strValue.Trim())) return true;
            return false;
        }
        /// <summary>
        /// 开单列表选择操作
        /// </summary>
        /// <param name="ischeck"></param>
        private void IsDataGridViewCheckBox()
        {
            try
            {
               if (gvPartsMsgList.Rows.Count != 0)

                {
                    bool ischeck = (bool)((DataGridViewCheckBoxCell)gvPartsMsgList.Rows[0].Cells["colCheck"]).EditedFormattedValue;
                    bool SetCheck = ischeck == true ? true : false;
                    for (int i = 1; i < gvPartsMsgList.Rows.Count; i++)
                    {
                        ((DataGridViewCheckBoxCell)gvPartsMsgList.Rows[i].Cells["colCheck"]).Value = SetCheck;//开单列表选择状态
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }
        /// <summary>
        /// 根据盘点单ID获取表头和尾部信息
        /// </summary>
        /// <param name="CheckBillId"></param>
        private void GetBillHeadEndMessage(string CheckBillId)
        {
            try
            {
                if (CheckBillId != null)
                {
                    //查询一条盘点单信息
                    DataTable ReceiptTable = DBHelper.GetTable(CheckBillLogMsg, CheckBillTable, "*", CheckBillID + "='" + CheckBillId + "'", "", "");
                    CommonFuncCall.FillEntityByTable(CheckBillEntity, ReceiptTable);
                    CommonFuncCall.FillControlsByEntity(this, CheckBillEntity);
                    if (status == WindowStatus.Copy)
                    {
                        lblorder_num.Text = string.Empty;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        /// <summary>
        /// 根据盘点单ID获取配件信息表
        /// </summary>
        /// <param name="RecBillIdValue"></param>
        private void GetBillPartsMsg(string RecBillIdValue)
        {
            try
            {

                DataTable PartsMsgList = DBHelper.GetTable(CheckPartLogMsg, CheckPartTable, "*", CheckBillID + "='" + RecBillIdValue + "'", "", "");//获取配件信息表
                if (PartsMsgList.Rows.Count > 0)
                {

                    foreach (DataRow dr in PartsMsgList.Rows)
                    {
                        DataGridViewRow dgvr = gvPartsMsgList.Rows[gvPartsMsgList.Rows.Add()];
                        dgvr.Cells["partsnum"].Value = dr["parts_code"];
                        dgvr.Cells["partname"].Value = dr["parts_name"];
                        dgvr.Cells["PartSpec"].Value = dr["model"];
                        dgvr.Cells["drawingnum"].Value = dr["drawing_num"];
                        dgvr.Cells["unitname"].Value = dr["unit_name"];
                        dgvr.Cells["partbrand"].Value = dr["parts_brand"];
                        dgvr.Cells["CarFactoryCode"].Value = dr["car_parts_code"];
                        dgvr.Cells["BarCode"].Value = dr["parts_barcode"];
                        dgvr.Cells["Papcounts"].Value = dr["paper_count"];
                        dgvr.Cells["FmOffCount"].Value = dr["firmoffer_count"];
                        dgvr.Cells["ProLossCount"].Value = dr["profitloss_count"];
                        dgvr.Cells["BusinesPrice"].Value = dr["business_price"];
                        dgvr.Cells["Calcmoney"].Value = dr["money"];
                        dgvr.Cells["remarks"].Value = dr["remark"];

                    }
                }
                else
                {
                    MessageBoxEx.Show("要查询的配件信息不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }
        /// <summary>
        /// 配件新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PartAdd_Click(object sender, EventArgs e)
        {
            try
            {
                PartsAdd();//配件新增
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message);
            }
        }


        private void PartDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult DgResult = MessageBoxEx.Show("您确定要删除选中记录行项！", "删除操作", MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if (DgResult == DialogResult.Yes)
                {
                    for (int i = 0; i < gvPartsMsgList.SelectedRows.Count; i++)
                    {

                        gvPartsMsgList.Rows.RemoveAt(i);
                        MessageBoxEx.Show("删除成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void ExcelImport_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> PartFieldDic = new Dictionary<string, string>();//配件列表的列名
                for (int i = 1; i < gvPartsMsgList.Columns.Count; i++)
                {//添加配件列字段名和标题文本
                    PartFieldDic.Add(gvPartsMsgList.Columns[i].Name.ToString(), gvPartsMsgList.Columns[i].HeaderText.ToString());
                }
                //弹出导入Excel对话框
                FrmExcelImport frmExcel = new FrmExcelImport(PartFieldDic, ImportTitle);
                DialogResult DgResult = frmExcel.ShowDialog();
                if (DgResult == DialogResult.OK)
                {

                    string PNumExcelField = string.Empty;
                    string PNamExcelField = string.Empty;
                    string PSpcExcelField = string.Empty;
                    string PDrwExcelField = string.Empty;
                    string PUntExcelField = string.Empty;
                    string PBrdExcelField = string.Empty;
                    string PCfcExcelField = string.Empty;
                    string PBrcExcelField = string.Empty;
                    string PPcntExcelField = string.Empty;
                    string PFcntExcelField = string.Empty;
                    string PLcntExcelField = string.Empty;
                    string PBspExcelField = string.Empty;
                    string PCmyExcelField = string.Empty;
                    string PRmkExcelField = string.Empty;

                    //获取与单据列名匹配的Excel表格列名
                    foreach (DictionaryEntry DicEty in frmExcel.MatchFieldHTable)
                    {
                        string BillField = DicEty.Key.ToString();//获取匹配单据的列名
                        string ExcelField = DicEty.Value.ToString();//获取匹配Excel表格的列名

                        switch (BillField)
                        {
                            case PNum:
                                PNumExcelField = ExcelField;
                                break;
                            case PNam:
                                PNamExcelField = ExcelField;
                                break;
                            case PSpc:
                                PSpcExcelField = ExcelField;
                                break;
                            case PDrw:
                                PDrwExcelField = ExcelField;
                                break;
                            case PUnt:
                                PUntExcelField = ExcelField;
                                break;
                            case PBrd:
                                PBrdExcelField = ExcelField;
                                break;
                            case PCfc:
                                PCfcExcelField = ExcelField;
                                break;
                            case PBrc:
                                PBrcExcelField = ExcelField;
                                break;
                            case PPcnt:
                                PPcntExcelField = ExcelField;
                                break;
                            case PFcnt:
                                PFcntExcelField = ExcelField;
                                break;
                            case PLcnt:
                                PLcntExcelField = ExcelField;
                                break;
                            case PBsp:
                                PBspExcelField = ExcelField;
                                break;
                            case PCmy:
                                PCmyExcelField = ExcelField;
                                break;
                            case PRmk:
                                PRmkExcelField = ExcelField;
                                break;

                        }
                    }
                    if (ValidityCellIsnull(frmExcel.ExcelTable))
                    {
                        for (int i = 0; i < frmExcel.ExcelTable.Rows.Count; i++)
                        {

                            DataGridViewRow DgRow = gvPartsMsgList.Rows[gvPartsMsgList.Rows.Add()];//创建新行项

                            DgRow.Cells[PNum].Value = frmExcel.ExcelTable.Rows[i][PNumExcelField].ToString();
                            DgRow.Cells[PNam].Value = frmExcel.ExcelTable.Rows[i][PNamExcelField].ToString();
                            DgRow.Cells[PSpc].Value = frmExcel.ExcelTable.Rows[i][PSpcExcelField].ToString();
                            DgRow.Cells[PDrw].Value = frmExcel.ExcelTable.Rows[i][PDrwExcelField].ToString();
                            DgRow.Cells[PUnt].Value = frmExcel.ExcelTable.Rows[i][PUntExcelField].ToString();
                            DgRow.Cells[PBrd].Value = frmExcel.ExcelTable.Rows[i][PBrdExcelField].ToString();
                            DgRow.Cells[PCfc].Value = frmExcel.ExcelTable.Rows[i][PCfcExcelField].ToString();
                            DgRow.Cells[PBrc].Value = frmExcel.ExcelTable.Rows[i][PBrcExcelField].ToString();
                            DgRow.Cells[PPcnt].Value = frmExcel.ExcelTable.Rows[i][PPcntExcelField].ToString();
                            DgRow.Cells[PFcnt].Value = frmExcel.ExcelTable.Rows[i][PFcntExcelField].ToString();
                            DgRow.Cells[PLcnt].Value = frmExcel.ExcelTable.Rows[i][PLcntExcelField].ToString();
                            DgRow.Cells[PBsp].Value = frmExcel.ExcelTable.Rows[i][PBspExcelField].ToString();
                            DgRow.Cells[PCmy].Value = frmExcel.ExcelTable.Rows[i][PCmyExcelField].ToString();
                            DgRow.Cells[PRmk].Value = frmExcel.ExcelTable.Rows[i][PRmkExcelField].ToString();


                             
                        }

                    }

                    frmExcel.ExcelTable.Clear();//清空所有配件信息
                    MessageBoxEx.Show("导入成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }


            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }
        /// <summary>
        /// 验证导入excel表格是否有空值
        /// </summary>
        /// <param name="ExcelTable">导入的表格</param>
        /// <returns></returns>
        private bool ValidityCellIsnull(DataTable ExcelTable)
        {
            for (int i = 0; i < ExcelTable.Rows.Count; i++)
            {
                for (int j = 0; j < ExcelTable.Columns.Count; j++)
                {
                    if (string.IsNullOrEmpty(ExcelTable.Rows[i][j].ToString()))
                    {
                        MessageBoxEx.Show("Excel配件信息模板中存在空数据行项！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        gvPartsMsgList.Rows.Clear();//清空之前导入数据
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// 盘点单配件信息添加
        /// </summary>
        private void PartsAdd()
        {
            try
            {

                frmParts frm = new frmParts();
                DialogResult result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string PartsCode = frm.PartsCode;
                    if (gvPartsMsgList.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow dr in gvPartsMsgList.Rows)
                        {

                            if (dr.Cells["parts_code"].Value.ToString() == PartsCode)
                            {
                                MessageBoxEx.Show("该配件信息已经存在与列表中，不能再次添加!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    DataTable dt = GetPartsByCode(PartsCode);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return;
                    }
                    else
                    {

                        foreach (DataRow dr in dt.Rows)
                        {
                            DataGridViewRow dgvr = gvPartsMsgList.Rows[gvPartsMsgList.Rows.Add()];
                            GetGridViewRowByDr(dgvr, dr);
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message);
            }
        }

        /// <summary>
        /// 根据ID获取配件信息
        /// </summary>
        /// <param name="PartsID"></param>
        /// <returns></returns>
        private DataTable GetPartsByCode(string PartsCode)
        {
            //有问题，需要修改
            DataTable dt = DBHelper.GetTable("", "tb_parts", "*", string.Format("ser_parts_code='{0}'", PartsCode), "", "");
            return dt;
        }

        /// <summary>
        /// 单个添加或编辑配件
        /// </summary>
        /// <param name="dgvr"></param>
        /// <param name="dr"></param>
        /// <param name="relation_order"></param>
        /// <param name="HandleType"></param>
        private void GetGridViewRowByDr(DataGridViewRow dgvr, DataRow dr)
        {
            try
            {
                dgvr.Cells["partsnum"].Value = dr["ser_parts_code"];
                dgvr.Cells["partname"].Value = dr["parts_name"];
                dgvr.Cells["PartSpec"].Value = dr["model"];
                dgvr.Cells["drawingnum"].Value = dr["drawing_num"];
                dgvr.Cells["unitname"].Value = dr["sales_unit_name"];
                dgvr.Cells["partbrand"].Value = dr["parts_brand"];
                dgvr.Cells["CarFactoryCode"].Value = dr["car_parts_code"];
                dgvr.Cells["BarCode"].Value = dr["parts_barcode"];
                dgvr.Cells["Papcounts"].Value = dr["sales_unit_quantity"];//账面数量
                dgvr.Cells["remarks"].Value = dr["remark"];

            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }


        /// <summary>
        /// 数据保存/提交 方法
        /// </summary>
        /// <param name="HandleTypeName">保存/提交</param>
        private void SaveOrSubmitMethod(string HandleTypeName)
        {
            try
            {

                if (CheckListInfo())
                {
                    gvPartsMsgList.EndEdit();
                    string opName = "盘点单操作";
                    lblorder_num.Text = CommonUtility.GetNewNo(DataSources.EnumProjectType.ReceiptBill);//获取其它收货单编号
                    if (HandleTypeName == submit)
                    {
                        txtorder_status_name.Caption = DataSources.GetDescription(DataSources.EnumAuditStatus.SUBMIT, true);
                    }
                    else if (HandleTypeName == save)
                    {
                        txtorder_status_name.Caption = DataSources.GetDescription(DataSources.EnumAuditStatus.DRAFT, true);
                    }

                    List<SysSQLString> listSql = new List<SysSQLString>();
                    if (status == WindowStatus.Add || status == WindowStatus.Copy)
                    {
                        StockCheckId = Guid.NewGuid().ToString();
                        AddReceiptBillSql(listSql, CheckBillEntity, StockCheckId, HandleTypeName);
                        opName = "新增盘点单";
                        AddOrEditPartsListMsg(listSql, StockCheckId, WindowStatus.Add);

                    }
                    else if (status == WindowStatus.Edit)
                    {
                        EditReceiptBillSql(listSql, CheckBillEntity, StockCheckId, HandleTypeName);
                        opName = "修改盘点单";
                        AddOrEditPartsListMsg(listSql, StockCheckId, WindowStatus.Edit);

                    }
                    

                    if (DBHelper.BatchExeSQLStringMultiByTrans(opName, listSql))
                    {
                        MessageBoxEx.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        string QueryWhere = " enable_flag=1 ";//获取查询条件
                        UCCheckBM.GetCheckBillList(QueryWhere);//更新单据列表
                        deleteMenuByTag(this.Tag.ToString(), UCCheckBM.Name);
                    }
                    else
                    {
                        MessageBoxEx.Show("保存失败！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        /// <summary>
        /// 验证表中列表数据信息完整性
        /// </summary>
        private bool CheckListInfo()
        {

            foreach (DataGridViewRow dgvr in gvPartsMsgList.Rows)
            {

                string PartCount = CommonCtrl.IsNullToString(dgvr.Cells["FmOffCount"].Value);//配件数量
                if (PartCount.Length == 0)
                {
                    MessageBoxEx.Show("请输入实盘数量!","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    gvPartsMsgList.CurrentCell = dgvr.Cells["FmOffCount"];
                    return false;
                }


            }
            return true;
        }

        /// <summary>
        /// 添加情况下组装sql的方法
        /// </summary>
        /// <param name="listSql"></param>
        /// <param name="purchase_billing_id"></param>
        private void AddReceiptBillSql(List<SysSQLString> listSql, tb_parts_stock_check stockCheckEntity, string StockCheckId, string HandleType)
        {
            try
            {
                const string NoDelFlag = "1";//默认删除标记1表示未删除，0表示删除
                string Save = DataSources.GetDescription(DataSources.EnumOperateType.save, true);//保存操作
                string Submit = DataSources.GetDescription(DataSources.EnumOperateType.submit, true);//提交操作
                //SQL语句拼装操作
                SysSQLString sysStringSql = new SysSQLString();
                sysStringSql.cmdType = CommandType.Text;
                Dictionary<string, string> dicParam = new Dictionary<string, string>();//保存SQL语句参数值
                CommonFuncCall.FillEntityByControls(this, stockCheckEntity);
                stockCheckEntity.stock_check_id = StockCheckId;

                stockCheckEntity.update_by = GlobalStaticObj.UserID;
                stockCheckEntity.operators = GlobalStaticObj.UserID;
                stockCheckEntity.enable_flag = NoDelFlag;
                if (HandleType == Save)
                {
                    stockCheckEntity.order_status = Convert.ToInt32(DataSources.EnumAuditStatus.DRAFT).ToString();
                    stockCheckEntity.order_status_name = DataSources.GetDescription(DataSources.EnumAuditStatus.DRAFT, true);
                }
                else if (HandleType == Submit)
                {
                    stockCheckEntity.order_status = Convert.ToInt32(DataSources.EnumAuditStatus.SUBMIT).ToString();
                    stockCheckEntity.order_status_name = DataSources.GetDescription(DataSources.EnumAuditStatus.SUBMIT, true);
                }
                if (stockCheckEntity != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" Insert Into tb_parts_stock_check( ");
                    StringBuilder sb_PrValue = new StringBuilder();
                    StringBuilder sb_PrName = new StringBuilder();
                    foreach (PropertyInfo info in stockCheckEntity.GetType().GetProperties())
                    {
                        string name = info.Name;
                        object value = info.GetValue(stockCheckEntity, null);
                        sb_PrName.Append("," + name);//数据表字段名
                        sb_PrValue.Append(",@" + name);//数据表字段值
                        dicParam.Add(name, value == null ? "" : value.ToString());
                    }
                    sb.Append(sb_PrName.ToString().Substring(1, sb_PrName.ToString().Length - 1) + ") Values (");//追加字段名
                    sb.Append(sb_PrValue.ToString().Substring(1, sb_PrValue.ToString().Length - 1) + ");");//追加字段值
                    //完成SQL语句的拼装
                    sysStringSql.sqlString = sb.ToString();
                    sysStringSql.Param = dicParam;
                    listSql.Add(sysStringSql);
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question); MessageBoxEx.Show(ex.Message);
            }
        }

        /// <summary>
        /// 编辑情况下组装sql的方法
        /// </summary>
        /// <param name="listSql"></param>
        /// <param name="purchase_billing_id"></param>
        /// <param name="model"></param>
        private void EditReceiptBillSql(List<SysSQLString> listSql, tb_parts_stock_check stockCheckEntity, string StockCheckId, string HandleType)
        {
            try
            {
                const string NoDelFlag = "1";//默认删除标记，1表示未删除，0表示删除
                string Save = DataSources.GetDescription(DataSources.EnumOperateType.save, true);//保存操作
                string Submit = DataSources.GetDescription(DataSources.EnumOperateType.submit, true);//提交操作
                SysSQLString sysStrSql = new SysSQLString();
                sysStrSql.cmdType = CommandType.Text;//sql字符串语句执行函数
                Dictionary<string, string> dicParam = new Dictionary<string, string>();//参数
                CommonFuncCall.FillEntityByControls(this, CheckBillEntity);

                stockCheckEntity.handle = GlobalStaticObj.UserID;
                stockCheckEntity.operators = GlobalStaticObj.UserID;
                stockCheckEntity.enable_flag = NoDelFlag;
                if (HandleType == Save)
                {
                    stockCheckEntity.order_status = Convert.ToInt32(DataSources.EnumAuditStatus.DRAFT).ToString();
                    stockCheckEntity.order_status_name = DataSources.GetDescription(DataSources.EnumAuditStatus.DRAFT, true);
                }
                else if (HandleType == Submit)
                {
                    stockCheckEntity.order_status = Convert.ToInt32(DataSources.EnumAuditStatus.SUBMIT).ToString();
                    stockCheckEntity.order_status_name = DataSources.GetDescription(DataSources.EnumAuditStatus.SUBMIT, true);
                }
                if (CheckBillEntity != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" Update tb_parts_stock_check Set ");
                    bool isFirstValue = true;
                    foreach (PropertyInfo info in stockCheckEntity.GetType().GetProperties())
                    {
                        string name = info.Name;
                        object value = info.GetValue(stockCheckEntity, null);
                        if (isFirstValue)
                        {
                            isFirstValue = false;
                            sb.Append(name);
                            sb.Append("=");
                            sb.Append("@" + name);
                        }
                        else
                        {
                            sb.Append("," + name);
                            sb.Append("=");
                            sb.Append("@" + name);
                        }
                        dicParam.Add(name, value == null ? "" : value.ToString());
                    }
                    sb.Append(" where stock_check_id='" + StockCheckId + "';");
                    sysStrSql.sqlString = sb.ToString();
                    sysStrSql.Param = dicParam;
                    listSql.Add(sysStrSql);//完成SQL语句的拼装
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }


        /// <summary>
        /// 根据其它收货单主键ID和表头信息对配件信息列表进行添加或编辑
        /// </summary>
        /// <param name="listSQL">要执行的SQL语句列表</param>
        /// <param name="CheckId">出入库单号</param>
        private void AddOrEditPartsListMsg(List<SysSQLString> listSQL, string CheckId, WindowStatus WinState)
        {
            try
            {
                if (gvPartsMsgList.Rows.Count > 0)
                {

                    //循环获取配件信息表中的数据记录，添加到数据库中
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb_ColName = new StringBuilder();
                    StringBuilder sb_ColValue = new StringBuilder();
                    string ColName = string.Empty;
                    for (int i = 2; i < gvPartsMsgList.Columns.Count; i++)
                    {
                        ColName = gvPartsMsgList.Columns[i].Name.ToString();
                        sb_ColName.Append("," + ColName);//数据表字段名
                        sb_ColValue.Append(",@" + ColName);//数据表值的参数名

                    }
                    if (WinState == WindowStatus.Edit)
                    {
                        SysSQLString SQLStr = new SysSQLString();//创建存储要删除sql语句的实体类
                        SQLStr.cmdType = CommandType.Text;//sql语句执行格式
                        Dictionary<string, string> DicDelParam = new Dictionary<string, string>();
                        StringBuilder sbDelSql = new StringBuilder();
                        DicDelParam.Add("stock_check_id", CheckId);
                        sbDelSql.AppendFormat("delete from {0} where {1}=@{1}", CheckPartTable, CheckBillID);
                        SQLStr.Param = DicDelParam;//获取删除的参数名值对
                        SQLStr.sqlString = sbDelSql.ToString();//获取执行删除的操作
                        listSQL.Add(SQLStr);//添加到执行实例对像中
                    }
                    sb.Append("insert into tb_parts_stock_check_p (stock_check_parts_id,stock_check_id,num");
                    sb.Append(sb_ColName.ToString() + ") values(@stock_check_parts_id,@stock_check_id,@num" + sb_ColValue.ToString() + ");");
                    int SerialNum = 1;//序号
                    foreach (DataGridViewRow DgVRow in gvPartsMsgList.Rows)
                    {

                        SysSQLString StrSqlParts = new SysSQLString();//创建存储要执行的sql语句的实体类
                        StrSqlParts.cmdType = CommandType.Text;//指定sql语句执行格式
                        Dictionary<string, string> DicPartParam = new Dictionary<string, string>();//字段参数值集合
                        DicPartParam.Add("stock_check_parts_id", Guid.NewGuid().ToString());
                        DicPartParam.Add("stock_check_id", CheckId);
                        DicPartParam.Add("num", SerialNum.ToString());
                        DicPartParam.Add("parts_code", DgVRow.Cells["partsnum"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["partsnum"].Value.ToString());
                        DicPartParam.Add("parts_name", DgVRow.Cells["partname"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["partname"].Value.ToString());
                        DicPartParam.Add("model", DgVRow.Cells["PartSpec"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["PartSpec"].Value.ToString());
                        DicPartParam.Add("drawing_num", DgVRow.Cells["drawingnum"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["drawingnum"].Value.ToString());
                        DicPartParam.Add("unit_name", DgVRow.Cells["unitname"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["unitname"].Value.ToString());
                        DicPartParam.Add("parts_brand", DgVRow.Cells["partbrand"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["partbrand"].Value.ToString());
                        DicPartParam.Add("car_parts_code", DgVRow.Cells["CarFactoryCode"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["CarFactoryCode"].Value.ToString());
                        DicPartParam.Add("parts_barcode", DgVRow.Cells["BarCode"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["BarCode"].Value.ToString());
                        DicPartParam.Add("paper_count", DgVRow.Cells["Papcounts"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["Papcounts"].Value.ToString());
                        DicPartParam.Add("firmoffer_count", DgVRow.Cells["FmOffCount"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["FmOffCount"].Value.ToString());
                        DicPartParam.Add("business_price", DgVRow.Cells["BusinesPrice"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["BusinesPrice"].Value.ToString());
                        DicPartParam.Add("money", DgVRow.Cells["Calcmoney"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["Calcmoney"].Value.ToString());
                        DicPartParam.Add("remark", DgVRow.Cells["remarks"].Value.ToString() == string.Empty ? "" : DgVRow.Cells["remarks"].Value.ToString());
                        StrSqlParts.sqlString = sb.ToString();//获取执行的sql语句
                        StrSqlParts.Param = DicPartParam;//获取执行的参数值
                        listSQL.Add(StrSqlParts);//添加记录行到list列表中
                        SerialNum++;//自动增加序号
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }


    }
}
