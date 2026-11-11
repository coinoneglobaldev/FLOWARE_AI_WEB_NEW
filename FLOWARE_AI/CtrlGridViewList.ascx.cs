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
    public partial class CtrlGridViewList : System.Web.UI.UserControl
    {
        #region Private Fields
        int nGridHeight = 150, nGridRowCount = 25;
        private string nGridWidth = "0", _StrPlaceHoldr = "", _strDestinationCtrl = "", nGridTextWidth = "0";
        public MenuType Type;
        private Boolean bIsMultiLine = false;
        private static int totalPages = 10;

        // GridView-specific properties
        private string _rowIndex = "";
        private string _gridViewClientID = "";
        #endregion

        #region Public Enum
        public enum MenuType
        {
            CustomerList,
            UomList,
            ItemRegList,
            CustomerCodeList,
            ItemCodeList
            
        }
        #endregion

        #region Public Properties - Existing
        [DisplayName("Choose Model")]
        public MenuType AccountType
        {
            get { return Type; }
            set { Type = value; }
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
            set { TxtCaption.TabIndex = value; }
        }

        public TextBox ControlTextBox
        {
            get { return TxtCaption; }
        }

        public TextBox InnerTextBox
        {
            get { return TxtCaption; }
        }

        public string SelectedText
        {
            get { return TxtCaption.Text.Trim(); }
            set { TxtCaption.Text = value; }
        }

        public string SelectedValue
        {
            get { return HdnId.Value.Trim(); }
            set { HdnId.Value = value; }
        }

        public string ParentControl
        {
            set { HdnParent.Value = value; }
        }

        public string ParentSubControl
        {
            set { HdnSubParent.Value = value; }
        }

        public string IdControl
        {
            get { return HdnId.ClientID; }
        }

        public string TextControl
        {
            get { return TxtCaption.ClientID; }
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

        public string UserIdCtrl
        {
            get { return HdnUserId.Value; }
            set { HdnUserId.Value = value; }
        }

        public string DestinationControls
        {
            get { return HdnDestCtrl.Value; }
            set { HdnDestCtrl.Value = value; }
        }
        #endregion

        #region GridView-Specific Properties (NEW)
        public string RowIndex
        {
            get { return _rowIndex; }
            set
            {
                _rowIndex = value;
                // Auto-generate unique wrapper ID for GridView rows
                if (!string.IsNullOrEmpty(value))
                {
                    gridWrapper.ClientIDMode = ClientIDMode.Static;
                    gridWrapper.ID = $"gridWrapper_{value}";
                }
            }
        }
        // JS to run after a value is picked
        public string OnClientSelect
        {
            set
            {
                TxtCaption.Attributes["data-onselect"] = value;
                TxtCaption.Attributes["data-hdn"] = HdnId.ClientID;
            }
        }
        public string GridViewClientID
        {
            get { return _gridViewClientID; }
            set { _gridViewClientID = value; }
        }

        public string GetWrapperClientID()
        {
            return gridWrapper?.ClientID ?? $"gridWrapper_{_rowIndex}";
        }
        #endregion

        #region Page_Load (Enhanced for GridView)
        protected void Page_Load(object sender, EventArgs e)
        {
            TxtCaption.Attributes.Add("autocomplete", "off");

            // Set HdnFlag based on AccountType
            SetHdnFlag();

            if (!IsPostBack)
            {
                try
                {
                    // GridView context initialization
                    if (!string.IsNullOrEmpty(RowIndex))
                    {
                        HdnParent.Value = GridViewClientID ?? "";

                        // Set QueryString values if available (for non-GridView usage)
                        if (!string.IsNullOrEmpty(Request.QueryString["CID"]))
                            HdnCmpId.Value = Request.QueryString["CID"];
                        if (!string.IsNullOrEmpty(Request.QueryString["BID"]))
                            HdnBrId.Value = Request.QueryString["BID"];
                        if (!string.IsNullOrEmpty(Request.QueryString["FID"]))
                            HdnFaId.Value = Request.QueryString["FID"];
                        if (!string.IsNullOrEmpty(Request.QueryString["UID"]))
                            HdnUserId.Value = Request.QueryString["UID"];
                    }
                }
                catch (Exception ex)
                {
                    // Log error silently
                    System.Diagnostics.Debug.WriteLine("CtrlGridViewList Error: " + ex.Message);
                }
            }

            // Apply styling
            ApplyStyling();

            // Set placeholder
            TxtCaption.Attributes.Add("placeholder", PlaceHoldr);
        }
        #endregion

        #region Helper Methods
        private void SetHdnFlag()
        {
            string typeStr = Type.ToString();
            switch (typeStr)
            {
                case "AccountCrList": HdnFlag.Value = "CR"; break;
                case "AccountLedger": HdnFlag.Value = "ACC"; break;
                case "AllAccountLedger": HdnFlag.Value = "ALLACC"; break;
                case "AccountDrList": HdnFlag.Value = "CP"; break;
                case "AccountJrList": HdnFlag.Value = "JR"; break;
                case "BankAccountList": HdnFlag.Value = "BNK"; break;
                case "CashAccountList": HdnFlag.Value = "CSH"; break;
                case "AgentList": HdnFlag.Value = "AGNT"; break;
                case "SupplierList": HdnFlag.Value = "SUP"; break;
                case "SupplierLedger": HdnFlag.Value = "SUP_LGR"; break;
                case "SupplierType": HdnFlag.Value = "SUP_GRP"; break;
                case "CustomerType": HdnFlag.Value = "CUST_GRP"; break;
                case "EmployeeList": HdnFlag.Value = "EMP"; break;
                case "UomList": HdnFlag.Value = "UOM"; break;
                case "WtsapGpList": HdnFlag.Value = "WPGRP"; break;
                case "CustomerList": HdnFlag.Value = "CUST"; break;
                case "CustomerLedger": HdnFlag.Value = "CUST_LGR"; break;
                case "ItemGroupList": HdnFlag.Value = "GRP"; break;
                case "AccGroupList": HdnFlag.Value = "ACCGRP"; break;
                case "CountryList": HdnFlag.Value = "CNTY"; break;
                case "LocationList": HdnFlag.Value = "LOC"; break;
                case "PaymentTerms": HdnFlag.Value = "PAY"; break;
                case "ItemRegList": HdnFlag.Value = "ITM"; break;
                case "ItemBrandList": HdnFlag.Value = "BRND"; break;
                case "ItmSubGrpList": HdnFlag.Value = "SUBGRP"; break;
                case "TaxList": HdnFlag.Value = "ALTAX"; break;
                case "CurrencyList": HdnFlag.Value = "CUR"; break;
                case "AllCountryList": HdnFlag.Value = "ALCN"; break;
                case "DesignationList": HdnFlag.Value = "DESG"; break;
                case "BarCodeList": HdnFlag.Value = "BATCH"; break;
                case "AssetList": HdnFlag.Value = "ASSET"; break;
                case "UserGroupList": HdnFlag.Value = "UGRP"; break;
                case "EnquiryCustNameList": HdnFlag.Value = "ENQY"; break;
                case "EnquiryCompList": HdnFlag.Value = "ENQYC"; break;
                case "VehicleNoList": HdnFlag.Value = "VECH"; break;
                case "PurchaseReceiptList": HdnFlag.Value = "PIR"; break;
                case "DepartmentList": HdnFlag.Value = "DEPT"; break;
                case "DepartmentSubList": HdnFlag.Value = "SDEPT"; break;
                case "ShippingList": HdnFlag.Value = "SHPG"; break;
                case "BankCashAccountList": HdnFlag.Value = "BNKCSH"; break;
                case "ProductionRawMaterial": HdnFlag.Value = "PRM"; break;
                case "CostcentreMainList": HdnFlag.Value = "MCSTR"; break;
                case "CostcentreSubList": HdnFlag.Value = "SCSTR"; break;
                case "CostcentreList": HdnFlag.Value = "CSTR"; break;
                case "EmpBankNameList": HdnFlag.Value = "EMPBNK"; break;
                case "EmpBankBranchList": HdnFlag.Value = "EMPBRNCH"; break;
                case "AssetGroupList": HdnFlag.Value = "ASST"; break;
                case "ExpenseGroupList": HdnFlag.Value = "EXPE"; break;
                case "IncomecGroupList": HdnFlag.Value = "INCM"; break;
                case "LiabilitiGroupList": HdnFlag.Value = "LBTY"; break;
                case "HrDepartmentList": HdnFlag.Value = "HRRPT"; break;
                case "ProfileList": HdnFlag.Value = "PRF_LST"; break;
                case "BilletList": HdnFlag.Value = "BLT_LST"; break;
                case "IngotsList": HdnFlag.Value = "IGT_LST"; break;
                case "ProductionItemList": HdnFlag.Value = "PRDITM"; break;
                case "ItemTypeList": HdnFlag.Value = "ITMTYPE"; break;
                case "StateList": HdnFlag.Value = "ST"; break;
                case "CustomerCodeList": HdnFlag.Value = "CUSTC"; break;
                case "ItemCodeList": HdnFlag.Value = "ITMC"; break;
                case "SupplierCodeList": HdnFlag.Value = "SUPC"; break;
                case "BankAccCodeList": HdnFlag.Value = "BNKCD"; break;
            }
        }
        private void ApplyStyling()
        {
            if (IsMultiLineBox)
            {
                TxtCaption.TextMode = TextBoxMode.MultiLine;
                TxtCaption.CssClass = "textbox_Multilinefield_style";
            }

            // ✅ NOW SAFE - TABLE1 has runat="server"
            TABLE1.Style.Add("width", (GridWidth == "0" ? "100%" : GridWidth + "px"));
            TxtCaption.Style.Add("width", (GridTextWidth == "0" ? "100%" : GridTextWidth + "px"));
            hdnGridRowCount.Value = GridRowCount.ToString();
        }


        #endregion

        #region Public Methods
        public void SetNextControl(string PrmNextControlClientID)
        {
            InnerTextBox.Attributes["data-next"] = PrmNextControlClientID;
        }
        #endregion

        #region Event Handlers
        protected void HdnSubParent_ValueChanged(object sender, EventArgs e)
        {
            // Handle if needed
        }
        #endregion
    }
}
