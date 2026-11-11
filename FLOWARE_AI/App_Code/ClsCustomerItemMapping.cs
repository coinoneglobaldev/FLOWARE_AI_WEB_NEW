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
    public class ClsCustomerItemMapping : ClsCommonBaseMaster
    {
        public int nCustomerId = 0, nUomId = 0, nSplitUomId = 0, nCompanyId = 0, nBranchId = 0, nFaId = 0, nDefinedUserId = 0, nMailId = 0;
        public string cItmCode = "", cRemarks = "", cItemDesc = "", cItmUom = "", cFromMail = "",cAICustomerName="";
        public ClsCustomerItemMapping() : base("", "ID", 0, 0, 0)
        {

        }
        public ClsCustomerItemMapping(int PrmCompanyId, int PrmBranchId, int PrmFaId)
          : base("", "ID", PrmCompanyId, PrmBranchId, PrmFaId)
        {
        }
        public int CustomerId
        {
            get
            { return nCustomerId; }
            set
            { nCustomerId = value; }
        }
        public string ItmCode
        {
            get
            { return cItmCode; }
            set
            { cItmCode = value; }
        }
        public string ItemDesc
        {
            get
            { return cItemDesc; }
            set
            { cItemDesc = value; }
        }
        public string ItmUom
        {
            get
            { return cItmUom; }
            set
            { cItmUom = value; }
        }
        public int MailId
        {
            get
            { return nMailId; }
            set
            { nMailId = value; }
        }
        public string FromMail
        {
            get
            { return cFromMail; }
            set
            { cFromMail = value; }
        }
        public int SplitUomId
        {
            get
            { return nSplitUomId; }
            set
            { nSplitUomId = value; }
        }
        public string AICustomerName
        {
            get
            { return cAICustomerName; }
            set
            { cAICustomerName = value; }
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

            objFields.AddParameterFields("Prm_nCustomerId", DbType.Int32, CustomerId);
            objFields.AddParameterFields("Prm_cItmCode", DbType.String, ItmCode);
            objFields.AddParameterFields("Prm_cItemDesc", DbType.String, ItemDesc);
            objFields.AddParameterFields("Prm_cItmUom", DbType.String, ItmUom);
            objFields.AddParameterFields("Prm_nItemId", DbType.Int32, ItemId);
            objFields.AddParameterFields("Prm_nUomId", DbType.Int32, UomId);
            objFields.AddParameterFields("Prm_nMailId", DbType.Int32, MailId);
            objFields.AddParameterFields("Prm_cFromMail", DbType.String, FromMail);
            objFields.AddParameterFields("Prm_nSplitUomId", DbType.Int32, SplitUomId);
            objFields.AddParameterFields("Prm_nTokenNo", DbType.Double, TokenNo);
            objFields.AddParameterFields("Prm_cAICustomerName", DbType.String, AICustomerName);

            if (PrmDbFlag == "MAP" || PrmDbFlag == "UCUS" || PrmDbFlag == "UEXP" || PrmDbFlag == "UCHK")
            {
                return objDBManager.GetRecord("ProAICustomerItemMapping", objFields) as DataSet;
            }
            else
            {
                ArrayList arr = new ArrayList(objDBManager.UpdateRecord("ProAICustomerItemMapping", objFields));
                return arr[0].ToString();
            }
        }
        public void FnUpdateAIOrderDetails()
        {
            ManipulateData("MAP");
        }
        public void FnUpdateCustomerDetails()
        {
            ManipulateData("UCUS");
        }
        public void FnUpdateExportStatus()
        {
            ManipulateData("UEXP");
        }
        public void FnUpdateCheckStatus()
        {
            ManipulateData("UCHK");
        }
        #endregion
    }
}