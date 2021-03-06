﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HXC.UI.Library.Content;
using HXC.UI.Library.Verify;

namespace HXC.UI.Library.Controls
{
    /// <summary>
    /// ExtComboBox
    /// </summary>
    /// <versioning>
    ///     <version number="1.0.0.0">
    ///         <author>Kord Kuo</author> 
    ///         <datetime>2015/1/12 15:53:22</datetime>
    ///         <comment>create</comment>
    ///     </version>
    /// </versioning>
    public partial class ExtComboBox : ComboBox, IContentControl, IBorderStyle, INotifyPropertyChanged
    {
        #region Constructor -- 构造函数
        public ExtComboBox()
        {
            InitializeComponent();

            InitEvent();

            InitProperty();
        }
        #endregion

        #region Field -- 字段
        internal Object _oldValue;
        internal Color _defaultBorderColor = DefaultStyle.DefaultBorderColor;
        internal ToolTip _toolTip;
        internal Boolean _hasError;
        internal Boolean _changBorderColor = true;
        #endregion

        #region Property -- 属性

        #endregion

        #region Method -- 方法
        private void InitEvent()
        {
            SelectedIndexChanged += ExtComboBox_SelectedIndexChanged;
            DrawItem += ComboBox_DrawItem;
        }
        private void InitProperty()
        {
            
        }
        internal void ShowErrorTip()
        {
            if (!_hasError) return;
            _changBorderColor = false;
            BorderColor = Color.Red;
            _changBorderColor = true;

            if (!ShowError || !_hasError) return;
            if (_toolTip == null) _toolTip = new ToolTip();
            _toolTip.Show(VerifyManager.GetVerifyMessage(this), this, Size.Width, 0, 3000);
        }
        internal void RemoveErrorTip()
        {
            if (!_hasError) return;
            if (ShowError && _toolTip != null)
            {
                _toolTip.Hide(this);
            }

            _changBorderColor = false;
            BorderColor = _defaultBorderColor;
            _changBorderColor = true;
        }
        internal void HideErrorTip()
        {
            if (!_hasError) return;
            if (ShowError && _toolTip != null)
            {
                _toolTip.Hide(this);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        public override string ToString()
        {
            return Value == null ? String.Empty : Value.ToString();
        }
        #endregion

        #region Event -- 事件
        private void ExtComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Equals(Value, SelectedValue))
            {
                Value = SelectedValue;
            }
        }
        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            var foreBrush = new SolidBrush(e.ForeColor);
            e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, foreBrush, e.Bounds,
                StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }
        #endregion

        #region Interface -- 接口
        #region INotifyPropertyChanged -- 属性更改通知
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            Invalidate(true);
            var handler = PropertyChanged;
            if (handler == null) return;
            handler(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region IBorderStyle -- 控件边框
        private int _borderWidth = 1;

        [Browsable(false)]
        [Category("Extensions")]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                if (_borderWidth == value) return;
                _borderWidth = value;
                var handler = BorderWidthChanged;
                if (handler == null) return;
                handler(this, null);
                OnPropertyChanged("BorderWidth");
            }
        }

        private Color _borderColor = DefaultStyle.DefaultBorderColor;

        [Browsable(false)]
        [Category("Extensions")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (Equals(_borderColor != value)) return;
                _borderColor = value;
                OnPropertyChanged("BorderColor");
            }
        }

        private int _cornerRadiu = 5;

        [Browsable(false)]
        [Category("Extensions")]
        public int CornerRadiu
        {
            get { return _cornerRadiu; }
            set
            {
                if (_cornerRadiu == value) return;
                _cornerRadiu = value;
                OnPropertyChanged("CornerRadiu");
            }
        }

        [Browsable(false)]
        [Category("Extensions")]
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(new Point(BorderWidth / 2, BorderWidth / 2),
                    new Size(Size.Width - BorderWidth, Size.Height - BorderWidth));
            }
        }

        public event EventHandler BorderWidthChanged;
        #endregion

        #region IContentControl -- 内容控件接口
        private object _value;
        [Browsable(true)]
        [Category("Extensions")]
        public object Value
        {
            get { return _value; }
            set
            {
                if (Equals(value, _value)) return;
                _value = value;
                if(!Equals(SelectedValue, value))
                {
                    if (value == null) 
                        SelectedIndex = -1;
                    else
                        SelectedIndex = Items.IndexOf(value);
                }
                var valueChanged = ValueChanged;
                if (valueChanged != null)
                {
                    ValueChanged.Invoke(this, null);
                }
            }
        }
        [Browsable(true)]
        [Category("Extensions")]
        public string DisplayValue
        {
            get { return Text; }
            set { Text = value; }
        }
        public void EmptyValue()
        {
            SelectedIndex = -1;
        }

        /// <summary>
        /// 控件内容
        /// </summary>
        [Browsable(true)]
        [Category("Extensions")]
        [Description("控件内容")]
        public object Content
        {
            get { return DataSource; }
            set { DataSource = value; }
        }

        private String _contentTypeName = String.Empty;

        /// <summary>
        /// 控件内容类型名称
        /// </summary>
        [Browsable(true)]
        [Category("Extensions")]
        [Description("控件内容类型名称")]
        public String ContentTypeName
        {
            get { return _contentTypeName; }
            set
            {
                _contentTypeName = value;
                ValueMember = "Value";
                DisplayMember = "Text";
                ContentTypeManager.SetContent(this);
            }
        }

        private Object _contentTypeParameter;
        /// <summary>
        /// 控件内容类型参数
        /// </summary>
        [Browsable(true)]
        [Category("Extensions")]
        [Description("控件内容类型参数")]
        public Object ContentTypeParameter
        {
            get { return _contentTypeParameter; }
            set
            {
                _contentTypeParameter = value;
                ValueMember = "Value";
                DisplayMember = "Text";
                ContentTypeManager.SetContent(this);
            }
        }
        public bool Verify(bool showError = false)
        {
            var result = VerifyManager.Verify(this);
            if (!result && showError)
            {
                _hasError = true;
                ShowErrorTip();
            }
            else
            {
                RemoveErrorTip();
                _hasError = false;
            }
            return result;
        }
        public bool InputtingVerify(bool showError = false)
        {
            return VerifyManager.InputtingVerify(this);
        }
        public bool ShowError { get; set; }
        public string VerifyTypeName { get; set; }
        public Type VerifyType { get; set; }
        public string VerifyCondition { get; set; }
        public string InputtingVerifyCondition { get; set; }
        public event EventHandler ValueChanged;
        #endregion
        #endregion
    }

    /// <summary>
    /// 选择项类，用于ComboBox或者ListBox添加项
    /// </summary>
    [Serializable]
    public class ListItem
    {
        public ListItem(object value, string text)
        {
            Value = value;
            Text = text;
        }
        public override string ToString()
        {
            return Value == null ? String.Empty : Value.ToString();
        }
        public object Value { get; set; }

        public string Text { get; set; }
    }
}
