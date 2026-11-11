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

namespace FLOWARE_AI.ADMIN
{
    public partial class Direct_New : ClsPageEvents
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
                    ViewState["DT"] = FnGetGeneralXml(objMail);
                    FnBindZMailRepeater();
                    FnBindZMailRepeaterExp();
                    //FnGridViewBinding("");
                }
            }
            catch (Exception ex) { _strError = ex.Message; }
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
            ScriptManager.RegisterStartupScript(this, GetType(), "forceReinit",
                "setTimeout(function(){ if (window.initAll) initAll(); }, 100);", true);
        }
        private void FnBindZMailRepeater()
        {
            try
            {
                objMail.AICustomerName = TxtCust.Text.Trim();
                objMail.SubjectMail = TxtOrderNo.Text.Trim();
                ViewState["DT"] = objMail.FnGetDirectDataExport();
                FnGridViewBinding("PEND");
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        private void FnBindZMailRepeaterExp()
        {
            try
            {
                
                objMail.AICustomerName = TxtCust1.Text.Trim();
                objMail.SubjectMail = TxtOrder1.Text.Trim();
                ViewState["DT"] = objMail.FnGetDirectDataExport();
                FnGridViewBinding("EXP");
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        private void FnGridViewBinding(string PrmFlag)
        {
            try
            {
                DataTable dt = ViewState["DT"] as DataTable;
                DataView dv = new DataView(dt);
                if (PrmFlag == "EXP")
                {
                    dv.RowFilter = "IsExport = 1";
                    GrdVwRecords.DataSource = dv;
                    GrdVwRecords.DataBind();
                    LblMsgE.Visible = false;
                }
                else if (PrmFlag == "PEND")
                {
                    dv.RowFilter = "IsExport = 0";
                    GrdVwMailDtls.DataSource = dv;
                    GrdVwMailDtls.DataBind();
                    LblMsg.Visible = false;
                }
                else
                {
                    GrdVwRecords.DataSource = ViewState["DT"] as DataTable;
                    GrdVwRecords.DataBind();
                    GrdVwMailDtls.DataSource = ViewState["DT"] as DataTable;
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
            Label LnkDate = e.Row.FindControl("LnkDate") as Label;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "IsExport").ToString() == "1")
                {
                    e.Row.CssClass += " export-row";
                }
                //if (LnkDate.Text == "")
                //{
                //    LnkDate.Text = "";
                //}
            }
        }
        protected void GrdVwMailDtls_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GrdVwMailDtls.PageIndex = e.NewPageIndex;
                FnGridViewBinding("");
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
                TabContainer1.ActiveTabIndex = 0;
            }
            catch (Exception ex)
            {
                FnPopUpAlert(ex.Message);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                FnBindZMailRepeaterExp();
                TabContainer1.ActiveTabIndex = 1;

            }
            catch (Exception ex)
            {
                FnPopUpAlert(ex.Message);
            }
        }
    }
}