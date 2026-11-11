using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
//using LOTUS_GENERAL;
using System.Collections.Generic;
using System.Drawing;
//using AjaxControlToolkit;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AjaxControlToolkit;
using LOTUS_GENERAL;
namespace FLOWARE_AI.App_Code
{

    public partial class ClsPageEvents : System.Web.UI.Page
    {
        public int _nRptHeight = 0, _nRptWidth = 0, nSlNo = 0;
        public string _strFileFormat = "", _strUrl = "", _strLnk = "", _strTitle = "", _strHdr = "", _strDestPath = "", _strImgeByte = "", _strError = "", _strmasterScript;
        public DataSet DS_RECORD = null;
        public DataTable DT_RECORD = null, DT_TEMP = null;
        public DataView _dwMainRecord = null, _dwChildRecod = null;

        ClsUserRights objUserRights;

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClsUserPrevilage ObjUsrPrevilage = new ClsUserPrevilage();
                objUserRights = new ClsUserRights();
                //string str = ObjUsrPrevilage.DecryptQueryString(Request.QueryString["MID"].ToString().Replace(" ", "+"));

                ViewState["ID"] = "0";
                ViewState["CHLD_ID"] = "0";
                ViewState["DT"] = "";
                ViewState["DT_CHILD"] = "";
                ViewState["DT_UPDATE"] = DateTime.Now.ToString();
                objUserRights.USERID = 1; //ObjUsrPrevilage.FnIsNumeric(ObjUsrPrevilage.DecryptQueryString(Request.QueryString["UID"].ToString().Replace(" ", "+")));
                objUserRights.COMPANYID = 1;// ObjUsrPrevilage.FnIsNumeric(ObjUsrPrevilage.DecryptQueryString(Request.QueryString["CID"].ToString()));
                objUserRights.BRANCHID = 1;// ObjUsrPrevilage.FnIsNumeric(ObjUsrPrevilage.DecryptQueryString(Request.QueryString["BID"].ToString()));
                objUserRights.FAYEARID = 1;// ObjUsrPrevilage.FnIsNumeric(ObjUsrPrevilage.DecryptQueryString(Request.QueryString["FID"].ToString()));
                objUserRights.MENUID = 1;// ObjUsrPrevilage.FnIsNumeric(ObjUsrPrevilage.DecryptQueryString(Request.QueryString["MID"].ToString().Replace(" ", "+")));
                objUserRights.USERGROUPID = 1;// ObjUsrPrevilage.FnIsNumeric(ObjUsrPrevilage.DecryptQueryString(Request.QueryString["UGRPID"].ToString().Replace(" ", "+")));

                objUserRights.TTYPE = "";
                ObjUsrPrevilage.CompanyId = objUserRights.COMPANYID;
                ObjUsrPrevilage.BranchId = objUserRights.BRANCHID;
                ObjUsrPrevilage.FaId = objUserRights.FAYEARID;
                ObjUsrPrevilage.UserId = objUserRights.USERID;
                ObjUsrPrevilage.MenuId = objUserRights.MENUID;

