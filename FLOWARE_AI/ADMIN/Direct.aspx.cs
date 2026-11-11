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
    public partial class Direct : ClsPageEvents
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
                    CtrlFromDate.DateText = DateTime.Now.ToString("dd/MMM/yyyy").Replace("-", "/");
                    CtrlToDate.DateText = DateTime.Now.AddDays(1).ToString("dd/MMM/yyyy").Replace("-", "/");
                    CtrlToDate1.DateText = DateTime.Now.AddDays(1).ToString("dd/MMM/yyyy").Replace("-", "/");
                    //FnBindZMailRepeater();
                    //FnBindZMailRepeaterExp();
                    FnGridViewBinding("");
                    HrefExport.Attributes.Add("href", "../ADMIN/Export.aspx?UID=" + Request.QueryString["UID"].ToString());
                    HrefAIOrdering.Attributes.Add("href", "../ADMIN/OrderParsingNew.aspx?UID=" + Request.QueryString["UID"].ToString());
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
        private void FnFindRecord()
        {
            try
            {
                objMail.FromDate = objMail.FnDateTime(CtrlFromDate.DateText.Trim().Replace("/", "-"));
                objMail.ToDate = objMail.FnDateTime(CtrlToDate.DateText.Trim().Replace("/", "-"));

                objMail.AICustomerName = TxtCust.Text.Trim();
                objMail.OrderRefNo = TxtOrderNo.Text.Trim();
                ViewState["DT"] = objMail.FnGetDirectDataExport();
                FnGridViewBinding("PEND");
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        private void FnFindRecord_Srch()
        {
            try
            {
                objMail.FromDate = objMail.FnDateTime(CtrlFrDate.DateText.Trim().Replace("/", "-"));
                objMail.ToDate = objMail.FnDateTime(CtrlToDate1.DateText.Trim().Replace("/", "-")); 

                objMail.AICustomerName = TxtCust1.Text.Trim();
                objMail.OrderRefNo = TxtOrder1.Text.Trim();
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

                string value = Convert.ToString(dt.Rows[0][0]);

                if (value == "0")
                {
                    GrdVwRecords.DataSource = ViewState["DT"] as DataTable;
                    GrdVwRecords.DataBind();
                    GrdVwMailDtls.DataSource = ViewState["DT"] as DataTable;
                    GrdVwMailDtls.DataBind();
                }
                else 
                {
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
                FnFindRecord();
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
                FnFindRecord_Srch();
                TabContainer1.ActiveTabIndex = 1;

            }
            catch (Exception ex)
            {
                FnPopUpAlert(ex.Message);
            }
        }
    }
}

