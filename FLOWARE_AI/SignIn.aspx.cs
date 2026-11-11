using FLOWARE_AI.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FLOWARE_AI
{
    public partial class SignIn : System.Web.UI.Page
    {
        ClsUserLog ObjCls = new ClsUserLog();
        protected void Page_Init(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            TxtUserName.Attributes.Add("autocomplete", "off");
            TxtPwd.Attributes.Add("autocomplete", "off");
            if (!IsPostBack)
            {
            }
        }
        public void FnPopUpAlert(string PrmMsgString)
        {
            Page objPage = (HttpContext.Current.CurrentHandler) as Page;
            if (objPage != null)
            {
                ScriptManager.RegisterClientScriptBlock(objPage, typeof(ClsPageEvents), "Msg", PrmMsgString, false);
            }
        }


        public void FnFocus(Control PrmCntrl)
        {
            ScriptManager1.SetFocus(PrmCntrl);
        }
        protected void BtnSignIn_Click(object sender, EventArgs e)
        {

            string strAppPage = "";
            int iUId = 0, iGrpId = 0;
            try
            {
                DataTable dtLog = ObjCls.FnLogIn(TxtUserName.Text.Trim(), ObjCls.EncryptQueryString(TxtPwd.Text.Trim()));
                if (dtLog.Rows.Count > 0)
                {
                    iUId = ObjCls.FnIsNumeric(dtLog.Rows[0]["UserId"].ToString());
                    iGrpId = ObjCls.FnIsNumeric(dtLog.Rows[0]["UserGrpId"].ToString());
                    if (iUId > 0 && iGrpId > 0)
                    {
                        Session["UNAME"] = TxtUserName.Text.Trim();
                        Session["UID"] = iUId.ToString();
                        Session["UGRPID"] = iGrpId.ToString();
                        Session["APPPATH"] = strAppPage;
                        Response.Redirect("DashBoard/DashBoardNew.aspx?UID=" + ObjCls.EncryptQueryString(iUId.ToString()));
                    }
                    else if (iUId < 0 && iGrpId < 0)
                    {
                        FnPopUpAlert(ObjCls.FnAlertMessage("This User is already logged. Try another user.. Otherwise contact administartor"));
                        FnFocus(TxtUserName);
                    }
                    else
                    {
                        FnPopUpAlert(ObjCls.FnAlertMessage("Invalid User Name & Password. Try again.."));
                        FnFocus(TxtUserName);
                    }
                }
            }
            catch (Exception ex)
            {
                FnPopUpAlert((ex.Message));
                FnFocus(TxtUserName);
            }
        }
    }
}