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
    public partial class BlockedMails : ClsPageEvents
    {
        private static readonly HttpClient httpClient = new HttpClient();
        ClsAIMailDetails objMail = new ClsAIMailDetails();
        DataTable DTGeneral;        
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
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
       
        private void FnBindZMailRepeater()
        {
            try
            {
                objMail.FromMail = TxtMail.Text.Trim();
                objMail.AICustomerName = TxtCustomer.Text.Trim();
                ViewState["DT"] = objMail.FnGetBlockedMailList();
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
                
                if (e.CommandName == "UNBLOCK")
                {
                    int mailId = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    Label LblFrmMail = (Label)row.FindControl("LnkFrmMail");
                    objMail.ID = mailId;
                    objMail.FromMail = LblFrmMail.Text.Trim();
                    string str = objMail.FnDeleteFromBlockedEmailList() as string;
                    FnBindZMailRepeater();
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('" + str.Replace("'", "\\'") + "');", true);
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
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

