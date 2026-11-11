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
    public partial class Inbox_New : ClsPageEvents
    {
        private static readonly HttpClient httpClient = new HttpClient();
        ClsAIMailDetails objMail = new ClsAIMailDetails();
        DataTable DTGeneral;
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
                    FnBindZMailRepeater();
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

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // ✅ Re-init ALL dropdowns after ANY postback
            ScriptManager.RegisterStartupScript(this, GetType(), "forceReinit", "setTimeout(function(){ if (window.initAll) initAll(); }, 100);", true);
        }
        private void FnBindZMailRepeater()
        {
            try
            {
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
                _dwMainRecord = new DataView(ViewState["DT"] as DataTable);
                // _dwMainRecord.RowFilter = " TrDate>='" + objMail.FnDateTime(CtrlFromDate.DateText.Trim().Replace("/", "-"), "dd-MM-yyyy") + "' AND TrDate<='" + objMail.FnDateTime(CtrlToDate.DateText.Trim().Replace("/", "-"), "dd-MM-yyyy") + "'";
                if (_dwMainRecord.ToTable().Rows.Count > 0)
                {
                    GrdVwMailDtls.DataSource = _dwMainRecord.ToTable();
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
                    FnBindZMailRepeater();
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Deleted!');", true);
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
                FnBindZMailRepeater();
            }
            catch (Exception ex)
            {
                FnPopUpAlert(ex.Message);
            }
        }
    }
}