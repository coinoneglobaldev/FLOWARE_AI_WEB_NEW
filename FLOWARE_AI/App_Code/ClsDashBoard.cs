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
    public class ClsDashBoard: ClsCommonBaseMaster
    {
        public string _strSql = "";
        public ClsDashBoard() : base("", "ID", 0, 0, 0)
        {

        }
        public ClsDashBoard(int PrmCompanyId, int PrmBranchId, int PrmFaId)
          : base("", "ID", PrmCompanyId, PrmBranchId, PrmFaId)
        {

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

            if (PrmDbFlag == "S")
            {
                return objDBManager.GetRecord("", objFields) as DataSet;
            }
            else
            {
                ArrayList arr = new ArrayList(objDBManager.UpdateRecord("", objFields));
                return arr[0].ToString();
            }
        }
        public DataTable FnGetDashBoardData()
        {
            _strSql = "SELECT COUNT(nId) AS Count FROM TblAIMailLISTDetails WHERE nActive=1 AND nIsDelete=0 AND nIsConvert=1 UNION ALL " +
                "SELECT COUNT(nId) AS Count FROM TblAIMailDetails WHERE nActive=1 AND nIsDelete=0 AND nIsChk=1 AND nIsExport=0 UNION ALL " +
                "SELECT COUNT(nId) AS Count FROM TblAIMailDetails WHERE nActive=1 AND nIsDelete=0 AND nIsChk=0 AND nIsExport=0 UNION ALL " +
                "SELECT COUNT(nId) AS Count FROM TblAIMailDetails WHERE nActive=1 AND nIsDelete=0 AND nIsChk=1 AND nIsExport=1 UNION ALL " +
                "SELECT COUNT(nId) AS Count FROM TblItem WHERE nActive=1 AND nIsDelete=0 UNION ALL" +
                " SELECT COUNT(nId) AS Count FROM TblCustomer WHERE nActive=1 AND nIsDelete=0";
            
            return FnGetDataSet(_strSql).Tables[0];
        }
        #endregion
    }
}