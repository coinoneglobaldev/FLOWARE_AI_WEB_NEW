using LOTUS_DATA;
using LOTUS_GENERAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace FLOWARE_AI.App_Code
{
    public class ClsAICustList : ClsCommonBaseMaster
    {
        public int nIsPdf, nIsLink, nIsText;
        public string cCode = "", cName = "";
        public ClsAICustList() : base("", "nId", 0, 0, 0)
        {
        }

        public ClsAICustList(int PrmCompanyId, int PrmBranchId, int PrmFaId)
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

            objFields.AddParameterFields("Prm_cCode", DbType.String, Code);
            objFields.AddParameterFields("Prm_cName", DbType.String, Name);

            if (PrmDbFlag.Equals("S") || PrmDbFlag.Equals("AS"))
            {
                return objDBManager.GetRecord("ProAICustList", objFields) as DataSet;
            }
            else
            {
                ArrayList arr = new ArrayList(objDBManager.UpdateRecord("ProAICustList", objFields));
                return arr[0].ToString();
            }
        }

        public DataTable FnGetCustList()
        {
            return (ManipulateData("AS") as DataSet).Tables[0];
        }
        public void FnInsertRecord()
        {
            ManipulateData("N");
        }
        #endregion

    }
}