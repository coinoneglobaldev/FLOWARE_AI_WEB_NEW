using LOTUS_DATA;
using LOTUS_GENERAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using FLOWARE_AI.App_Code;

namespace FLOWARE_AI.App_Code
{
    public class ClsAICustomerList : ClsCommonBaseMaster
    {
        public int nIsPdf, nIsLink, nIsText;
        public string cCode="", cName="";
        public ClsAICustomerList() : base("", "nId", 0, 0, 0)
        {
        }

        public ClsAICustomerList(int PrmCompanyId, int PrmBranchId, int PrmFaId)
            : base("", "nId", PrmCompanyId, PrmBranchId, PrmFaId)
        {
        }

        #region IMasterCommands Members

        public override DataTable CreatePropertyTable(string PrmTableName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void GetDataRow(string PrmDataId, DataTable PrmDtRecord)
        {
            base.GetDataRow(PrmDataId, PrmDtRecord);
        }

        public override object ManipulateData(string PrmDbFlag)
        {
            base.ManipulateData(PrmDbFlag);
            ClsDBManager objDBManager = new ClsDBManager();
            objDBManager.DataSetting = DBSource.GENERAL;
            
            objFields.AddParameterFields("Prm_cCode", DbType.String, cCode);
            objFields.AddParameterFields("Prm_cName", DbType.String, cName);
            objFields.AddParameterFields("Prm_nIsDelete", DbType.Boolean, false);
            objFields.AddParameterFields("Prm_nIsPdf", DbType.Int32, nIsPdf);
            objFields.AddParameterFields("Prm_nIsLink", DbType.Int32, nIsLink);
            objFields.AddParameterFields("Prm_nIsText", DbType.Int32, nIsText);

            if (PrmDbFlag.Equals("S") || PrmDbFlag.Equals("ML") || PrmDbFlag.Equals("MC") || PrmDbFlag.Equals("MP"))
            {
                return objDBManager.GetRecord("ProAICustomerData", objFields) as DataSet;
            }
            else
            {
                ArrayList arr = new ArrayList(objDBManager.UpdateRecord("ProAICustomerData", objFields));
                return arr[0].ToString();
            }
        }
        public DataTable FnGetCustomerList()
        {
            return (ManipulateData("MC") as DataSet).Tables[0];
        }
        public DataTable FnGetProductList()
        {
            return (ManipulateData("MP") as DataSet).Tables[0];
        }
        public DataTable GetMailBodyHtmlFromDB()
        {
            return (ManipulateData("MB") as DataSet).Tables[0];
        }
        public String GetCustomer()
        {
            DataSet ds = ManipulateData("CU") as DataSet;

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["CustomerName"].ToString();
            }

            return "";

        }
        public DataTable GetCustomerCode()
        {
            return (ManipulateData("CC") as DataSet).Tables[0];
        }
        public string FnSynchronizeCustomers()
        {
            string str = ManipulateData("SR") as string;
            return str;
        }
        #endregion

    }
}