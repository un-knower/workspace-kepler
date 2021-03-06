﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ServiceStationClient.ComponentUI;
using SYSModel;
using HXCServerWinForm.CommonClass;
using SYSModel;
using HXCServerWinForm;
using HXC_FuncUtility;
using BLL;
using Utility.Common;
using ServiceStationClient.ComponentUI.DataGrid;
namespace HXCServerWinForm.UCForm
{
    /// <summary>
    /// 附件信息
    /// </summary>
    public partial class UCAttachment : UserControl
    {
        string tableName = string.Empty;//关联对象
        string tableNameKeyValue = string.Empty;//关联对象主键值
        bool readOnly = false;//是否只读
        public UCAttachment()
        {
            InitializeComponent();
            dgvAttachment.ReadOnly = false;
            colAttType.ReadOnly = true;
        }
        /// <summary>
        /// 关联对象
        /// </summary>
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        /// <summary>
        /// 关联对象ID值
        /// </summary>
        public string TableNameKeyValue
        {
            get { return tableNameKeyValue; }
            set { tableNameKeyValue = value; }
        }
        /// <summary>
        /// 过滤文件类型
        /// </summary>
        public string Filter = "(*.jpg,*.png,*.jpeg,*.bmp,*.gif)|*.jgp;*.png;*.jpeg;*.bmp;*.gif|All files(*.*)|*.*";

        private void tsmiAdd_Click(object sender, EventArgs e)
        {
            colAttPath.Filter = Filter;
            DataGridViewRow dgvr = dgvAttachment.Rows[dgvAttachment.Rows.Add()];
        }

        private void dgvAttachment_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            if (dgvAttachment.Rows[e.RowIndex].Cells[e.ColumnIndex].OwningColumn.Name == colAttPath.Name)
            {
                string fileName = CommonCtrl.IsNullToString(dgvAttachment.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                string type = string.Empty;
                if (fileName.Length > 0)
                {
                    switch (Path.GetExtension(fileName).ToLower())
                    {
                        case ".doc":
                        case ".docx":
                        case ".txt":
                        case ".xls":
                        case ".xlsx":
                            type = "文档";
                            break;
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                        case ".gif":
                            type = "图片";
                            break;
                        default:
                            type = "其他";
                            break;
                    }
                    dgvAttachment.Rows[e.RowIndex].Cells[colAttType.Name].Value = type;
                    dgvAttachment.Rows[e.RowIndex].Cells[colAttName.Name].Value = Path.GetFileName(fileName);
                }
            }
        }

        /// <summary>
        /// 检查输入的正确性
        /// </summary>
        /// <returns></returns>
        public bool CheckAttachment()
        {
            dgvAttachment.EndEdit();
            bool isCheck = true;
            foreach (DataGridViewRow dgvr in dgvAttachment.Rows)
            {
                string name = CommonCtrl.IsNullToString(dgvr.Cells[colAttName.Name].Value);
                if (name.Length == 0)
                {
                    //MessageBoxEx.Show("请输入附件名称!");
                    //dgvAttachment.CurrentCell = dgvr.Cells[colAttName.Name];
                    //isCheck = false;
                    //break;
                }

                string path = CommonCtrl.IsNullToString(dgvr.Cells[colAttPath.Name].Value);
                if (path.Length == 0)
                {
                    //MessageBoxEx.Show("请选择附件!");
                    //dgvAttachment.CurrentCell = dgvr.Cells[colAttPath.Name];
                    //isCheck = false;
                    //break;
                    continue;
                }
                if (!File.Exists(path))
                {
                    continue;
                }
                string guid = Guid.NewGuid().ToString() + Path.GetExtension(path);
                if (!FileOperation.UploadFile(path, guid))
                {
                    MessageBoxEx.Show("附件上传失败!");
                    isCheck = false;
                    break;
                }
                dgvr.Cells[colAttPath.Name].Tag = guid;
            }
            return isCheck;
        }

