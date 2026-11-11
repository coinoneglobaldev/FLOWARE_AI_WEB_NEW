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
    public partial class KeyWords : System.Web.UI.Page
    {
        ClsAICustList ObjCls = new ClsAICustList();
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                FnFindRecord();
                FnGridViewBinding();
            }
        }
        public void FnAssignProperty()
        {
            ObjCls.Code = TxtCode.Text;
            ObjCls.Name = TxtName.Text;
        }
        public void FnAssignProperty_Srch()
        {
            ObjCls.Code = TxtCode.Text;
            ObjCls.Name = TxtName.Text;
        }
        public void FnFindRecord()
        {
            FnAssignProperty_Srch();
            dt = ObjCls.FnGetCustList();
        }
        public void FnClear()
        {
            TxtName.Text = "";
            TxtCode.Text = "";
        }
        private void FnGridViewBinding()
        {            
            GrdVwRecords.DataSource = dt;
            GrdVwRecords.DataBind();
        }
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            FnFindRecord();
            FnGridViewBinding();
        }
        protected void GrdVwRecords_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrdVwRecords.PageIndex = e.NewPageIndex;
            FnGridViewBinding();
        }
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            FnAssignProperty();
            ObjCls.FnInsertRecord();
            FnClear();
            FnFindRecord();
            FnGridViewBinding();
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record Inserted Successfully!');", true);
        }
    }
}