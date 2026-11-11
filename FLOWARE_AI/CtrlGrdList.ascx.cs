using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace FLOWARE_AI
{
    public partial class CtrlGrdList : System.Web.UI.UserControl
    {
        int nGridHeight = 150, nGridRowCount = 25;
        private string nGridWidth = "0", _StrPlaceHoldr = "", _strDestinationCtrl = "", nGridTextWidth = "0";
        public MenuType Type;
        private Boolean bIsMultiLine = false;
        private static int totalPages = 10;
        public enum MenuType
        {
            
            CustomerList,        
            ItemRegList,
            CustomerCodeList,
            ItemCodeList,
            UomList
        }
        [DisplayName("Choose Model")]
        public MenuType AccountType
        {
            get
            {
                return Type;
            }
            set
            {
                Type = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            TxtCaption.Attributes.Add("autocomplete", "off");

            
            if (Type.ToString().Equals("UomList"))
            {
                HdnFlag.Value = "UOM";
            }
            
            else if (Type.ToString().Equals("CustomerList"))
            {
                HdnFlag.Value = "CUST";
            }            
            else if (Type.ToString().Equals("ItemRegList"))
            {
                HdnFlag.Value = "ITM";
            }            
            else if (Type.ToString().Equals("CustomerCodeList"))
            {
                HdnFlag.Value = "CUSTC";
            }
            else if (Type.ToString().Equals("ItemCodeList"))
            {
                HdnFlag.Value = "ITMC";
            }
           
            
            if (!IsPostBack) //
            {
                try
                {
                    //HdnCmpId.Value = Request.QueryString["CID"].ToString();
                    //HdnBrId.Value = Request.QueryString["BID"].ToString();
                    //HdnFaId.Value = Request.QueryString["FID"].ToString();
                    //HdnUserId.Value = Request.QueryString["UID"].ToString();

                    HdnCmpId.Value = "1";
                    HdnBrId.Value = "1";
                    HdnFaId.Value = "1";
                    HdnUserId.Value = "1";
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                }
            }
            if (IsMultiLineBox == true)
            {
                TxtCaption.TextMode = TextBoxMode.MultiLine;
                TxtCaption.CssClass = "textbox_Multilinefield_style";
            }


            TABLE1.Visible = true;
            TABLE1.Attributes["style"] = (GridWidth == "0" ? "right:0px; width:100%;" : "width:" + GridWidth + "px !important");
            TxtCaption.Attributes["style"] += (GridTextWidth == "0" ? "width:100%;" : "width:" + GridTextWidth + "px !important");
            //TABLE1.Width = (GridWidth.ToString() == "0" ? " style='right:0px; width:100%;" : " style = 'width:" + GridWidth + "px!important");
            //TxtCaption.Attributes.Add("width", (GridTextWidth.ToString() == "0" ? " style='width:100%;" : " style = 'width:" + GridTextWidth + "px!important"));
            TABLE1.Height = GridHeight.ToString();

            hdnGridRowCount.Value = GridRowCount.ToString();
            devGridList.Style.Value = "overflow: none; width: 100%; height: 1000px; position: static;";
            //HiddenAccType.Value = Type
            DivLst.Attributes.Add("onchange", "toggleImgDiv('" + this.ClientID + "',event);");
            DivLst.Attributes.Add("onClick", "toggleImgDiv('" + this.ClientID + "',event);");

            //TxtCaption.Attributes.Add("onkeyup", "txtOnChange('" + this.ClientID + "',event);");
            //TxtCaption.Attributes.Add("onblur", "FnOnHide('" + this.ClientID + "');");
            TxtCaption.Attributes.Add("placeholder", PlaceHoldr);

            TxtCaption.Attributes.Add("onkeydown","return txtOnKeyDown('" + this.ClientID + "',event);");
            TxtCaption.Attributes.Add("onkeyup","txtOnChange('" + this.ClientID + "',event);");
        }
        public string PlaceHoldr
        {
            get { return _StrPlaceHoldr; }
            set { _StrPlaceHoldr = value; }
        }
        public string GridTextWidth
        {
            get { return nGridTextWidth; }
            set { nGridTextWidth = value; }
        }
        public Boolean IsMultiLineBox
        {
            get { return bIsMultiLine; }
            set { bIsMultiLine = value; }
        }
        public int GridHeight
        {
            get { return nGridHeight; }
            set { nGridHeight = value; }
        }
        public string GridWidth
        {
            get { return nGridWidth; }
            set { nGridWidth = value; }
        }
        public int GridRowCount
        {
            get { return nGridRowCount; }
            set { nGridRowCount = value; }
        }
        public short TabIndex
        {
            set
            { TxtCaption.TabIndex = value; }
        }
        public TextBox ControlTextBox
        {
            get
            { return TxtCaption; }
        }
        public TextBox InnerTextBox
        {
            get { return TxtCaption; }
        }
        public void SetNextControl(string PrmNextControlClientID)
        {
            InnerTextBox.Attributes["data-next"] = PrmNextControlClientID;
        }

        public string SelectedText
        {
            get
            { return TxtCaption.Text.Trim(); }
            set
            { TxtCaption.Text = value; }
        }
        public string SelectedValue
        {
            get
            { return HdnId.Value.Trim(); }
            set
            { HdnId.Value = value; }
        }
        public string SelectedCode
        {
            get
            { return HdnCode.Value.Trim(); }
            set
            { HdnCode.Value = value; }
        }
        public string ParentControl
        {
            set
            { HdnParent.Value = value; }
        }
        public string ParentSubControl
        {
            set
            { HdnSubParent.Value = value; }
        }
        public string IdControl
        {
            get
            { return HdnId.ClientID; }
        }
        public string TextControl
        {
            get
            { return TxtCaption.ClientID; }
        }
        public string CurrencyIdControl
        {
            get
            { return HdnCurrencyId.ClientID; }
        }
        public Boolean FnIsEnable
        {
            set
            {
                TxtCaption.Enabled = value;
                if (value == true)
                {
                    TxtCaption.BackColor = System.Drawing.Color.White;
                }
                else
                {
                    TxtCaption.BackColor = System.Drawing.ColorTranslator.FromHtml("#fddcfd");
                }
            }
        }
        public Boolean IsCurrencySelect
        {
            set
            { HdnIsCurrency.Value = value.ToString(); }
        }
        public string UserIdCtrl
        {
            get
            { return HdnUserId.Value; }
            set
            { HdnUserId.Value = value; }
        }
        public string DestinationControls
        {
            get
            { return HdnDestCtrl.Value; }
            set
            { HdnDestCtrl.Value = value; }
        }
        public string HdnMeterReadingControl
        {
            get
            { return HdnMeterReading.ClientID; }
        }
        public string VehicleMeterReading
        {
            get
            { return HdnMeterReading.Value; }
            set
            { HdnMeterReading.Value = value; }
        }
        public void FnClearControls()
        {
            TxtCaption.Text = "";
            HdnId.Value = "0";
            HdnItmGrpId.Value = "0";
            HdnItmSubGrpId.Value = "0";
            HdnItmBrndId.Value = "0";
            HdnAvgWt.Value = "0";
        }
        protected void HdnSubParent_ValueChanged(object sender, EventArgs e)
        {

        }
        //==================
        public string SelectedItemGrpId
        {
            get
            { return HdnItmGrpId.Value.Trim(); }
            set
            { HdnItmGrpId.Value = value; }
        }
        public string SelectedItemSubGrpId
        {
            get
            { return HdnItmSubGrpId.Value.Trim(); }
            set
            { HdnItmSubGrpId.Value = value; }
        }
        public string SelectedItemBrndId
        {
            get
            { return HdnItmBrndId.Value.Trim(); }
            set
            { HdnItmBrndId.Value = value; }
        }
        public string SelectedCurrencyId
        {
            get
            { return HdnCurrencyId.Value.Trim(); }
            set
            { HdnCurrencyId.Value = value; }
        }
        public string SelectedItemAvgWt
        {
            get
            { return HdnAvgWt.Value.Trim(); }
            set
            { HdnAvgWt.Value = value; }
        }
    }
}