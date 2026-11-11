
using FLOWARE_AI.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FLOWARE_AI.ADMIN
{
    public partial class MapItems : ClsPageEvents
    {
        ClsAIMailDetails objMail = new ClsAIMailDetails();
        ClsCustomerItemMapping ObjCls = new ClsCustomerItemMapping();
        ClsOrderParsing ObjParse = new ClsOrderParsing();

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                   LoadMappingData(Request.QueryString["MailId"]);
                    
                }
                RegisterGridControlMasterScript();
                CtrlGrdCustCode.DestinationControls = CtrlGrdCustomer.IdControl + "," + CtrlGrdCustomer.TextControl;
                CtrlGrdCustomer.DestinationControls = CtrlGrdCustCode.IdControl + "," + CtrlGrdCustCode.TextControl;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        private void RegisterGridControlMasterScript()
        {
            try
            {
                string cMasterScript = FnRegisterGridControlMasterScript();
                ScriptManager.RegisterStartupScript(this, GetType(), "grid_master_" + DateTime.Now.Ticks, cMasterScript, true);
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }

        }
        private void LoadMappingData(string mailId)
        {
            try
            {
                objMail.MailId = ObjCls.FnIsNumeric(Request.QueryString["MailId"]);
                objMail.FromMail = Request.QueryString["From"].ToString();
                objMail.TokenNo=ObjCls.FnIsDouble(Request.QueryString["TokenNo"].ToString());
                objMail.AICustomerName = LblCustName.Text.Trim();
                objMail.IsEdit = 0;
                ViewState["DT_CHILD"] = objMail.FnGetOrderData();
                DataTable Dt = ViewState["DT_CHILD"] as DataTable;
                if (Dt != null && Dt.Rows.Count > 0)
                {
                    DataRow firstRow = Dt.Rows[0];
                    LblCustName.Text = Convert.ToString(firstRow["AICustomerName"]);
                    TxtPOp.Text = Convert.ToString(firstRow["OrderNo"]);
                    CtrlOrderDate.DateText = firstRow["OrderDate"] != DBNull.Value ? Convert.ToDateTime(firstRow["OrderDate"]).ToString("dd/MMM/yyyy") : "";
                    CtrlDelDate.DateText = firstRow["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(firstRow["DeliveryDate"]).ToString("dd/MMM/yyyy") : "";

                    CtrlGrdCustCode.SelectedValue= Convert.ToString(firstRow["CustomerId"]);
                    CtrlGrdCustCode.SelectedText = Convert.ToString(firstRow["CustomerCode"]);

                    CtrlGrdCustomer.SelectedValue = Convert.ToString(firstRow["CustomerId"]);
                    CtrlGrdCustomer.SelectedText = Convert.ToString(firstRow["CustomerName"]);
                    FnGridViewBinding("");
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void BtnApplyMapping_Click1(object sender, EventArgs e)
        {
            try
            {
                if (CtrlGrdCustomer.SelectedValue == "0")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select a valid customer'); }, 100);", true);
                    return;
                }
                if (CtrlOrderDate.DateText == "")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select valid order date'); }, 100);", true);
                    return;
                }
                if (CtrlDelDate.DateText == "")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select valid delivery date'); }, 100);", true);
                    return;
                }

                // ---------- PASS 1: validate every row, nothing is saved here ----------
                List<string> noItemRows = new List<string>();
                List<string> zeroQtyRows = new List<string>();

                foreach (GridViewRow row in GrdMapItems.Rows)
                {
                    if (row.RowType != DataControlRowType.DataRow) continue;

                    string rowNo = ((GrdMapItems.PageIndex * GrdMapItems.PageSize) + row.RowIndex + 1).ToString();

                    CtrlGridViewList ctrlItem = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemDtl");
                    TextBox txtQty = (TextBox)row.FindControl("TxtQty");   // change the ID if your quantity control is named differently

                    string itemId = ctrlItem == null ? "" : ctrlItem.SelectedValue;
                    if (string.IsNullOrWhiteSpace(itemId) || ObjCls.FnIsNumeric(itemId) == 0)
                        noItemRows.Add(rowNo);

                    double qty = (txtQty == null) ? 0 : ObjCls.FnIsDouble(txtQty.Text);
                    if (qty == 0)
                        zeroQtyRows.Add(rowNo);
                }

                // 1) Item not chosen: stop, no confirm option
                if (noItemRows.Count > 0)
                {
                    string msg = "Item not chosen at row " + string.Join(", ", noItemRows) + ". Please choose an item to continue.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "itemAlert",
                        "setTimeout(function(){ alert('" + HttpUtility.JavaScriptStringEncode(msg) + "'); }, 100);", true);
                    HdnConfirmZeroQty.Value = "";
                    return;
                }

                // 2) Quantity zero: ask the user, save nothing until they say yes
                if (zeroQtyRows.Count > 0 && HdnConfirmZeroQty.Value != "1")
                {
                    string q = "Quantity at row " + string.Join(", ", zeroQtyRows) + " is zero. Do you want to continue your export?";
                    string js = "setTimeout(function(){ " +
                                "if (confirm('" + HttpUtility.JavaScriptStringEncode(q) + "')) { " +
                                "document.getElementById('" + HdnConfirmZeroQty.ClientID + "').value = '1'; " +
                                "__doPostBack('" + BtnApplyMapping.UniqueID + "', ''); " +   // use your export button's ID here
                                "} else { " +
                                "document.getElementById('" + HdnConfirmZeroQty.ClientID + "').value = ''; " +
                                "} }, 100);";
                    ScriptManager.RegisterStartupScript(this, GetType(), "qtyConfirm", js, true);
                    return;   // nothing saved yet
                }
                HdnConfirmZeroQty.Value = "";   // reset for next time

                // ---------- PASS 2: everything is valid, now save ----------
                ObjCls.CustomerId = ObjCls.FnIsNumeric(CtrlGrdCustomer.SelectedValue);
                ObjCls.MailId = ObjCls.FnIsNumeric(Request.QueryString["MailId"]);
                ObjCls.FromMail = Request.QueryString["From"];
                ObjCls.AICustomerName = LblCustName.Text.Trim();
                ObjCls.TokenNo = ObjCls.FnIsDouble(Request.QueryString["TokenNo"].ToString());

                foreach (GridViewRow row in GrdMapItems.Rows)
                {
                    if (row.RowType != DataControlRowType.DataRow) continue;

                    Label LblCode = (Label)row.FindControl("LblCode");
                    Label LblName = (Label)row.FindControl("LblName");
                    Label LblUom = (Label)row.FindControl("LblUom");
                    CtrlGridViewList CtrlGrdViewItemDtl = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemDtl");
                    CtrlGridViewList CtrlGrdViewUom = (CtrlGridViewList)row.FindControl("CtrlGrdViewUom");
                    TextBox TxtRemarks = (TextBox)row.FindControl("TxtRemarks");

                    ObjCls.ItmCode = LblCode.Text;
                    ObjCls.ItemDesc = LblName.Text;
                    ObjCls.ItmUom = LblUom.Text;
                    ObjCls.Remarks = TxtRemarks.Text.Trim();
                    ObjCls.ItemId = ObjCls.FnIsNumeric(CtrlGrdViewItemDtl.SelectedValue);
                    ObjCls.UomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
                    ObjCls.SplitUomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
                    ObjCls.FnUpdateAIOrderDetails();
                }

                ObjCls.FnUpdateCustomerDetails();
                ObjCls.FnUpdateExportStatus();
                ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Record Inserted Successfully'); }, 100);", true);
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }

        protected void GrdMapItems_PreRender(object sender, EventArgs e)
        {
            try
            {
                base.OnPreRender(e);
                SetupGridChildControls();
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }

        }
        private void SetupGridChildControls()
        {
            try
            {
                CtrlGridViewList CtrlGrdViewItemCode, CtrlGrdViewItemDtl, CtrlGridViewUom;
                foreach (GridViewRow row in GrdMapItems.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CtrlGrdViewItemCode = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemCode");
                        CtrlGrdViewItemDtl = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemDtl");
                        CtrlGridViewUom = (CtrlGridViewList)row.FindControl("CtrlGrdViewUom");

                        if (CtrlGrdViewItemCode != null)
                        {
                            CtrlGrdViewItemCode.DestinationControls = CtrlGrdViewItemDtl.IdControl + "," + CtrlGrdViewItemDtl.TextControl + "," + CtrlGridViewUom.IdControl + "," + CtrlGridViewUom.TextControl;
                        }
                        if (CtrlGrdViewItemDtl != null)
                        {
                            CtrlGrdViewItemDtl.DestinationControls = CtrlGrdViewItemCode.IdControl + "," + CtrlGrdViewItemCode.TextControl + "," + CtrlGridViewUom.IdControl + "," + CtrlGridViewUom.TextControl;
                        }
                        CtrlGridViewUom.ParentControl = CtrlGrdViewItemDtl.IdControl;
                    }
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        public void FnGridViewBinding(string PrmFlag)
        {
            GrdMapItems.DataSource = ViewState["DT_CHILD"] as DataTable;
            //GrdVwRecords.DataKeyNames = new String[] { ObjCls.KeyName };
            GrdMapItems.DataBind();
            GrdMapItems.SelectedIndex = -1;
            
        }
        protected void GrdMapItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                Label LblCode, LblName, LblUom;
                TextBox TxtQty, TxtPrice, TxtRemarks;
                HiddenField HdnAmt;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Button btnDelete = e.Row.FindControl("BtnDelete") as Button;
                    LblCode = e.Row.FindControl("LblCode") as Label;
                    LblName = e.Row.FindControl("LblName") as Label;
                    LblUom = e.Row.FindControl("LblUom") as Label;
                    TxtQty = e.Row.FindControl("TxtQty") as TextBox;
                    HdnAmt = e.Row.FindControl("HdnAmt") as HiddenField;
                    TxtPrice = e.Row.FindControl("TxtPrice") as TextBox;
                    TxtRemarks = e.Row.FindControl("TxtRemarks") as TextBox;

                    CtrlGridViewList CtrlGrdViewItemCode, CtrlGrdViewItemDtl, CtrlGrdViewUom;
                    CtrlGrdViewItemCode = e.Row.FindControl("CtrlGrdViewItemCode") as CtrlGridViewList;
                    CtrlGrdViewItemDtl = e.Row.FindControl("CtrlGrdViewItemDtl") as CtrlGridViewList;
                    CtrlGrdViewUom = e.Row.FindControl("CtrlGrdViewUom") as CtrlGridViewList;
                    double Qty = ObjCls.FnIsDouble(TxtQty.Text);

                    TxtQty.Attributes.Add("onkeyup", "return FnUpdateOrderDetails('" + ObjCls.FnIsDouble(Request.QueryString["TokenNo"].ToString()) + "','" + DataBinder.Eval(e.Row.DataItem, "Id").ToString() + "','" + CtrlGrdViewItemDtl.IdControl + "','" + CtrlGrdViewUom.IdControl + "','" + TxtQty.ClientID + "','" + TxtPrice.ClientID + "','" + TxtRemarks.ClientID + "');");
                    TxtPrice.Attributes.Add("onkeyup", "return FnUpdateOrderDetails('" + ObjCls.FnIsDouble(Request.QueryString["TokenNo"].ToString()) + "','" + DataBinder.Eval(e.Row.DataItem, "Id").ToString() + "','" + CtrlGrdViewItemDtl.IdControl + "','" + CtrlGrdViewUom.IdControl + "','" + TxtQty.ClientID + "','" + TxtPrice.ClientID + "','" + TxtRemarks.ClientID + "');");
                    TxtRemarks.Attributes.Add("onkeyup", "return FnUpdateOrderDetails('" + ObjCls.FnIsDouble(Request.QueryString["TokenNo"].ToString()) + "','" + DataBinder.Eval(e.Row.DataItem, "Id").ToString() + "','" + CtrlGrdViewItemDtl.IdControl + "','" + CtrlGrdViewUom.IdControl + "','" + TxtQty.ClientID + "','" + TxtPrice.ClientID + "','" + TxtRemarks.ClientID + "');");

                    if (CtrlGrdViewItemCode.SelectedValue == "0" || CtrlGrdViewItemDtl.SelectedValue == "0" 
                        || CtrlGrdViewUom.SelectedValue == "0" || Qty == 0)
                    {
                        e.Row.CssClass += "export-row3";
                    }
                    if (btnDelete != null)
                    {
                        string subject = e.Row.Cells[1].Text; // Subject column index
                        btnDelete.OnClientClick = $"return confirm('Delete Item');";
                    }
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CtrlGrdCustomer.SelectedValue == "0")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select a valid customer'); }, 100);", true);
                    return;
                }
                ObjCls.CustomerId = ObjCls.FnIsNumeric(CtrlGrdCustomer.SelectedValue);
                ObjCls.MailId = ObjCls.FnIsNumeric(Request.QueryString["MailId"]);
                ObjCls.FromMail = Request.QueryString["From"];
                ObjCls.AICustomerName = LblCustName.Text.Trim();
                ObjCls.TokenNo = ObjCls.FnIsDouble(Request.QueryString["TokenNo"].ToString());

                HiddenField HdnId, hdnItemId;
                Label LblCode, LblName, LblUom;
                CtrlGridViewList CtrlGrdViewItemDtl, CtrlGrdViewUom;
                TextBox TxtRemarks;

                foreach (GridViewRow row in GrdMapItems.Rows)
                {
                    HdnId = (HiddenField)row.FindControl("HdnId");
                    LblCode = (Label)row.FindControl("LblCode");
                    LblName = (Label)row.FindControl("LblName");
                    LblUom = (Label)row.FindControl("LblUom");
                    CtrlGrdViewItemDtl = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemDtl");
                    CtrlGrdViewUom = (CtrlGridViewList)row.FindControl("CtrlGrdViewUom");
                    hdnItemId = (HiddenField)row.FindControl("HdnItemId");
                    TxtRemarks = (TextBox)row.FindControl("TxtRemarks");

                    ObjCls.ItmCode = LblCode.Text.ToString();
                    ObjCls.ItemDesc = LblName.Text.ToString();
                    ObjCls.ItmUom = LblUom.Text.ToString();
                    ObjCls.Remarks = TxtRemarks.Text.Trim();

                    ObjCls.ItemId = ObjCls.FnIsNumeric(CtrlGrdViewItemDtl.SelectedValue);
                    ObjCls.UomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
                    ObjCls.SplitUomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
                    ObjCls.FnUpdateAIOrderDetails();


                }
                ObjCls.FnUpdateCustomerDetails();
                ObjCls.FnUpdateCheckStatus();
                ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Record Inserted Successfully '); }, 100);", true);
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }

        protected void GrdMapItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DeleteMail")
                {
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    HiddenField HdnId = (HiddenField)row.FindControl("HdnId");
                    ObjParse.ID = ObjParse.FnIsNumeric(HdnId.Value);
                    ObjParse.MailId = objMail.FnIsNumeric(Request.QueryString["MailId"]);
                    ObjParse.FromMail = Request.QueryString["From"].ToString();
                    ObjParse.TokenNo = ObjCls.FnIsDouble(Request.QueryString["TokenNo"].ToString());
                    ObjParse.AICustomerName = LblCustName.Text.Trim();
                    ViewState["DT_CHILD"] = ObjParse.FnDeleteMailDataItems();
                    FnGridViewBinding("");

                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Deleted!');", true);
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
    }
}