        /// <summary>
        /// 附件SQL
        /// </summary>
        public List<SQLObj> AttachmentSql
        {
            get
            {
                List<SQLObj> listSql = new List<SQLObj>();
                foreach (DataGridViewRow dgvr in dgvAttachment.Rows)
                {
                    //如果什么都没输就不保存了
                    if (CommonCtrl.IsNullToString(dgvr.Cells[colAttName.Name].Value).Length == 0 &&
                        CommonCtrl.IsNullToString(dgvr.Cells[colAttType.Name].Value).Length == 0 &&
                        CommonCtrl.IsNullToString(dgvr.Cells[colAttPath.Name].Tag).Length == 0 &&
                        CommonCtrl.IsNullToString(dgvr.Cells[colAttRemark.Name].Value).Length == 0)
                    {
                        continue;
                    }
                    SQLObj obj = new SQLObj();
                    obj.cmdType = CommandType.Text;
                    Dictionary<string, ParamObj> dic = new Dictionary<string, ParamObj>();
                    dic.Add("att_name", new ParamObj("att_name", dgvr.Cells["colAttName"].Value, SysDbType.NVarChar, 15));
                    dic.Add("att_type", new ParamObj("att_type", dgvr.Cells["colAttType"].Value, SysDbType.NVarChar, 15));
                    dic.Add("att_path", new ParamObj("att_path", dgvr.Cells["colAttPath"].Tag, SysDbType.VarChar, 40));
                    dic.Add("remark", new ParamObj("remark", dgvr.Cells["colAttRemark"].Value, SysDbType.NVarChar, 300));
                    obj.Param = dic;
                    string attID = CommonCtrl.IsNullToString(dgvr.Cells["colAttID"].Value);
                    if (attID.Length == 0)
                    {
                        attID = Guid.NewGuid().ToString();
                        dic.Add("is_main", new ParamObj("is_main", (int)DataSources.EnumYesNo.NO, SysDbType.VarChar, 5));
                        dic.Add("relation_object", new ParamObj("relation_object", TableName, SysDbType.NVarChar, 30));
                        dic.Add("relation_object_id", new ParamObj("relation_object_id", TableNameKeyValue, SysDbType.VarChar, 40));
                        dic.Add("enable_flag", new ParamObj("enable_flag", (int)DataSources.EnumEnableFlag.USING, SysDbType.VarChar, 5));
                        dic.Add("create_by", new ParamObj("create_by", GlobalStaticObj_Server.Instance.UserID, SysDbType.NVarChar, 40));
                        dic.Add("create_time", new ParamObj("create_time",Common.LocalDateTimeToUtcLong(DateTime.Now), SysDbType.BigInt));
                        obj.sqlString = @"insert into [attachment_info] ([att_id],[att_name],[att_type],[att_path],[remark],[relation_object],[relation_object_id],[enable_flag],[create_by],[create_time],[is_main])
                            values (@att_id,@att_name,@att_type,@att_path,@remark,@relation_object,@relation_object_id,@enable_flag,@create_by,@create_time,@is_main);";
                    }
                    else
                    {
                        dic.Add("update_by", new ParamObj("update_by", GlobalStaticObj_Server.Instance.UserID, SysDbType.NVarChar, 40));
                        dic.Add("update_time", new ParamObj("update_time", Common.LocalDateTimeToUtcLong(DateTime.Now), SysDbType.BigInt));
                        obj.sqlString = @"update [attachment_info] set [att_name]=@att_name,[att_type]=@att_type,[att_path]=@att_path,[remark]=@remark,
                        [update_by]=@update_by,[update_time]=@update_time where [att_id]=@att_id;";
                    }
                    dic.Add("att_id", new ParamObj("att_id", attID, SysDbType.VarChar, 40));
                    listSql.Add(obj);
                }
                return listSql;
            }

        }