                ObjUsrPrevilage.FnPrevilage();
                objUserRights.INSERT = ObjUsrPrevilage.ISINSERTROLE;
                objUserRights.UPDATE = ObjUsrPrevilage.ISUPDATEROLE;
                objUserRights.DELETE = ObjUsrPrevilage.ISDELETEROLE;
                objUserRights.PRINT = ObjUsrPrevilage.ISPRINTROLE;
                objUserRights.PRINTPREVIEW = ObjUsrPrevilage.ISPRINTPREVIEWROLE;
                objUserRights.EMAIL = ObjUsrPrevilage.ISEMAILROLE;
                ViewState["USERRIGHTS"] = objUserRights;
            }
            if (ViewState["USERRIGHTS"] != null)
            {
                objUserRights = ViewState["USERRIGHTS"] as ClsUserRights;
            }
        }
        public void FnPopUpAlert(string PrmMsgString)
        {
            Page objPage = (HttpContext.Current.CurrentHandler) as Page;
            if (objPage != null)
            {
                ScriptManager.RegisterClientScriptBlock(objPage, typeof(ClsPageEvents), "Msg", PrmMsgString, false);
            }
        }
        public void FnAlertDelete(LinkButton PrmLnkBtn)
        {
            PrmLnkBtn.Attributes.Add("OnClick", "return confirm('Do you want to delete this record');");
        }
        public void FnAlertConfirm(Button PrmLnkBtn)
        {
            PrmLnkBtn.Attributes.Add("OnClick", "return confirm('Do you want to approve this record');");
        }
        public string FnGetQueryString(string PrmFrmPage, string PrmId)
        {
            return PrmFrmPage + "?CNTRID=" + PrmId + "&UID=" + Request.QueryString["UID"].ToString() + "&CID=" + Request.QueryString["CID"].ToString() + "&BID=" + Request.QueryString["BID"].ToString() + "&FID=" + Request.QueryString["FID"].ToString() + "&MID=" + Request.QueryString["MID"].ToString() + "&UGRPID=" + Request.QueryString["UGRPID"].ToString() + "&TTYPE=" + Request.QueryString["TTYPE"].ToString() + "&WIDTH=" + Request.QueryString["WIDTH"].ToString() + "&HEIGHT=" + Request.QueryString["HEIGHT"].ToString();
        }
        public string FnGetQueryString(string PrmFrmPage, string PrmId, string PrmTType, string PrmTitle, string PrmDid, string PrmPid, string PrmDate)
        {
            return PrmFrmPage + "?TITLE=" + PrmTitle + "&CNTRID=" + PrmId + "&UID=" + Request.QueryString["UID"].ToString() + "&CID=" + Request.QueryString["CID"].ToString() + "&BID=" + Request.QueryString["BID"].ToString() + "&FID=" + Request.QueryString["FID"].ToString() + "&MID=" + Request.QueryString["MID"].ToString() + "&UGRPID=" + Request.QueryString["UGRPID"].ToString() + "&TTYPE=" + PrmTType + "&STYLE=1&UNAME=" + Request.QueryString["UNAME"].ToString() + "&DATE=" + PrmDate + "&DID=" + PrmDid + "&PID=" + PrmPid + "&&WIDTH=" + Request.QueryString["WIDTH"].ToString() + "&HEIGHT=" + Request.QueryString["HEIGHT"].ToString();
        }
        public void FnShowOutPut(string PrmOutPut, ClsCommonBaseMaster PrmBaseMaster, bool PrmIsPrint)
        {
            string[] _strMsg = PrmOutPut.Split('/');
            if (PrmIsPrint == true)
            {
                PrmBaseMaster.ID = PrmBaseMaster.FnIsNumeric(_strMsg[0]);
                FnPrintRecord(PrmBaseMaster);
                FnCancel();
            }
            else
            {
                if (_strMsg.Length > 1)
                {
                    if (_strMsg[0].Equals("0"))
                    {
                        FnPopUpAlertBox(_strMsg[1]);
                        //FnPopUpAlert(PrmBaseMaster.FnAlertMessage(_strMsg[1]));
                    }
                    else if (_strMsg[1].Equals("Cannot Set Lead status as complete,Mandatory Workflows are pending to complete"))
                    {
                        FnPopUpAlertFailureBox(_strMsg[1]);
                    }
                    else
                    {
                        FnPopUpAlertBox(_strMsg[1]);
                        //FnPopUpAlert(PrmBaseMaster.FnAlertMessage(_strMsg[1]));
                        FnCancel();
                    }
                }
                else
                {
                    FnPopUpAlertFailureBox((_strMsg.Length > 1 ? _strMsg[1] : _strMsg[0]));
                    //FnPopUpAlert(PrmBaseMaster.FnAlertMessage((_strMsg.Length > 1 ? _strMsg[1] : _strMsg[0])));
                }
            }
        }
        public void FnShowOutPutGridVw(string PrmOutPut, ClsCommonBaseMaster PrmBaseMaster, bool PrmIsPrint)
        {
            string[] _strMsg = PrmOutPut.Split('/');
            if (PrmIsPrint == true)
            {
                PrmBaseMaster.ID = PrmBaseMaster.FnIsNumeric(_strMsg[0]);
                FnPrintRecord(PrmBaseMaster);
            }
            else
            {
                if (_strMsg.Length > 1)
                {
                    if (_strMsg[0].Equals("0"))
                    {
                        FnPopUpAlert(PrmBaseMaster.FnAlertMessage(_strMsg[1]));
                    }
                    else
                    {
                        FnPopUpAlertBox(_strMsg[1]);
                    }
                }
                else
                {
                    FnPopUpAlertFailureBox((_strMsg.Length > 1 ? _strMsg[1] : _strMsg[0]));
                }
            }
        }
        private void FnUnClearOutPut(string PrmOutPut, ClsCommonBaseMaster PrmBaseMaster, bool PrmIsAdd)
        {
            string[] _strMsg = PrmOutPut.Split('/');
            if (PrmIsAdd == true)
            {
                ViewState["ID"] = _strMsg[0].ToString();
            }
            FnPopUpAlertFailureBox((_strMsg.Length > 1 ? _strMsg[1] : _strMsg[0]));
            //FnPopUpAlert(PrmBaseMaster.FnAlertMessage((_strMsg.Length > 1 ? _strMsg[1] : _strMsg[0])));
        }
        public virtual void ManiPulateDataEvent_Clicked(string PrmFlag, ClsCommonBaseMaster PrmBaseMaster, bool PrmIsPrint)
        {
            try
            {
                switch (PrmFlag.ToUpper())
                {
                    case "NEW":
                        if (objUserRights.INSERT == true)
                        {
                            FnShowOutPut(PrmBaseMaster.SaveRecord() as string, PrmBaseMaster, PrmIsPrint);
                        }
                        else
                        {
                            FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Save New Record"));
                        }
                        break;
                    case "GENINV":
                        if (objUserRights.INSERT == true)
                        {
                            string strMsg = PrmBaseMaster.AutoGenerateRecord(PrmFlag) as string;
                        }
                        else
                        {
                            FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Generate New Record"));
                        }
                        break;
                    case "INSERT"://If record save then cold't clear the filed
                        if (objUserRights.INSERT == true)
                        {
                            FnUnClearOutPut(PrmBaseMaster.SaveRecord() as string, PrmBaseMaster, PrmIsPrint);
                        }
                        else
                        {
                            FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Save New Record"));
                        }
                        break;
                    case "UPDATE":
                        if (objUserRights.UPDATE == true)
                        {
                            FnShowOutPut(PrmBaseMaster.UpdateRecord() as string, PrmBaseMaster, PrmIsPrint);
                        }
                        else
                        {
                            FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Update Record"));
                        }
                        break;
                    case "MODIFY"://If record save then cold't clear the filed
                        if (objUserRights.INSERT == true)
                        {
                            FnUnClearOutPut(PrmBaseMaster.UpdateRecord() as string, PrmBaseMaster, PrmIsPrint);
                        }
                        else
                        {
                            FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Update Record"));
                        }
                        break;
                    case "DELETE":
                        if (objUserRights.DELETE == true)
                        {
                            FnShowOutPut(PrmBaseMaster.DeleteRecord() as string, PrmBaseMaster, false);
                            //FnPopUpAlert(PrmBaseMaster.FnAlertMessage(PrmBaseMaster.DeleteRecord() as string));
                        }
                        else
                        {
                            FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Delete Record"));
                        }
                        break;
                    case "CLEAR":
                        FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Clear Record"));
                        break;
                    case "CLOSE":
                        FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Close Record"));
                        break;
                    case "PRINT":
                        if (objUserRights.PRINT == true)
                        {
                            FnPrintRecord(PrmBaseMaster);
                        }
                        else
                        {
                            FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Print Record"));
                        }
                        break;
                    case "PRINT_EXCEL":
                        if (objUserRights.PRINT == true)
                        {
                            FnPrintRecordExcel(PrmBaseMaster);
                        }
                        else
                        {
                            FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Print Record"));
                        }
                        break;
                    case "DTPRINT":
                        string str = Request.QueryString["WIDTH"].ToString();
                        _nRptWidth = FnGetRptWidth(PrmBaseMaster);
                        _nRptHeight = FnGetRptHeight(PrmBaseMaster);
                        FnPrintDetailedRecord(PrmBaseMaster);
                        break;
                    case "FIND":
                        FnFindRecord(PrmBaseMaster);
                        break;
                    case "SEARCH":
                        FnSearchRecord(PrmBaseMaster);
                        break;
                    case "HELP":
                        FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Help Record"));
                        break;
                    case "FIRST":
                        FnAssignProperty(PrmBaseMaster);
                        PrmBaseMaster.ID = PrmBaseMaster.FnIsNumeric(PrmBaseMaster.FnGetFirstId());
                        if (PrmBaseMaster.ID > 0)
                        {
                            ViewState["ID"] = PrmBaseMaster.ID.ToString();
                            FnFindRecord(PrmBaseMaster);
                        }
                        break;
                    case "PREVIOUS":
                        FnAssignProperty(PrmBaseMaster);
                        PrmBaseMaster.ID = PrmBaseMaster.FnIsNumeric(PrmBaseMaster.FnGetPreviousId());
                        if (PrmBaseMaster.ID > 0)
                        {
                            ViewState["ID"] = PrmBaseMaster.ID.ToString();
                            FnFindRecord(PrmBaseMaster);
                        }
                        break;
                    case "NEXT":
                        FnAssignProperty(PrmBaseMaster);
                        PrmBaseMaster.ID = PrmBaseMaster.FnIsNumeric(PrmBaseMaster.FnGetNextId());
                        if (PrmBaseMaster.ID > 0)
                        {
                            ViewState["ID"] = PrmBaseMaster.ID.ToString();
                            FnFindRecord(PrmBaseMaster);
                        }
                        break;
                    case "LAST":
                        FnAssignProperty(PrmBaseMaster);
                        PrmBaseMaster.ID = PrmBaseMaster.FnIsNumeric(PrmBaseMaster.FnGetLastId());
                        if (PrmBaseMaster.ID > 0)
                        {
                            ViewState["ID"] = PrmBaseMaster.ID.ToString();
                            FnFindRecord(PrmBaseMaster);
                        }
                        break;
                    case "GET":
                        FnAssignProperty(PrmBaseMaster);
                        PrmBaseMaster.ID = PrmBaseMaster.FnIsNumeric(PrmBaseMaster.FnGetRefNoId());
                        if (PrmBaseMaster.ID > 0)
                        {
                            ViewState["ID"] = PrmBaseMaster.ID.ToString();
                            FnFindRecord(PrmBaseMaster);
                        }
                        break;
                    case "ITEM_NEW":// Child Insertion
                        ViewState["DT_CHILD"] = PrmBaseMaster.SaveChildRecord() as DataTable;
                        break;
                    case "ITEM_UPDATE":// Child Updation,
                        ViewState["DT_CHILD"] = PrmBaseMaster.UpdateChildRecord() as DataTable;
                        break;
                    case "ITEM_DELETE":// Child Deletion
                        ViewState["DT_CHILD"] = PrmBaseMaster.DeleteChildRecord() as DataTable;
                        break;
                    case "ITEM_FIND":// Token wise item pending display
                        FnFindChildRecord(PrmBaseMaster);
                        break;
                }
            }
            catch (Exception ex)
            {
                PrmBaseMaster.FnAlertMessage(ex.Message);
            }
        }
        public virtual void FnAssignProperty(ClsCommonBaseMaster PrmBaseItems)
        {
            PrmBaseItems.ID = PrmBaseItems.FnIsNumeric(ViewState["ID"].ToString());
            PrmBaseItems.CompanyId = objUserRights.COMPANYID;
            PrmBaseItems.BranchId = objUserRights.BRANCHID;
            PrmBaseItems.FaId = objUserRights.FAYEARID;
            PrmBaseItems.UserId = objUserRights.USERID;
            PrmBaseItems.TType = objUserRights.TTYPE;
            PrmBaseItems.UpdateDate = PrmBaseItems.FnDateTime(ViewState["DT_UPDATE"].ToString());
        }
        public virtual void FnAssignChildProperty(ClsCommonBaseMaster PrmBaseItems)
        {
            PrmBaseItems.ID = PrmBaseItems.FnIsNumeric(ViewState["CHLD_ID"].ToString());
            PrmBaseItems.CompanyId = objUserRights.COMPANYID;
            PrmBaseItems.BranchId = objUserRights.BRANCHID;
            PrmBaseItems.FaId = objUserRights.FAYEARID;
            PrmBaseItems.UserId = objUserRights.USERID;
            PrmBaseItems.TType = objUserRights.TTYPE;
        }
        public virtual void FnCancel()
        {
            ViewState["ID"] = "0";
            ViewState["DT_UPDATE"] = DateTime.Now.ToString();
            FnInitializeForm();
        }
        public virtual void FnClearItems(string PrmFlag)
        {
            ViewState["CHLD_ID"] = "0";
        }
        public virtual void FnFindRecord(ClsCommonBaseMaster PrmBaseMaster)
        {
            DataSet dsVal = PrmBaseMaster.FindRecord() as DataSet;
            ViewState["DT"] = dsVal.Tables[0];
            if (dsVal.Tables.Count.Equals(2))
            {
                ViewState["DT_CHILD"] = dsVal.Tables[1];
            }
        }
        public void FnSearchRecord(ClsCommonBaseMaster PrmBaseMaster)
        {
            DataSet dsVal = PrmBaseMaster.SearchRecord() as DataSet;
            ViewState["DT"] = dsVal.Tables[0];
        }
        public DataTable FnSearchRecordTable(ClsCommonBaseMaster PrmBaseMaster)
        {
            DataSet dsVal = PrmBaseMaster.SearchRecord() as DataSet;
            return dsVal.Tables[0];
        }
        public void FnEditListRecord(ClsCommonBaseMaster PrmBaseMaster)
        {
            DataSet dsVal = PrmBaseMaster.EditListRecord() as DataSet;
            ViewState["DT"] = dsVal.Tables[0];
            if (dsVal.Tables.Count.Equals(2))
            {
                ViewState["DT_CHILD"] = dsVal.Tables[1];
            }
        }
        public DataSet FnFindDataSetRecord(ClsCommonBaseMaster PrmBaseMaster)
        {
            return PrmBaseMaster.FindRecord() as DataSet;
        }
        public virtual void FnFindChildRecord(ClsCommonBaseMaster PrmBaseMaster)
        {
            ViewState["DT_CHILD"] = PrmBaseMaster.FindRecord() as DataTable;
        }
        public virtual void FnFocus(Control PrmCntrl)
        {
            ScriptManager ScrtManager = (ScriptManager)Master.FindControl("ScriptManager1");
            ScrtManager.SetFocus(PrmCntrl);
        }
        public ClsUserRights FnGetRights()
        {
            return objUserRights;
        }
        public virtual void FnInitializeForm()
        {
        }
        public void FnPopUpAlertBox(string PrmMsgString)
        {
            Page objPage = (HttpContext.Current.CurrentHandler) as Page;
            if (objPage != null)
            {
                ScriptManager.RegisterStartupScript(objPage, typeof(ClsPageEvents), "Pop", "FnNotifySuccess('" + PrmMsgString + "');", true);
            }
        }
        public void FnPopUpAlertFailureBox(string PrmMsgString)
        {
            Page objPage = (HttpContext.Current.CurrentHandler) as Page;
            if (objPage != null)
            {
                ScriptManager.RegisterStartupScript(objPage, typeof(ClsPageEvents), "Pop", "FnNotifyError('" + PrmMsgString + "');", true);
            }
        }
        public void FnPopUpAlertWarningBox(string PrmMsgString)
        {
            Page objPage = (HttpContext.Current.CurrentHandler) as Page;
            if (objPage != null)
            {
                ScriptManager.RegisterStartupScript(objPage, typeof(ClsPageEvents), "Pop", "FnNotifyWarning('" + PrmMsgString + "');", true);
            }
        }
        public void FnPopUpAlertInformationBox(string PrmMsgString)
        {
            Page objPage = (HttpContext.Current.CurrentHandler) as Page;
            if (objPage != null)
            {
                ScriptManager.RegisterStartupScript(objPage, typeof(ClsPageEvents), "Pop", "FnNotifyInformation('" + PrmMsgString + "');", true);
            }
        }
        public string FnGetTripToken(ClsCommonBaseMaster PrmBaseMaster, string PrmDid, string PrmUid)
        {
            return PrmBaseMaster.DecryptQueryString(PrmDid) + "_" + PrmUid + "_" + DateTime.Now.ToString("ddMMyyyy");
        }
        public DataTable FnGetGeneralXml(ClsCommonBaseMaster PrmBaseMaster)
        {
            return PrmBaseMaster.FnConvertStringToDataTable(PrmBaseMaster.FnReadXmlFile(Server.MapPath("~/XML_NULL//GENERAL.xml"))) as DataTable;
        }
        //========================================================== PRINT ===========================================
        public virtual void FnPrintRecord(ClsCommonBaseMaster PrmBaseMaster)
        {
            if (objUserRights.PRINT == true)
            {
                ClsReportGeneral ObjRpt = new ClsReportGeneral();
                ObjRpt._nRptWidth = _nRptWidth;
                ObjRpt._nRptHeight = _nRptHeight;
                string _strRpt = ObjRpt.FnVoucherReportWindow(PrmBaseMaster.ID.ToString(), PrmBaseMaster.RefNo.ToString(), objUserRights.COMPANYID.ToString(), objUserRights.BRANCHID.ToString(), objUserRights.FAYEARID.ToString(), objUserRights.USERID.ToString(), objUserRights.MENUID.ToString(), objUserRights.TTYPE);
                FnPopUpAlert(_strRpt);
            }
            else
            {
                FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Print Record"));
            }
        }
        public void FnPrintBarCode(ClsCommonBaseMaster PrmBaseMaster, string PrmBarCode)
        {
            ClsReportGeneral ObjRpt = new ClsReportGeneral();
            ObjRpt._nRptWidth = _nRptWidth;
            ObjRpt._nRptHeight = _nRptHeight;
            string _strRpt = ObjRpt.FnVoucherReportWindow(PrmBaseMaster.ID.ToString(), PrmBaseMaster.RefNo.ToString(), objUserRights.COMPANYID.ToString(), objUserRights.BRANCHID.ToString(), objUserRights.FAYEARID.ToString(), objUserRights.USERID.ToString(), objUserRights.MENUID.ToString(), PrmBarCode.Trim(), "BARCODE");
            FnPopUpAlert(_strRpt);
        }
        public void FnPrintOrder(ClsCommonBaseMaster PrmBaseMaster, string PrmOrderType)
        {
            ClsReportGeneral ObjRpt = new ClsReportGeneral();
            ObjRpt._nRptWidth = _nRptWidth;
            ObjRpt._nRptHeight = _nRptHeight;
            string _strRpt = ObjRpt.FnVoucherReportWindow(PrmBaseMaster.ID.ToString(), PrmBaseMaster.RefNo.ToString(), objUserRights.COMPANYID.ToString(), objUserRights.BRANCHID.ToString(), objUserRights.FAYEARID.ToString(), objUserRights.USERID.ToString(), objUserRights.MENUID.ToString(), objUserRights.TTYPE, PrmOrderType);
            FnPopUpAlert(_strRpt);
        }
        public void FnPopUpChild(ClsCommonBaseMaster PrmBaseMaster, string PrmHeader, string PrmURL, int PrmWidth, int PrmHeight, bool PrmResize)
        {
            string _strRpt = PrmBaseMaster.FnPopUpWindow(PrmHeader, PrmURL, PrmWidth, PrmHeight, PrmResize);
            FnPopUpAlert(_strRpt);
        }
        public void FnPrintDetailedRecord(ClsCommonBaseMaster PrmBaseMaster)
        {
            if (objUserRights.PRINT == true)
            {
                Session["RPT"] = PrmBaseMaster;
                ClsReportGeneral ObjRpt = new ClsReportGeneral();
                ObjRpt._nRptWidth = FnGetRptWidth(PrmBaseMaster);
                ObjRpt._nRptHeight = FnGetRptHeight(PrmBaseMaster);
                string _strRpt = ObjRpt.FnDetailedReportWindow(objUserRights.COMPANYID.ToString(), objUserRights.BRANCHID.ToString(), objUserRights.FAYEARID.ToString(), objUserRights.USERID.ToString(), objUserRights.MENUID.ToString(), objUserRights.TTYPE);
                FnPopUpAlert(_strRpt);
            }
            else
            {
                FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Print Record"));
            }
        }
        public void FnPrintDetailedRecord(ClsCommonBaseMaster PrmBaseMaster, string PrmTType)
        {
            if (objUserRights.PRINT == true)
            {
                Session["RPT"] = PrmBaseMaster;
                ClsReportGeneral ObjRpt = new ClsReportGeneral();
                ObjRpt._nRptWidth = FnGetRptWidth(PrmBaseMaster);
                ObjRpt._nRptHeight = FnGetRptHeight(PrmBaseMaster);
                string _strRpt = ObjRpt.FnDetailedReportWindow(objUserRights.COMPANYID.ToString(), objUserRights.BRANCHID.ToString(), objUserRights.FAYEARID.ToString(), objUserRights.USERID.ToString(), objUserRights.MENUID.ToString(), PrmTType, PrmTType);
                FnPopUpAlert(_strRpt);
            }
            else
            {
                FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Print Record"));
            }
        }
        public void FnPrintDetailedRecord(ClsCommonBaseMaster PrmBaseMaster, string PrmTType, string PrmRptTType)
        {
            if (objUserRights.PRINT == true)
            {
                Session["RPT"] = PrmBaseMaster;
                ClsReportGeneral ObjRpt = new ClsReportGeneral();
                ObjRpt._nRptWidth = FnGetRptWidth(PrmBaseMaster);
                ObjRpt._nRptHeight = FnGetRptHeight(PrmBaseMaster);
                string _strRpt = ObjRpt.FnDetailedReportWindow(objUserRights.COMPANYID.ToString(), objUserRights.BRANCHID.ToString(), objUserRights.FAYEARID.ToString(), objUserRights.USERID.ToString(), objUserRights.MENUID.ToString(), PrmTType, PrmRptTType);
                FnPopUpAlert(_strRpt);
            }
            else
            {
                FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Print Record"));
            }
        }
        public virtual void FnPrintRecordExcel(ClsCommonBaseMaster PrmBaseMaster)
        {
            if (objUserRights.PRINT == true)
            {
                ClsReportGeneral ObjRpt = new ClsReportGeneral();
                string _strRpt = ObjRpt.FnVoucherReportExcel(PrmBaseMaster.ID.ToString(), PrmBaseMaster.RefNo.ToString(), objUserRights.COMPANYID.ToString(), objUserRights.BRANCHID.ToString(), objUserRights.FAYEARID.ToString(), objUserRights.USERID.ToString(), objUserRights.MENUID.ToString(), objUserRights.TTYPE);
                FnPopUpAlert(_strRpt);
            }
            else
            {
                FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Print Record"));
            }
        }
        public void FnPrintDetailedRecordExcel(ClsCommonBaseMaster PrmBaseMaster)
        {
            if (objUserRights.PRINT == true)
            {
                Session["RPT"] = PrmBaseMaster;
                ClsReportGeneral ObjRpt = new ClsReportGeneral();
                string _strRpt = ObjRpt.FnDetailedReportExcel(objUserRights.COMPANYID.ToString(), objUserRights.BRANCHID.ToString(), objUserRights.FAYEARID.ToString(), objUserRights.USERID.ToString(), objUserRights.MENUID.ToString(), objUserRights.TTYPE);
                FnPopUpAlert(_strRpt);
            }
            else
            {
                FnPopUpAlert(PrmBaseMaster.FnAlertMessage(" You Have No permission To Print Record"));
            }
        }
        public string FnGetHeight(ClsCommonBaseMaster PrmBaseMaster, int PrmRedVal)
        {
            return (PrmBaseMaster.FnIsNumeric(Request.QueryString["HEIGHT"].ToString()) - PrmRedVal).ToString() + "px";
        }
        public string FnGetWidth(ClsCommonBaseMaster PrmBaseMaster, int PrmRedVal)
        {
            return (PrmBaseMaster.FnIsNumeric(Request.QueryString["WIDTH"].ToString()) - PrmRedVal).ToString() + "px";
        }
        public int FnGetHeight(ClsCommonBaseMaster PrmBaseMaster)
        {
            return (PrmBaseMaster.FnIsNumeric(Request.QueryString["HEIGHT"].ToString()) - 50);
        }
        public int FnGetWidth(ClsCommonBaseMaster PrmBaseMaster)
        {
            return (PrmBaseMaster.FnIsNumeric(Request.QueryString["WIDTH"].ToString()) - 50);
        }
        public int FnGetRptHeight(ClsCommonBaseMaster PrmBaseMaster)
        {
            int iVal = PrmBaseMaster.FnIsNumeric(Request.QueryString["HEIGHT"].ToString());
            if (PrmBaseMaster.FnIsNumeric(Request.QueryString["HEIGHT"].ToString()) > 0)
            {
                iVal = iVal - 50;
            }
            return iVal;
        }
        public int FnGetRptWidth(ClsCommonBaseMaster PrmBaseMaster)
        {
            int iVal = PrmBaseMaster.FnIsNumeric(Request.QueryString["WIDTH"].ToString());
            if (PrmBaseMaster.FnIsNumeric(Request.QueryString["WIDTH"].ToString()) > 0)
            {
                iVal = iVal - 50;
            }
            return iVal;
        }
        public string FnGetUserType(ClsCommonBaseMaster PrmBaseMaster, int PrmUserId)
        {
            string strSql = "SELECT cTType FROM TblUser WHERE nId=" + PrmUserId;
            DataTable dt = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            return (dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : "");
        }
        //============================================================================== END
        public string FnGetPDFFilePath()
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                return asdr.GetValue("PDF_PATH", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string FnGetDefaultPurchaseAccount()
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                return asdr.GetValue("PUID", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void FnGetDefaultSalesAccount(ref string PrmSlId, ref string PrmSlAcc)
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                PrmSlId = asdr.GetValue("SLID", typeof(string)).ToString();
                PrmSlAcc = asdr.GetValue("SLACC", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string FnGetDefaultSalesAccount()
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                return asdr.GetValue("SLID", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void FnGetDefaultCashAccount(ref string PrmId, ref string PrmAcc)
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                PrmId = asdr.GetValue("CSHID", typeof(string)).ToString();
                PrmAcc = asdr.GetValue("CSHACC", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string FnGetDefaultOfficeTime()
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                return asdr.GetValue("DTIME", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string FnGetDefaultTarget()
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                return asdr.GetValue("TARGET", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string FnGetSalesTType()
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                return asdr.GetValue("SALES_TTYPE", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string FnGetPurchaseTType()
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                return asdr.GetValue("PURCHASE_TTYPE", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string FnGetDateFormat(object PrmDate)
        {
            string strDate = "";
            if (FnDateTime(PrmDate, "").Equals("01/Jan/1800"))
            {
                strDate = "";
            }
            else
            {
                strDate = FnDateTime(PrmDate, "");
            }
            return strDate;
        }
        public string FnDateTime(object PrmVal, string PrmDateTimeFormat)
        {
            DateTime dtVal;
            try
            {
                PrmDateTimeFormat = (PrmDateTimeFormat.Trim().Length > 0 ? PrmDateTimeFormat : "dd/MMM/yyyy");
                dtVal = Convert.ToDateTime(PrmVal);
                if (dtVal < Convert.ToDateTime("1800-01-01"))
                {
                    dtVal = Convert.ToDateTime("1800-01-01");
                }
            }
            catch (Exception)
            {
                dtVal = Convert.ToDateTime("1800-01-01");
            }
            return (dtVal.ToString(PrmDateTimeFormat).Replace("-", "/").Equals("01/Jan/1800") ? "" : dtVal.ToString(PrmDateTimeFormat));
        }
        public string FnReplaceString(string PrmValue)
        {
            string str = PrmValue.Trim();
            return HttpUtility.HtmlEncode(PrmValue).Replace("'", "\\'");
        }
        public string FnSetFormatedValue(double PrmVal)
        {
            return (PrmVal != 0 ? String.Format("{0:0.00}", PrmVal) : "");
        }
        public string FnSetFormatedValueNew(double PrmVal)
        {
            return String.Format("{0:0.00}", PrmVal);
        }
        public string FnGetFromDate(ClsCommonBaseMaster PrmBaseMaster, int PrmFaId)
        {
            string _strFrmDate = "", _strToDate = "";
            PrmBaseMaster.FaId = PrmFaId;
            PrmBaseMaster.FnGetFaYearDate(ref _strFrmDate, ref _strToDate);
            DateTime dtVal = Convert.ToDateTime(_strFrmDate);
            return PrmBaseMaster.FnDateTime(dtVal, "");
        }
        public string FnGetGrdDateFormat(ClsCommonBaseMaster PrmBaseMaster, object PrmDate)
        {
            string strDate = "";
            if (PrmBaseMaster.FnDateTime(PrmDate, "").Equals("01/Jan/1800"))
            {
                strDate = "";
            }
            else
            {
                strDate = PrmBaseMaster.FnDateTime(PrmDate, "");
            }
            return strDate;
        }
        public string FnGetMenuPrefix(ClsCommonBaseMaster PrmBaseMaster, int PrmMenuId)
        {
            DataTable _dtGet = PrmBaseMaster.FnGetDataSet("SELECT cPrefix FROM TblMenu WHERE nMenuId=" + PrmMenuId).Tables[0];
            return (_dtGet.Rows.Count > 0 ? _dtGet.Rows[0][0].ToString() : "");
        }
        public void FnSetRadioButtonValue(RadioButtonList PrmRadBtnLst, string PrmVal)
        {
            for (int iRw = 0; iRw <= PrmRadBtnLst.Items.Count - 1; iRw++)
            {
                if (PrmVal.Trim().Equals(PrmRadBtnLst.Items[iRw].Value.ToString()))
                {
                    PrmRadBtnLst.Items[iRw].Selected = true;
                }
            }
        }
        public void FnSetCheckboxListValue(CheckBoxList PrmChkBoxLst, string PrmVal)
        {
            string[] strSrcVal = PrmVal.Trim().Split(',');
            for (int iItm = 0; iItm <= strSrcVal.Length - 1; iItm++)
            {
                for (int iRw = 0; iRw <= PrmChkBoxLst.Items.Count - 1; iRw++)
                {
                    if (strSrcVal[iItm].Trim().Equals(PrmChkBoxLst.Items[iRw].Value.ToString()))
                    {
                        PrmChkBoxLst.Items[iRw].Selected = true;
                    }
                }
            }
        }
        public void FnUnSelectCheckboxListValue(CheckBoxList PrmChkBoxLst)
        {
            for (int iRw = 0; iRw <= PrmChkBoxLst.Items.Count - 1; iRw++)
            {
                PrmChkBoxLst.Items[iRw].Selected = false;
            }
        }
        public void FnSetCheckboxListText(CheckBoxList PrmChkBoxLst, string PrmVal)
        {
            string[] strSrcVal = PrmVal.Trim().Split(',');
            for (int iItm = 0; iItm <= strSrcVal.Length - 1; iItm++)
            {
                for (int iRw = 0; iRw <= PrmChkBoxLst.Items.Count - 1; iRw++)
                {
                    if (strSrcVal[iItm].Trim().Equals(PrmChkBoxLst.Items[iRw].Text.ToString()))
                    {
                        PrmChkBoxLst.Items[iRw].Selected = true;
                    }
                }
            }
        }
        public string FnGetCheckboxListValue(CheckBoxList PrmChkBoxLst)
        {
            string strVal = "";
            for (int iRw = 0; iRw <= PrmChkBoxLst.Items.Count - 1; iRw++)
            {
                if (PrmChkBoxLst.Items[iRw].Selected == true)
                {
                    strVal = strVal + PrmChkBoxLst.Items[iRw].Value + ",";
                }
            }
            return strVal;
        }
        public string FnGetCheckboxListText(CheckBoxList PrmChkBoxLst)
        {
            string strVal = "";
            for (int iRw = 0; iRw <= PrmChkBoxLst.Items.Count - 1; iRw++)
            {
                if (PrmChkBoxLst.Items[iRw].Selected == true)
                {
                    strVal = strVal + PrmChkBoxLst.Items[iRw].Text + ",";
                }
            }
            return strVal;
        }
        public int FnGetLastItemCode(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId)
        {
            string strsql = "SELECT MAX(cCode) FROM TblItem WHERE nCompanyId=" + PrmCmpId;
            return PrmBaseMaster.FnIsNumeric(PrmBaseMaster.FnExecuteScalar(strsql).ToString());
        }
        public DataTable FnGetSetPriceList(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId)
        {
            string strSql = "SELECT nId AS Id,(CONVERT(VARCHAR,nRefNo) + ' - ' + cName) AS Name FROM TblItemSetPriceMaster WHERE nCompanyId=" + PrmCmpId + " AND nActive=1 AND nIsDelete=0 ORDER By nRefNo ASC";
            return PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
        }
        public DataTable FnGetBasePriceList(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId)
        {
            string strSql = "SELECT nId AS Id,(CONVERT(VARCHAR,nRefNo) + ' - ' + cName) AS Name FROM TblItemPriceMaster WHERE nCompanyId=" + PrmCmpId + " AND nActive=1 AND nIsDelete=0 ORDER By nRefNo ASC";
            return PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
        }
        public void FnBindingDropDownList(ClsDropdownRecordList PrmList, DataTable PrmDtLst, DropDownList PrmDropDown, string PrmCaption)
        {
            PrmDropDown.DataSource = PrmDtLst;
            PrmDropDown.DataTextField = "Name";
            PrmDropDown.DataValueField = "Id";
            PrmDropDown.DataBind();
            PrmList.FnNullDropDownList(PrmDropDown, (PrmCaption.Trim().Length > 0 ? PrmCaption : "---Select---"), "0");
        }
        public void FnBindingDropDownList(ClsDropdownRecordList PrmList, DataTable PrmDtLst, DropDownList PrmDropDown, string PrmValue, string PrmText, string PrmCaption)
        {
            PrmDropDown.DataSource = PrmDtLst;
            PrmDropDown.DataTextField = PrmText;
            PrmDropDown.DataValueField = PrmValue;
            PrmDropDown.DataBind();
            PrmList.FnNullDropDownList(PrmDropDown, (PrmCaption.Trim().Length > 0 ? PrmCaption : "---Select---"), "0");
        }
        public void FnSetDropDownValue(DropDownList PrmDropDown, string PrmVal)
        {
            try
            {
                PrmDropDown.Text = PrmVal;
            }
            catch (Exception ex)
            {
                PrmDropDown.Text = "0";
                string str = ex.Message;
            }
        }
        public DataTable FnGetCustomerDetails(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId, int PrmAccId, string PrmCode)
        {
            string strSql = "SELECT nId AS Id,cName AS Name,cCode AS Code,cPAddress AS Address FROM TblRegistration WHERE nCompanyId =" + PrmCmpId + " AND nId=" + PrmAccId + " AND cCode='" + PrmCode + "'";
            return PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
        }
        //=========================================================================================
        public bool FnAlterationStatusVisible(ClsCommonBaseMaster PrmBaseMaster, string PrmStatus)
        {
            return (PrmBaseMaster.FnIsNumeric(PrmStatus) > 0 ? true : false);
        }
        public Color FnGetOrderFuctionStatus_Color(string PrmStatus)
        {
            Color _ClrStatus;
            if (PrmStatus.Equals("BAPTISM"))
            {
                _ClrStatus = ColorTranslator.FromHtml("#d5fc00a6");
            }
            else
            {
                _ClrStatus = ColorTranslator.FromHtml("#FDF2E9");
            }
            return _ClrStatus;
        }
        public Color FnGetOrderDueDate_Color(string PrmDueDate, string PrmOptDate)
        {
            Color _ClrStatus;
            if (PrmDueDate.Trim().Equals(PrmOptDate.Trim()))
            {
                _ClrStatus = ColorTranslator.FromHtml("#f9eb98");
            }
            else
            {
                _ClrStatus = Color.Red;
            }
            return _ClrStatus;
        }
        public Color FnGetOrderUnit_Color(string PrmStatus)
        {
            Color _ClrStatus;
            if (PrmStatus.Equals("2"))
            {
                _ClrStatus = ColorTranslator.FromHtml("#d9dff1");
            }
            else if (PrmStatus.Equals("3"))
            {
                _ClrStatus = ColorTranslator.FromHtml("#f9d5f4");
            }
            else if (PrmStatus.Equals("5"))
            {
                _ClrStatus = ColorTranslator.FromHtml("#f7c984");
            }
            else
            {
                _ClrStatus = ColorTranslator.FromHtml("#c7efc4");
            }
            return _ClrStatus;
        }
        public string FnGetDateDiffDays(DateTime PrmFrmDate, DateTime PrmToDate)
        {
            return (Convert.ToDateTime(PrmFrmDate.ToString("dd/MMM/yyyy")) - Convert.ToDateTime(PrmToDate.ToString("dd/MMM/yyyy"))).TotalDays.ToString();
        }
        //=========== FILE UPLOAD =======================================================
        public string FnServerUploadPath(string PrmFileName)
        {
            return Server.MapPath(Request.ApplicationPath) + PrmFileName.Trim();
        }
        public string[] FnGetImageFormat(ref string PrmFormat)
        {
            PrmFormat = "Only .JPEG || .jpg ||.jpeg ||.JPG ||.png || .PNG ||.tif ||.tiff ||.TIF ||.TIFF ||.jfif ||.JFIF Image formats are allowed.";
            string[] allowed = { ".JPEG", ".jpg", ".jpeg", ".JPG", ".png", ".PNG", ".tif", ".tiff", ".TIF", ".TIFF", ".jfif", ".JFIF" };
            return allowed;
        }
        public string[] FnGetAllFormat(ref string PrmFormat)
        {
            PrmFormat = "Only .JPEG || .jpg ||.jpeg ||.JPG ||.png || .PNG ||.tif ||.tiff ||.TIF ||.TIFF ||.jfif ||.JFIF Image formats are allowed.";
            string[] allowed = { ".JPEG", ".jpg", ".jpeg", ".JPG", ".png", ".PNG", ".tif", ".tiff", ".TIF", ".TIFF", ".jfif", ".JFIF", ".pdf", ".PDF" };
            return allowed;
        }
        //public DataTable FnGetProjectList(ClsToolsIssue PrmBaseMaster, int ProjectId)
        //{
        //    string strSql = "select TM.nId Id,TM.nRefNo Name  from TblToolsMaster TM inner join TblProject P on P.nId=TM.nProjectId where TM.nProjectId= " + ProjectId;
        //    return PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
        //}
        public string FnSaveUploadFileNameWithToken(ClsCommonBaseMaster PrmBaseMaster, string PrmFileName, string PrmPrefix)
        {
            string[] source = PrmFileName.Trim().Split('\\');
            string[] sourcefile = source[source.GetUpperBound(0)].Split('.');
            string sourcefileExt = sourcefile[sourcefile.GetUpperBound(0)];
            string tok = PrmBaseMaster.FnGetTokenId().ToString();
            PrmFileName = tok + "." + sourcefileExt;
            return PrmFileName;
            //return PrmPrefix.Trim() + PrmFileName + "." + sourcefileExt;
            //return "arc.png";
        }
        //public DataTable FnGetEmployeeList(ClsToolsIssue PrmBaseMaster, int EmpId)
        //{
        //    string strSql = "select TM.nId Id,TM.nRefNo Name from TblToolsMaster TM inner join TblEmployee E on E.nId=TM.nEmployeeId where TM.nEmployeeId=" + EmpId;
        //    return PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
        //}
        public string[] FnGetCSVFormat(ref string PrmFormat)
        {
            PrmFormat = "Only .CSV || .csv ||.xls ||.XLS";
            string[] allowed = { ".CSV", ".csv", ".xls", ".XLS" };
            return allowed;
        }
        public string FnSaveUploadFileName(ClsCommonBaseMaster PrmBaseMaster, string PrmFileName, string PrmPrefix)
        {
            string[] source = PrmFileName.Trim().Split('\\');
            string[] sourcefile = source[source.GetUpperBound(0)].Split('.');
            string sourcefileExt = sourcefile[sourcefile.GetUpperBound(0)];
            return PrmFileName;
            //return PrmPrefix.Trim() + PrmFileName + "." + sourcefileExt;
            //return "arc.png";
        }
        public bool FnValidateFileSize(AsyncFileUpload PrmFileUploadCtrl, decimal PrmSize, int PrmHeight, int PrmWidth)
        {
            bool bVal = true;
            if (PrmSize > 0)
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(PrmFileUploadCtrl.PostedFile.InputStream);
                int height = img.Height;
                int width = img.Width;
                decimal size = Math.Round(((decimal)PrmFileUploadCtrl.PostedFile.ContentLength / (decimal)1024), 2);
                if (size > PrmSize)
                {
                    FnPopUpAlert("File size must not exceed " + PrmSize.ToString() + " KB.");
                    bVal = false;
                }
                else if (height > PrmHeight)
                {
                    FnPopUpAlert("Height must not exceed " + PrmHeight.ToString() + "px.");
                    bVal = false;
                }
                else if (width > PrmWidth)
                {
                    FnPopUpAlert("Width must not exceed " + PrmWidth.ToString() + "px.");
                    bVal = false;
                }
            }
            return bVal;
        }
        public string[] FnGetPdfFormat(ref string PrmFormat)
        {
            PrmFormat = "Only .pdf || .PDF Document formats are allowed.";
            string[] allowed = { ".pdf", ".PDF" };
            return allowed;
        }
        public string FnDocFileIcon()
        {
            return "..\\images\\DocIcon.png";
        }
        public string FnVehicleRegIcon()
        {
            return "..\\images\\VchReg.png";
        }
        public string FnVehicleImgIcon()
        {
            return "..\\images\\VchImg.png";
        }
        public string FnProfileIcon()
        {
            return "..\\images\\profileIcon.png";
        }
        public string FnDocFilePath(string PrmFlag, string PrmFileName)
        {
            string strFile = "";
            if (PrmFlag.Equals("VCH"))
            {
                strFile = "\\UploadedFiles\\Vehicle\\" + PrmFileName.Replace("~", "").Trim();
            }
            else if (PrmFlag.Equals("ITM"))
            {
                strFile = "\\UploadedFiles\\Products\\" + PrmFileName.Replace("~", "").Trim();
            }
            else if (PrmFlag.Equals("DOC") || PrmFlag.Equals("CSV"))
            {
                strFile = "\\UploadedFiles\\Documents\\" + PrmFileName.Replace("~", "").Trim();
            }
            return strFile;
        }
        public string FnProfileFilePath(string PrmFileName)
        {
            return "\\UploadedFiles\\Profile\\" + PrmFileName.Replace("~", "").Trim();
        }
        public string FnUploadDocFilePath(string PrmFlag, string PrmFileName)
        {
            string strFile = "";
            if (PrmFlag.Equals("VCH_REG") || PrmFlag.Equals("VCH_IMG"))
            {
                strFile = "\\UploadedFiles\\Vehicle\\" + PrmFileName.Replace("~", "").Trim();
            }
            return strFile;
        }
        public void FnBindDocumetPath(HyperLink PrmHyLnkFile, string PrmFileName, string PrmDocType)
        {
            PrmHyLnkFile.Height = 30;
            PrmHyLnkFile.Width = 30;
            PrmHyLnkFile.ImageHeight = 30;
            PrmHyLnkFile.ImageWidth = 30;
            if (PrmDocType.Equals("VCH_REG"))
            {
                PrmHyLnkFile.ImageUrl = FnVehicleRegIcon();
            }
            else if (PrmDocType.Equals("VCH_IMG"))
            {
                PrmHyLnkFile.ImageUrl = FnVehicleImgIcon();
            }
            else if (PrmDocType.Equals("PRF"))
            {
                PrmHyLnkFile.ImageUrl = FnProfileIcon();
            }
            else if (PrmDocType.Equals("DOC"))
            {
                PrmHyLnkFile.ImageUrl = FnDocFileIcon();
            }
            if (PrmFileName.Trim().Length > 0)
            {
                PrmHyLnkFile.NavigateUrl = FnDocFilePath(PrmDocType, PrmFileName);
                PrmHyLnkFile.Target = "_blank";
            }
            else
            {
                PrmHyLnkFile.ToolTip = "";
                PrmHyLnkFile.NavigateUrl = "";
            }
        }
        public void FnGetPopUpWindowDispaly(string PrmHeader, HyperLink PrmHyLink, int PrmWidth, int PrmHeight, string PrmURL, Label PrmLblScript)
        {
            string strScript = "<script type='text/javascript'>";
            strScript = strScript + "function FnPopUpDisplay" + PrmHyLink.ID + "(){var ReportWinEdit" + PrmHyLink.ID + "=dhtmlwindow.open('ReportWindowEdit" + PrmHyLink.ID + "', 'iframe', '" + PrmURL + "', '" + PrmHeader + "', 'width=" + PrmWidth.ToString() + "px,height=" + PrmHeight.ToString() + "px,resize=0,scrolling=0,center=1', 'recal');}";
            strScript = strScript + "</script>";
            PrmHyLink.Attributes.Add("onClick", "return FnPopUpDisplay" + PrmHyLink.ID + "();");
            PrmLblScript.Text = PrmLblScript.Text + strScript;
            //Page objPage = (HttpContext.Current.CurrentHandler) as Page;
            //ScriptManager.RegisterClientScriptBlock(objPage, typeof(ClsPageEvents), "Msg", strScript, false);
        }
        public void FnGetPopUpWindowDispaly(string PrmHeader, LinkButton PrmLnkBtn, int PrmWidth, int PrmHeight, string PrmURL, Label PrmLblScript)
        {
            PrmLblScript.Text = "";
            string strScript = "<script type='text/javascript'>";
            strScript = strScript + "function FnPopUpDisplay" + PrmLnkBtn.ID + "(){var ReportWinEdit" + PrmLnkBtn.ID + "=dhtmlwindow.open('ReportWindowEdit" + PrmLnkBtn.ID + "', 'iframe', '" + PrmURL + "', '" + PrmHeader + "', 'width=" + PrmWidth.ToString() + "px,height=" + PrmHeight.ToString() + "px,resize=0,scrolling=0,center=1', 'recal');}";
            strScript = strScript + "</script>";
            PrmLblScript.Text = strScript;
            PrmLnkBtn.Attributes.Add("onClick", "return FnPopUpDisplay" + PrmLnkBtn.ID + "();");
            //Page objPage = (HttpContext.Current.CurrentHandler) as Page;
            //ScriptManager.RegisterClientScriptBlock(objPage, typeof(ClsPageEvents), "Msg", strScript, false);
        }
        public string FnGetSubString(object PrmVal)
        {
            return (PrmVal.ToString().Trim().Length > 19 ? PrmVal.ToString().Trim().Substring(0, 20) + "...." : PrmVal.ToString());
        }
        public string FnGetSubString(object PrmVal, int PrmLength)
        {
            return (PrmVal.ToString().Trim().Length > (PrmLength + 6) ? PrmVal.ToString().Trim().Substring(0, PrmLength) + "...." : PrmVal.ToString());
        }
        public bool FnVisibleFileBrowser(DropDownList PrmDdlCtrl)
        {
            bool bVal = false;
            if (PrmDdlCtrl.SelectedValue.ToString().Equals("VIDEO FILE") || PrmDdlCtrl.SelectedValue.ToString().Equals("IMAGE FILE") || PrmDdlCtrl.SelectedValue.ToString().Equals("DOC FILE"))
            {
                bVal = true;
            }
            return bVal;
        }
        public Color FnSelectedRowColor()
        {
            Color _ClrStatus = ColorTranslator.FromHtml("#DCFBDA");
            return _ClrStatus;
        }
        public Color FnTransparentColor()
        {
            return Color.Transparent; ;
        }
        public Color FnGridChildRowColor()
        {
            return Color.LightGoldenrodYellow;
        }
        public Color FnGridReturnRowColor()
        {
            Color _ClrStatus = ColorTranslator.FromHtml("#fae6d2");
            return _ClrStatus;
        }
        public Color FnGridMaintananceRowColor()
        {
            Color _ClrStatus = ColorTranslator.FromHtml("#a6f4f0");
            return _ClrStatus;
        }
        public string FnRemoveLasteVal(string PrmVal)
        {
            return (PrmVal.Trim().Length > 0 ? PrmVal.Remove(PrmVal.Trim().Length - 1) : "");
        }
        public string FnGetOrderRefNo(string PrmAccID, string PrmRefNo)
        {
            return PrmAccID + "/" + PrmRefNo;
        }
        public string FnRemoveArrayValue(string PrmVal, string PrmRemoveVal)
        {
            string strVal = "";
            string[] str = PrmVal.Split(',');
            var myList = str.ToList();
            myList.Remove(PrmRemoveVal);
            foreach (string strNew in myList)
            {
                strVal = strVal + strNew + ",";
            }
            return FnRemoveLasteVal(strVal);
        }
        public string FnGetUserWiseAccId(ClsCommonBaseMaster PrmBaseMaster, int PrmUserId)
        {
            string strSql = "SELECT nStaffId FROM TblUser WHERE nId=" + PrmUserId;
            DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            return (DT_RECORD.Rows.Count > 0 ? DT_RECORD.Rows[0][0].ToString() : "0");
        }
        public string FnGetUserWiseAccId(ClsCommonBaseMaster PrmBaseMaster, int PrmUserId, ref string PrmCustomer)
        {
            string strId = "0";
            string strSql = "SELECT nStaffId,(SELECT cName FROM TblRegistration WHERE nId=nStaffId) AS Customer FROM TblUser WHERE nId=" + PrmUserId;
            DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            if (DT_RECORD.Rows.Count > 0)
            {
                strId = DT_RECORD.Rows[0][0].ToString();
                PrmCustomer = DT_RECORD.Rows[0][1].ToString();
            }
            return strId;
        }
        public string FnGetCompanyWiseAccId(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId)
        {
            string strSql = "SELECT nSupplierId FROM TblCompany WHERE nId=" + PrmCmpId;
            DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            return (DT_RECORD.Rows.Count > 0 ? DT_RECORD.Rows[0][0].ToString() : "0");
        }
        public string FnGetCompanyWiseAccId(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId, ref string PrmEmail)
        {
            string strId = "0";
            string strSql = "SELECT nSupplierId,cEmail FROM TblCompany WHERE nId=" + PrmCmpId;
            DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            if (DT_RECORD.Rows.Count > 0)
            {
                strId = DT_RECORD.Rows[0][0].ToString();
                PrmEmail = DT_RECORD.Rows[0][1].ToString();
            }
            return strId;
        }
        public string FnGetValidValue(object PrmVal)
        {
            double _iVal = 0;
            try
            {
                _iVal = Convert.ToDouble(PrmVal);
            }
            catch (Exception)
            {
                _iVal = 0;
            }
            return (_iVal > 0 ? PrmVal.ToString() : "");
        }
        public bool ThumbnailCallback()
        {
            return false;
        }
        public byte[] FnGenerateThumbnail(string PrmFilePath, ref string PrmImgUrl)
        {
            byte[] bytes = null;
            string path = PrmFilePath;
            System.Drawing.Image image = System.Drawing.Image.FromFile(path);
            using (System.Drawing.Image thumbnail = image.GetThumbnailImage(150, 175, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    thumbnail.Save(memoryStream, ImageFormat.Png);
                    bytes = new byte[memoryStream.Length];
                    memoryStream.Position = 0;
                    memoryStream.Read(bytes, 0, (int)bytes.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    PrmImgUrl = "data:image/png;base64," + base64String;
                }
            }
            return bytes;
        }
        public static bool IsValidPassword(string plainText)
        {
            Regex regex = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{7,15}$");
            Match match = regex.Match(plainText);
            return match.Success;
        }
        public void FnFileUplodSuccessErrorScript(AsyncFileUpload PrmFileUpload)
        {
            PrmFileUpload.OnClientUploadComplete = "FnUploadComplete";
            PrmFileUpload.OnClientUploadError = "FnUploadError";
        }
        public void FnFileUplodSuccessErrorScript(AsyncFileUpload PrmFileUpload, string PrmSuccess, string PrmError)
        {
            PrmFileUpload.OnClientUploadComplete = PrmSuccess;
            PrmFileUpload.OnClientUploadError = PrmError;
        }
        public void FnConvertByteToImge(byte[] PrmBytes, HtmlImage PrmImage)
        {
            try
            {
                //PrmImage.Width = 150;
                //PrmImage.Height = 175;
                if (PrmBytes.ToString().Trim().Length > 0)
                {
                    //byte[] bImg = (byte[])((Session["IMGBYTES"]));
                    string base64String = Convert.ToBase64String(PrmBytes, 0, PrmBytes.Length);
                    PrmImage.Src = "data:image/png;base64," + base64String;
                }
                else
                {
                    PrmImage.Src = "~//images//none.png";
                }
            }
            catch (Exception ex)
            {
                PrmImage.Src = "~//images/none.png";
            }
        }
        public void FnSetImage(HtmlImage PrmImage, string PrmFilePath)
        {
            try
            {
                string profileImagePath = Path.Combine(Server.MapPath("~\\UploadedFiles\\Profile\\"), PrmFilePath);

                // Check if the file exists and set the image source accordingly
                if (File.Exists(profileImagePath))
                {
                    PrmImage.Src = ResolveUrl("~\\UploadedFiles\\Profile\\" + PrmFilePath);
                }
                else
                {
                    PrmImage.Src = ResolveUrl("~//images//none.png");
                }
            }
            catch (Exception ex)
            {
                PrmImage.Src = "~//images//none.png";
            }
        }
        public void FnNoneImage(HtmlImage PrmImage)
        {
            PrmImage.Src = "~//images/none.png";
        }
        public void FnGetItemPicture(ClsCommonBaseMaster PrmBaseMaster, int PrmItemId, HtmlImage PrmImage, CheckBox PrmChkExpire)
        {
            try
            {
                string strSql = "SELECT cImageByte AS ImageByte,cImagePath AS ImagePath,nExpDays AS ExpDays FROM TblItem WHERE nId=" + PrmItemId;
                DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
                if (DT_RECORD.Rows.Count > 0)
                {
                    PrmChkExpire.Enabled = false;
                    PrmChkExpire.Checked = (PrmBaseMaster.FnIsDouble(DT_RECORD.Rows[0]["ExpDays"].ToString()) > 0 ? true : false);
                    byte[] bImg = (byte[])((DT_RECORD.Rows[0]["ImageByte"]));
                    string base64String = Convert.ToBase64String(bImg, 0, bImg.Length);
                    PrmImage.Src = "data:image/png;base64," + base64String;
                }
                else
                {
                    PrmChkExpire.Checked = false;
                    PrmImage.Src = "~//images/none.png";
                }
            }
            catch (Exception ex)
            {
                PrmChkExpire.Checked = false;
                PrmImage.Src = "~//images/none.png";
            }
        }
        public void FnGetModalPoUp(string PrmId, string PrmName, string PrmUrl, HtmlButton PrmBtn, HtmlContainerControl PrmLabel)
        {
            PrmId = "Mdl_" + PrmId;
            string strPopUp = "<div class='modal fade' id='" + PrmId + "' tabindex='-1' aria-labelledby='ModalPoup' aria-hidden='true'>"
                                + "<div class='modal-dialog'><div class='modal-content'><div class='modal-header'>"
                                + "<h1 class='modal-title fs-5' id='ModalPoup'>" + PrmName + "</h1>"
                                + "<button type = 'button' class='btn-close' data-bs-dismiss='modal' aria-label='Close'></button></div>"
                                + "<div class='modal-body'>"
                                + "<iframe runat = 'server'id='iFrameUrl' src='" + PrmUrl + "' style='width: 100%; height: 300px;'></iframe>"
                                + "</div></div></div></div>";
            PrmLabel.InnerHtml = PrmLabel.InnerHtml + strPopUp;
            PrmBtn.Attributes.Add("data-bs-target", "#" + PrmId);
        }
        public void FnGetModalPopUp(string PrmId, string PrmName, string PrmUrl, LinkButton PrmBtn, HtmlContainerControl PrmLabel, int PrmHeight)
        {
            FnGetModalGridPoUp(PrmId, PrmName, PrmUrl, PrmBtn, PrmLabel, "", PrmHeight);
        }
        public void FnGetModalPoUpSamll(string PrmId, string PrmName, string PrmUrl, LinkButton PrmBtn, HtmlContainerControl PrmLabel, int PrmHeight)
        {
            FnGetModalGridPoUp(PrmId, PrmName, PrmUrl, PrmBtn, PrmLabel, "modal-sm", PrmHeight);
        }
        public void FnGetModalPoUpLarge(string PrmId, string PrmName, string PrmUrl, LinkButton PrmBtn, HtmlContainerControl PrmLabel, int PrmHeight)
        {
            FnGetModalGridPoUp(PrmId, PrmName, PrmUrl, PrmBtn, PrmLabel, "modal-lg", PrmHeight);
        }
        public void FnGetModalPoUpExtra(string PrmId, string PrmName, string PrmUrl, LinkButton PrmBtn, HtmlContainerControl PrmLabel, int PrmHeight)
        {
            FnGetModalGridPoUp(PrmId, PrmName, PrmUrl, PrmBtn, PrmLabel, "modal-xl", PrmHeight);
        }
        private void FnGetModalGridPoUp(string PrmId, string PrmName, string PrmUrl, LinkButton PrmBtn, HtmlContainerControl PrmLabel, string PrmType, int PrmHeight)
        {
            PrmHeight = (PrmHeight > 0 ? PrmHeight : 300);
            PrmType = " modal-dialog " + PrmType;
            PrmId = "Mdl_" + PrmId;
            string strPopUp = "<div class='modal fade' id='" + PrmId + "' tabindex='-1' aria-labelledby='ModalPoup' aria-hidden='true'>"
                                + "<div class='" + PrmType + "'><div class='modal-content'><div class='modal-header'>"
                                + "<h1 class='modal-title fs-5' id='ModalPoup'>" + PrmName + "</h1>"
                                + "<button type = 'button' class='btn-close' data-bs-dismiss='modal' aria-label='Close'></button></div>"
                                + "<div class='modal-body'>"
                                + "<iframe runat = 'server'id='iFrameUrl' src='" + PrmUrl + "' style='width: 100%; height:" + PrmHeight + "px;'></iframe>"
                                + "</div></div></div></div>";
            PrmLabel.InnerHtml = PrmLabel.InnerHtml + strPopUp;
            PrmBtn.Attributes.Add("data-bs-target", "#" + PrmId);
        }
        //============= FINANACE MASTER ===============
        public string FnGetPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@!#$%;[]|";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public static string FnGetRandomAlphanumericString(string PrmCode, int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";
            return PrmCode + FnGetRandomString(length, alphanumericCharacters);
        }

        public static string FnGetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }
        public string FnGetSupplierName(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId)
        {
            string strSql = "SELECT cName AS Name FROM TblCompany WHERE nId=" + PrmCmpId;
            DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            return (DT_RECORD.Rows.Count > 0 ? DT_RECORD.Rows[0][0].ToString() : "");
        }
        public string FnGetSupplierImage(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId, ref int PrmIsImage)
        {
            string strVal = "";
            string strSql = "SELECT cUrl AS Url,cName AS Name FROM TblCompany WHERE nId=" + PrmCmpId;
            DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            if (DT_RECORD.Rows.Count > 0)
            {
                if (DT_RECORD.Rows[0][0].ToString().Trim().Length > 0)
                {
                    PrmIsImage = 1;
                    strVal = DT_RECORD.Rows[0][0].ToString();
                }
                else
                {
                    strVal = DT_RECORD.Rows[0][1].ToString();
                }
            }
            return strVal;
        }
        //=========================== DASH BOARD DETAILS
        public string FnGetDashBoardUrl(string PrmTitle, string PrmUrl, string PrmCntrId, string PrmCmpId, string PrmBrId, string PrmFaId, string PrmTType, string PrmUserId, string PrmUsrGrpId, string PrmMenuId, string PrmUserName)
        {
            return PrmUrl + "?HDR=Dashboard&TTYPE=" + PrmTType + "&TITLE=" + PrmTitle + "&CNTRID=" + PrmCntrId + "&UID=" + PrmUserId + "&UGRPID=" + PrmUsrGrpId + "&CID=" + PrmCmpId + "&BID=" + PrmBrId + "&FID=" + PrmFaId + "&MID=" + PrmMenuId + "&WIDTH=1000&HEIGHT=1000&DID=0&DATE=" + DateTime.Now.ToString() + "&UNAME=" + PrmUserName;
        }
        public string FnGetPageUrl(string PrmTitle, string PrmUrl, string PrmCntrId, string PrmCmpId, string PrmBrId, string PrmFaId, string PrmTType, string PrmUserId, string PrmUsrGrpId, string PrmMenuId, string PrmUserName, string PrmPID, string PrmDID)
        {
            return PrmUrl + "?HDR=" + PrmTitle + "&TTYPE=" + PrmTType + "&TITLE=" + PrmTitle + "&CNTRID=" + PrmCntrId + "&UID=" + PrmUserId + "&UGRPID=" + PrmUsrGrpId + "&CID=" + PrmCmpId + "&BID=" + PrmBrId + "&FID=" + PrmFaId + "&MID=" + PrmMenuId + "&WIDTH=1000&HEIGHT=1000&DID=0&DATE=" + DateTime.Now.ToString() + "&UNAME=" + PrmUserName + "&PID=" + PrmPID + "&DID=" + PrmDID;
        }
        public string FnGetPageUrlCsv(string PrmTitle, string PrmUrl, string PrmCntrId, string PrmMonth, string PrmYear, string PrmDepId, string PrmPjctId, string PrmCmpId, string PrmBrId, string PrmFaId, string PrmTType, string PrmUserId, string PrmUsrGrpId, string PrmMenuId, string PrmUserName, string PrmPID, string PrmDID)
        {
            return PrmUrl + "?HDR=" + PrmTitle + "&TTYPE=" + PrmTType + "&TITLE=" + PrmTitle + "&CNTRID=" + PrmCntrId + "&MONTH=" + PrmMonth + "&YEAR=" + PrmYear + "&DEPID=" + PrmDepId + "&PJID=" + PrmPjctId + "&UID=" + PrmUserId + "&UGRPID=" + PrmUsrGrpId + "&CID=" + PrmCmpId + "&BID=" + PrmBrId + "&FID=" + PrmFaId + "&MID=" + PrmMenuId + "&WIDTH=1000&HEIGHT=1000&DID=0&DATE=" + DateTime.Now.ToString() + "&UNAME=" + PrmUserName + "&PID=" + PrmPID + "&DID=" + PrmDID;
        }
        public void FnGetInventoryDashboard(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId, Repeater PrmRptrRecentLst, Repeater PrmRptrIssueLst, Repeater PrmRptrExpireLst, ref string PrmCnt1, ref string PrmCnt2, ref string PrmCnt3, ref string PrmCnt4, ref string PrmCnt5, ref string PrmCnt6, ref string PrmCnt7, ref string PrmCnt8)
        {
            string strSql = "SELECT TOP 10 DBO.fnFormatDate (TrDate, 'MON') AS MonthVal,DBO.fnFormatDate (TrDate, 'DD') AS DateVal,(ItemName) AS ItemDesc,Qty,Remarks FROM VwInvoiceItems WHERE TType='PI' AND CompanyId=" + PrmCmpId + " ORDER BY RefNo DESC";
            PrmRptrRecentLst.DataSource = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            PrmRptrRecentLst.DataBind();

            strSql = "SELECT TOP 10 DBO.fnFormatDate (TrDate, 'MON') AS MonthVal,DBO.fnFormatDate (TrDate, 'DD') AS DateVal,(ItemName) AS ItemDesc,Qty,Remarks FROM VwInvoiceItems WHERE TType='SL' AND CompanyId=" + PrmCmpId + " ORDER BY RefNo DESC";
            PrmRptrIssueLst.DataSource = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            PrmRptrIssueLst.DataBind();

            strSql = "SELECT TOP 10 DBO.fnFormatDate (DueDate, 'MON') AS MonthVal,DBO.fnFormatDate (DueDate, 'DD') AS DateVal,(ItemName) AS ItemDesc,Qty,Remarks FROM VwInvoiceItems WHERE DueDate>=GETDATE() AND ExpDays>0 AND TType='PI' AND CompanyId=" + PrmCmpId + " ORDER BY RefNo ASC";
            PrmRptrExpireLst.DataSource = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            PrmRptrExpireLst.DataBind();

            strSql = "SELECT COUNT(*) AS CNT,'ITM' AS TTYPE FROM TblItem WHERE nCompanyId = " + PrmCmpId
                    + "UNION ALL SELECT COUNT(*) AS CNT,'RCNT' AS TTYPE FROM TblInvoice WHERE cTType ='PI' AND nCompanyId =" + PrmCmpId
                    + "UNION ALL SELECT COUNT(*) AS CNT,'ICNT' AS TTYPE FROM TblInvoice WHERE cTType = 'SL' AND nCompanyId= " + PrmCmpId
                    + "UNION ALL SELECT INV.nItemId AS CNT,'LSTK' AS TTYPE FROM TblInvoiceItems INV WHERE INV.nCompanyId=" + PrmCmpId
                    + " GROUP BY INV.nItemId HAVING(SUM(INV.nPuQty) - SUM(INV.nSlQty)) < (SELECT MAX(ITM.nMinlevel) from TblItem ITM WHERE ITM.nId = INV.nItemId) "
                    + " UNION ALL SELECT INV.nItemId AS CNT,'MSTK' AS TTYPE FROM TblInvoiceItems INV WHERE INV.nCompanyId=" + PrmCmpId
                    + " GROUP BY INV.nItemId HAVING(SUM(INV.nPuQty) - SUM(INV.nSlQty)) > (SELECT MAX(ITM.nMaxLevel) from TblItem ITM WHERE ITM.nId = INV.nItemId)"
                    + " UNION ALL SELECT INV.nItemId AS CNT,'EXP' AS TTYPE FROM TblInvoiceItems INV INNER JOIN TblItem ITM ON ITM.nId = INV.nItemId AND ITM.nExpDays > 0 AND INV.dDueDate <= DATEADD(DAY, 30, GETDATE()) "
                    + " WHERE INV.nCompanyId=" + PrmCmpId
                    + " UNION ALL SELECT INV.nItemId AS CNT,'OSTK' AS TTYPE FROM TblInvoiceItems INV WHERE INV.nCompanyId=" + PrmCmpId
                    + " GROUP BY INV.nItemId "
                    + " HAVING(SUM(INV.nPuQty) - SUM(INV.nSlQty)) <= 0";
            DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            DataView DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='ITM'";
            PrmCnt1 = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='LSTK'";
            PrmCnt2 = DwList.ToTable().Rows.Count.ToString();

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='OSTK'";
            PrmCnt3 = DwList.ToTable().Rows.Count.ToString();
            PrmCnt4 = DwList.ToTable().Rows.Count.ToString();

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='MSTK'";
            PrmCnt5 = DwList.ToTable().Rows.Count.ToString();

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='EXP'";
            PrmCnt6 = DwList.ToTable().Rows.Count.ToString();

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='RCNT'";
            PrmCnt7 = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='ICNT'";
            PrmCnt8 = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");
        }
        public void FnGetCustomerDashboard(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId, int PrmCustId, int PrmSupId, ref string PrmCntBasket, ref string PrmCntOrderList, ref string PrmCntTotalAmt, ref string PrmCntOfferItms, ref string PrmCntNotice, ref string PrmCntTempCnt)
        {
            string strSql = "SELECT COUNT(DISTINCT nItemId) AS CNT,'CART' AS TTYPE,0 AS Amt FROM TblCartItems WHERE nActive=1 AND nCompanyId = " + PrmCmpId + " AND nDrId=" + PrmCustId + " AND nCrId=" + PrmSupId
                    + " UNION ALL SELECT COUNT(*) AS CNT,'ORDCNT' AS TTYPE,0 AS Amt FROM TblOrder WHERE nActive=1 AND cTType ='ORD' AND nCompanyId =" + PrmCmpId + " AND nDrId=" + PrmCustId + " AND nCrId=" + PrmSupId
                    + " UNION ALL SELECT 0 AS CNT,'ORD' AS TTYPE,nGrandTotal AS Amt FROM TblOrder WHERE nId IN(SELECT MAX(nId) FROM TblOrder WHERE nActive=1 AND cTType ='ORD' AND nCompanyId =" + PrmCmpId + " AND nDrId=" + PrmCustId + " AND nCrId=" + PrmSupId + ")"
                    + " UNION ALL SELECT COUNT(DISTINCT nItemId) AS CNT,'OFFR' AS TTYPE,0 AS Amt FROM TblInvoiceItems WHERE nActive=1 AND nCompanyId= " + PrmCmpId
                    + " UNION ALL SELECT COUNT(*) AS CNT,'ALERT' AS TTYPE,0 AS Amt FROM TblMessage WHERE nActive=1 AND nIsDelete=0 AND nCompanyId=" + PrmCmpId
                    + " UNION ALL SELECT COUNT(*) AS CNT,'TEMP' AS TTYPE,0 AS Amt FROM TblTemplate WHERE  nActive=1 AND nCompanyId =" + PrmCmpId + " AND nDrId=" + PrmCustId + " AND nCrId=" + PrmSupId;
            DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            DataView DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='CART'";
            PrmCntBasket = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='ORDCNT'";
            PrmCntOrderList = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='ORD'";
            PrmCntTotalAmt = (DwList.ToTable().Rows.Count > 0 ? "£" + Math.Round(PrmBaseMaster.FnIsDouble(DwList.ToTable().Rows[0]["Amt"].ToString()), 2).ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='OFFR'";
            PrmCntOfferItms = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='ALERT'";
            PrmCntNotice = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='TEMP'";
            PrmCntTempCnt = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");
        }
        public void FnGetSupplierDashboard(ClsCommonBaseMaster PrmBaseMaster, int PrmCmpId, int PrmSupId, ref string PrmCntTotalOrder, ref string PrmCntPendingOrder, ref string PrmCntTodayOrder, ref string PrmCntTomorrowOrder)
        {
            string strDate = DateTime.Now.ToString("yyyy-MM-dd");
            string strDateTmrw = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            string strSql = "SELECT COUNT(*) AS CNT,'ORDCNT' AS TTYPE,0 AS Amt FROM TblOrder WHERE nActive=1 AND nIsDelete=0 AND cTType ='ORD' AND nCompanyId =" + PrmCmpId
                    + " UNION ALL SELECT COUNT(*) AS CNT,'PNDG' AS TTYPE,0 AS Amt FROM TblOrder WHERE nIsExport=0 AND nActive=1 AND nIsDelete=0 AND cTType ='ORD' AND nCompanyId =" + PrmCmpId
                    + " UNION ALL SELECT COUNT(*) AS CNT,'TODAY' AS TTYPE,0 AS Amt FROM TblOrder WHERE nActive=1 AND nIsDelete=0 AND cTType ='ORD' AND nCompanyId= " + PrmCmpId + " AND dTrDate='" + PrmBaseMaster.FnDateTime(strDate, "")
                    + "' UNION ALL SELECT COUNT(*) AS CNT,'TMRW' AS TTYPE,0 AS Amt FROM TblOrder WHERE nActive=1 AND nIsDelete=0 AND cTType ='ORD' AND nCompanyId=" + PrmCmpId + " AND dDueDate='" + PrmBaseMaster.FnDateTime(strDateTmrw, "") + "'";
            DT_RECORD = PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
            DataView DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='ORDCNT'";
            PrmCntTotalOrder = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='PNDG'";
            PrmCntPendingOrder = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='TODAY'";
            PrmCntTodayOrder = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");

            DwList = new DataView(DT_RECORD);
            DwList.RowFilter = "TTYPE='TMRW'";
            PrmCntTomorrowOrder = (DwList.ToTable().Rows.Count > 0 ? DwList.ToTable().Rows[0]["CNT"].ToString() : "0");
        }
        public DataTable FnGetCustomerItemList(ClsCommonBaseMaster PrmBaseMaster, double PrmTokenNo, int PrmCmpId, int PrmBrId, int PrmFaId, string PrmTType, string PrmItemDesc)
        {
            string strSql = "SELECT* FROM VwChildItems WHERE CompanyId = " + PrmCmpId + " AND TokenNo = " + PrmTokenNo + " AND BranchId = " + PrmBrId + " AND FaId = " + PrmFaId + " AND TType = '" + PrmTType + "'";
            if (PrmItemDesc.Trim().Length > 0)
            {
                strSql = strSql + " AND (ItemName LIKE '%" + PrmItemDesc + "%' OR ItemCode LIKE '%" + PrmItemDesc + "%')";
            }
            strSql = strSql + " ORDER BY ItmGrpName,ItemName,ItemCode ASC";
            return PrmBaseMaster.FnGetDataSet(strSql).Tables[0];
        }
        public double FnGetTotal(ClsCommonBaseMaster PrmBaseMaster, double PrmTokenNo, int PrmCmpId, int PrmBrId, int PrmFaId, string PrmTType)
        {
            string strSql = "SELECT COUNT(*) AS Cnt,SUM(nAmount) AS NetTotal,SUM(nTaxValue) AS TaxValue,SUM(nTotAmt) AS GrandTotal FROM TblChildItems WHERE nTotAmt> 0 AND nTokenId=" + PrmTokenNo + " AND cTType= '" + PrmTType + "' AND nCompanyId=" + PrmCmpId + " AND nBranchId=" + PrmBrId + " AND nFaId=" + PrmFaId;
            return PrmBaseMaster.FnIsDouble(PrmBaseMaster.FnGetDataSet(strSql).Tables[0].Rows[0]["GrandTotal"].ToString());
        }
        public void FnGetDefaultOfferCategory(ref string PrmId, ref string PrmName)
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                PrmId = asdr.GetValue("CATID", typeof(string)).ToString();
                PrmName = asdr.GetValue("CATNAME", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void FnGetDefaultBox(ref string PrmId, ref string PrmName)
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                PrmId = asdr.GetValue("BOXID", typeof(string)).ToString();
                PrmName = asdr.GetValue("BOXNAME", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string FnGetJsonFilePath()
        {
            try
            {
                AppSettingsReader asdr = new AppSettingsReader();
                return asdr.GetValue("JSON_PATH", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void FnBindingDropDownList(ClsDropdownRecordList PrmList, DataTable PrmDtLst, DropDownList PrmDropDown, string PrmCaption, string PrmSetVal)
        {
            PrmDropDown.DataSource = PrmDtLst;
            PrmDropDown.DataTextField = "Name";
            PrmDropDown.DataValueField = "Id";
            PrmDropDown.DataBind();
            PrmList.FnNullDropDownList(PrmDropDown, (PrmCaption.Trim().Length > 0 ? PrmCaption : "---Select---"), "0");
            FnSetDropDownValue(PrmDropDown, PrmSetVal);
        }

        public string FnRegisterGridControlMasterScript()
        {
            _strmasterScript = @"
                (function() {
                    window.GridViewControls = window.GridViewControls || {};

                    function getCtrl(wrapperId, suffix) {
                        var wrapper = document.getElementById(wrapperId);
                        if (!wrapper) return null;

                        var el = wrapper.querySelector('[id*=""' + suffix + '""], [name*=""' + suffix + '""], #' + suffix + '');
                        return el;
                    }

                    function getVal(wrap, suffix) {
                        var e = getCtrl(wrap, suffix);
                        return e ? e.value : '';
                    }

                    function txtOnChangeGrid(input, e) {
                            var wrapper = input.closest('.grid-wrapper');
                            if (!wrapper || !wrapper.id) return;

                            var wrapperId = wrapper.id;
                            var cnt = getCtrl(wrapperId, 'HdnCount');
                            var table1 = getCtrl(wrapperId, 'TABLE1');
                            var txtValue = input.value.trim();

                            // ✅ NEW: Close dropdown if textbox is empty
                            if (txtValue === '') {
                                if (table1) table1.style.display = 'none';
                                if (cnt) cnt.value = '0';
                                return;
                            }

                            // ESC key - close dropdown
                            if (e.keyCode === 27) {
                                if (table1) table1.style.display = 'none';
                                if (cnt) cnt.value = '0';
                                return;
                            }

                            // Your existing logic
                            if (cnt && cnt.value === '0') {
                                toggleImgDivGrid(wrapperId);
                            } else {
                                WebServiceCallGrid(1, wrapperId);
                            }
                        }


                    function toggleFromArrow(el, e) {
                        e.preventDefault();
                        var wrapper = el.closest('.grid-wrapper');
                        if (wrapper && wrapper.id) {
                            toggleImgDivGrid(wrapper.id);
                        }
                    }

                                    function toggleImgDivGrid(wrapperId) {
                                        var cnt = getCtrl(wrapperId, 'HdnCount');
                                        var tbl = getCtrl(wrapperId, 'TABLE1');
                                        var wrapper = document.getElementById(wrapperId);
                                        var txtCaption = getCtrl(wrapperId, 'TxtCaption');

                                        if (!cnt || !tbl || !wrapper || !txtCaption) return;

                                        var rect = wrapper.getBoundingClientRect();

                                        // Your exact positioning + styles
                                        tbl.style.cssText = `
                                            min-height: 150px !important;
                                                                max-height: 350px !important;
                                                                position: absolute !important;
                                                                display: block !important;
                                                                background: white !important;
                                                                border-radius: 8px !important;
                                                                box-shadow: 0 12px 40px rgba(0,0,0,0.2) !important;
                                                                overflow: hidden !important;
                                        `;

                                        // ✅ HOVER LISTENERS - Your requirement
                                        var closeTimer;
                                        var hoverTimeout = 150; // ms delay to allow moving to dropdown

                                        var startCloseTimer = function() {
                                            closeTimer = setTimeout(function() {
                                                cnt.value = '0';
                                                tbl.style.display = 'none';
                                                wrapper.removeEventListener('mouseleave', startCloseTimer);
                                                tbl.removeEventListener('mouseenter', clearCloseTimer);
                                            }, hoverTimeout);
                                        };

                                        var clearCloseTimer = function() {
                                            if (closeTimer) {
                                                clearTimeout(closeTimer);
                                                closeTimer = null;
                                            }
                                        };

                                        wrapper.addEventListener('mouseleave', startCloseTimer);
                                        tbl.addEventListener('mouseenter', clearCloseTimer);
                                        tbl.addEventListener('mouseleave', startCloseTimer);

                                        // Your exact logic
                                        if (cnt.value === '0') {
                                            cnt.value = '1';
                                            WebServiceCallGrid(1, wrapperId);
                                        }
                                    }



                    function WebServiceCallGrid(pageNo, wrapperId) {
                        if (typeof WebGridSelection_Grid === 'undefined') {
                            console.error('WebGridSelection_Grid not loaded');
                            return;
                        }
                        var TxtSearch = getCtrl(wrapperId, 'TxtCaption')?.value || '';
                        var hdnRowCount = getVal(wrapperId, 'hdnGridRowCount');

                        var HdnFlag = getVal(wrapperId, 'HdnFlag');
                        var HdnCmpId = getVal(wrapperId, 'HdnCmpId');
                        var HdnBrId = getVal(wrapperId, 'HdnBrId');
                        var HdnFaId = getVal(wrapperId, 'HdnFaId');
                        var HdnUsrId = getVal(wrapperId, 'HdnUserId');
                        var HdnParent = getVal(wrapperId, 'HdnParent');
                        var HdnSubParent = getVal(wrapperId, 'HdnSubParent');
                        var HdnDestCtrl = getVal(wrapperId, 'HdnDestCtrl');
                        console.log(HdnParent);
                        if (HdnParent && HdnParent.length > 0) {
                        var parentCtrl = document.getElementById(HdnParent);
                        if (parentCtrl) HdnParent = parentCtrl.value;
                        console.log(parentCtrl.value);
                           }
                        var txtValue = getCtrl(wrapperId, 'TxtCaption')?.value || '';
                         WebGridSelection_Grid.PopulateGrid_GridView(
        HdnFlag, HdnCmpId, HdnBrId, HdnFaId, HdnUsrId, 
        TxtSearch, wrapperId, HdnParent, HdnSubParent, 
        parseInt(hdnRowCount, 10) || 25, pageNo, HdnDestCtrl,
        function(result) { handleGridResult(result); },
        function(err) { console.error('Service error:', err); }
    );
                    }

                                                function handleGridResult(result) {
                                var wrapperId = result[1];
                                var table1 = getCtrl(wrapperId, 'TABLE1');

                                if (!table1) return;

                                var parser = new DOMParser();
                                var doc = parser.parseFromString(result[0], 'text/xml');

                                var gridNode  = doc.getElementsByTagName('GRID')[0];
                                var pagerNode = doc.getElementsByTagName('PAGER')[0];

                                var gridHtml  = gridNode  ? gridNode.textContent  : '';
                                var pagerHtml = pagerNode ? pagerNode.textContent : '';

                                var dvContent     = table1.querySelector('.dropdown-content');
                                var lblPageNumber = table1.querySelector('.dropdown-footer');

                                if (dvContent)     dvContent.innerHTML     = gridHtml;
                                if (lblPageNumber) lblPageNumber.innerHTML = pagerHtml;

                                // === 1) Row click: select item ===
                                if (dvContent) {
                                    dvContent.querySelectorAll('a[data-id]').forEach(function (a) {
                                        a.onclick = function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var text = this.getAttribute('data-text') || this.textContent.trim();
                                            var id   = this.getAttribute('data-id')   || '';

                                            var txtCaption = getCtrl(wrapperId, 'TxtCaption');
                                            var hdnId      = getCtrl(wrapperId, 'HdnId');
                                            var hdnCount   = getCtrl(wrapperId, 'HdnCount');

                                            if (txtCaption) txtCaption.value = text;
                                            if (hdnId)      hdnId.value      = id;
                                            if (hdnCount)   hdnCount.value   = '0';

                                            table1.style.display = 'none';
                                        };
                                    });
                                }

                                if (lblPageNumber) {
                                    lblPageNumber.querySelectorAll('a').forEach(function (a) {
                                        var onclickAttr = a.getAttribute('onclick') || '';

                                        // Expecting something like: WebServiceCallGrid(2, 'gridWrapper_3')
                                        var match = onclickAttr.match(/WebServiceCallGrid\((\d+)/);
                                        if (!match) return;

                                        var pageNo = parseInt(match[1], 10);
                                        a.removeAttribute('onclick');   // avoid inline handler

                                        a.onclick = function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            WebServiceCallGrid(pageNo, wrapperId); // <=== THIS makes page 2,3,... work
                                        };
                                    });
                                }
                            }


                    window.initGridViewControl = function(wrapperId) {
                        var input = getCtrl(wrapperId, 'TxtCaption');
                        var arrow = document.querySelector('#' + wrapperId + ' .dropdown-srch1');

                        if (input) {
                            input.onkeyup = function(e) { txtOnChangeGrid(this, e); };
                        }
                        if (arrow) {
                            arrow.onclick = function(e) { toggleFromArrow(this, e); };
                        }
                        console.log('✓ Control initialized:', wrapperId);
                    };

                    function initAll() {
                        var wrappers = document.querySelectorAll('.grid-wrapper');
                        wrappers.forEach(function(wrapper, index) {
                            if (!wrapper.id) wrapper.id = 'gridWrapper_' + index;
                            initGridViewControl(wrapper.id);
                        });
                        console.log('✓ All ' + wrappers.length + ' controls initialized');
                    }

                    initAll();
                    if (typeof(Sys) !== 'undefined') {
                        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initAll);
                    }
                })();
             ";
            return _strmasterScript;
        }
        public void FnBindDocumetPathUrl(HyperLink PrmHyLnkFile, string PrmFilUrl, string PrmDocType)
        {
            PrmHyLnkFile.Height = 30;
            PrmHyLnkFile.Width = 30;
            PrmHyLnkFile.ImageHeight = 30;
            PrmHyLnkFile.ImageWidth = 30;
            if (PrmDocType.Equals("VCH_REG"))
            {
                PrmHyLnkFile.ImageUrl = FnVehicleRegIcon();
            }
            else if (PrmDocType.Equals("VCH_IMG"))
            {
                PrmHyLnkFile.ImageUrl = FnVehicleImgIcon();
            }
            else if (PrmDocType.Equals("PRF"))
            {
                PrmHyLnkFile.ImageUrl = FnProfileIcon();
            }
            else if (PrmDocType.Equals("DOC") || PrmDocType.Equals("IMAGE"))
            {
                PrmHyLnkFile.ImageUrl = FnDocFileIcon();
            }
            if (PrmFilUrl.Trim().Length > 0)
            {
                PrmHyLnkFile.Visible = true;
                PrmHyLnkFile.NavigateUrl = PrmFilUrl;
                PrmHyLnkFile.Target = "_blank";
            }
            else
            {
                PrmHyLnkFile.ToolTip = "";
                PrmHyLnkFile.NavigateUrl = "";
            }
        }
        public void FnShowAlert(string message)
        {
            string script = "setTimeout(function(){ alert('" + message.Replace("\\", "\\\\").Replace("'", "\'").Replace("\r", "").Replace("\n", "\\n") + "');}, 100);";
            ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", script, true);
        }
    }
}



//string str = "";
// foreach (Control objPar in Form.FindControl("ContentPlaceHolder1").Controls ) 
// {
//     str = objPar.ClientID.ToString();
// }

//Dim fcName As IFooterCommand = CType(Form.FindControl("ContentPlaceHolder1").FindControl("CtrlFooterCommand"), IFooterCommand) ctl00_ContentPlaceHolder1_CtrlCommand1
//ICommnadInterFace _CtrlCmnd = (ICommnadInterFace)Form.FindControl("ContentPlaceHolder1").FindControl("CtrlCommand");
//_CtrlCmnd.SaveText = "Save";
//_CtrlCmnd.SaveToolTip = "Save";
//_CtrlCmnd.SaveCommandArgument = "NEW";