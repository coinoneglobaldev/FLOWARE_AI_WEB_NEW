using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FLOWARE_AI
{
    public partial class CtrlDate : System.Web.UI.UserControl
    {
        ClsCommonVariables ObjCls = new ClsCommonVariables();
        Boolean bVal = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            TxtDate.Attributes.Add("autocomplete", "off");
            CDdlr.Controls.Add(ObjCls.FnShowCalender("TxtDate", "Img1"));
            if (!IsPostBack)
            {
                if (IsVisibleDate == true)
                {
                    DateText = DateTime.Now.ToString("dd/MMM/yyyy").Replace("-", "/");//HH:mm:ss
                }
            }
        }
        public CalendarExtender FnShowCalenderNew(string PrmTargetControlID, string PrmPopupButtonID)
        {
            CalendarExtender Clndr = new CalendarExtender();
            Clndr.TargetControlID = PrmTargetControlID;
            Clndr.PopupButtonID = PrmPopupButtonID;
            Clndr.OnClientDateSelectionChanged = "FnGetDateValue";
            Clndr.Format = "dd/MMM/yyyy";
            return Clndr;
        }
        public void FnNewDate()
        {
            TxtDate.Text = DateTime.Now.ToString("dd/MMM/yyyy").Replace("-", "/");
        }
        public TextBox ControlTextBox
        {
            get
            { return TxtDate; }
        }
        public Boolean IsVisibleDate
        {
            get
            {
                return bVal;
            }
            set
            {
                bVal = value;
            }
        }
        public Boolean IsReadOnlyDate
        {
            set
            {
                TxtDate.ReadOnly = value;
            }
        }
        public string DateText
        {
            get
            {
                string strDate = TxtDate.Text.Trim();
                return (TxtDate.Text.Trim().Length > 0 ? strDate : "");
            }
            set
            {
                TxtDate.Text = value.ToString();
            }
        }
        public string TextControl
        {
            get
            { return TxtDate.ClientID; }
        }
        public Boolean FnIsEnable
        {
            set
            {
                TxtDate.Enabled = value;
                if (value == true)
                {
                    TxtDate.BackColor = System.Drawing.Color.White;
                }
                else
                {
                    TxtDate.BackColor = System.Drawing.ColorTranslator.FromHtml("#fddcfd");
                }
            }
        }
    }
}