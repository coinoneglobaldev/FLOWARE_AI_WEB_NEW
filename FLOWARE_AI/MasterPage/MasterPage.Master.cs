using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FLOWARE_AI.MasterPage
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        ClsDropdownRecordList ObjCls = new ClsDropdownRecordList();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

               /* if (Session["UID"] == null)
                {
                    if (!Request.Url.AbsolutePath.Contains("Default.aspx"))
                    {
                        Response.Redirect("SignIn.aspx");
                    }
                }*/
                if (!IsPostBack)
                {
                    ViewState["UID"] = ObjCls.DecryptQueryString(Request.QueryString["UID"].ToString());
                    if (ObjCls.FnIsNumeric(ViewState["UID"].ToString()) > 1)
                    {
                        LnkCustomers.Visible = false;
                        LnkProducts.Visible = false;
                        LnkPrompt.Visible = false;
                    }
                }
                HrefDashboard.Attributes.Add("href", "../DashBoard/DashboardNew.aspx?UID=" + Request.QueryString["UID"].ToString());
                HrefProducts.Attributes.Add("href", "../ADMIN/Products.aspx?UID=" + Request.QueryString["UID"].ToString());
                HrefCustomers.Attributes.Add("href", "../ADMIN/Customers.aspx?UID=" + Request.QueryString["UID"].ToString());
                HrefPromt.Attributes.Add("href", "../ADMIN/Prompt.aspx?UID=" + Request.QueryString["UID"].ToString());

                HrefAiOrdering.Attributes.Add("href", "../ADMIN/OrderParsingNew.aspx?UID=" + Request.QueryString["UID"].ToString());
                //HrefInbox.Attributes.Add("href", "../ADMIN/Inbox_New.aspx?UID=" + Request.QueryString["UID"].ToString());
                //HrefExport.Attributes.Add("href", "../ADMIN/Export_New.aspx?UID=" + Request.QueryString["UID"].ToString());
                //HrefDirect.Attributes.Add("href", "../ADMIN/Direct_New.aspx?UID=" + Request.QueryString["UID"].ToString());

                HrefInbox.Attributes.Add("href", "../ADMIN/Inbox.aspx?UID=" + Request.QueryString["UID"].ToString());
                HrefExport.Attributes.Add("href", "../ADMIN/Export.aspx?UID=" + Request.QueryString["UID"].ToString());
                HrefDirect.Attributes.Add("href", "../ADMIN/Direct.aspx?UID=" + Request.QueryString["UID"].ToString());
                HrefKeyWords.Attributes.Add("href", "../ADMIN/KeyWords.aspx?UID=" + Request.QueryString["UID"].ToString());
                HrefBMails.Attributes.Add("href", "../ADMIN/BlockedMails.aspx?UID=" + Request.QueryString["UID"].ToString());

                ServiceReference ScrAjax = new ServiceReference();
                ScrAjax.Path = "../Web_GetResultService.asmx";
                ScriptManager1.Services.Add(ScrAjax);

                ServiceReference Scr = new ServiceReference();
                Scr.Path = "../Web_GridSelection.asmx";
                ScriptManager1.Services.Add(Scr);

                ServiceReference ScrGrid = new ServiceReference();
                ScrGrid.Path = "../WebGridSelection_Grid.asmx";
                ScriptManager1.Services.Add(ScrGrid);

               /* ServiceReference ScrGrid1 = new ServiceReference();
                ScrGrid1.Path = "../WebGridSelection_New.asmx";
                ScriptManager1.Services.Add(ScrGrid1);*/

            }
            catch (Exception ex)
            {
                Response.Redirect("SignIn.aspx");
            }
        }
        public string FnUrl()
        {
            return "";
        }
    }
}