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

namespace yuTongWebService.CreatePartReturn {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="partReturnCreateBinding", Namespace="http://isg.yutong.com/partReturnCreate/")]
    public partial class partReturnCreateService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback partReturnCreateOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public partReturnCreateService() {
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
        public event partReturnCreateCompletedEventHandler partReturnCreateCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://isg.yutong.com/partReturnCreate/", ResponseNamespace="http://isg.yutong.com/partReturnCreate/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("Result", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Result partReturnCreate([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string RequestUser, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string RequestTime, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string RequestType, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string create_time_start, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string create_time_end, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] clientInfo clientInfo) {
            object[] results = this.Invoke("partReturnCreate", new object[] {
                        RequestUser,
                        RequestTime,
                        RequestType,
                        create_time_start,
                        create_time_end,
                        clientInfo});
            return ((Result)(results[0]));
        }
        
        /// <remarks/>
        public void partReturnCreateAsync(string RequestUser, string RequestTime, string RequestType, string create_time_start, string create_time_end, clientInfo clientInfo) {
            this.partReturnCreateAsync(RequestUser, RequestTime, RequestType, create_time_start, create_time_end, clientInfo, null);
        }
        
        /// <remarks/>
        public void partReturnCreateAsync(string RequestUser, string RequestTime, string RequestType, string create_time_start, string create_time_end, clientInfo clientInfo, object userState) {
            if ((this.partReturnCreateOperationCompleted == null)) {
                this.partReturnCreateOperationCompleted = new System.Threading.SendOrPostCallback(this.OnpartReturnCreateOperationCompleted);
            }
            this.InvokeAsync("partReturnCreate", new object[] {
                        RequestUser,
                        RequestTime,
                        RequestType,
                        create_time_start,
                        create_time_end,
                        clientInfo}, this.partReturnCreateOperationCompleted, userState);
        }
        
        private void OnpartReturnCreateOperationCompleted(object arg) {
            if ((this.partReturnCreateCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.partReturnCreateCompleted(this, new partReturnCreateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://isg.yutong.com/partReturnCreate/")]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1026")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://isg.yutong.com/partReturnCreate/")]
    public partial class Result {
        
        private string stateField;
        
        private string errorMsgField;
        
        private string info_status_ytField;
        
        private string old_bill_numField;
        
        private string com_codeField;
        
        private string create_time_startField;
        
        private string create_time_endField;
        
        private ResultPartDetail[] partDetailsField;
        
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
        public string old_bill_num {
            get {
                return this.old_bill_numField;
            }
            set {
                this.old_bill_numField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string com_code {
            get {
                return this.com_codeField;
            }
            set {
                this.com_codeField = value;
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
        public ResultPartDetail[] PartDetails {
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://isg.yutong.com/partReturnCreate/")]
    public partial class ResultPartDetail {
        
        private string service_noField;
        
        private string car_parts_codeField;
        
        private string parts_remarksField;
        
        private string change_numField;
        
        private string send_numField;
        
        private string process_modeField;
        
        private string remarksField;
        
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
        public string parts_remarks {
            get {
                return this.parts_remarksField;
            }
            set {
                this.parts_remarksField = value;
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
        public string remarks {
            get {
                return this.remarksField;
            }
            set {
                this.remarksField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void partReturnCreateCompletedEventHandler(object sender, partReturnCreateCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class partReturnCreateCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal partReturnCreateCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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