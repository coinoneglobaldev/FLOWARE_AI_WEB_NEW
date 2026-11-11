using FLOWARE_AI.App_Code;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FLOWARE_AI.ADMIN
{
    public partial class OrderParsingNew : ClsPageEvents
    {
        private static readonly HttpClient httpClient = new HttpClient();
        ClsCustomerItemMapping ObjCls = new ClsCustomerItemMapping();
        ClsAIMailDetails objMail = new ClsAIMailDetails();
        ClsOrderParsing ObjParse = new ClsOrderParsing();
        DataTable Dt = null, DT_TEMP, DTInsert,dt;
        string MailStatus, _selectedMailPdfPath, _PromptPDF, _selectedMailAllLinkUrl, _selectedMailJSON, _selectedMailLinkUrl, _cOrderNo, _cCustomerName, _dDeliveryDate,
            _dOrderDate, _SelectedMailSubject, _SelectedMailDate, _SelectedMailBody, _SelectedMailFrom, _SelectedMailUId, _SelectedMailBodyHTML;
        int SlNo = 0;
        string PDFPathUrl="";
        
        public class MailSummary
        {
            public string UniqueId { get; set; }
            public string FromEmail { get; set; }
            public string EmailSubject { get; set; }
            public DateTime DateReceived { get; set; }
            public string EmailBody { get; set; }
            public List<string> AttachmentPaths { get; set; } = new List<string>();
            public List<string> Links { get; set; } = new List<string>();
            public Dictionary<string, string> LinkContents { get; set; } = new Dictionary<string, string>();
        }
        private static List<MailSummary> _staticCache = new List<MailSummary>();
        private List<MailSummary> CachedMails
        {
            get => _staticCache;
            set => _staticCache = value ?? new List<MailSummary>();
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                if (!IsPostBack)
                {
                    FnInitializeForm();

                    HrefDirect.Attributes.Add("href", "../ADMIN/Direct.aspx?UID=" + Request.QueryString["UID"].ToString());
                    HrefExport.Attributes.Add("href", "../ADMIN/Export.aspx?UID=" + Request.QueryString["UID"].ToString());
                }
                //RegisterGridControlMasterScript();
                CtrlGrdItemCode.DestinationControls = CtrlGrdItem.IdControl + "," + CtrlGrdItem.TextControl + "," + CtrlGrdUom.IdControl + "," + CtrlGrdUom.TextControl;
                CtrlGrdItem.DestinationControls = CtrlGrdItemCode.IdControl + "," + CtrlGrdItemCode.TextControl + "," + CtrlGrdUom.IdControl + "," + CtrlGrdUom.TextControl;
                CtrlGrdUom.ParentControl = CtrlGrdItem.IdControl;
                CtrlGrdCustDtl.DestinationControls = LblAddress.ClientID;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        public override void FnInitializeForm()
        {
            try
            {
                int iCmpId = FnGetRights().COMPANYID, iBrId = FnGetRights().BRANCHID, iFaId = FnGetRights().FAYEARID;
                ObjCls = new ClsCustomerItemMapping(iCmpId, iBrId, iFaId);
                FnAssignPrompt();
                ViewState["DT"] = objMail.FnGetMailMasterData();
                ViewState["DT_CHILD"] = FnGetGeneralXml(ObjCls);
                
                FnGridViewBinding("MAIL");
                FnGridViewBinding("");
                TabContainer1.ActiveTabIndex = 0;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        public string FnGetEmailDateTime(string PrmEmail, string PrmDateTime)
        {
            PrmDateTime = ObjCls.FnDateTime(PrmDateTime, "dd/MMM/yyyy HH:mm:ss");
            return PrmEmail.ToString() + " - " + PrmDateTime.ToString();
        }
        public void FnClear()
        {
            TxtPONo.Text = "";
            CtrlOrderDate.DateText = "";
            CtrlDelDate.DateText = "";
            CtrlGrdCustDtl.FnClearControls();
            LblCustName.Text = "";
            LblSubject.Text = "";
            LblAddress.Text = "";
            TxtAreainput.Text = "";
            FnInitializeForm();
        }
        public void FnclearIndexChanged()
        {
            CtrlGrdCustDtl.FnClearControls();
            LblCustName.Text = "";
        }
        public void FnClearItems()
        {
            CtrlGrdItemCode.FnClearControls();
            CtrlGrdItem.FnClearControls();
            CtrlGrdUom.FnClearControls();
            TxtOrderQty.Text = "0";
            TxtPrice.Text = "0";
        }
        public void ManiPulateDataEvent_Clicked(object sender, EventArgs e)
        {

        }
        public void FnAssignPrompt()
        {
            try
            {
                DT_TEMP = objMail.FnGetPromptData();
                if (DT_TEMP.Rows.Count > 0)
                {
                    foreach (DataRow row in DT_TEMP.Rows)
                    {
                        string promptType = Convert.ToString(row["cPromptType"]).Trim().ToUpper();
                        string promptData = Convert.ToString(row["cPromptData"]).Trim();

                        switch (promptType)
                        {
                            case "PDF":
                                ViewState["PromptPDF"] = promptData;
                                break;
                            case "LINK":
                                ViewState["PromptLINK"] = promptData;
                                break;
                            case "TEXT":
                                ViewState["PromptTEXT"] = promptData;
                                break;
                            case "MBODY":
                                ViewState["PromptMBODY"] = promptData;
                                break;
                        }
                    }
                }
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
        public void FnGridViewBinding(string PrmFlag)
        {
            if (PrmFlag.Equals("MAIL"))
            {
                GrdVwRecords.DataSource = ViewState["DT"] as DataTable;
                //GrdVwRecords.DataKeyNames = new String[] { ObjCls.KeyName };
                GrdVwRecords.DataBind();
                GrdVwRecords.SelectedIndex = -1;
            }
            else
            {
                GrdVwOrderDtls.DataSource = ViewState["DT_CHILD"] as DataTable;
                //GrdVwOrderDtls.DataKeyNames = new String[] { objMail.KeyName };
                GrdVwOrderDtls.DataBind();
                GrdVwOrderDtls.SelectedIndex = -1;
                TabContainer1.ActiveTabIndex = 0;
            }
        }
        protected void TabContainer1_ActiveTabChanged(object sender, EventArgs e)
        {

        }
        protected void GrdVwRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //FnclearIndexChanged();
                objMail.MailId =ObjCls.FnIsNumeric(((HiddenField)GrdVwRecords.SelectedRow.FindControl("HdnId")).Value);
                objMail.FromMail = ((LinkButton)GrdVwRecords.SelectedRow.FindControl("LnkFromMail")).Text;
                objMail.TokenNo= ObjCls.FnIsDouble(((HiddenField)GrdVwRecords.SelectedRow.FindControl("HdnTokenId")).Value);
                objMail.AICustomerName = LblCustName.Text.Trim();
                objMail.IsEdit = 0;
                ViewState["DT_CHILD"] = objMail.FnGetOrderData();

                HdnMailId.Value= ObjCls.FnIsNumeric(((HiddenField)GrdVwRecords.SelectedRow.FindControl("HdnId")).Value).ToString();
                HdnMailFrom.Value= ((LinkButton)GrdVwRecords.SelectedRow.FindControl("LnkFromMail")).Text;
                HdnTokenNo.Value =objMail.TokenNo.ToString();
                HdnMailInbIds.Value= ObjCls.FnIsNumeric(((HiddenField)GrdVwRecords.SelectedRow.FindControl("HdnMailInbId")).Value).ToString();
                HdnMailAccIds.Value= ObjCls.FnIsNumeric(((HiddenField)GrdVwRecords.SelectedRow.FindControl("HdnMailAccId")).Value).ToString();
                DataTable DT_CHILD = ViewState["DT_CHILD"] as DataTable;

                if (DT_CHILD != null && DT_CHILD.Rows.Count > 0)
                {
                    DataRow firstRow = DT_CHILD.Rows[0];
                    TxtPONo.Text = Convert.ToString(firstRow["OrderNo"]);
                    if (Convert.ToDateTime(firstRow["OrderDate"]).ToString("dd-MM-yyyy") == "01-01-1800")
                    {
                        CtrlOrderDate.DateText = "";
                    }
                    else
                    {
                        CtrlOrderDate.DateText = Convert.ToDateTime(firstRow["OrderDate"]).ToString("dd/MMM/yyyy");
                    }
                    if (Convert.ToDateTime(firstRow["DeliveryDate"]).ToString("dd-MM-yyyy") == "01-01-1800")
                    {
                        CtrlDelDate.DateText = "";
                    }
                    else
                    {
                        CtrlDelDate.DateText = Convert.ToDateTime(firstRow["DeliveryDate"]).ToString("dd/MMM/yyyy");
                    }
                    LblCustName.Text = Convert.ToString(firstRow["AICustomerName"]);
                    LblAddress.Text= Convert.ToString(firstRow["PAddress"]);
                    CtrlGrdCustDtl.SelectedValue = Convert.ToString(firstRow["CustomerId"]);
                    CtrlGrdCustDtl.SelectedText = Convert.ToString(firstRow["CustomerName"]);
                    FnGetTotal();
                }
                else
                {
                    TxtPONo.Text = "";
                    CtrlOrderDate.DateText = "";
                    CtrlDelDate.DateText = "";
                    CtrlGrdCustDtl.SelectedText = "";
                    LblCustName.Text = "";
                    TxtPriceTot.Text = "0";
                    LblAddress.Text = "";
                }
                FnGridViewBinding("");
                FnFindHTMLView(ObjCls.FnIsNumeric(((HiddenField)GrdVwRecords.SelectedRow.FindControl("HdnId")).Value));
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void FnFindHTMLView(int nMailId)
        {
            PDFPathUrl = "";
            objMail.MailId = nMailId;
            HdnPDFMailId.Value = nMailId.ToString();
            DataTable DTB = objMail.GetMailBodyHtmlFromDB();
            if (DTB != null && DTB.Rows.Count > 0)
            {
                string htmlFileName = Convert.ToString(DTB.Rows[0]["MailBody"]);
                string folderPath = ConfigurationManager.AppSettings["HTML_PATH"];
                string filePath = System.IO.Path.Combine(folderPath, htmlFileName);
                PDFPathUrl = Convert.ToString(DTB.Rows[0]["PDFPathUrl"]);
                LblSubject.Text= Convert.ToString(DTB.Rows[0]["SubjectMail"]);
                if (File.Exists(filePath))
                {
                    if (PDFPathUrl != "")
                    {
                        FnBindDocumetPathUrl(HyLnkDoc, PDFPathUrl, "DOC");
                        LitMailBody.Text = "OPEN PDF";
                        HyLnkDoc.Visible = true;
                        FnOpenEmailPopup(nMailId);
                    }
                    else
                    {
                        string htmlContent = File.ReadAllText(filePath, Encoding.UTF8);
                        LitMailBody.Text = htmlContent;
                        HyLnkDoc.Visible = false;
                    }
                }
                else
                {
                    HyLnkDoc.Visible = false;
                    LitMailBody.Text = "<b>HTML file not found.</b>";
                }
            }
            TabContainer1.ActiveTabIndex = 1;
        }
        private void FnOpenEmailPopup(int nMailId)
        {
            string url = ResolveUrl("~/ADMIN/ViewEmail.aspx") + "?MailId=" + Server.UrlEncode(nMailId.ToString()) + "&From=" + Server.UrlEncode(HdnMailFrom.Value) + "&TokenNo=" + Server.UrlEncode(HdnTokenNo.Value);
            string script = "PopUpIframeLeft('View Email','800','650','view-email-frame','" + HttpUtility.JavaScriptStringEncode(url) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenEmailPopup_" + nMailId.ToString(), script, true);
        }
        protected void GrdVwRecords_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "IsChk").ToString() == "1")
                {
                    e.Row.CssClass += "export-row1";
                }
            }
        }
        protected async void LnkTxtSubmit_Click(object sender, EventArgs e)
        {
            if (CtrlGrdCustDtl.SelectedValue == "0")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select a valid customer'); }, 100);", true);
                return;
            }
            HdnTokenNo.Value = ObjCls.FnGetTokenId().ToString();
            string inputData = TxtAreainput.Text.Trim();
            if (string.IsNullOrWhiteSpace(inputData))
            {
                return;
            }
            string jsonResult = await GetJsonFromChatGPT(inputData);
            if (jsonResult.StartsWith("```json")) jsonResult = jsonResult.Substring(7).Trim();
            if (jsonResult.StartsWith("```")) jsonResult = jsonResult.Substring(3).Trim();
            if (jsonResult.EndsWith("```")) jsonResult = jsonResult.Substring(0, jsonResult.Length - 3).Trim();
            try
            {
                JArray jsonArray = JArray.Parse(jsonResult);
                if (jsonArray.Count > 0)
                {
                    JObject first = (JObject)jsonArray[0];
                    TxtPONo.Text = (string)first["OrderNumber"] ?? "";
                    CtrlOrderDate.DateText = DateTime.Now.ToString("dd/MMM/yyyy"); //(string)first["OrderDate"] ?? "";
                    int nHr = DateTime.Now.Hour;
                    if(nHr>=13)
                    {
                        CtrlDelDate.DateText = DateTime.Now.AddDays(1).ToString("dd/MMM/yyyy");
                    }
                    else
                    {
                        CtrlDelDate.DateText = DateTime.Now.ToString("dd/MMM/yyyy");
                    }
                    LblCustName.Text = CtrlGrdCustDtl.SelectedText;
                }
                // Convert to DataTable
                JArray itemsArray = (JArray)jsonArray[0]["Items"];
                DataTable dt = itemsArray.ToObject<DataTable>();
                DataTable DResult = FnCreateDataTableGeneral(dt);
                HdnType.Value = "TEXT";
                ViewState["DT_CHILD"] = FnInsertTextPDFLinkToDb(DResult);
                FnGridViewBinding("");
                TabContainer1.ActiveTabIndex = 2;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected async void LnkPDFSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = "";
                // Priority 1: Session
                if (Session["UploadedFilePath"] != null)
                    filePath = Session["UploadedFilePath"].ToString();
                // Priority 2: Hidden field
                if (string.IsNullOrEmpty(filePath))
                    filePath = HdnFilePath.Value;

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    await ProcessManualPdfAndExtract(filePath);
                    LblResult.Text = "File uploaded successfully!";
                    LblResult.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    LblResult.Text = "No file selected.";
                    LblResult.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                LblResult.Text = "Error: " + ex.Message;
                LblResult.ForeColor = System.Drawing.Color.Red;
            }
        }
        protected void FileUploadDoc_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            try
            {
                if (FileUploadDoc.HasFile)
                {
                    // Get file name and save path
                    string fileName = System.IO.Path.GetFileName(FileUploadDoc.FileName);
                    string savePath = Server.MapPath("~/Firebasefile/");
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    string pdfFile = System.IO.Path.Combine(savePath, fileName);
                    FileUploadDoc.SaveAs(pdfFile);
                    HdnFilePath.Value = pdfFile;
                    Session["UploadedFilePath"] = pdfFile;
                    LblResult.Text = "File uploaded successfully!";
                    LblResult.ForeColor = System.Drawing.Color.Green;
                    ScriptManager.GetCurrent(this).RegisterPostBackControl(LnkPDFSubmit); // Ensure the button is a full postback control if necessary, but forcing an update is usually sufficient.
                    ScriptManager.GetCurrent(this).SetFocus(LnkPDFSubmit); // Optional: Set focus to the next action button
                }
                else
                {
                    LblResult.Text = "No file selected.";
                    LblResult.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                LblResult.Text = "Error: " + ex.Message;
                LblResult.ForeColor = System.Drawing.Color.Red;
            }
        }
        protected async void LnkSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                HdnTokenNo.Value = ObjCls.FnGetTokenId().ToString();
                ScriptManager.RegisterStartupScript(this, GetType(), "showLoader", "showLoader();", true);
                string url = TxtLink.Text.Trim();
                if (string.IsNullOrEmpty(url))
                {
                    return;
                }
                string jsonOutput = await GetJsonFromUrlLinkAsync(url);
                JObject jsonObject = JObject.Parse(jsonOutput);
                TxtPONo.Text = jsonObject["OrderNumber"]?.ToString();
                CtrlOrderDate.DateText = jsonObject["OrderDate"]?.ToString();
                CtrlDelDate.DateText = jsonObject["DeliveryDate"]?.ToString();
                LblCustName.Text = jsonObject["CustomerName"]?.ToString();

                DataTable dt = ConvertJsonToDataTable(jsonOutput);
                DataTable DResult = FnCreateDataTableGeneral(dt);
                HdnType.Value = "LINK";
                ViewState["DT_CHILD"] = FnInsertTextPDFLinkToDb(DResult);
                FnGridViewBinding("");
                TabContainer1.ActiveTabIndex = 4;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void BtnAddRow_Click(object sender, EventArgs e)
        {
            try
            {
                objMail.TokenNo = objMail.FnIsDouble(HdnTokenNo.Value);
                objMail.FItemId = ObjCls.FnIsNumeric(CtrlGrdItem.SelectedValue);
                objMail.MailId =ObjCls.FnIsNumeric(HdnMailId.Value);
                objMail.FUomId = ObjCls.FnIsNumeric(CtrlGrdUom.SelectedValue);
                objMail.Qty = ObjCls.FnIsDouble(TxtOrderQty.Text.Trim());
                objMail.FromMail = HdnMailFrom.Value;
                objMail.AICustomerName = LblCustName.Text.Trim();
                objMail.Rate =ObjCls.FnIsDouble(TxtPrice.Text.ToString());
                objMail.IsEdit =1;
                //objMail.CustomerId =ObjCls.FnIsNumeric(CtrlGrdCustDtl.SelectedValue);
                objMail.FnAddNewItemToOrder();

                //ObjParse.TokenNo = objMail.FnIsDouble(HdnTokenNo.Value);
                //ObjParse.MailId = ObjCls.FnIsNumeric(HdnMailId.Value);
                ViewState["DT_CHILD"] = objMail.FnGetOrderData();
                //DataTable DTInsert = ObjParse.FnGetExtractItemAsNewOrder();
                FnGridViewBinding("");
                FnClearItems();

            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        
        protected void GrdVwOrderDtls_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                Label LblCode, LblName, LblUom;
                CtrlGridViewList CtrlGrdViewItemCode, CtrlGrdViewItemDtl, CtrlGrdViewUom;
                HiddenField HdnIds, HdnItemId, HdnAmt;
                TextBox TxtQty, TxtPrice, TxtRemarks;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Button btnDelete = e.Row.FindControl("BtnDelete") as Button;
                    LblCode = e.Row.FindControl("LblCode") as Label;
                    LblName = e.Row.FindControl("LblName") as Label;
                    LblUom = e.Row.FindControl("LblUom") as Label;

                    HdnIds = e.Row.FindControl("HdnIds") as HiddenField;
                    HdnItemId = e.Row.FindControl("HdnItemId") as HiddenField;
                    HdnAmt = e.Row.FindControl("HdnAmt") as HiddenField;

                    TxtQty = e.Row.FindControl("TxtQty") as TextBox;
                    TxtPrice = e.Row.FindControl("TxtPrice") as TextBox;
                    TxtRemarks= e.Row.FindControl("TxtRemarks") as TextBox;

                    CtrlGrdViewItemCode = e.Row.FindControl("CtrlGrdViewItemCode") as CtrlGridViewList;
                    CtrlGrdViewItemDtl = e.Row.FindControl("CtrlGrdViewItemDtl") as CtrlGridViewList;
                    CtrlGrdViewUom = e.Row.FindControl("CtrlGrdViewUom") as CtrlGridViewList;

                    //TxtQty.Attributes.Add("onkeyup", "return FnUpdateOrderDetails('" + HdnTokenNo.Value.ToString() + "','" + DataBinder.Eval(e.Row.DataItem, "Id").ToString() + "','" + CtrlGrdViewItemDtl.IdControl + "','" + CtrlGrdViewUom.IdControl + "','" + TxtQty.ClientID + "','" + TxtPrice.ClientID + "','" + TxtRemarks.ClientID + "');");
                    //TxtPrice.Attributes.Add("onkeyup", "return FnUpdateOrderDetails('" + HdnTokenNo.Value.ToString() + "','" + DataBinder.Eval(e.Row.DataItem, "Id").ToString() + "','" + CtrlGrdViewItemDtl.IdControl + "','" + CtrlGrdViewUom.IdControl + "','" + TxtQty.ClientID + "','" + TxtPrice.ClientID + "','" + TxtRemarks.ClientID + "');");
                    //TxtRemarks.Attributes.Add("onkeyup", "return FnUpdateOrderDetails('" + HdnTokenNo.Value.ToString() + "','" + DataBinder.Eval(e.Row.DataItem, "Id").ToString() + "','" + CtrlGrdViewItemDtl.IdControl + "','" + CtrlGrdViewUom.IdControl + "','" + TxtQty.ClientID + "','" + TxtPrice.ClientID + "','" + TxtRemarks.ClientID + "');");

                    string updateJs = "FnUpdateOrderDetails('" + HdnTokenNo.Value + "','" + DataBinder.Eval(e.Row.DataItem, "Id") + "','" + CtrlGrdViewItemDtl.IdControl + "','" + CtrlGrdViewUom.IdControl + "','" + TxtQty.ClientID + "','" + TxtPrice.ClientID + "','" + TxtRemarks.ClientID + "','" + CtrlGrdViewItemCode.IdControl + "');";

                    TxtQty.Attributes.Add("onkeyup", "return " + updateJs);
                    TxtPrice.Attributes.Add("onkeyup", "return " + updateJs);
                    TxtRemarks.Attributes.Add("onkeyup", "return " + updateJs);

                    CtrlGrdViewItemCode.OnClientSelect = updateJs;
                    CtrlGrdViewItemDtl.OnClientSelect = updateJs;
                    CtrlGrdViewUom.OnClientSelect = updateJs;


                    double Qty = ObjCls.FnIsDouble(TxtQty.Text);

                    if (CtrlGrdViewItemCode.SelectedValue=="0"|| CtrlGrdViewItemDtl.SelectedValue=="0" ||
                        CtrlGrdViewUom.SelectedValue == "0"||Qty==0)
                    {
                        e.Row.CssClass += "export-row3";
                    }
                    if (btnDelete != null)
                    {
                        btnDelete.OnClientClick = $"return confirm('Delete Item');";
                    }
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void GrdVwRecords_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DeleteMail")
                {
                    int mailId = Convert.ToInt32(e.CommandArgument);
                    objMail.ID = mailId;
                    objMail.FnDeleteMailData();
                    ViewState["DT"] = objMail.FnGetMailMasterData();
                    FnGridViewBinding("MAIL");
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Deleted!');", true);
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void GrdVwOrderDtls_PreRender(object sender, EventArgs e)
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
        protected void GrdVwOrderDtls_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DeleteMail")
                {
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    HiddenField HdnId=(HiddenField)row.FindControl("HdnIds");
                    ObjParse.ID =ObjParse.FnIsNumeric(HdnId.Value);
                    ObjParse.MailId= objMail.FnIsNumeric(HdnMailId.Value);
                    ObjParse.FromMail = HdnMailFrom.Value;
                    ObjParse.TokenNo =ObjCls.FnIsDouble(HdnTokenNo.Value);
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
        protected void BtnSaveData_Click(object sender, EventArgs e)
        {
            try
            {
                if (CtrlGrdCustDtl.SelectedValue == "0")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select a valid customer'); }, 100);", true);
                    return;
                }
                ObjCls.CustomerId = ObjCls.FnIsNumeric(CtrlGrdCustDtl.SelectedValue);
                ObjCls.MailId = ObjCls.FnIsNumeric(HdnMailId.Value);
                ObjCls.FromMail = HdnMailFrom.Value;
                ObjCls.TokenNo =ObjCls.FnIsDouble(HdnTokenNo.Value);
                ObjCls.AICustomerName = LblCustName.Text.Trim();

                HiddenField HdnId, hdnItemId;
                Label LblCode, LblName, LblUom;
                CtrlGridViewList CtrlGrdViewItemDtl, CtrlGrdViewUom;

                foreach (GridViewRow row in GrdVwOrderDtls.Rows)
                {
                    HdnId = (HiddenField)row.FindControl("HdnId");
                    LblCode = (Label)row.FindControl("LblCode");
                    LblName = (Label)row.FindControl("LblName");
                    LblUom = (Label)row.FindControl("LblUom");
                    CtrlGrdViewItemDtl = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemDtl");
                    CtrlGrdViewUom = (CtrlGridViewList)row.FindControl("CtrlGrdViewUom");
                    hdnItemId = (HiddenField)row.FindControl("HdnItemId");
                    TextBox TxtRemarks = (TextBox)row.FindControl("TxtRemarks");

                    ObjCls.ItmCode = LblCode.Text.ToString();
                    ObjCls.ItemDesc = LblName.Text.ToString();
                    ObjCls.ItmUom = LblUom.Text.ToString();

                    ObjCls.ItemId = ObjCls.FnIsNumeric(CtrlGrdViewItemDtl.SelectedValue);
                    ObjCls.UomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
                    ObjCls.SplitUomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
                    ObjCls.Remarks = TxtRemarks.Text.Trim();
                    ObjCls.FnUpdateAIOrderDetails();

                }
                ObjCls.FnUpdateCustomerDetails();
                ObjCls.FnUpdateCheckStatus();
                FnClear();
                //ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Record Inserted Successfully '); }, 100);", true);
                string script = @"setTimeout(function () {alert('Record Inserted Successfully');  if (typeof FnCloseAllPopups === 'function') { FnCloseAllPopups(); } }, 100);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SaveSuccessAndClosePopups", script, true);
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void BtnApplyMapping_Click(object sender, EventArgs e)
        {
            try
            {
                if (CtrlGrdCustDtl.SelectedValue == "0")
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

                foreach (GridViewRow row in GrdVwOrderDtls.Rows)
                {
                    if (row.RowType != DataControlRowType.DataRow) continue;

                    string rowNo = ((GrdVwOrderDtls.PageIndex * GrdVwOrderDtls.PageSize) + row.RowIndex + 1).ToString();

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
                ObjCls.CustomerId = ObjCls.FnIsNumeric(CtrlGrdCustDtl.SelectedValue);
                ObjCls.MailId = ObjCls.FnIsNumeric(HdnMailId.Value);
                ObjCls.FromMail = HdnMailFrom.Value;
                ObjCls.AICustomerName = LblCustName.Text.Trim();
                ObjCls.TokenNo = ObjCls.FnIsDouble(HdnTokenNo.Value);

                foreach (GridViewRow row in GrdVwOrderDtls.Rows)
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
        //protected void BtnApplyMapping_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (CtrlGrdCustDtl.SelectedValue == "0")
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select a valid customer'); }, 100);", true);
        //            return;
        //        }
        //        if (CtrlOrderDate.DateText == "")
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select valid order date'); }, 100);", true);
        //            return;
        //        }
        //        if (CtrlDelDate.DateText == "")
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select valid delivery date'); }, 100);", true);
        //            return;
        //        }
        //        ObjCls.CustomerId = ObjCls.FnIsNumeric(CtrlGrdCustDtl.SelectedValue);
        //        ObjCls.MailId = ObjCls.FnIsNumeric(HdnMailId.Value);
        //        ObjCls.FromMail = HdnMailFrom.Value;
        //        ObjCls.AICustomerName = LblCustName.Text.Trim();
        //        ObjCls.TokenNo = ObjCls.FnIsDouble(HdnTokenNo.Value);

        //        HiddenField HdnId, hdnItemId;
        //        Label LblCode, LblName, LblUom;
        //        CtrlGridViewList CtrlGrdViewItemDtl, CtrlGrdViewUom;
        //        foreach (GridViewRow row in GrdVwOrderDtls.Rows)
        //        {
        //            HdnId = (HiddenField)row.FindControl("HdnId");
        //            LblCode = (Label)row.FindControl("LblCode");
        //            LblName = (Label)row.FindControl("LblName");
        //            LblUom = (Label)row.FindControl("LblUom");
        //            CtrlGrdViewItemDtl = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemDtl");
        //            CtrlGrdViewUom = (CtrlGridViewList)row.FindControl("CtrlGrdViewUom");
        //            hdnItemId = (HiddenField)row.FindControl("HdnItemId");
        //            TextBox TxtRemarks = (TextBox)row.FindControl("TxtRemarks");

        //            ObjCls.ItmCode = LblCode.Text.ToString();
        //            ObjCls.ItemDesc = LblName.Text.ToString();
        //            ObjCls.ItmUom = LblUom.Text.ToString();
        //            ObjCls.Remarks = TxtRemarks.Text.Trim();

        //            if (CtrlGrdViewItemDtl.SelectedValue == "")
        //            {
        //                ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Cannot Export To sysware,Please Choose Item'); }, 100);", true);
        //                return;
        //            }
        //            else
        //            {
        //                ObjCls.ItemId = ObjCls.FnIsNumeric(CtrlGrdViewItemDtl.SelectedValue);
        //                ObjCls.UomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
        //                ObjCls.SplitUomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
        //                ObjCls.FnUpdateAIOrderDetails();
        //            }
        //        }
        //        ObjCls.FnUpdateCustomerDetails();
        //        ObjCls.FnUpdateExportStatus();
        //        FnClear();
        //        //ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Record Inserted Successfully '); }, 100);", true);
        //        string script = @"setTimeout(function () {alert('Record Inserted Successfully');  if (typeof FnCloseAllPopups === 'function') { FnCloseAllPopups(); } }, 100);";
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "SaveSuccessAndClosePopups", script, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        _strError = ex.Message;
        //    }
        //}
        static async Task<string> AskChatGPT(string apiKey, string inputText, string instruction)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                string today = DateTime.Today.ToString("dd-MM-yyyy");
                string tomorrow = DateTime.Today.AddDays(1).ToString("dd-MM-yyyy");
                instruction = instruction.Replace("{{TODAY_DATE}}", today).Replace("{{TOMORROW_DATE}}", tomorrow);
               
                var payload = new
                {
                    model = "gpt-4.1-mini", // cheaper + fast
                    messages = new object[]
                    {
                new { role = "system", content = instruction },
                new { role = "user", content = inputText }
                    }
                };
                string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions",
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                );
                string result = await response.Content.ReadAsStringAsync();
                var parsed = JObject.Parse(result);
                return parsed["choices"]?[0]?["message"]?["content"]?.ToString();
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        private async Task<string> GetJsonFromUrlAsync(string url)
        {
            try
            {
                // Step 1: Fetch HTML from URL
                string htmlContent = await httpClient.GetStringAsync(url);
                // Step 2: OpenAI API key (replace with your valid one)
                string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                string prompt = "";
                if (ViewState["PromptLINK"] != null)
                {
                    prompt = ViewState["PromptLINK"].ToString();
                }
                else
                {
                    prompt = "You are a JSON converter. Convert HTML order details into structured JSON (e.g., SupplierName, OrderDate, Items, Quantity, Price, etc.). Always return valid JSON only.";
                }
                var requestBody = new
                {
                    model = "gpt-4.1-mini",
                    messages = new[]
                    {
                    new { role = "system", content =prompt },
                    new { role = "user", content = htmlContent }
                }
                };
                var response = await httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
                );
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);
                string jsonOutput = result?.choices?[0]?.message?.content ?? "No JSON output received.";
                // Step 5: Clean output — remove ```json, ``` etc.
                jsonOutput = Regex.Replace(jsonOutput, @"^```json\s*|\s*```$", "", RegexOptions.Multiline).Trim();
                jsonOutput = Regex.Replace(jsonOutput, @"^```|```$", "", RegexOptions.Multiline).Trim();
                return jsonOutput.Trim();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Server.MapPath("~/ErrorLog.txt"),
                    $"{DateTime.Now}: {ex.Message}\n");
                return "Error: " + ex.Message;
            }
        }
        private DataTable ConvertJsonToDataTable(string json)
        {
            try
            {
                json = json.Trim();
                JToken token = JToken.Parse(json);
                if (token.Type == JTokenType.Array)
                {
                    return JsonConvert.DeserializeObject<DataTable>(json);
                }
                else if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;
                    foreach (var property in obj.Properties())
                    {
                        if (property.Value.Type == JTokenType.Array)
                        {
                            JArray arr = (JArray)property.Value;
                            DataTable dt1 = new DataTable();
                            foreach (JProperty propCol in ((JObject)arr[0]).Properties())
                            {
                                if (propCol.Name == "Quantity")
                                    dt1.Columns.Add(propCol.Name, typeof(decimal));
                                else
                                    dt1.Columns.Add(propCol.Name);
                            }
                            foreach (JObject objRow in arr)
                            {
                                DataRow row1 = dt1.NewRow();
                                foreach (var propCol in objRow.Properties())
                                {
                                    if (propCol.Name == "Quantity")
                                        row1[propCol.Name] = propCol.Value.ToObject<decimal>();
                                    else
                                        row1[propCol.Name] = propCol.Value.ToString();
                                }
                                dt1.Rows.Add(row1);
                            }
                            return dt1;
                        }
                    }
                    DataTable dt = new DataTable();
                    foreach (var prop in obj.Properties())
                    {
                        dt.Columns.Add(prop.Name);
                    }

                    DataRow row = dt.NewRow();
                    foreach (var prop in obj.Properties())
                    {
                        row[prop.Name] = prop.Value.ToString();
                    }
                    dt.Rows.Add(row);
                    return dt;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private async Task<string> GetJsonFromChatGPT(string inputText)
        {
            try
            {
                string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                string prompt = "";

                if (ViewState["PromptTEXT"] != null)
                {
                    prompt = ViewState["PromptTEXT"].ToString();
                }
                else
                {
                    prompt = "You are a JSON converter. Convert tabular/plain text into structured JSON.";
                }
                string today = DateTime.Today.ToString("dd-MM-yyyy");
                string tomorrow = DateTime.Today.AddDays(1).ToString("dd-MM-yyyy");
                prompt = prompt.Replace("{{TODAY_DATE}}", today).Replace("{{TOMORROW_DATE}}", tomorrow);
                var requestBody = new
                {
                    model = "gpt-4.1-mini", // or gpt-4o-mini
                    messages = new[]
                    {
                new { role = "system", content =prompt },
                new { role = "user", content = inputText }
                    }
                };
                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));
                string responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                dynamic result = JsonConvert.DeserializeObject(responseContent);
                string jsonOutput = result.choices[0].message.content;
                return jsonOutput.Trim();

            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

        }
        private async Task ProcessManualPdfAndExtract(string pdfFile)
        {
            try
            {
                HdnTokenNo.Value = ObjCls.FnGetTokenId().ToString();
                string extractedText = "";
                using (PdfReader reader = new PdfReader(pdfFile))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                        extractedText += PdfTextExtractor.GetTextFromPage(reader, page);
                }
                extractedText = extractedText.Replace("\r", " ").Replace("\n", " ").Trim();
                string openAiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                string promptToSend = "";
                if (ViewState["PromptPDF"] != null)
                {
                    promptToSend = ViewState["PromptPDF"].ToString();
                }
                else
                {
                    promptToSend = "Extract the Code, Item Name, UOM, Quantity, and Price from the below text. Respond in pure JSON array format.";
                }
                string today = DateTime.Today.ToString("dd-MM-yyyy");
                string tomorrow = DateTime.Today.AddDays(1).ToString("dd-MM-yyyy");
                promptToSend = promptToSend.Replace("{{TODAY_DATE}}", today).Replace("{{TOMORROW_DATE}}", tomorrow);

                string result = await AskChatGPT(openAiKey, extractedText, promptToSend);
                // Clean markdown fences if present
                result = result.Replace("```json", "").Replace("```", "").Trim();
                JArray jsonArray = JArray.Parse(result);
                foreach (var obj in jsonArray)
                {
                    var cust = obj["CustomerName"]?.ToString();
                    if (!string.IsNullOrEmpty(cust))
                    {
                        // Remove prefix 'Williams of London' and extra spaces
                        cust = Regex.Replace(cust, @"^.*?\bKitchen\b", "Kitchen", RegexOptions.IgnoreCase).Trim();
                        obj["CustomerName"] = cust;
                    }
                }
                DataTable dt = new DataTable();
                if (jsonArray.Count > 0)
                {
                    JObject first = (JObject)jsonArray[0];
                    TxtPONo.Text = (string)first["OrderNumber"] ?? "";
                    CtrlOrderDate.DateText = (string)first["OrderDate"] ?? "";
                    CtrlDelDate.DateText = (string)first["DeliveryDate"] ?? "";
                    LblCustName.Text = (string)first["CustomerName"] ?? "";
                    if (first["Items"] != null && first["Items"].Type == JTokenType.Array)
                    {
                        JArray itemsArray = (JArray)first["Items"];
                        dt = itemsArray.ToObject<DataTable>();
                    }
                }
                DataTable DResult = FnCreateDataTableGeneral(dt);
                HdnType.Value = "PDF";
                ViewState["DT_CHILD"] = FnInsertTextPDFLinkToDb(DResult);
                FnGridViewBinding("");
                TabContainer1.ActiveTabIndex = 3;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }

        }

        protected void GrdVwOrderDtls_Sorting(object sender, GridViewSortEventArgs e)
        {

        }

        private async Task<string> GetJsonFromUrlLinkAsync(string url)
        {
            try
            {
                // Step 1: Fetch HTML from URL
                string htmlContent = await httpClient.GetStringAsync(url);
                // Step 2: OpenAI API key (replace with your valid one)
                string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                // Step 3: Prepare API Request
                string prompt = "";
                if (ViewState["PromptLINK"] != null)
                {
                    prompt = ViewState["PromptLINK"].ToString();
                }
                else
                {
                    prompt = "You are a JSON converter. Convert HTML order details into structured JSON (e.g., SupplierName, OrderDate, Items, Quantity, Price, etc.). Always return valid JSON only.";
                }
                string today = DateTime.Today.ToString("dd-MM-yyyy");
                string tomorrow = DateTime.Today.AddDays(1).ToString("dd-MM-yyyy");
                prompt = prompt.Replace("{{TODAY_DATE}}", today).Replace("{{TOMORROW_DATE}}", tomorrow);
                var requestBody = new
                {
                    model = "gpt-4.1-mini",
                    messages = new[]
                    {
                    new { role = "system", content = prompt },
                    new { role = "user", content = htmlContent }
                }
                };
                var response = await httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
                );
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);
                string jsonOutput = result?.choices?[0]?.message?.content ?? "No JSON output received.";
                jsonOutput = Regex.Replace(jsonOutput, @"^```json\s*|\s*```$", "", RegexOptions.Multiline).Trim();
                jsonOutput = Regex.Replace(jsonOutput, @"^```|```$", "", RegexOptions.Multiline).Trim();
                return jsonOutput.Trim();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Server.MapPath("~/ErrorLog.txt"),
                    $"{DateTime.Now}: {ex.Message}\n");
                return "Error: " + ex.Message;
            }

        }
        protected void GrdVwRecords_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {

                GrdVwRecords.PageIndex = e.NewPageIndex;
                FnGridViewBinding("MAIL");
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
                foreach (GridViewRow row in GrdVwOrderDtls.Rows)
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
        protected void BtnClear_Click(object sender, EventArgs e)
        {
            FnClear();
        }
        protected async void LnkBtnReExtract_Click(object sender, EventArgs e)
        {
            List<MailSummary> mailList = new List<MailSummary>();
            try
            {
               // HdnMailAccIds.Value = "5";
                int nMailAccId =ObjCls.FnIsNumeric(HdnMailAccIds.Value);
                DataTable DTMailAcc = objMail.FnGetMailUserNamePassword(nMailAccId);
                string cEmailId = DTMailAcc.Rows[0]["EmailId"].ToString();
                string cEmailAppPassword = DTMailAcc.Rows[0]["EmailAppPassword"].ToString();
                var cMailList = await ReExtractMailsAsync(cEmailId,cEmailAppPassword,nMailAccId,ObjCls.FnIsNumeric(HdnMailInbIds.Value));
                CachedMails = cMailList;
                if (CachedMails.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('UID not present in INBOX.Mail was moved FROM INBOX or deleted.'); }, 100);", true);
                    return;
                }
                await FngetMailOrderDetails(HdnMailInbIds.Value, HdnMailAccIds.Value);
                DataTable DT_CHILD = ViewState["DT_CHILD"] as DataTable;
                if (DT_CHILD != null && DT_CHILD.Rows.Count > 0)
                {
                    DataRow firstRow = DT_CHILD.Rows[0];
                    TxtPONo.Text = Convert.ToString(firstRow["OrderNo"]);
                    if (Convert.ToDateTime(firstRow["OrderDate"]).ToString("dd-MM-yyyy") == "01-01-1800")
                    {
                        CtrlOrderDate.DateText = "";
                    }
                    else
                    {
                        CtrlOrderDate.DateText = Convert.ToDateTime(firstRow["OrderDate"]).ToString("dd/MMM/yyyy");
                    }
                    if (Convert.ToDateTime(firstRow["DeliveryDate"]).ToString("dd-MM-yyyy") == "01-01-1800")
                    {
                        CtrlDelDate.DateText = "";
                    }
                    else
                    {
                        CtrlDelDate.DateText = Convert.ToDateTime(firstRow["DeliveryDate"]).ToString("dd/MMM/yyyy");
                    }
                    LblCustName.Text = Convert.ToString(firstRow["AICustomerName"]);
                    CtrlGrdCustDtl.SelectedValue = Convert.ToString(firstRow["CustomerId"]);
                    CtrlGrdCustDtl.SelectedText = Convert.ToString(firstRow["CustomerName"]);
                    ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Re-Extracted Successfully'); }, 100);", true);
                }
                else
                {
                    TxtPONo.Text = "";
                    CtrlOrderDate.DateText = "";
                    CtrlDelDate.DateText = "";
                    CtrlGrdCustDtl.SelectedText = "";
                    LblCustName.Text = "";
                }
                FnGridViewBinding("");
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        private async Task<List<MailSummary>> ReExtractMailsAsync(string cEmailId, string cEmailAppPassword, int nEmailCredId,int nInBoxId)
        {
            //nInBoxId = 241693;
            List<MailSummary> mailList = new List<MailSummary>();
            string bodyText = "";
            string folderPath = "";
            string uniqueFileName = "";
            string filePath = "";

            try
            {
                using (var client = new ImapClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.Timeout = 180000;
                    await client.ConnectAsync("imap.gmail.com", 993, true);
                    await client.AuthenticateAsync(cEmailId, cEmailAppPassword);
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);
                    int intervalSetting = int.TryParse(ConfigurationManager.AppSettings["MailDays"], out var days) ? days : 4;
                    var uids = await inbox.SearchAsync(SearchQuery.DeliveredAfter(DateTime.Now.AddDays(-intervalSetting)));
                    UniqueId uid = new UniqueId((uint)nInBoxId);
                    var result = await inbox.SearchAsync(SearchQuery.Uids(new List<UniqueId> { uid }));
                    if (result == null || result.Count == 0)
                    {
                        _strError = "UID not present in INBOX. Mail was moved or deletedddddddd.uid=" + uid;
                        return mailList;
                    }

                    // Fetch ONLY that mail
                    var message = await inbox.GetMessageAsync(uid);

                    var mailSummary = new MailSummary
                    {
                        UniqueId = uid.Id.ToString(),
                        DateReceived = message.Date.DateTime,
                        FromEmail = message.From.ToString(),
                        EmailSubject = message.Subject,
                        EmailBody = message.TextBody ?? message.HtmlBody ?? "(No Content)"
                    };


                    foreach (var attachment in message.Attachments)
                    {
                        if (attachment is MimePart part && part.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            folderPath = FnGetPDFFilePath();
                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);
                            uniqueFileName = System.IO.Path.GetFileNameWithoutExtension(part.FileName) + "_" + Guid.NewGuid() + System.IO.Path.GetExtension(part.FileName);
                            filePath = System.IO.Path.Combine(folderPath, uniqueFileName);
                            using (var stream = File.Create(filePath))
                            {
                                await part.Content.DecodeToAsync(stream);
                            }
                            mailSummary.AttachmentPaths.Add(filePath);
                        }
                    }
                    mailList.Add(mailSummary);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
            return mailList;
        }
        private async Task FngetMailOrderDetails(string cUniqueId, string nEmailCredId)
        {
            try
            {
                var mail = CachedMails.FirstOrDefault(m => m.UniqueId == cUniqueId);
                if (mail == null) return;
                string mSubject = mail.EmailSubject;
                string mFrom = MailboxAddress.Parse(mail.FromEmail).Address;
                string MBody = (mail.EmailBody ?? "").Replace("\n", "\r\n");
                _selectedMailPdfPath = "";
                _selectedMailLinkUrl = "";
                MailStatus = "1";
                if (mail.AttachmentPaths != null && mail.AttachmentPaths.Count > 0)
                {
                    dt = await FnProcessPdfAndExtract(mail.AttachmentPaths.FirstOrDefault());
                    _selectedMailPdfPath = mail.AttachmentPaths.FirstOrDefault();
                }
                if (dt == null || dt.Rows.Count == 0)
                {
                    string bodyHtml = await FnGetMailHtmlBodyAsync(mail.UniqueId, nEmailCredId);
                    List<string> links = await FnGetMailHtmlBodyLinkAsync(mail.UniqueId, nEmailCredId);
                    if (links != null && links.Count > 0)
                        _selectedMailAllLinkUrl = (links != null && links.Count > 0) ? string.Join(",", links) : string.Empty;
                    List<string> validDomains = objMail.FnGetValidLinkDomains();
                    string validLink = null;
                    if (links != null && links.Count > 0 &&
                        validDomains != null && validDomains.Count > 0)
                    {
                        validLink = links.FirstOrDefault(url =>
                            validDomains.Any(domain =>
                                url.IndexOf(domain, StringComparison.OrdinalIgnoreCase) >= 0));
                    }
                    if (validLink != null)
                    {
                        _selectedMailAllLinkUrl = (validLink != null && links.Count > 0) ? string.Join(",", validLink) : string.Empty;
                        string jsonOutput = await FnGetJsonFromUrlAsync(validLink);
                        _selectedMailJSON = jsonOutput;
                        dt = FnConvertJsonToDataTable(jsonOutput);
                        _selectedMailLinkUrl = validLink;
                        if (string.IsNullOrWhiteSpace(jsonOutput))
                        {
                            return;
                        }
                        /*using (JsonDocument doc = JsonDocument.Parse(jsonOutput))
                          {
                              var root = doc.RootElement;
                              string orderNumber = root.TryGetProperty("OrderNumber", out var orderNoElem) ? orderNoElem.ToString() : "Not found";
                              _cOrderNo = orderNumber;
                              string LinkcustomerName = root.TryGetProperty("CustomerName", out var custN) ? custN.GetString() : root.TryGetProperty("CustomerNumber", out var custElem) ? custElem.GetString() :
                                   root.TryGetProperty("LocationName", out var locElem) ? locElem.GetString() : "Not found";
                              _cCustomerName = LinkcustomerName;
                              string deliveryDate = root.TryGetProperty("DeliveryDate", out var delElem) ? delElem.GetString() : "Not found";
                              if (DateTime.TryParse(deliveryDate, out DateTime delDt))
                              {
                                  _dDeliveryDate = delDt.ToString("dd/MMM/yyyy");
                              }
                              else
                              {
                                  _dDeliveryDate = "";
                              }
                              string sentDate = root.TryGetProperty("SentDate", out var sentElem) ? sentElem.GetString() : root.TryGetProperty("OrderDate", out var OrdrDate) ? OrdrDate.GetString() : "Not found";
                              if (DateTime.TryParseExact(sentDate, new[] { "yyyy-MM-dd", "MM/dd/yyyy", "dd/MMM/yyyy", "dd-MMM-yyyy", "MM-dd-yyyy", "dd/MMM/yyyy", "dd/MMM/yyyy HH:mm:ss" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                              {
                                  _dOrderDate = parsedDate.ToString("dd/MMM/yyyy");
                              }
                              else
                              {
                                  _dOrderDate = "";
                              }
                          }*/
                        using (JsonDocument doc = JsonDocument.Parse(jsonOutput))
                        {
                            JsonElement root = doc.RootElement;

                            // Handle JSON Array
                            if (root.ValueKind == JsonValueKind.Array)
                            {
                                if (root.GetArrayLength() > 0)
                                {
                                    root = root[0];
                                }
                                else
                                {
                                    throw new Exception("JSON array is empty.");
                                }
                            }

                            // Ensure JSON Object
                            if (root.ValueKind != JsonValueKind.Object)
                            {
                                throw new Exception("Invalid JSON format. Expected an object.");
                            }

                            string orderNumber = root.TryGetProperty("OrderNumber", out JsonElement orderNoElem)
                                ? orderNoElem.ToString() : "Not found";

                            _cOrderNo = orderNumber;

                            string LinkcustomerName =
                                root.TryGetProperty("CustomerName", out JsonElement custN) ? custN.ToString() :
                                root.TryGetProperty("CustomerNumber", out JsonElement custElem) ? custElem.ToString() :
                                root.TryGetProperty("LocationName", out JsonElement locElem) ? locElem.ToString() :
                                "Not found";

                            _cCustomerName = LinkcustomerName;

                            string deliveryDate = root.TryGetProperty("DeliveryDate", out JsonElement delElem)
                                ? delElem.ToString() : "Not found";

                            DateTime delDt;

                            if (DateTime.TryParse(deliveryDate, out delDt))
                            {
                                _dDeliveryDate = delDt.ToString("dd/MMM/yyyy");
                            }
                            else
                            {
                                _dDeliveryDate = "";
                            }

                            string sentDate =
                                root.TryGetProperty("SentDate", out JsonElement sentElem) ? sentElem.ToString() :
                                root.TryGetProperty("OrderDate", out JsonElement OrdrDate) ? OrdrDate.ToString() :
                                "Not found";

                            DateTime parsedDate;

                            if (DateTime.TryParseExact(
                                sentDate,
                                new[] { "yyyy-MM-dd", "MM/dd/yyyy", "dd/MMM/yyyy",
                "dd-MMM-yyyy", "MM-dd-yyyy", "dd/MMM/yyyy HH:mm:ss" },
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None,
                                out parsedDate))
                            {
                                _dOrderDate = parsedDate.ToString("dd/MMM/yyyy");
                            }
                            else
                            {
                                _dOrderDate = "";
                            }
                        }
                    }
                    else
                    {
                        string bodyText = MBody;
                        string possibleJson = await GetJsonFromChatGPTMBODY(bodyText);
                        if (!string.IsNullOrEmpty(possibleJson))
                        {
                            _selectedMailJSON = possibleJson;
                            using (JsonDocument doc = JsonDocument.Parse(possibleJson))
                            {
                                var root = doc.RootElement;
                                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                                {
                                    var orderObj = root[0];
                                    string orderNumber = orderObj.TryGetProperty("OrderNumber", out var orderNoElem) ? orderNoElem.GetString() : orderObj.TryGetProperty("OrderID", out var orderIdElem) ? orderIdElem.GetString() : orderObj.TryGetProperty("Order", out var orderElem) ? orderElem.GetString() : "Not found";
                                    _cOrderNo = orderNumber;
                                    string CustomerName = orderObj.TryGetProperty("CustomerName", out var custElem) ? custElem.GetString() : "Not found";
                                    _cCustomerName = CustomerName;
                                    string orderDateStr = orderObj.TryGetProperty("OrderDate", out var ordDateElem) ? ordDateElem.GetString() : "";
                                    if (DateTime.TryParse(orderDateStr, out DateTime ordDate)) _dOrderDate = ordDate.ToString("dd/MMM/yyyy"); else _dOrderDate = "";
                                    string deliveryDateStr = orderObj.TryGetProperty("DeliveryDate", out var delDateElem) ? delDateElem.GetString() : "";
                                    if (DateTime.TryParse(deliveryDateStr, out DateTime delDate)) _dDeliveryDate = delDate.ToString("dd/MMM/yyyy"); else _dDeliveryDate = "";
                                }
                            }
                            possibleJson = possibleJson.Replace("```json", "").Replace("```", "").Trim();
                            JArray jsonArray = JArray.Parse(possibleJson);
                            if (jsonArray.Count > 0)
                            {
                                JObject first = (JObject)jsonArray[0];
                                if (first["Items"] != null && first["Items"].Type == JTokenType.Array)
                                {
                                    JArray itemsArray = (JArray)first["Items"];
                                    dt = itemsArray.ToObject<DataTable>();
                                }
                            }
                        }
                        else
                        {
                            dt = FnExtractOrderDetailsFromMBody(bodyText);
                        }
                    }
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains("ProductID"))
                        dt.Columns["ProductID"].ColumnName = "Code";
                    if (dt.Columns.Contains("ID"))
                        dt.Columns["ID"].ColumnName = "Code";
                    if (dt.Columns.Contains("ItemCode"))
                        dt.Columns["ItemCode"].ColumnName = "Code";
                    if (!dt.Columns.Contains("Code"))
                    {
                        dt.Columns.Add("Code", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Code"] = "";
                        }
                    }
                    if (dt.Columns.Contains("Name"))
                        dt.Columns["Name"].ColumnName = "ItemName";
                    if (dt.Columns.Contains("Product"))
                        dt.Columns["Product"].ColumnName = "ItemName";
                    if (dt.Columns.Contains("Item Name"))
                        dt.Columns["Item Name"].ColumnName = "ItemName";
                    if (!dt.Columns.Contains("ItemName"))
                    {
                        dt.Columns.Add("ItemName", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["ItemName"] = "";
                        }
                    }
                    if (dt.Columns.Contains("Unit"))
                        dt.Columns["Unit"].ColumnName = "UOM";
                    if (!dt.Columns.Contains("UOM"))
                    {
                        dt.Columns.Add("UOM", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["UOM"] = "";
                        }
                    }
                    if (dt.Columns.Contains("Count"))
                        dt.Columns["Count"].ColumnName = "Quantity";
                    if (dt.Columns.Contains("DefaultQuantity"))
                        dt.Columns["DefaultQuantity"].ColumnName = "Quantity";
                    if (!dt.Columns.Contains("Quantity"))
                    {
                        dt.Columns.Add("Quantity", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Quantity"] = "";
                        }
                    }
                    if (!dt.Columns.Contains("Price"))
                    {
                        dt.Columns.Add("Price", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Price"] = "";
                        }
                    }
                    if (!dt.Columns.Contains("MRP"))
                    {
                        dt.Columns.Add("MRP", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["MRP"] = "";
                        }
                    }
                    if (!dt.Columns.Contains("FreeQty"))
                    {
                        dt.Columns.Add("FreeQty", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["FreeQty"] = "";
                        }
                    }
                    if (!dt.Columns.Contains("DiscPerc"))
                    {
                        dt.Columns.Add("DiscPerc", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["DiscPerc"] = "";
                        }
                    }
                    if (!dt.Columns.Contains("DiscAmt"))
                    {
                        dt.Columns.Add("DiscAmt", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["DiscAmt"] = "";
                        }
                    }
                    string MbodyHtml = await FnGetMailHtmlBodyAsync(mail.UniqueId, nEmailCredId);
                    _SelectedMailSubject = mail.EmailSubject;
                    _SelectedMailDate = mail.DateReceived.ToString();
                    _SelectedMailBody = "";
                    _SelectedMailFrom = mFrom;
                    _SelectedMailUId = cUniqueId;
                    _SelectedMailBodyHTML = MbodyHtml;
                    //------------------------------------------INSERT--------------------------------------------------------------------


                    //if (!string.IsNullOrEmpty(_SelectedMailUId) && !string.IsNullOrEmpty(_selectedMailJSON))
                    //{
                    //    FnSaveJsonToFileAndUpdate(_selectedMailJSON, _SelectedMailUId);
                    //}
                    ////-------Added on 12-01-2026-----------------------------------------------
                    //if (nTokenId > 0 && !string.IsNullOrEmpty(_SelectedMailBodyHTML) && !string.IsNullOrEmpty(_SelectedMailUId))
                    //{
                    //    FnSaveHtmlToFileAndUpdate(_SelectedMailBodyHTML, nTokenId, _SelectedMailUId);
                    //}
                    //---------------------------------------------------------------------------
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        ObjParse.TokenNo = ObjCls.FnIsDouble(HdnTokenNo.Value);
                        ObjParse.MailId = ObjCls.FnIsNumeric(HdnMailId.Value);
                        //ObjParse.MailUId = HdnMailInbIds.Value;
                        ObjParse.FnDeleteitemsFromPOBeforeReExtract();

                        foreach (DataRow row in dt.Rows)
                        {
                            if (row == null) continue;
                            ObjParse.OrderNo = _cOrderNo;
                            ObjParse.OrderDate = ObjParse.FnDateTime(_dOrderDate);
                            ObjParse.DeliveryDate = ObjParse.FnDateTime(_dDeliveryDate);
                            ObjParse.AICustomerName = _cCustomerName;
                            ObjParse.TType = "AI";
                            ObjParse.AIItemCode = Convert.ToString(row["Code"]).Trim();
                            ObjParse.AIItemName = Convert.ToString(row["ItemName"]).Trim();
                            ObjParse.UomName = Convert.ToString(row["UOM"]).Trim();
                            ObjParse.AIQty = Convert.ToString(row["Quantity"]).Trim();
                            ObjParse.AIPrice = Convert.ToString(row["Price"]).Trim();
                            ObjParse.MRP = ObjParse.FnIsDouble(row["MRP"].ToString().Trim());
                            ObjParse.FreeQty = ObjParse.FnIsDouble(row["FreeQty"].ToString());
                            ObjParse.DiscPerc = ObjParse.FnIsDouble(row["DiscPerc"].ToString());
                            ObjParse.DiscAmt = ObjParse.FnIsDouble(row["DiscAmt"].ToString());
                            ObjParse.Qty = ObjCls.FnIsDouble(row["Quantity"].ToString().Trim());
                            ObjParse.Price = ObjCls.FnIsDouble(row["MRP"].ToString());
                            ObjParse.TotalAmt = ObjParse.Qty * ObjParse.Price;
                            ObjParse.FnReExtractInsertToPODetails();
                        }
                        objMail.MailId = ObjCls.FnIsNumeric(HdnMailId.Value);
                        objMail.TokenNo = ObjCls.FnIsDouble(HdnTokenNo.Value);
                        objMail.AICustomerName = LblCustName.Text.Trim();
                        objMail.IsEdit = 0;
                        ViewState["DT_CHILD"] = objMail.FnGetOrderData();
                    }
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        private async Task<string> GetJsonFromChatGPTMBODY(string inputText)
        {
            try
            {
                string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                string prompt = "";
                if (ViewState["PromptMBODY"] != null)
                {
                    prompt = ViewState["PromptMBODY"].ToString();
                }
                else
                {
                    prompt = "You are a JSON converter. Convert tabular/plain text into structured JSON.";
                }
                string today = DateTime.Today.ToString("dd-MM-yyyy");
                string tomorrow = DateTime.Today.AddDays(1).ToString("dd-MM-yyyy");
                prompt = prompt.Replace("{{TODAY_DATE}}", today).Replace("{{TOMORROW_DATE}}", tomorrow);
                var requestBody = new
                {
                    model = "gpt-4.1-mini", // or gpt-4o-mini
                    messages = new[]
                    {
                new { role = "system", content =prompt },
                new { role = "user", content = inputText }
                    }
                };
                var response = await httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
                );
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);
                string jsonOutput = result.choices[0].message.content;
                return jsonOutput.Trim();
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in GrdVwRecords.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("ChkSelect");
                    HiddenField hdnId = (HiddenField)row.FindControl("HdnId");
                    if (chk != null && chk.Checked)
                    {
                        objMail.ID = Convert.ToInt32(hdnId.Value);
                        objMail.FnDeleteMailData();
                    }
                }

                ViewState["DT"] = objMail.FnGetMailMasterData();
                FnGridViewBinding("MAIL");
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }

        private async Task<string> FnGetMailHtmlBodyAsync(string uniqueId, string cEmailCredId)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    DataTable CDT = objMail.FnGetMailCred();
                    int id = Convert.ToInt32(cEmailCredId);
                    DataRow[] rows = CDT.Select("nId = " + id);
                    List<MailSummary> allMailList = new List<MailSummary>();
                    await client.ConnectAsync("imap.gmail.com", 993, true);
                    if (rows.Length > 0)
                    {
                        DataRow dr = rows[0];
                        string cEmailId = Convert.ToString(dr["cEmailId"]).Trim();
                        string cEmailAppPassword = Convert.ToString(dr["cEmailAppPassword"]).Trim();
                        await client.AuthenticateAsync(cEmailId, cEmailAppPassword);
                    }
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    var message = await inbox.GetMessageAsync(new UniqueId(uint.Parse(uniqueId)));
                    await client.DisconnectAsync(true);
                    return message.HtmlBody ?? message.TextBody ?? "(No Body)";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        private async Task<List<string>> FnGetMailHtmlBodyLinkAsync(string uniqueId, string cEmailCredId)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    DataTable CDT = objMail.FnGetMailCred();


                    int id = Convert.ToInt32(cEmailCredId);
                    DataRow[] rows = CDT.Select("nId = " + id);
                    List<MailSummary> allMailList = new List<MailSummary>();
                    await client.ConnectAsync("imap.gmail.com", 993, true);
                    if (rows.Length > 0)
                    {
                        DataRow dr = rows[0];
                        string cEmailId = Convert.ToString(dr["cEmailId"]).Trim();
                        string cEmailAppPassword = Convert.ToString(dr["cEmailAppPassword"]).Trim();
                        await client.AuthenticateAsync(cEmailId, cEmailAppPassword);
                    }
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    var message = await inbox.GetMessageAsync(new UniqueId(uint.Parse(uniqueId)));
                    await client.DisconnectAsync(true);
                    string bodyHtml = message.HtmlBody ?? message.TextBody ?? "";
                    var links = Regex.Matches(bodyHtml, @"https?://[^\s'\""<>()]+").Cast<Match>().Select(m => WebUtility.HtmlDecode(m.Value)).Distinct().ToList();
                    return links;
                }
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
        private DataTable FnExtractOrderDetailsFromMBody(string bodyText)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Code");
            dt.Columns.Add("ItemName");
            dt.Columns.Add("UOM");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Price");
            try
            {
                string cleanText = System.Text.RegularExpressions.Regex.Replace(bodyText, "<.?>", string.Empty);
                cleanText = System.Net.WebUtility.HtmlDecode(cleanText);
                string pattern = @"(\d+)\s+*([^]+)*\s\\s[A-Za-z\s()]\s*([^]+)*\s([\d\sx]+)\s+([\d.]+)\s+£?([\d.]+)";
                var matches = System.Text.RegularExpressions.Regex.Matches(cleanText, pattern);
                foreach (System.Text.RegularExpressions.Match m in matches)
                {
                    DataRow dr = dt.NewRow();
                    dr["Code"] = m.Groups[1].Value.Trim();
                    dr["ItemName"] = m.Groups[3].Value.Trim();
                    dr["UOM"] = m.Groups[4].Value.Trim().Replace("x", "x ").Trim();
                    dr["Quantity"] = m.Groups[5].Value.Trim();
                    dr["Price"] = m.Groups[6].Value.Trim();
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
                return dt;
            }
        }
        private async Task<string> FnGetJsonFromUrlAsync(string url)
        {
            using (var localHttpClient = new HttpClient())
            {
                try
                {
                    string htmlContent = await localHttpClient.GetStringAsync(url);
                    string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                    localHttpClient.DefaultRequestHeaders.Clear();
                    localHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                    string Promptlink = ViewState["PromptLINK"].ToString();
                    string today = DateTime.Today.ToString("dd-MM-yyyy");
                    string tomorrow = DateTime.Today.AddDays(1).ToString("dd-MM-yyyy");
                    Promptlink = Promptlink.Replace("{{TODAY_DATE}}", today).Replace("{{TOMORROW_DATE}}", tomorrow);

                    var requestBody = new
                    {
                        model = "gpt-4.1-mini",
                        messages = new[]
                   {
                    new { role = "system", content = Promptlink },
                    new { role = "user", content = htmlContent }
                   }
                    };
                    // Post request to OpenAI
                    var response = await localHttpClient.PostAsync(
                        "https://api.openai.com/v1/chat/completions",
                        new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
                    );
                    // Process API Response
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        return $"Error: OpenAI API request failed with status code {response.StatusCode}. Response: {responseContent}";
                    }
                    dynamic result = JsonConvert.DeserializeObject(responseContent);
                    string jsonOutput = result?.choices?[0]?.message?.content ?? "No JSON output received.";
                    jsonOutput = Regex.Replace(jsonOutput, @"^```json\s*|\s*```$", "", RegexOptions.Multiline).Trim();
                    jsonOutput = Regex.Replace(jsonOutput, @"^```|```$", "", RegexOptions.Multiline).Trim();
                    return jsonOutput.Trim();
                }
                catch (Exception ex)
                {
                    //string logFilePath = System.IO.Path.Combine(Application.StartupPath, "ErrorLog.txt");
                    //File.AppendAllText(logFilePath, $"{DateTime.Now}: {ex.Message} - URL: {url}\n{ex.StackTrace}\n");
                    return "Error: " + ex.Message;
                }
            }
        }
        private DataTable FnConvertJsonToDataTable(string json)
        {
            try
            {
                json = json.Trim();

                JToken token = JToken.Parse(json);

                JToken itemsToken = null;

                if (token.Type == JTokenType.Array)
                {
                    if (token.HasValues)
                    {
                        JToken firstObject = token.First;

                        if (firstObject.Type == JTokenType.Object)
                        {
                            itemsToken = firstObject["Items"];
                        }
                    }
                }
                else if (token.Type == JTokenType.Object)
                {
                    itemsToken = token["Items"];

                    // Support direct arrays inside an object
                    if (itemsToken == null)
                    {
                        foreach (JProperty property in ((JObject)token).Properties())
                        {
                            if (property.Value.Type == JTokenType.Array)
                            {
                                itemsToken = property.Value;
                                break;
                            }
                        }
                    }

                    // Single object without Items array
                    if (itemsToken == null)
                    {
                        DataTable dt = new DataTable();

                        foreach (JProperty prop in ((JObject)token).Properties())
                        {
                            dt.Columns.Add(prop.Name);
                        }

                        DataRow row = dt.NewRow();

                        foreach (JProperty prop in ((JObject)token).Properties())
                        {
                            row[prop.Name] = prop.Value.ToString();
                        }

                        dt.Rows.Add(row);

                        return dt;
                    }
                }

                if (itemsToken != null && itemsToken.Type == JTokenType.Array)
                {
                    return JsonConvert.DeserializeObject<DataTable>(
                        itemsToken.ToString());
                }

                return null;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
                return null;
            }
            /*  try
              {
                  json = json.Trim();
                  JToken token = JToken.Parse(json);
                  if (token.Type == JTokenType.Array)
                  {
                      return JsonConvert.DeserializeObject<DataTable>(json);
                  }
                  else if (token.Type == JTokenType.Object)
                  {
                      var obj = (JObject)token;
                      foreach (var property in obj.Properties())
                      {
                          if (property.Value.Type == JTokenType.Array)
                          {
                              return JsonConvert.DeserializeObject<DataTable>(property.Value.ToString());
                          }
                      }
                      DataTable dt = new DataTable();
                      foreach (var prop in obj.Properties())
                      {
                          dt.Columns.Add(prop.Name);
                      }

                      DataRow row = dt.NewRow();
                      foreach (var prop in obj.Properties())
                      {
                          row[prop.Name] = prop.Value.ToString();
                      }
                      dt.Rows.Add(row);
                      return dt;
                  }
                  return null;
              }
              catch (Exception ex)
              {
                  return null;
              }*/
        }
        private async Task<DataTable> FnProcessPdfAndExtract(string pdfFile)
        {
            DataTable dtItems = new DataTable();
            try
            {
                if (string.IsNullOrWhiteSpace(pdfFile))
                {
                    return dtItems;
                }
                if (!File.Exists(pdfFile))
                {
                    return dtItems;
                }
                string extractedText = "";
                using (PdfReader reader = new PdfReader(pdfFile))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                        extractedText += PdfTextExtractor.GetTextFromPage(reader, page);
                }
                extractedText = extractedText.Replace("\r", " ").Replace("\n", " ").Trim();
                string openAiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                string promptToSend = ViewState["PromptPDF"].ToString();
                string today = DateTime.Today.ToString("dd-MM-yyyy");
                string tomorrow = DateTime.Today.AddDays(1).ToString("dd-MM-yyyy");
                promptToSend = promptToSend.Replace("{{TODAY_DATE}}", today).Replace("{{TOMORROW_DATE}}", tomorrow);


                string result = await AskChatGPT(openAiKey, extractedText, promptToSend);
                result = result.Replace("```json", "").Replace("```", "").Trim();
                JArray jsonArray = JArray.Parse(result);
                _selectedMailJSON = result;
                //foreach (var obj in jsonArray)
                //{
                //    var cust = obj["CustomerName"]?.ToString();
                //    if (!string.IsNullOrEmpty(cust))
                //    {
                //        cust = Regex.Replace(cust, @"^.*?\bKitchen\b", "Kitchen", RegexOptions.IgnoreCase).Trim();
                //        obj["CustomerName"] = cust;
                //    }
                //}
                if (jsonArray.Count > 0)
                {
                    JObject first = (JObject)jsonArray[0];
                    _cOrderNo = (string)first["OrderNumber"] ?? "";
                    if (first["OrderDate"] != null && DateTime.TryParse(first["OrderDate"].ToString(), out DateTime orderDate))
                    {
                        _dOrderDate = orderDate.ToString("dd/MMM/yyyy");
                    }
                    else
                    {
                        _dOrderDate = "";
                    }
                    if (first["DeliveryDate"] != null && DateTime.TryParse(first["DeliveryDate"].ToString(), out DateTime deliveryDate))
                    {
                        _dDeliveryDate = deliveryDate.ToString("dd/MMM/yyyy");
                    }
                    else
                    {
                        _dDeliveryDate = "";
                    }
                    _cCustomerName = (string)first["CustomerName"] ?? "";
                    if (first["Items"] != null && first["Items"].Type == JTokenType.Array)
                    {
                        JArray itemsArray = (JArray)first["Items"];
                        dtItems = itemsArray.ToObject<DataTable>();
                    }
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
                return new DataTable();
            }
            return dtItems;
        }
        public DataTable FnCreateDataTableGeneral(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Columns.Contains("code"))
                    dt.Columns["code"].ColumnName = "AICode";
                if (dt.Columns.Contains("Item Name"))
                    dt.Columns["Item Name"].ColumnName = "AIItemName";
                if (dt.Columns.Contains("ItemName"))
                    dt.Columns["ItemName"].ColumnName = "AIItemName";
                if (dt.Columns.Contains("description"))
                    dt.Columns["description"].ColumnName = "AIItemName";
                if (dt.Columns.Contains("unit"))
                    dt.Columns["unit"].ColumnName = "UOM";
                if (dt.Columns.Contains("quantity"))
                    dt.Columns["quantity"].ColumnName = "Quantity";
                if (dt.Columns.Contains("unit_price"))
                    dt.Columns["unit_price"].ColumnName = "Price";
                if (dt.Columns.Contains("ItemId"))
                    dt.Columns["ItemId"].ColumnName = "ItemId";
                if (dt.Columns.Contains("UomId"))
                    dt.Columns["UomId"].ColumnName = "UomId";
                if (dt.Columns.Contains("UomName"))
                    dt.Columns["UomName"].ColumnName = "UomName";
                if (dt.Columns.Contains("Remarks"))
                    dt.Columns["Remarks"].ColumnName = "Remarks";

                if (!dt.Columns.Contains("Code"))
                    dt.Columns.Add("Code", typeof(string));
                if (!dt.Columns.Contains("AICode"))
                    dt.Columns.Add("AICode", typeof(string));
                if (!dt.Columns.Contains("ItemName"))
                    dt.Columns.Add("ItemName", typeof(string));
                if (!dt.Columns.Contains("UOM"))
                    dt.Columns.Add("UOM", typeof(string));
                if (!dt.Columns.Contains("Quantity"))
                    dt.Columns.Add("Quantity", typeof(string));
                if (!dt.Columns.Contains("Price"))
                    dt.Columns.Add("Price", typeof(string));
                if (!dt.Columns.Contains("ItemCode"))
                    dt.Columns.Add("ItemCode", typeof(string));
                if (!dt.Columns.Contains("ItemId"))
                    dt.Columns.Add("ItemId", typeof(string));
                if (!dt.Columns.Contains("UomId"))
                    dt.Columns.Add("UomId", typeof(string));
                if (!dt.Columns.Contains("UomName"))
                    dt.Columns.Add("UomName", typeof(string));
                if (!dt.Columns.Contains("Remarks"))
                    dt.Columns.Add("Remarks", typeof(string));
            }
            return dt;
        }
        public DataTable FnInsertTextPDFLinkToDb(DataTable DResult)
        {
            try
            {
                foreach (DataRow row in DResult.Rows)
                {
                    ObjParse.TokenNo = ObjCls.FnIsDouble(HdnTokenNo.Value);
                    ObjParse.OrderNo = TxtPONo.Text.Trim();
                    ObjParse.OrderDate = ObjCls.FnDateTime(CtrlOrderDate.DateText);
                    ObjParse.DeliveryDate = ObjCls.FnDateTime(CtrlDelDate.DateText);
                    ObjParse.AICustomerName = LblCustName.Text.Trim();
                    ObjParse.AIItemName = row["AIItemName"].ToString();
                    ObjParse.AIItemCode = row["AICode"].ToString();
                    ObjParse.Qty =ObjCls.FnIsDouble(row["Quantity"].ToString());
                    ObjParse.ItemRemarks= row["Remarks"].ToString();

                    ObjParse.UomName = row["UOM"].ToString();
                    ObjParse.Price = ObjCls.FnIsDouble(row["Price"].ToString().Trim());
                    ObjParse.TType = HdnType.Value;
                    ObjParse.FnAddExtractItemAsNewOrder();
                }                
            }
            catch(Exception ex)
            {
                _strError = ex.Message;
            }
            DataTable DTInsert = ObjParse.FnGetExtractItemAsNewOrder();
            DataRow firstRow = DTInsert.Rows[0];
            TxtPONo.Text = Convert.ToString(firstRow["OrderNo"]);
            //TxtAreainput.Text = "";
            return DTInsert;

        }
        private void FnGetTotal()
        {
            double dTotal = 0;
            DataTable _dtRecords = ViewState["DT_CHILD"] as DataTable;
            if (_dtRecords.Rows.Count > 0)
            {            
                dTotal = ObjCls.FnIsDouble(_dtRecords.Compute("SUM(TotalPrice)", ""));           
            }
            TxtPriceTot.Text = "Total:- £ " + FnSetFormatedValue(dTotal);
            
        }
    }
}