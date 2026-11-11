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
    public partial class Products : ClsPageEvents
    {
        ClsAIProductList ObjCls = new ClsAIProductList();
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
            ObjCls.cCode = TxtCode.Text;
            ObjCls.cName = TxtName.Text;
            dt = ObjCls.FnGetProductList();
            // Bind to GridView
            GrdVwRecords.DataSource = dt;
            GrdVwRecords.DataBind();
        }
        protected void GrdVwRecords_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrdVwRecords.PageIndex = e.NewPageIndex;
            BindStaticGrid();
        }
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            BindStaticGrid();
        }
        protected void BtnUpdateItems_Click(object sender, EventArgs e)
        {
           string str=ObjCls.FnSynchronizeProducts();
            base.FnShowAlert(str);
        }
    }
}