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
    public partial class DirectNews : ClsPageEvents
    {
        private static readonly HttpClient httpClient = new HttpClient();
        ClsAIMailDetails objMail = new ClsAIMailDetails();
        DataTable DTGeneral;
        protected override void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnSearch1_Click(object sender, EventArgs e)
        {

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
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "IsExport").ToString() == "1")
                {
                    e.Row.CssClass += " export-row";
                }
            }
        }
        private void FnBindZMailRepeater()
        {
            try
            {
                objMail.FromDate = objMail.FnDateTime(CtrlFromDate.DateText.Trim().Replace("/", "-"));
                objMail.ToDate = objMail.FnDateTime(CtrlToDate.DateText.Trim().Replace("/", "-"));

                objMail.cSubjectMail = TxtSubject.Text.Trim();
                objMail.cFromMail = TxtMail.Text.Trim();
                ViewState["DT"] = objMail.FnGetDirectDataExport();
                //FnGridViewBinding();
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
                //FnGridViewBinding();
            }
            catch (Exception ex)
            {
                FnPopUpAlert(ex.Message);
            }
        }
        
    }
}