        public void GetAttachmentSql(List<SysSQLString> listSql)
        {
            foreach (DataGridViewRow dgvr in dgvAttachment.Rows)
            {
                //如果什么都没输就不保存了
                if (CommonCtrl.IsNullToString(dgvr.Cells[colAttName.Name].Value).Length == 0 &&
                    CommonCtrl.IsNullToString(dgvr.Cells[colAttType.Name].Value).Length == 0 &&
                    CommonCtrl.IsNullToString(dgvr.Cells[colAttPath.Name].Tag).Length == 0 &&
                    CommonCtrl.IsNullToString(dgvr.Cells[colAttRemark.Name].Value).Length == 0)
                {
                    continue;
                }
                SysSQLString obj = new SysSQLString();
                obj.cmdType = CommandType.Text;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("att_name", CommonCtrl.IsNullToString(dgvr.Cells["colAttName"].Value));
                dic.Add("att_type", CommonCtrl.IsNullToString(dgvr.Cells["colAttType"].Value));
                dic.Add("att_path", CommonCtrl.IsNullToString(dgvr.Cells["colAttPath"].Tag));
                dic.Add("remark", CommonCtrl.IsNullToString(dgvr.Cells["colAttRemark"].Value));
                obj.Param = dic;
                string attID = CommonCtrl.IsNullToString(dgvr.Cells["colAttID"].Value);
                if (attID.Length == 0)
                {
                    attID = Guid.NewGuid().ToString();
                    dic.Add("is_main", ((int)DataSources.EnumYesNo.NO).ToString());
                    dic.Add("relation_object", TableName);
                    dic.Add("relation_object_id", TableNameKeyValue);
                    dic.Add("enable_flag", ((int)DataSources.EnumEnableFlag.USING).ToString());
                    dic.Add("create_by", GlobalStaticObj_Server.Instance.UserID);
                    dic.Add("create_time", Common.LocalDateTimeToUtcLong(DateTime.Now).ToString());
                    obj.sqlString = @"insert into [attachment_info] ([att_id],[att_name],[att_type],[att_path],[remark],[relation_object],[relation_object_id],[enable_flag],[create_by],[create_time],[is_main])
                            values (@att_id,@att_name,@att_type,@att_path,@remark,@relation_object,@relation_object_id,@enable_flag,@create_by,@create_time,@is_main);";
                }
                else
                {
                    dic.Add("update_by", GlobalStaticObj_Server.Instance.UserID);
                    dic.Add("update_time", Common.LocalDateTimeToUtcLong(DateTime.Now).ToString());
                    obj.sqlString = @"update [attachment_info] set [att_name]=@att_name,[att_type]=@att_type,[att_path]=@att_path,[remark]=@remark,
                        [update_by]=@update_by,[update_time]=@update_time where [att_id]=@att_id;";
                }
                dic.Add("att_id", attID);
                listSql.Add(obj);
            }
        }

