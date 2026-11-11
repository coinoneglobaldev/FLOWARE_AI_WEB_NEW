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
    public class ClsAIMailDetails : ClsCommonBaseMaster
    {
        public int nFCusId=0, nFItemId=0, nFUomId=0,nCustomerId = 0,nMailId=0,nIsEdit=0;
        public string cFromMail, cSubjectMail, cMailBody, cAttachment1, cAttachment2, cAttachment3, cMailUId, _strSql = "";
        public string cLink1, cLink2, cLink3, cRemarks, cTType,cAICustomerName = "",
            cUniqueMailId="",cRetMsg="";
        public DateTime dMailDate=Convert.ToDateTime("1800-01-01"), dReceivedDate= Convert.ToDateTime("1800-01-01");

        public ClsAIMailDetails() : base("", "nId", 0, 0, 0)
        {
        }
        public ClsAIMailDetails(int PrmCompanyId, int PrmBranchId, int PrmFaId)
            : base("", "nId", PrmCompanyId, PrmBranchId, PrmFaId)
        {
        }
        public int CustomerId
        {
            get
            { return nCustomerId; }
            set
            { nCustomerId = value; }
        }
        public int MailId
        {
            get
            { return nMailId; }
            set
            { nMailId = value; }
        }
        public int IsEdit
        {
            get
            { return nIsEdit; }
            set
            { nIsEdit = value; }
        }
        public string FromMail
        {
            get
            { return cFromMail; }
            set
            { cFromMail = value; }
        }
        public string SubjectMail
        {
            get
            { return cSubjectMail; }
            set
            { cSubjectMail = value; }
        }
        public string UniqueMailId
        {
            get
            { return cUniqueMailId; }
            set
            { cUniqueMailId = value; }
        }
        public DateTime MailDate
        {
            get
            { return dMailDate; }
            set
            { dMailDate = value; }
        }
        public DateTime ReceivedDate
        {
            get
            { return dReceivedDate; }
            set
            { dReceivedDate = value; }
        }
        public string MailBody
        {
            get
            { return cMailBody; }
            set
            { cMailBody = value; }
        }
        public int FItemId
        {
            get
            { return nFItemId; }
            set
            { nFItemId = value; }
        }
        public int FUomId
        {
            get
            { return nFUomId; }
            set
            { nFUomId = value; }
        }
        public string AICustomerName
        {
            get
            { return cAICustomerName; }
            set
            { cAICustomerName = value; }
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
            objFields.AddParameterFields("Prm_cFromMail", DbType.String, FromMail);
            objFields.AddParameterFields("Prm_cSubjectMail", DbType.String, SubjectMail);
            objFields.AddParameterFields("Prm_dMailDate", DbType.DateTime,MailDate);
            objFields.AddParameterFields("Prm_dReceivedDate", DbType.DateTime,ReceivedDate);
            objFields.AddParameterFields("Prm_cMailBody", DbType.String, MailBody);            
            objFields.AddParameterFields("Prm_nMailId", DbType.Int32, MailId);
            objFields.AddParameterFields("Prm_nCustomerId", DbType.Int32, CustomerId);

            objFields.AddParameterFields("Prm_nFItemId", DbType.Int32, FItemId);
            objFields.AddParameterFields("Prm_nFUomId", DbType.Int32, FUomId);

            objFields.AddParameterFields("Prm_dFromDate", DbType.DateTime, FromDate);
            objFields.AddParameterFields("Prm_dToDate", DbType.DateTime, ToDate);
            objFields.AddParameterFields("Prm_nQty", DbType.Double, Qty);
            objFields.AddParameterFields("Prm_nTokenNo", DbType.Double, TokenNo);
            objFields.AddParameterFields("Prm_cAICustomerName", DbType.String, AICustomerName);
            objFields.AddParameterFields("Prm_nPrice", DbType.Double, Rate);
            objFields.AddParameterFields("Prm_cUniqueMailId", DbType.String, UniqueMailId);
            objFields.AddParameterFields("Prm_nIsEdit", DbType.Int32, IsEdit);

            if (PrmDbFlag.Equals("S") || PrmDbFlag.Equals("ML") || PrmDbFlag.Equals("MD") || PrmDbFlag.Equals("PT") || PrmDbFlag.Equals("MC") ||
                PrmDbFlag.Equals("MB") || PrmDbFlag.Equals("CU") || PrmDbFlag.Equals("CC") || PrmDbFlag.Equals("IN") || PrmDbFlag.Equals("IC") ||
                PrmDbFlag.Equals("UO") || PrmDbFlag.Equals("DL") || PrmDbFlag.Equals("INB") || PrmDbFlag.Equals("DEXP") || PrmDbFlag.Equals("D")
                || PrmDbFlag.Equals("ASMP")|| PrmDbFlag.Equals("EXP") || PrmDbFlag.Equals("ADD") || 
                PrmDbFlag.Equals("VL") || PrmDbFlag.Equals("BLKL") || PrmDbFlag.Equals("PO_CH"))
            {
                return objDBManager.GetRecord("ProAIMailTableDetails", objFields) as DataSet;
            }
            else
            {
                ArrayList arr = new ArrayList(objDBManager.UpdateRecord("ProAIMailTableDetails", objFields));
                return arr[0].ToString();
            }
        }
        //================================================
        public void InsertAIMailDetails()
        {
            ManipulateData("I");
        }
        public DataTable FnGetMailData()
        {
            return (ManipulateData("ML") as DataSet).Tables[0];
        }
        public DataTable FnGetMailDataInbox()
        {
            return (ManipulateData("INB") as DataSet).Tables[0];
        }
        public DataTable FnGetMailDataExport()
        {
            return (ManipulateData("EXP") as DataSet).Tables[0];
        }
        public DataTable FnGetDirectDataExport()
        {
            return (ManipulateData("DEXP") as DataSet).Tables[0];
        }
        public DataTable FnGetOrderData()
        {
            return (ManipulateData("MD") as DataSet).Tables[0];
        }      
        
        public DataTable FnGetOrderAssignMapItems()
        {
            return (ManipulateData("ASMP") as DataSet).Tables[0];
        }
        public DataTable GetAIMailDetails()
        {
            return (ManipulateData("S") as DataSet).Tables[0];
        }
        public DataTable FnGetPromptData()
        {
            return (ManipulateData("PT") as DataSet).Tables[0];
        }
        public DataTable FnGetMailCred()
        {
            return (ManipulateData("MC") as DataSet).Tables[0];
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
        public DataTable FnGetItemNameData()
        {
            return (ManipulateData("IN") as DataSet).Tables[0];
        }
        public DataTable FnGetItemUOMData()
        {
            return (ManipulateData("UO") as DataSet).Tables[0];
        }
        public DataTable FnGetItemCodeData()
        {
            return (ManipulateData("IC") as DataSet).Tables[0];
        }
        public void FnExportData()
        {
            ManipulateData("IE");
        }       
        public void FnDeleteMailData()
        {
            ManipulateData("DL");
        }
        public void FnDeleteDateWise()
        {
            ManipulateData("D");
        }
        public List<string> FnGetValidLinkDomains()
        {
            DataTable dt = (ManipulateData("VL") as DataSet).Tables[0]; ;
            List<string> domains = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                domains.Add(row["cValidLinkUrl"].ToString());
            }
            return domains;
        }
        public void FnAddNewItemToOrder()
        {
            ManipulateData("ADD");
        }
        public string FnInsertToBlockedEmailList()
        {           
           cRetMsg= ManipulateData("BLK") as string;
            return cRetMsg;
        }
        public DataTable FnGetBlockedMailList()
        {
            return (ManipulateData("BLKL") as DataSet).Tables[0];
        }
        public string FnDeleteFromBlockedEmailList()
        {
            cRetMsg=ManipulateData("UBLK") as string;
            return cRetMsg;
        }
        //==========================================
        public DataTable FnGetMailMasterData()
        {
            //_strSql = "SELECT (ROW_NUMBER() OVER (ORDER BY nId DESC)) AS SlNo, nId AS Id,nTokenId AS TokenId,dMailDate AS MailDate,cFromMail AS FromMail,cSubjectMail AS SubjectMail,nIsChk AS IsChk  FROM TblAIMailDetails WHERE ISNULL(nIsDelete,0) = 0 AND nIsExport=0 ";
            _strSql = "SELECT (ROW_NUMBER() OVER (ORDER BY ML.nId DESC)) AS SlNo, ML.nId AS Id,ML.nTokenId AS TokenId,ML.dMailDate AS MailDate," +
                "ML.cFromMail AS FromMail,ML.cSubjectMail AS SubjectMail,ML.nIsChk AS IsChk,ML.cUniqueMailId AS UniqueInboxId," +
                "EMA.nId AS MailAccId FROM TblAIMailDetails ML INNER JOIN TblAIMailListDetails MLST ON ML.cUniqueMailId = MLST.cUniqueMailId " +
                "INNER JOIN TblAIEmailAccounts EMA ON MLST.nEmailCredId = EMA.nId WHERE ISNULL(ML.nIsDelete,0) = 0 AND nIsExport = 0 ";

            return FnGetDataSet(_strSql).Tables[0];
        }
        public DataTable FnGetMailUserNamePassword(int PrmMailAccId)
        {
            _strSql = "SELECT cEmailId EmailId,cEmailAppPassword EmailAppPassword FROM TblAIEmailAccounts WHERE nId=" + PrmMailAccId;
            return FnGetDataSet(_strSql).Tables[0];
        }
        
        #endregion
    }
}