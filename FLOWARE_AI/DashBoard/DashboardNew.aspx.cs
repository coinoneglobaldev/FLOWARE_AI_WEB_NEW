using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FLOWARE_AI.App_Code;

namespace FLOWARE_AI.DashBoard
{
    public partial class DashboardNew : ClsPageEvents
    {
        ClsDashBoard ObjCls = new ClsDashBoard();
        DataTable dt;
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    /*if (Session["UID"] == null)
                    {
                        // Check if the user is already on the SignIn page
                        if (!Request.Url.AbsolutePath.Contains("Default.aspx"))
                        {
                            Response.Redirect("SignIn.aspx");
                        }
                    }*/
                    ViewState["UID"] = ObjCls.DecryptQueryString(Request.QueryString["UID"].ToString());
                    if (ObjCls.FnIsNumeric(ViewState["UID"].ToString()) > 1)
                    {
                        lnkTotalOrders.HRef = "javascript:void(0);";
                        lnkTotalOrders.Style.Add("cursor", "not-allowed");
                        LnkExpComp.HRef = "javascript:void(0);";
                        LnkExpComp.Style.Add("cursor", "not-allowed");
                        LnkExpPend.HRef = "javascript:void(0);";
                        LnkExpPend.Style.Add("cursor", "not-allowed");
                        LnkMapPend.HRef = "javascript:void(0);";
                        LnkMapPend.Style.Add("cursor", "not-allowed");
                        LnkTotCust.HRef = "javascript:void(0);";
                        LnkTotCust.Style.Add("cursor", "not-allowed");
                        LnkTotPrdct.HRef = "javascript:void(0);";
                        LnkTotPrdct.Style.Add("cursor", "not-allowed");
                    }
                    else
                    {
                        lnkTotalOrders.Attributes.Add("href", "../ADMIN/OrderParsingNew.aspx?UID=" + Request.QueryString["UID"].ToString());
                        LnkExpPend.Attributes.Add("href", "../ADMIN/Inbox.aspx?UID=" + Request.QueryString["UID"].ToString());
                        LnkMapPend.Attributes.Add("href", "../ADMIN/Inbox.aspx?UID=" + Request.QueryString["UID"].ToString());
                        LnkExpComp.Attributes.Add("href", "../ADMIN/Export.aspx?UID=" + Request.QueryString["UID"].ToString());
                        LnkTotPrdct.Attributes.Add("href", "../ADMIN/Products.aspx?UID=" + Request.QueryString["UID"].ToString());
                        LnkTotCust.Attributes.Add("href", "../ADMIN/Customers.aspx?UID=" + Request.QueryString["UID"].ToString());
                    }
                    dt = ObjCls.FnGetDashBoardData();
                    LblTotOrder.Text = dt.Rows[0][0].ToString();
                    LblExpPend.Text = dt.Rows[1][0].ToString();
                    LblMappPend.Text = dt.Rows[2][0].ToString();
                    LblExpComp.Text = dt.Rows[3][0].ToString();
                    LblTotProducts.Text = dt.Rows[4][0].ToString();
                    LblTotCust.Text = dt.Rows[5][0].ToString();
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
                Response.Redirect("SignIn.aspx");
            }
        }
    }
}