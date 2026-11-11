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


[ScriptService]
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Web_GetResultService : System.Web.Services.WebService
{
    double dVal = 0,dVal1=0;
    string _strSql = "", strVal = "", _strField = "";
    string[] strList = null;
    DataTable _dtVal = null;
    double SplitPrice = 0;
    ClsWebServiceRecordList ObjCls = new ClsWebServiceRecordList();
    ClsAIMailDetails ObjAI = new ClsAIMailDetails();

    public Web_GetResultService()
    {
    }
    [WebMethod]
    public string[] FnGetCustomerName(string PrmCustCode)
    {
        //strList = new string[7];
        strList = new string[1];
        //ObjCls.CustomerId = PrmCustCode;
        string PrmCustName = ObjAI.GetCustomer();
        strList.SetValue(PrmCustName.ToString(), 0);
        return strList;
    }
    [WebMethod]
    public string FnUpdateOrderDetails(string PrmToken, string PrmId, string PrmItemId, string PrmUomId, string PrmQty, string PrmRate,string PrmRemarks)
    {
        dVal = ObjCls.FnIsDouble(PrmQty) * ObjCls.FnIsDouble(PrmRate);
        _strSql = "UPDATE TblAIPODetails_Child SET nItemId=" + ObjCls.FnIsNumeric(PrmItemId) + " ,nUomId=" + ObjCls.FnIsNumeric(PrmUomId) + ",nQty=" + ObjCls.FnIsDouble(PrmQty) + "," +
            "cAIQty='" + ObjCls.FnIsDouble(PrmQty) + "',nPrice=" + ObjCls.FnIsDouble(PrmRate) + ",cItemRemarks='" + PrmRemarks + "',nTotalAmt=" + dVal;
        _strSql = _strSql + " WHERE nId=" + ObjCls.FnIsNumeric(PrmId) + " AND nTokenId=" + ObjCls.FnIsDouble(PrmToken);
        ObjCls.FnExecuteNonQuery(_strSql);

        _strSql = "UPDATE TblAIPODetails SET nItemId=" + ObjCls.FnIsNumeric(PrmItemId) + " ,nUomId=" + ObjCls.FnIsNumeric(PrmUomId) + ",nQty=" + ObjCls.FnIsDouble(PrmQty) + "," +
           "cAIQty='" + ObjCls.FnIsDouble(PrmQty) + "',nPrice=" + ObjCls.FnIsDouble(PrmRate) + ",cItemRemarks='" + PrmRemarks + "',nTotalAmt=" + dVal;
        _strSql = _strSql + " WHERE nId=" + ObjCls.FnIsNumeric(PrmId) + " AND nTokenId=" + ObjCls.FnIsDouble(PrmToken);
        ObjCls.FnExecuteNonQuery(_strSql);
        return dVal.ToString();
    }
}
