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
    public class ClsPrompt: ClsCommonBaseMaster
    {
        public string cPromptData = "", cPromptType = "";

        public ClsPrompt() : base("", "ID", 0, 0, 0)
        {

        }

        public ClsPrompt(int PrmCompanyId, int PrmBranchId, int PrmFaId)
          : base("", "ID", PrmCompanyId, PrmBranchId, PrmFaId)
        {
        }       
        public string PromptData
        {
            get
            { return cPromptData; }
            set
            { cPromptData = value; }
        }
        public string PromptType
        {
            get
            { return cPromptType; }
            set
            { cPromptType = value; }
        }
        
        #region IMasterCommands Members
        public override System.Data.DataTable CreatePropertyTable(string PrmTableName)
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

            objFields.AddParameterFields("Prm_cPromptData", DbType.String, PromptData);
            objFields.AddParameterFields("Prm_cPromptType", DbType.String, PromptType);            

            if ((PrmDbFlag == "PRMT")|| (PrmDbFlag == "SAVE"))
            {
                return objDBManager.GetRecord("ProAIPromptData", objFields) as DataSet;
            }
            else
            {
                ArrayList arr = new ArrayList(objDBManager.UpdateRecord("ProAIPromptData", objFields));
                return arr[0].ToString();
            }
        }
        public DataTable FnGetPromptDetails()
        {
            return (ManipulateData("PRMT") as DataSet).Tables[0];
        }
        public void FnSavePromptData()
        {
            ManipulateData("SAVE");
        }
        #endregion
    }
}