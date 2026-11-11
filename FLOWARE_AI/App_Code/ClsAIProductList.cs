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
    public class ClsAIProductList : ClsCommonBaseMaster
    {
        public string cCode = "", cName = "";

        public ClsAIProductList() : base("", "nId", 0, 0, 0)
        {
        }
        public ClsAIProductList(int PrmCompanyId, int PrmBranchId, int PrmFaId)
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

            if (PrmDbFlag.Equals("S")  || PrmDbFlag.Equals("MP"))
            {
                return objDBManager.GetRecord("ProAIProductData", objFields) as DataSet;
            }
            else
            {
                ArrayList arr = new ArrayList(objDBManager.UpdateRecord("ProAIProductData", objFields));
                return arr[0].ToString();
            }
        }
     
        public DataTable FnGetProductList()
        {
            return (ManipulateData("MP") as DataSet).Tables[0];
        }
        public string FnSynchronizeProducts()
        {
           string str= ManipulateData("SR") as string;
            return str;
        }


        #endregion



    }
}