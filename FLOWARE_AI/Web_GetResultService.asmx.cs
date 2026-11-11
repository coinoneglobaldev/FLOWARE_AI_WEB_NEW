using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Services;
using System.Text;
using System.Data;
using System.Configuration;

using FLOWARE_AI.App_Code;


namespace FLOWARE_AI
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]

    public class Web_GetResultService : System.Web.Services.WebService
    {
      FLOWARE_AI.ClsAIMailDetails ObjCls = new ClsAIMailDetails();
        string[] strList = null;
        string strSql = "";
        public Web_GetResultService()
        {
        }
        [WebMethod]
        public string[] FnGetCustomerName(string PrmCustCode)
        {
            strList = new string[7];
            ObjCls.cCustomerId = PrmCustCode;
            string PrmCustName = ObjCls.GetCustomer();
            strList.SetValue(PrmCustName.ToString(), 0);
            return strList;
        }
       
        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string[] FnGetCustomerName(string PrmCustCode)
        //{
        //    ClsAIMailDetails ObjCls = new ClsAIMailDetails();
        //    string[] strList = new string[1];
        //    ObjCls.cCustomerId = PrmCustCode;
        //    strList[0] = ObjCls.GetCustomer();
        //    return strList;
        //}


    }
}
