using FLOWARE_AI.App_Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FLOWARE_AI.ADMIN
{
    public partial class ViewEmail : ClsPageEvents
    {
        ClsAIMailDetails objCls = new ClsAIMailDetails();
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    objCls.MailId =objCls.FnIsNumeric(Request.QueryString["MailId"]);
                    DataTable DTB =objCls.GetMailBodyHtmlFromDB();
                    if (DTB != null && DTB.Rows.Count > 0)
                    {
                        LoadMailBody(DTB);
                        string pdfUrl =Convert.ToString(DTB.Rows[0]["PDFPathUrl"]);
                        if (!string.IsNullOrWhiteSpace(pdfUrl))
                        {
                            pdfUrl = pdfUrl.Trim();
                            pdfUrl = pdfUrl.Replace("\\", "/");
                            if (!pdfUrl.StartsWith("http://",StringComparison.OrdinalIgnoreCase) && !pdfUrl.StartsWith("https://",StringComparison.OrdinalIgnoreCase))
                            {
                                pdfUrl ="http://" + pdfUrl.TrimStart('/');
                            }

                            string script ="window.addEventListener('load', function() {" + "loadPDF('" + HttpUtility.JavaScriptStringEncode(pdfUrl) + "');" + "});";
                            ClientScript.RegisterStartupScript(this.GetType(),"LoadPDF",script,true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;

                LitMailBody.Text ="<b>Error:</b> " + Server.HtmlEncode(ex.Message);
            }
        }
        private void LoadMailBody(DataTable DTB)
        {
            string htmlFileName =Convert.ToString(DTB.Rows[0]["MailBody"]);
            if (!string.IsNullOrWhiteSpace(htmlFileName))
            {
                string folderPath =ConfigurationManager.AppSettings["HTML_PATH"];
                string filePath =Path.Combine(folderPath, htmlFileName);
                //string filePath= @"D:\github\FLOWARE_AI_WEB_NEW\FLOWARE_AI\UploadedFiles\MailHtmlFiles\80602270826.html";
                if (File.Exists(filePath))
                {
                    string htmlContent =File.ReadAllText(filePath,Encoding.UTF8);
                    LitMailBody.Text = htmlContent;
                }
                else
                {
                    LitMailBody.Text ="<b>Mail body HTML file not found.</b>";
                }
            }
            else
            {
                LitMailBody.Text ="<b>No mail body available.</b>";
            }

        }
    }
}