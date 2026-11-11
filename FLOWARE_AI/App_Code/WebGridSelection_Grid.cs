using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Services;
using System.Text;
using System.Data;
//using FLOWARE_AI.App_Code;

[ScriptService]
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class WebGridSelection_Grid : System.Web.Services.WebService
{
    string _Results = "", _PageResults = "", _strName = "", strCtlr = "";
    ClsWebServiceRecordList ObjCls = new ClsWebServiceRecordList();
    DataTable dtList; 
    DataView dwList = null;
    public WebGridSelection_Grid()
    {
    }
    [WebMethod]
    public string[] PopulateGrid_GridView(string PrmFlag, string PrmCmpId, string PrmBrId, string PrmFaId, string PrmUserId, string PrmName, string PrmCtrlId, string PrmParentId, string PrmSubParentId, int rowCount, int pageNo, string PrmDestCtrls)
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
        if (PrmFlag.Equals("ITM"))
        {
            strSrc = PrmName.Split(' ');
            strName = strSrc[0];
        }
        else
        {
            strName = PrmName.Trim();
        }
        ObjCls.Name = strName.Trim();

        if (PrmFlag.Equals("CUSTC")|| PrmFlag.Equals("ITM") || PrmFlag.Equals("ITMC") ||
            PrmFlag.Equals("ITMC")|| PrmFlag.Equals("UOM"))      //ARYA
        {
            dtList = ObjCls.FnGetGeneralRecordList(PrmFlag);
            if (dtList.Rows.Count <= 0)
            {
                if(strSrc.Length>1) 
                {
                    strName = strSrc[1];
                    dtList = ObjCls.FnGetGeneralRecordList(PrmFlag);
                }
            }
        }
        if (PrmFlag.Equals("ITM") && strSrc.Length > 1)
        {

            dwList = new DataView(dtList);
            dwList.RowFilter = " Name LIKE '%" + strSrc[1].Trim() + "%'";
            dtList = dwList.ToTable();
        }
        //=====================================================================

        if (PrmFlag.Equals("CUSTC")|| PrmFlag.Equals("CUST"))
        {
            _Results = FnGetGridGeneralSingleList(dtList.DefaultView, pageNo, rowCount, PrmCtrlId, PrmDestCtrls, PrmFlag);
        }
        else if (PrmFlag.Equals("UOM"))
        {
            _Results = FnGetGridGeneralSingleListUom(dtList.DefaultView, pageNo, rowCount, PrmCtrlId, PrmDestCtrls, PrmFlag);
        }
        else if (PrmFlag.Equals("ITMC"))
        {
            _Results = FnGetGridGeneralSingleListCode(dtList.DefaultView, pageNo, rowCount, PrmCtrlId, PrmDestCtrls, PrmFlag);
        }
        else if (PrmFlag.Equals("ITM"))
        {
            _Results = FnGetGridGeneralSingleListItem(dtList.DefaultView, pageNo, rowCount, PrmCtrlId, PrmDestCtrls, PrmFlag);
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
    private string FnGetGridGeneralSingleList(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid1'>");
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
                    strResults.Append("<tr class = 'alternatingrowstyle1'>");
                }
                strResults.Append("<td class = 'rowcell'>");
                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListControlValue('" + PrmCtrlId + "','" +  dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";
                
                }
                else
                {
                    _strName = "<a href='#' data-id='" + dv[i]["Id"].ToString() + "' data-text='" + FnReplaceString(dv[i]["Name"].ToString()) + "'>" + dv[i]["Name"].ToString() + "</a>";
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
    private string FnGetGridGeneralSingleListUom(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid1'>");
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
                    strResults.Append("<tr class = 'alternatingrowstyle1'>");
                }
                strResults.Append("<td class = 'rowcell'>");
                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListControlValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + FnReplaceString(dv[i]["Code"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";

                }
                else
                {
                    _strName = "<a href='#' data-id='" + dv[i]["Id"].ToString() + "' data-text='" + FnReplaceString(dv[i]["Name"].ToString()) + "'>" + dv[i]["Name"].ToString() + "</a>";
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
    private string FnGetGridGeneralSingleListItem(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid1'>");
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
                    strResults.Append("<tr class = 'alternatingrowstyle1'>");
                }
                strResults.Append("<td class = 'rowcell'>");
                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListControlValueCode('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Code"].ToString()) + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + dv[i]["UomId"].ToString() + "','" + FnReplaceString(dv[i]["UomName"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Code"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' data-id='" + dv[i]["Id"].ToString() + "' data-text='" + FnReplaceString(dv[i]["Name"].ToString()) + "'>" + dv[i]["Name"].ToString() + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("<td class = 'rowcell'>");
                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListControlValueCode('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Code"].ToString()) + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + dv[i]["UomId"].ToString() + "','" + FnReplaceString(dv[i]["UomName"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' data-id='" + dv[i]["Id"].ToString() + "' data-text='" + FnReplaceString(dv[i]["Name"].ToString()) + "'>" + dv[i]["Name"].ToString() + "</a>";
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
    private string FnGetGridGeneralSingleListCode(DataView dv, int intPageNo, int intRowsPerPage, string PrmCtrlId, string PrmDestCtrl, string PrmFlag)
    {
        StringBuilder strResults = new StringBuilder();
        if (dv.ToTable().Rows.Count > 0)
        {
            strResults.Append("<div class='general-container-grid account-group'>");
            strResults.Append("<table class='general-grid1'>");
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
                    strResults.Append("<tr class = 'alternatingrowstyle1'>");
                }
                strResults.Append("<td class = 'rowcell1'>");
                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListControlValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Code"].ToString()) + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + dv[i]["UomId"].ToString() + "','" + FnReplaceString(dv[i]["UomName"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Code"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' data-id='" + dv[i]["Id"].ToString() + "' data-text='" + FnReplaceString(dv[i]["Name"].ToString()) + "'>" + dv[i]["Name"].ToString() + "</a>";
                }
                strResults.Append(_strName);
                strResults.Append("</td>");
                //strResults.Append("<td>");
                //strResults.Append(dv[i]["Name"].ToString());
                //strResults.Append("</td>");
                strResults.Append("<td class = 'rowcell1'>");
                if (PrmDestCtrl.Trim().Length > 0)
                {
                    _strName = "<a href='#' onClick=\"return FnGetSelectedGeneralListControlValue('" + PrmCtrlId + "','" + dv[i]["Id"].ToString() + "','" + FnReplaceString(dv[i]["Code"].ToString()) + "','" + FnReplaceString(dv[i]["Name"].ToString()) + "','" + dv[i]["UomId"].ToString() + "','" + FnReplaceString(dv[i]["UomName"].ToString()) + "','" + PrmDestCtrl + "');\">" + dv[i]["Name"].ToString() + "</a>";
                }
                else
                {
                    _strName = "<a href='#' data-id='" + dv[i]["Id"].ToString() + "' data-text='" + FnReplaceString(dv[i]["Name"].ToString()) + "'>" + dv[i]["Name"].ToString() + "</a>";
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

    //=============================== PAGER SETTINGS /  COMMON FUNCTIONS ========================================================
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
            strResults.Append("<a href='javascript:;' onclick=\"WebServiceCallGrid(1, '");
            strResults.Append(PrmCtrlId);
            strResults.Append("')\" class='Grdpager'>|<<</a>");
        }

        // **Previous Button**
        if (intPageNo > 1)
        {
            strResults.Append("<a href='javascript:;' onclick=\"WebServiceCallGrid(");
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
                strResults.Append("<a href='javascript:;' onclick=\"WebServiceCallGrid(");
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
            strResults.Append("<a href='javascript:;' onclick=\"WebServiceCallGrid(");
            strResults.Append(intPageNo + 1);
            strResults.Append(", '");
            strResults.Append(PrmCtrlId);
            strResults.Append("')\" class='Grdpager'>></a>");
        }

        // **Last Page Button**
        if (intPageNo < intPageCount)
        {
            strResults.Append("<a href='javascript:;' onclick=\"WebServiceCallGrid(");
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














