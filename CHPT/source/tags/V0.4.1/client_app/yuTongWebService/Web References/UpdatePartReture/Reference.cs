﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.1026
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.1026 版自动生成。
// 
#pragma warning disable 1591

namespace yuTongWebService.UpdatePartReture {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="partRetureUpdateBinding", Namespace="http://isg.yutong.com/partRetureUpdate/")]
    public partial class partRetureUpdateService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback partRetureUpdateOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public partRetureUpdateService() {
            this.Url = "https://isgqas.yutong.com:2222/ISG/ws/invokeService";
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event partRetureUpdateCompletedEventHandler partRetureUpdateCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://isg.yutong.com/partRetureUpdate/", ResponseNamespace="http://isg.yutong.com/partRetureUpdate/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("Result", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Result partRetureUpdate([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string RequestUser, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string RequestTime, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string RequestType, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] partReturn partReturn, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] clientInfo clientInfo) {
            object[] results = this.Invoke("partRetureUpdate", new object[] {
                        RequestUser,
                        RequestTime,
                        RequestType,
                        partReturn,
                        clientInfo});
            return ((Result)(results[0]));
        }
        
        /// <remarks/>
        public void partRetureUpdateAsync(string RequestUser, string RequestTime, string RequestType, partReturn partReturn, clientInfo clientInfo) {
            this.partRetureUpdateAsync(RequestUser, RequestTime, RequestType, partReturn, clientInfo, null);
        }
        
        /// <remarks/>
        public void partRetureUpdateAsync(string RequestUser, string RequestTime, string RequestType, partReturn partReturn, clientInfo clientInfo, object userState) {
            if ((this.partRetureUpdateOperationCompleted == null)) {
                this.partRetureUpdateOperationCompleted = new System.Threading.SendOrPostCallback(this.OnpartRetureUpdateOperationCompleted);
            }
            this.InvokeAsync("partRetureUpdate", new object[] {
                        RequestUser,
                        RequestTime,
                        RequestType,
                        partReturn,
                        clientInfo}, this.partRetureUpdateOperationCompleted, userState);
        }
        
        private void OnpartRetureUpdateOperationCompleted(object arg) {
            if ((this.partRetureUpdateCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.partRetureUpdateCompleted(this, new partRetureUpdateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1026")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://isg.yutong.com/partRetureUpdate/")]
    public partial class partReturn {
        
        private string info_status_ytField;
        
        private string crm_old_bill_numField;
        
        private string sap_codeField;
        
        private string create_time_startField;
        
        private string create_time_endField;
        
        private partReturnPartDetail[] partDetailsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string info_status_yt {
            get {
                return this.info_status_ytField;
            }
            set {
                this.info_status_ytField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string crm_old_bill_num {
            get {
                return this.crm_old_bill_numField;
            }
            set {
                this.crm_old_bill_numField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string sap_code {
            get {
                return this.sap_codeField;
            }
            set {
                this.sap_codeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string create_time_start {
            get {
                return this.create_time_startField;
            }
            set {
                this.create_time_startField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string create_time_end {
            get {
                return this.create_time_endField;
            }
            set {
                this.create_time_endField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("PartDetail", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public partReturnPartDetail[] PartDetails {
            get {
                return this.partDetailsField;
            }
            set {
                this.partDetailsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1026")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://isg.yutong.com/partRetureUpdate/")]
    public partial class partReturnPartDetail {
        
        private string parts_idField;
        
        private string service_noField;
        
        private string car_parts_codeField;
        
        private string parts_remarkField;
        
        private string change_numField;
        
        private string send_numField;
        
        private string process_modeField;
        
        private string remarkField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string parts_id {
            get {
                return this.parts_idField;
            }
            set {
                this.parts_idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string service_no {
            get {
                return this.service_noField;
            }
            set {
                this.service_noField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string car_parts_code {
            get {
                return this.car_parts_codeField;
            }
            set {
                this.car_parts_codeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string parts_remark {
            get {
                return this.parts_remarkField;
            }
            set {
                this.parts_remarkField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string change_num {
            get {
                return this.change_numField;
            }
            set {
                this.change_numField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string send_num {
            get {
                return this.send_numField;
            }
            set {
                this.send_numField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string process_mode {
            get {
                return this.process_modeField;
            }
            set {
                this.process_modeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string remark {
            get {
                return this.remarkField;
            }
            set {
                this.remarkField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1026")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://isg.yutong.com/partRetureUpdate/")]
    public partial class Result {
        
        private string stateField;
        
        private string errorMsgField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string state {
            get {
                return this.stateField;
            }
            set {
                this.stateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string errorMsg {
            get {
                return this.errorMsgField;
            }
            set {
                this.errorMsgField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1026")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://isg.yutong.com/partRetureUpdate/")]
    public partial class clientInfo {
        
        private string clientIDField;
        
        private string serviceIDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string clientID {
            get {
                return this.clientIDField;
            }
            set {
                this.clientIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string serviceID {
            get {
                return this.serviceIDField;
            }
            set {
                this.serviceIDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void partRetureUpdateCompletedEventHandler(object sender, partRetureUpdateCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class partRetureUpdateCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal partRetureUpdateCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Result Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Result)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591