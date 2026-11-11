using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Services;
using System.Text;
using System.Data;

[ScriptService]
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Web_GridSelection : System.Web.Services.WebService
{
    string _Results = "", _PageResults = "", _strName = "", strCtlr = "";
    ClsWebServiceRecordList ObjCls = new ClsWebServiceRecordList();
    DataTable dtList;
    DataView dwList = null;
    public Web_GridSelection()
    {
    }

    [WebMethod]
    public string[] PopulateGrid(string PrmFlag, string PrmCmpId, string PrmBrId, string PrmFaId, string PrmUserId, string PrmName, string PrmCtrlId, string PrmParentId, string PrmSubParentId, int rowCount, int pageNo, string PrmDestCtrls)
    {
        ObjCls.CompanyId = ObjCls.FnIsNumeric(ObjCls.DecryptQueryString(PrmCmpId));
        ObjCls.BranchId = ObjCls.FnIsNumeric(ObjCls.DecryptQueryString(PrmBrId));
        ObjCls.FaId = ObjCls.FnIsNumeric(ObjCls.DecryptQueryString(PrmFaId));
        ObjCls.UserId = ObjCls.FnIsNumeric(ObjCls.DecryptQueryString(PrmUserId));
        ObjCls.UpdateDate = DateTime.Now;
        ObjCls.ControlId = PrmCtrlId.Trim();
        ObjCls.ParentId = ObjCls.FnIsNumeric(PrmParentId);
        ObjCls.ID = ObjCls.FnIsNumeric(PrmSubParentId);
        string strName = "";
        string[] strSrc = null;
        if (PrmFlag.Equals("ITM")|| PrmFlag.Equals("CUST"))
        {
            strSrc = PrmName.Split(' ');
            strName = strSrc[0];
        }
        else
        {
            strName = PrmName.Trim();
        }
        ObjCls.Name = strName.Trim();
        if (PrmFlag.Equals("CUST") || PrmFlag.Equals("ITM") || PrmFlag.Equals("CUSTC") || PrmFlag.Equals("ITMC") || PrmFlag.Equals("UOM"))      //ARYA
        {
            dtList = ObjCls.FnGetGeneralRecordList(PrmFlag);
        }
        if ((PrmFlag.Equals("ITM")) && strSrc.Length>1)
        {
            dwList = new DataView(dtList);
            dwList.RowFilter = " Name LIKE '%" + strSrc[1].Trim() + "%'";
            dtList = dwList.ToTable();
        }
        if (PrmFlag.Equals("CUST") && strSrc.Length > 1)
        {
            dwList = new DataView(dtList);
            dwList.RowFilter = " Name LIKE '%" + strSrc[1].Trim() + "%'";
            dtList = dwList.ToTable();
        }
        //=====================================================================
        if (PrmFlag.Equals("CUSTC"))
        {
            _Results = FnGetGridGeneralCodeFillList(dtList.DefaultView, pageNo, rowCount, PrmCtrlId, PrmDestCtrls, PrmFlag);
        }

        else if (PrmFlag.Equals("ITM"))
        {
            _Results = FnGetGridItemList(dtList.DefaultView, pageNo, rowCount, PrmCtrlId, PrmDestCtrls, PrmFlag);
        }
        else if (PrmFlag.Equals("ITMC"))
        {
            _Results = FnGetGridItemCodeList(dtList.DefaultView, pageNo, rowCount, PrmCtrlId, PrmDestCtrls, PrmFlag);
        }
        else if (PrmFlag.Equals("CUST"))
        {
            _Results = FnGetGridCustomerGeneralList(dtList.DefaultView, pageNo, rowCount, PrmCtrlId, PrmDestCtrls, PrmFlag);
        }
        else
        {
            _Results = FnGetGridGeneralSingleList(dtList.DefaultView, pageNo, rowCount, PrmCtrlId, PrmDestCtrls, PrmFlag);
        }

        _PageResults = (dtList.Rows.Count > 0 ? FnGetPager(dtList.Rows.Count, pageNo, rowCount, PrmCtrlId) : "");
        StringBuilder strResponse = new StringBuilder();
        strResponse.Append("<RESPONSE><VALUES><ERROR>False</ERROR><PAGER><![CDATA[");
        strResponse.Append(_PageResults);
        strResponse.Append("]]></PAGER><GRID><![CDATA[");
        strResponse.Append(_Results);
        strResponse.Append("]]></GRID></VALUES></RESPONSE>");
        string[] srcResult = new string[]
        {
            strResponse.ToString(),
            PrmCtrlId.ToString()
        };
        return srcResult;
    }
    //============================================ GRID TABLE FUNCTIONS =======================
   
    private string FnGetGridGeneralSingleList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid'>");
            strResults.Append("<thead><tr>"
                                + "<td scope='col'>Name</td></tr></thead>");
            int intStartIndex, intEndIndex, intRowCount;
            intRowCount = dv.Count;
            intStartIndex = ((intPageNo - 1) * intRowsPerPage);
            intEndIndex = (intStartIndex + intRowsPerPage) - 1;
            if (intRowCount <= intEndIndex)
            {
                intEndIndex = intRowCount - 1;
            }
            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                if (i % 2 == 0)
                {
                    strResults.Append("");
                }
                else
                {
                    strResults.Append("<tr class = 'alternatingrowstyle'>");
                }
                strResults.Append("<td class = 'rowcell'>");
                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListControlValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Remarks"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Name"].ToString() + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");               


                strResults.Append("</tr>");
            }
            strResults.Append("</table>");
            strResults.Append("</div>");
        }
        else
        {
            strResults.Append("");
        }
        return strResults.ToString();
    }
   
    private string FnGetGridGeneralCodeFillList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strCtlr = PrmDestCtrl.Trim();
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid'>");
            strResults.Append("<thead><tr>"
                                + "<td scope='col'>Code</td>"
                                + "<td scope='col'>Name</td></tr></thead>");

            int intStartIndex, intEndIndex, intRowCount;
            intRowCount = dv.Count;
            intStartIndex = ((intPageNo - 1) * intRowsPerPage);
            intEndIndex = (intStartIndex + intRowsPerPage) - 1;
            if (intRowCount <= intEndIndex)
            {
                intEndIndex = intRowCount - 1;
            }
            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                if (i % 2 == 0)
                {
                    strResults.Append("");
                }
                else
                {
                    strResults.Append("<tr class = 'alternatingrowstyle'>");
                }
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedCustomerCodeValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + PrmDestCtrl + "');\">" + dv[i]["Code"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Code"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");

                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedCustomerCodeValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Code"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");






                //strResults.Append("<td>");
                //strResults.Append(dv[i]["Name"].ToString());
                //strResults.Append("</td>");
                strResults.Append("</tr>");
            }
            strResults.Append("</table>");
            strResults.Append("</div>");
        }
        else
        {
            strResults.Append("");
        }
        return strResults.ToString();
    }
    private string FnGetGridItemCodeList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strCtlr = PrmDestCtrl.Trim();
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid'>");
            strResults.Append("<thead><tr>"
                                + "<td scope='col'>Code</td>"
                                + "<td scope='col'>Name</td></tr></thead>");

            int intStartIndex, intEndIndex, intRowCount;
            intRowCount = dv.Count;
            intStartIndex = ((intPageNo - 1) * intRowsPerPage);
            intEndIndex = (intStartIndex + intRowsPerPage) - 1;
            if (intRowCount <= intEndIndex)
            {
                intEndIndex = intRowCount - 1;
            }
            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                if (i % 2 == 0)
                {
                    strResults.Append("");
                }
                else
                {
                    strResults.Append("<tr class = 'alternatingrowstyle'>");
                }
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    //for large pages
                    //_strName = "<a href='#' onClick=\"return FnGetSelectedItemCodeValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["UomId"].ToString()) + "','" + FnReplaceString(dv[i]["UomName"].ToString()) + "','" + FnReplaceString(dv[i]["TaxId"].ToString()) + "','" + FnReplaceString(dv[i]["TaxName"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["GroupId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["SubGrpId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["BrandId"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Code"].ToString() + "</a>";
                    _strName = "<a href='#' onClick=\"return FnGetSelectedItemCodeValueBasic('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["UomId"].ToString()) + "','" + FnReplaceString(dv[i]["UomName"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["TaxId"].ToString()) + "','" + FnReplaceString(dv[i]["TaxName"].ToString()).ToString() + "','" + ObjCls.FnIsNumeric(dv[i]["GroupId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["SubGrpId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["BrandId"].ToString()) + "','" + ObjCls.FnIsDouble(dv[i]["AvgWt"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Code"].ToString() + "</a>";

                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Code"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    //for large pages
                    //_strName = "<a href='#' onClick=\"return FnGetSelectedItemCodeValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["UomId"].ToString()) + "','" + FnReplaceString(dv[i]["UomName"].ToString()) + "','" + FnReplaceString(dv[i]["TaxId"].ToString()) + "','" + FnReplaceString(dv[i]["TaxName"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["GroupId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["SubGrpId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["BrandId"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Code"].ToString() + "</a>";
                    _strName = "<a href='#' onClick=\"return FnGetSelectedItemCodeValueBasic('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["UomId"].ToString()) + "','" + FnReplaceString(dv[i]["UomName"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["TaxId"].ToString()) + "','" + FnReplaceString(dv[i]["TaxName"].ToString()).ToString() + "','" + ObjCls.FnIsNumeric(dv[i]["GroupId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["SubGrpId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["BrandId"].ToString()) + "','" + ObjCls.FnIsDouble(dv[i]["AvgWt"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";

                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Code"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                //strResults.Append("<td>");
                //strResults.Append(dv[i]["Name"].ToString());
                //strResults.Append("</td>");
                //strResults.Append("<td>");
                //strResults.Append(dv[i]["UomName"].ToString());
                //strResults.Append("</td>");
                //strResults.Append("<td>");
                //strResults.Append(dv[i]["TaxName"].ToString());
                //strResults.Append("</td>");
                strResults.Append("</tr>");
            }
            strResults.Append("</table>");
            strResults.Append("</div>");
        }
        else
        {
            strResults.Append("");
        }
        return strResults.ToString();
    }
    
    private string FnGetGridItemList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strCtlr = PrmDestCtrl.Trim();
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid'>");
            strResults.Append("<thead><tr>"
                                + "<td scope='col'>Name</td>"
                                + "<td scope='col'>Code</td></tr></thead>");
            

            int intStartIndex, intEndIndex, intRowCount;
            intRowCount = dv.Count;
            intStartIndex = ((intPageNo - 1) * intRowsPerPage);
            intEndIndex = (intStartIndex + intRowsPerPage) - 1;
            if (intRowCount <= intEndIndex)
            {
                intEndIndex = intRowCount - 1;
            }
            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                if (i % 2 == 0)
                {
                    strResults.Append("");
                }
                else
                {
                    strResults.Append("<tr class = 'alternatingrowstyle'>");
                }
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelecteItemListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["UomId"].ToString()) + "','" + FnReplaceString(dv[i]["UomName"].ToString()) + "','" + FnReplaceString(dv[i]["TaxId"].ToString()) + "','" + FnReplaceString(dv[i]["TaxName"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["GroupId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["SubGrpId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["BrandId"].ToString()) + "','" + ObjCls.FnIsDouble(dv[i]["AvgWt"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Name"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelecteItemListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["UomId"].ToString()) + "','" + FnReplaceString(dv[i]["UomName"].ToString()) + "','" + FnReplaceString(dv[i]["TaxId"].ToString()) + "','" + FnReplaceString(dv[i]["TaxName"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["GroupId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["SubGrpId"].ToString()) + "','" + ObjCls.FnIsNumeric(dv[i]["BrandId"].ToString()) + "','" + ObjCls.FnIsDouble(dv[i]["AvgWt"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Code"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Name"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                //strResults.Append("<td>");
                //strResults.Append(dv[i]["Code"].ToString());
                //strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(dv[i]["UomName"].ToString());
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(dv[i]["TaxName"].ToString());
                strResults.Append("</td>");
                strResults.Append("</tr>");
            }
            strResults.Append("</table>");
            strResults.Append("</div>");
        }
        else
        {
            strResults.Append("");
        }
        return strResults.ToString();
    }
    private string FnGetGridCustomerGeneralList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strCtlr = PrmDestCtrl.Trim();
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid'>");
            strResults.Append("<thead><tr>"
                                + "<td scope='col'>Code</td>"
                                + "<td scope='col'>Name</td></tr></thead>");

            int intStartIndex, intEndIndex, intRowCount;
            intRowCount = dv.Count;
            intStartIndex = ((intPageNo - 1) * intRowsPerPage);
            intEndIndex = (intStartIndex + intRowsPerPage) - 1;
            if (intRowCount <= intEndIndex)
            {
                intEndIndex = intRowCount - 1;
            }
            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                if (i % 2 == 0)
                {
                    strResults.Append("");
                }
                else
                {
                    strResults.Append("<tr class = 'alternatingrowstyle'>");
                }
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    //for large pages
                    _strName = "<a href='#' onClick=\"return FnGetSelectedCustomerGeneralValueAddress('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["PAddress"].ToString()).ToString() + "','" + PrmDestCtrl + "');\">" + dv[i]["Code"].ToString() + "</a>";
                    //_strName = "<a href='#' onClick=\"return FnGetSelectedItemCodeValueBasic('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + PrmDestCtrl + "');\">" + dv[i]["Code"].ToString() + "</a>";

                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Code"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedCustomerGeneralValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";

                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Name"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                //strResults.Append("<td>");
                //strResults.Append(dv[i]["Name"].ToString());
                //strResults.Append("</td>");
                strResults.Append("</tr>");
            }
            strResults.Append("</table>");
            strResults.Append("</div>");
        }
        else
        {
            strResults.Append("");
        }
        return strResults.ToString();
    }

    private string FnGetGridAgentList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strCtlr = PrmDestCtrl.Trim();
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid'>");
            strResults.Append("<thead><tr>"
                                + "<td scope='col'>Name</td>"
                                + "<td scope='col'>Code</td></tr></thead>");

            int intStartIndex, intEndIndex, intRowCount;
            intRowCount = dv.Count;
            intStartIndex = ((intPageNo - 1) * intRowsPerPage);
            intEndIndex = (intStartIndex + intRowsPerPage) - 1;
            if (intRowCount <= intEndIndex)
            {
                intEndIndex = intRowCount - 1;
            }
            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                if (i % 2 == 0)
                {
                    strResults.Append("");
                }
                else
                {
                    strResults.Append("<tr class = 'alternatingrowstyle'>");
                }
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelecteAccountLedgerListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["PAddress"].ToString()) + "','" + dv[i]["CurrencyId"].ToString() + "','" + FnReplaceString(dv[i]["Currency"].ToString()) + "','" + dv[i]["ConVal"].ToString() + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedAccGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + dv[i]["CurrencyId"].ToString() + "','" + FnReplaceString(dv[i]["Currency"].ToString()) + "','" + dv[i]["ConVal"].ToString() + "');\">" + dv[i]["Name"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(dv[i]["Code"].ToString());
                strResults.Append("</td>");
                strResults.Append("</tr>");
            }
            strResults.Append("</table>");
            strResults.Append("</div>");
        }
        else
        {
            strResults.Append("");
        }
        return strResults.ToString();
    }
    private string FnGetGridEnquiryCompanyList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strCtlr = PrmDestCtrl.Trim();
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid'>");
            strResults.Append("<thead><tr>"
                                + "<td scope='col'>Name</td>"
                                + "<td scope='col'>Code</td>"
                                + "<td scope='col'>Contact No</td></tr></thead>");

            int intStartIndex, intEndIndex, intRowCount;
            intRowCount = dv.Count;
            intStartIndex = ((intPageNo - 1) * intRowsPerPage);
            intEndIndex = (intStartIndex + intRowsPerPage) - 1;
            if (intRowCount <= intEndIndex)
            {
                intEndIndex = intRowCount - 1;
            }
            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                if (i % 2 == 0)
                {
                    strResults.Append("");
                }
                else
                {
                    strResults.Append("<tr class = 'alternatingrowstyle'>");
                }
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelecteCompanyListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["PAddress"].ToString()) + "','" + FnReplaceString(dv[i]["Email"].ToString()) + "','" + FnReplaceString(dv[i]["ContactNo"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Name"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(dv[i]["Code"].ToString());
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(dv[i]["ContactNo"].ToString());
                strResults.Append("</td>");
                strResults.Append("</tr>");
            }
            strResults.Append("</table>");
            strResults.Append("</div>");
        }
        else
        {
            strResults.Append("");
        }
        return strResults.ToString();
    }
    private string FnGetGridEnquiryCustomerList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strCtlr = PrmDestCtrl.Trim();
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid'>");
            strResults.Append("<thead><tr>"
                                + "<td scope='col'>Name</td>"
                                + "<td scope='col'>Contact No</td>"
                                + "<td scope='col'>Last Enq. No</td></tr></thead>");

            int intStartIndex, intEndIndex, intRowCount;
            intRowCount = dv.Count;
            intStartIndex = ((intPageNo - 1) * intRowsPerPage);
            intEndIndex = (intStartIndex + intRowsPerPage) - 1;
            if (intRowCount <= intEndIndex)
            {
                intEndIndex = intRowCount - 1;
            }
            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                if (i % 2 == 0)
                {
                    strResults.Append("");
                }
                else
                {
                    strResults.Append("<tr class = 'alternatingrowstyle'>");
                }
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelecteAccountLedgerListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()).ToString() + "','" + FnReplaceString(dv[i]["PAddress"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "');\">" + dv[i]["Name"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(dv[i]["ContactNo"].ToString());
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(dv[i]["Code"].ToString());
                strResults.Append("</td>");
                strResults.Append("</tr>");
            }
            strResults.Append("</table>");
            strResults.Append("</div>");
        }
        else
        {
            strResults.Append("");
        }
        return strResults.ToString();
    }
    private string FnGetGridPurchaseReceiptList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strCtlr = PrmDestCtrl.Trim();
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid'>");
            strResults.Append("<thead><tr>"
                                + "<td scope='col'>Ref No</td>"
                                + "<td scope='col'>Date</td>"
                                + "<td scope='col'>Bill No</td>"
                                + "<td scope='col'>Bill Date</td>"
                                + "<td scope='col'>GrandTotal</td></tr></thead>");

            int intStartIndex, intEndIndex, intRowCount;
            intRowCount = dv.Count;
            intStartIndex = ((intPageNo - 1) * intRowsPerPage);
            intEndIndex = (intStartIndex + intRowsPerPage) - 1;
            if (intRowCount <= intEndIndex)
            {
                intEndIndex = intRowCount - 1;
            }
            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                if (i % 2 == 0)
                {
                    strResults.Append("");
                }
                else
                {
                    strResults.Append("<tr class = 'alternatingrowstyle'>");
                }
                strResults.Append("<td class = 'rowcell'>");

                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectePurchaseReceiptListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["ReffNo"].ToString()) + "','" + dv[i]["AutoId"].ToString().ToString() + "','" + FnReplaceString(dv[i]["TrDate"].ToString()) + "','" + FnReplaceString(dv[i]["OptRefNo"].ToString()) + "','" + FnReplaceString(dv[i]["OptDate"].ToString()) + "','" + dv[i]["NetTotal"].ToString().ToString() + "','" + dv[i]["TaxTotal"].ToString() + "','" + dv[i]["GrandTotal"].ToString() + "','" + PrmDestCtrl + "');\">" + dv[i]["ReffNo"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["ReffNo"].ToString()) + "');\">" + dv[i]["ReffNo"].ToString().Replace("'", " ") + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(FnGetDateString(dv[i]["TrDate"].ToString()));
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(dv[i]["OptRefNo"].ToString());
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(FnGetDateString(dv[i]["OptDate"].ToString()));
                strResults.Append("</td>");
                strResults.Append("<td>");
                strResults.Append(dv[i]["GrandTotal"].ToString());
                strResults.Append("</td>");
                strResults.Append("</tr>");
            }
            strResults.Append("</table>");
            strResults.Append("</div>");
        }
        else
        {
            strResults.Append("");
        }
        return strResults.ToString();
    }
    // =============================== PAGER SETTINGS /  COMMON FUNCTIONS ========================================================
    public string FnReplaceString(string PrmValue)
    {
        string str = PrmValue.Trim();
        return HttpUtility.HtmlEncode(PrmValue).Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ");
    }
    private string FnGetPager(int intRowCount, int intPageNo, int intRowsPerPage, string PrmCtrlId)
    {
        StringBuilder strResults = new StringBuilder();

        // Calculate Total Pages
        int intPageCount = (int)Math.Ceiling((double)intRowCount / intRowsPerPage);

        // Validate Page Number
        if (intPageNo > intPageCount || intPageNo < 1)
        {
            intPageNo = 1;
        }

        // Pagination Display Settings
        int intPagesToShow = 4; // Display 4 pages at a time
        int intStartPage = Math.Max(1, intPageNo - (intPagesToShow / 2));
        int intEndPage = Math.Min(intPageCount, intStartPage + intPagesToShow - 1);

        // **First Page Button**
        if (intPageNo > 1)
        {
            strResults.Append("<a href='javascript:;' onclick=\"WebServiceCall(1, '");
            strResults.Append(PrmCtrlId);
            strResults.Append("')\" class='Grdpager'>|<<</a>");
        }

        // **Previous Button**
        if (intPageNo > 1)
        {
            strResults.Append("<a href='javascript:;' onclick=\"WebServiceCall(");
            strResults.Append(intPageNo - 1);
            strResults.Append(", '");
            strResults.Append(PrmCtrlId);
            strResults.Append("')\" class='Grdpager'><</a>");
        }

        // **Page Number Links**
        for (int k = intStartPage; k <= intEndPage; k++)
        {
            if (k == intPageNo)
            {
                strResults.Append("<span class='GrdpagerActive'>");
                strResults.Append(k);
                strResults.Append("</span>");
            }
            else
            {
                strResults.Append("<a href='javascript:;' onclick=\"WebServiceCall(");
                strResults.Append(k);
                strResults.Append(", '");
                strResults.Append(PrmCtrlId);
                strResults.Append("')\" class='GrdpagerInActive'>");
                strResults.Append(k);
                strResults.Append("</a>");
            }
        }

        // **Next Button**
        if (intPageNo < intPageCount)
        {
            strResults.Append("<a href='javascript:;' onclick=\"WebServiceCall(");
            strResults.Append(intPageNo + 1);
            strResults.Append(", '");
            strResults.Append(PrmCtrlId);
            strResults.Append("')\" class='Grdpager'>></a>");
        }

        // **Last Page Button**
        if (intPageNo < intPageCount)
        {
            strResults.Append("<a href='javascript:;' onclick=\"WebServiceCall(");
            strResults.Append(intPageCount);
            strResults.Append(", '");
            strResults.Append(PrmCtrlId);
            strResults.Append("')\" class='Grdpager'>>>|</a>");
        }

        return strResults.ToString();
    }
    public string FnGetDateString(object PrmVal)
    {
        string _strDateVal = "";
        if (PrmVal.ToString().Trim().Length > 0)
        {
            _strDateVal = (Convert.ToDateTime(PrmVal).ToString("dd/MMM/yyyy") == "01/Jan/1800" ? "" : Convert.ToDateTime(PrmVal).ToString("dd/MMM/yyyy"));
        }
        return _strDateVal;
    }

}

