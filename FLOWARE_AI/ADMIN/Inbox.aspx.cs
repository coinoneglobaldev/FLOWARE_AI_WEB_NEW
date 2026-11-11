using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Web.UI;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Xml;
using System.Collections;
using FLOWARE_AI.App_Code;
using System.Web.Services;
using System.Globalization;

namespace FLOWARE_AI.ADMIN
{
    public partial class Inbox : ClsPageEvents
    {
        private static readonly HttpClient httpClient = new HttpClient();
        ClsAIMailDetails objMail = new ClsAIMailDetails();
        DataTable DTGeneral;
        string str;
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
                RegisterGridControlMasterScript();
                if (!IsPostBack)
                {
                    CtrlFromDate.DateText= DateTime.Now.ToString("dd/MMM/yyyy").Replace("-", "/");
                    CtrlToDate.DateText = DateTime.Now.AddDays(1).ToString("dd/MMM/yyyy").Replace("-", "/");
                    FnFindRecord();
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

        private void FnFindRecord()
        {
            try
            {
                objMail.FromDate = objMail.FnDateTime(CtrlFromDate.DateText.Trim().Replace("/", "-"));
                objMail.ToDate = objMail.FnDateTime(CtrlToDate.DateText.Trim().Replace("/", "-"));
                objMail.SubjectMail = TxtSubject.Text.Trim();
                objMail.FromMail = TxtMail.Text.Trim();
                ViewState["DT"] = objMail.FnGetMailDataInbox();
                FnGridViewBinding();
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        private void FnGridViewBinding()
        {
            try
            {
                DataTable dtSrc = ViewState["DT"] as DataTable;
                _dwMainRecord = new DataView(dtSrc);

                string sortCol = Convert.ToString(ViewState["SortCol"]);
                if (!string.IsNullOrEmpty(sortCol) && dtSrc != null && dtSrc.Columns.Contains(sortCol))
                    _dwMainRecord.Sort = "[" + sortCol + "] " + (Convert.ToString(ViewState["SortDir"]) == "DESC" ? "DESC" : "ASC");

                DataTable dtView = _dwMainRecord.ToTable();
                if (dtView.Rows.Count > 0)
                {
                    GrdVwMailDtls.DataSource = dtView;
                    GrdVwMailDtls.DataBind();
                    LblMsg.Visible = false;
                }
                else
                {
                    LblMsg.Visible = true;
                    GrdVwMailDtls.DataSource = DTGeneral;
                    GrdVwMailDtls.DataBind();
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void GrdVwMailDtls_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try 
            { 
                if (e.CommandName == "DeleteMail")
                { 
                    int mailId = Convert.ToInt32(e.CommandArgument);
                    objMail.ID = mailId;
                    objMail.FnDeleteMailData();
                    FnFindRecord();
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg","alert('Deleted!');", true);
                }                
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }

        protected void GrdVwMailDtls_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    CheckBox ChkSelectAll = (CheckBox)e.Row.FindControl("ChkSelectAll");
                    ChkSelectAll.Attributes.Add("onclick", "return GridChkSelectAll();");

                    CheckBox ChkSelectAllB = (CheckBox)e.Row.FindControl("ChkSelectAllB");
                    ChkSelectAllB.Attributes.Add("onclick", "return GridChkSelectAllB();");

                    // mark the sorted column header
                    string sortCol = Convert.ToString(ViewState["SortCol"]);
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        DataControlFieldHeaderCell h = cell as DataControlFieldHeaderCell;
                        if (h != null && !string.IsNullOrEmpty(sortCol) && h.ContainingField.SortExpression == sortCol)
                            cell.CssClass += (Convert.ToString(ViewState["SortDir"]) == "DESC" ? " sort-desc" : " sort-asc");
                    }
                }
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Button btnDelete = e.Row.FindControl("BtnDelete") as Button;

                    if (DataBinder.Eval(e.Row.DataItem, "IsChk").ToString() == "1")
                    {
                        e.Row.CssClass += " export-row";
                    }
                    if (btnDelete != null)
                    {
                        string subject = e.Row.Cells[1].Text; // Subject column index
                        btnDelete.OnClientClick = $"return confirm('Delete mail: {subject}?');";
                    }                  
                }
            }
            catch (Exception ex)
            {
                FnPopUpAlert(ex.Message);
            }
        }
        protected void GrdVwMailDtls_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GrdVwMailDtls.PageIndex = e.NewPageIndex;
                FnGridViewBinding();
            }
            catch (Exception ex)
            {
                FnPopUpAlert(ex.Message);
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FnFindRecord();
            }
            catch (Exception ex)
            {
                FnPopUpAlert(ex.Message);
            }
        }
        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in GrdVwMailDtls.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("ChkSelect");
                    HiddenField HdnId = (HiddenField)row.FindControl("HdnId");
                    HiddenField HdnTokenId = (HiddenField)row.FindControl("HdnTokenId");
                    HiddenField HdncUniqueId = (HiddenField)row.FindControl("HdncUniqueId");
                    if (chk != null && chk.Checked)
                    {
                        objMail.ID = Convert.ToInt32(HdnId.Value);
                        objMail.TokenNo = objMail.FnIsDouble(HdnTokenId.Value);
                        objMail.UniqueMailId = HdncUniqueId.Value;
                        objMail.FnDeleteDateWise();
                    }
                }

                ViewState["DT"] = objMail.FnGetMailMasterData();
                FnFindRecord();

            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }

        protected void LnkBlock_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GrdVwMailDtls.Rows)
            {
                CheckBox chkB = (CheckBox)row.FindControl("ChkSelectB");
                HiddenField HdnId = (HiddenField)row.FindControl("HdnId");
                HiddenField HdnTokenId = (HiddenField)row.FindControl("HdnTokenId");
                HiddenField HdncUniqueId = (HiddenField)row.FindControl("HdncUniqueId");
                if (chkB != null && chkB.Checked)
                {
                    Label LblFrmMail = (Label)row.FindControl("LnkFrmMail");
                    objMail.MailId = Convert.ToInt32(HdnId.Value); ;
                    objMail.FromMail = LblFrmMail.Text.Trim();
                    str = objMail.FnInsertToBlockedEmailList() as string;
                }
            }
            ViewState["DT"] = objMail.FnGetMailMasterData();
            FnFindRecord();
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('" + str.Replace("'", "\\'") + "');", true);
        }
        protected void GrdVwMailDtls_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                string dir = "ASC";
                if (Convert.ToString(ViewState["SortCol"]) == e.SortExpression &&
                    Convert.ToString(ViewState["SortDir"]) == "ASC")
                    dir = "DESC";

                ViewState["SortCol"] = e.SortExpression;
                ViewState["SortDir"] = dir;

                GrdVwMailDtls.PageIndex = 0;
                FnGridViewBinding();
            }
            catch (Exception ex)
            {
                FnPopUpAlert(ex.Message);
            }
        }
    }
}