        /// <summary>
        /// 绑定附件
        /// </summary>
        public void BindAttachment()
        {
            StringBuilder sbWhere = new StringBuilder();
            sbWhere.AppendFormat("relation_object='{0}'", TableName);
            sbWhere.AppendFormat(" and relation_object_id='{0}'", TableNameKeyValue);
            sbWhere.AppendFormat(" and enable_flag='{0}'", (int)DataSources.EnumEnableFlag.USING);
            sbWhere.Append(" and is_main='0'");
            DataTable dt = DBHelper.GetTable("绑定附件", GlobalStaticObj_Server.DbPrefix + GlobalStaticObj_Server.CommAccCode, "attachment_info", "*", sbWhere.ToString(), "", "");
            dgvAttachment.RowCount = 0;
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }
            foreach (DataRow dr in dt.Rows)
            {
                DataGridViewRow dgvr = dgvAttachment.Rows[dgvAttachment.Rows.Add()];
                dgvr.Cells["colAttID"].Value = dr["att_id"];
                dgvr.Cells["colAttName"].Value = dr["att_name"];
                dgvr.Cells["colAttType"].Value = dr["att_type"];
                dgvr.Cells["colAttPath"].Value = dr["att_path"];
                dgvr.Cells["colAttPath"].Tag = dr["att_path"];
                dgvr.Cells["colAttRemark"].Value = dr["remark"];
            }
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="opName">操作名</param>
        /// <param name="tableName">表名</param>
        /// <param name="key">主健</param>
        /// <param name="checkName">选择控件名</param>
        /// <param name="keyName">主键控件名</param>
        private void BatchDelete(DataGridView dgv, string opName, string tableName, string key, string checkName, string keyName)
        {
            if (MessageBoxEx.Show("确认要删除选择的项吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }
            dgv.EndEdit();
            StringBuilder sbWhere = new StringBuilder();
            foreach (DataGridViewRow dgvr in dgv.Rows)
            {
                object isCheck = dgvr.Cells[checkName].Value;
                if (isCheck != null && (bool)isCheck)
                {
                    if (sbWhere.Length > 0)
                    {
                        sbWhere.Append(",");
                    }
                    string name = CommonCtrl.IsNullToString(dgvr.Cells[keyName].Value);
                    if (name.Length > 0)
                    {
                        sbWhere.AppendFormat("'{0}'", name);
                    }
                }
            }
            if (sbWhere.Length == 0)
            {
                DeleteDataGridViewRow(dgv, checkName);
                return;
            }

            if (DBHelper.BatchDeleteDataByWhere(opName, GlobalStaticObj_Server.DbPrefix + GlobalStaticObj_Server.CommAccCode, tableName, string.Format("{0} in ({1})", key, sbWhere.ToString())))
            {
                DeleteDataGridViewRow(dgv, checkName);
            }
            else
            {
                MessageBoxEx.Show("删除失败");
            }
        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="checkName"></param>
        private void DeleteDataGridViewRow(DataGridView dgv, string checkName)
        {
            for (int i = 0; i < dgv.RowCount; i++)
            {
                DataGridViewRow dgvr = dgv.Rows[i];
                object isCheck = dgvr.Cells[checkName].Value;
                //将选中的删除
                if (isCheck != null && (bool)isCheck)
                {
                    dgv.Rows.RemoveAt(i--);
                }
            }
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            BatchDelete(dgvAttachment, "删除附件信息", "attachment_info", "att_id", colCheckAtt.Name, colAttID.Name);
        }
        /// <summary>
        /// 只读
        /// </summary>
        [Browsable(true), DefaultValue(false), Description("只读属性")]
        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly = value;
                dgvAttachment.ReadOnly = readOnly;
                if (value)
                {
                    tsmiAdd.Visible = false;
                    tsmiDelete.Visible = false;
                }
                else
                {
                    tsmiAdd.Visible = true;
                    tsmiDelete.Visible = true;
                }
            }
        }

        private void tsmiDown_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.CurrentRow == null)
            {
                return;
            }

            string path = CommonCtrl.IsNullToString(dgvAttachment.CurrentRow.Cells[colAttPath.Name].Value);
            if (File.Exists(path))
            {


            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = path;
                string extension = Path.GetExtension(path);
                saveFileDialog.Filter = "所有文件（*）|*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Stream stream = FileOperation.DownLoadFile(path);
                    if (stream == null || !stream.CanRead)
                    {
                        return;
                    }
                    try
                    {
                        using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            const int bufferLength = 4096;
                            byte[] myBuffer = new byte[bufferLength];//数据缓冲区
                            int count;
                            while ((count = stream.Read(myBuffer, 0, bufferLength)) > 0)
                            {
                                fileStream.Write(myBuffer, 0, count);
                            }
                            fileStream.Close();
                            stream.Close();
                        }
                        MessageBoxEx.Show("下载成功！");
                    }
                    catch (Exception ex)
                    {
                        MessageBoxEx.Show("下载失败！");
                    }
                }
            }
        }
    }
}
