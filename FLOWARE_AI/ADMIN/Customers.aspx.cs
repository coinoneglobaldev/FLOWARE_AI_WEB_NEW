using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FLOWARE_AI.App_Code;

namespace FLOWARE_AI.ADMIN
{
    public partial class Customers : ClsPageEvents
    {
        ClsAICustomerList ObjCls = new ClsAICustomerList();
        DataTable dt;
        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindStaticGrid();
            }
        }
        private void BindStaticGrid()
        {

            dt = new DataTable();       
            ObjCls.cCode = TxtCode.Text;
            ObjCls.cName = TxtName.Text;
            dt = ObjCls.FnGetCustomerList();
            GrdVwRecords.DataSource = dt;
            GrdVwRecords.DataBind();
        }
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            BindStaticGrid();
        }

        protected void GrdVwRecords_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrdVwRecords.PageIndex = e.NewPageIndex;
            BindStaticGrid();
        }
        protected void BtnUpdateCust_Click(object sender, EventArgs e)
        {
            string str = ObjCls.FnSynchronizeCustomers();
            base.FnShowAlert(str);
        }
    }
}