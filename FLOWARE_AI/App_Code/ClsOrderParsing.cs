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
    public class ClsOrderParsing : ClsCommonBaseMaster
    {
        public int nCustomerId=0, nItemId = 0, nUomId = 0, nMailId=0;
        public string cOrderNo = "", cAICustomerName = "", cAICustomerCode = "", cAICustomerAddress = "", cAIItemCode = "", cAIItemName = "", 
            cUomName = "", cAIQty = "", cAIPrice = "";
        public string cRemarks = "", cPOType = "", cItemRemarks = "", cFromMail="";
        public decimal nQty, nPrice, nTotalAmt;
        public double nMRP=0,nFreeQty=0, nDiscPerc=0, nDiscAmt=0;
        public DateTime dOrderDate=Convert.ToDateTime("1800-01-01"), dDeliveryDate= Convert.ToDateTime("1800-01-01");

        public ClsOrderParsing() : base("", "ID", 0, 0, 0)
        {

        }
        public ClsOrderParsing(int PrmCompanyId, int PrmBranchId, int PrmFaId)
          : base("", "ID", PrmCompanyId, PrmBranchId, PrmFaId)
        {
        }           

        public string OrderNo
        {
            get
            { return cOrderNo; }
            set
            { cOrderNo = value; }
        }
        public DateTime DeliveryDate
        {
            get
            { return dDeliveryDate; }
            set
            { dDeliveryDate = value; }
        }
        public string AICustomerName
        {
            get
            { return cAICustomerName; }
            set
            { cAICustomerName = value; }
        }
        public string AICustomerCode
        {
            get
            { return cAICustomerCode; }
            set
            { cAICustomerCode = value; }
        }
        public string AICustomerAddress
        {
            get
            { return cAICustomerAddress; }
            set
            { cAICustomerAddress = value; }
        }
        public int CustomerId
        {
            get
            { return nCustomerId; }
            set
            { nCustomerId = value; }
        }
        public string AIItemCode
        {
            get
            { return cAIItemCode; }
            set
            { cAIItemCode = value; }
        }
        public string AIItemName
        {
            get
            { return cAIItemName; }
            set
            { cAIItemName = value; }
        }
        public string AIQty
        {
            get
            { return cAIQty; }
            set
            { cAIQty = value; }
        }
        public string AIPrice
        {
            get
            { return cAIPrice; }
            set
            { cAIPrice = value; }
        }
        public string ItemRemarks
        {
            get
            { return cItemRemarks; }
            set
            { cItemRemarks = value; }
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
        public double MRP
        {
            get
            { return nMRP; }
            set
            { nMRP = value; }
        }
        public double FreeQty
        {
            get
            { return nFreeQty; }
            set
            { nFreeQty = value; }
        }
        public double DiscPerc
        {
            get
            { return nDiscPerc; }
            set
            { nDiscPerc = value; }
        }
        public double DiscAmt
        {
            get
            { return nDiscAmt; }
            set
            { nDiscAmt = value; }
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
            objFields.AddParameterFields("Prm_cOrderNo", DbType.String, OrderNo);
            objFields.AddParameterFields("Prm_dOrderDate", DbType.DateTime, OrderDate);
            objFields.AddParameterFields("Prm_dDeliveryDate", DbType.DateTime, DeliveryDate);
            objFields.AddParameterFields("Prm_cAICustomerName", DbType.String, AICustomerName);
            objFields.AddParameterFields("Prm_cAICustomerCode", DbType.String, AICustomerCode);
            objFields.AddParameterFields("Prm_cAICustomerAddress", DbType.String, AICustomerAddress);
            objFields.AddParameterFields("Prm_nCustomerId", DbType.Int32, CustomerId);
            objFields.AddParameterFields("Prm_cAIItemCode", DbType.String, AIItemCode);
            objFields.AddParameterFields("Prm_cAIItemName", DbType.String, AIItemName);
            objFields.AddParameterFields("Prm_nItemId", DbType.Int32, ItemId);
            objFields.AddParameterFields("Prm_cUomName", DbType.String, UomName);
            objFields.AddParameterFields("Prm_nUomId", DbType.Int32, UomId);
            objFields.AddParameterFields("Prm_cAIQty", DbType.String, AIQty);
            objFields.AddParameterFields("Prm_nQty", DbType.Decimal, Qty);
            objFields.AddParameterFields("Prm_cAIPrice", DbType.String, AIPrice);
            objFields.AddParameterFields("Prm_nPrice", DbType.Decimal, Price);
            objFields.AddParameterFields("Prm_cItemRemarks", DbType.String, ItemRemarks);
            objFields.AddParameterFields("Prm_nMailId", DbType.Int32, MailId);
            objFields.AddParameterFields("Prm_cFromMail", DbType.String, FromMail);
            objFields.AddParameterFields("Prm_nTokenNo", DbType.Double, TokenNo);

            objFields.AddParameterFields("Prm_nMRP", DbType.Double, MRP);
            objFields.AddParameterFields("Prm_nFreeQty", DbType.Double, FreeQty);
            objFields.AddParameterFields("Prm_nDiscPerc", DbType.Double, DiscPerc);
            objFields.AddParameterFields("Prm_nDiscAmt", DbType.Double, DiscAmt);

            if (PrmDbFlag == ("S") || PrmDbFlag.Equals("DLIM")|| PrmDbFlag.Equals("NADD") || PrmDbFlag.Equals("SADD") || PrmDbFlag.Equals("DLEX")
                || PrmDbFlag.Equals("REEX") || PrmDbFlag.Equals("DLDR"))
            {
                return objDBManager.GetRecord("ProAIPODetails", objFields) as DataSet;
            }
            else
            {
                ArrayList arr = new ArrayList(objDBManager.UpdateRecord("ProAIPODetails", objFields));
                return arr[0].ToString();
            }
        }

        public void InsertAIPODetails()
        {
            ManipulateData("I");
        }
        public void FnExportData()
        {
            ManipulateData("IE");
        }
        public DataTable GetAIPODetails()
        {
            return (ManipulateData("S") as DataSet).Tables[0];
        }
        public DataTable FnDeleteMailDataItems()
        {
            return (ManipulateData("DLIM") as DataSet).Tables[0];
        }
        public void FnAddExtractItemAsNewOrder()
        {
            ManipulateData("NADD");
        }
        public void FnDeleteitemsFromPOBeforeReExtract()
        {
            ManipulateData("DLEX");
        }
        public DataTable FnGetExtractItemAsNewOrder()
        {
            return (ManipulateData("SADD") as DataSet).Tables[0];
        }
        public void FnReExtractInsertToPODetails()
        {
            ManipulateData("REEX");
        }
        public void FnDelItemsFromDirExpPopUp()
        {
            ManipulateData("DLDR");
        }
        #endregion
    }
}