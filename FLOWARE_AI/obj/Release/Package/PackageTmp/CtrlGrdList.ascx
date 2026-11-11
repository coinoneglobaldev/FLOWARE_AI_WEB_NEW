<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CtrlGrdList.ascx.cs" Inherits="FLOWARE_AI.CtrlGrdList" %>
<script language="javascript" type="text/javascript"> 
    var ctrlState = {};

    function txtOnKeyDown(DestCtrl, e) {

        var unicode = e.which || e.keyCode;
        var table = document.getElementById(DestCtrl + "_TABLE1");

        if (unicode == 40) {
            // Down arrow
            e.preventDefault();
            handleDownArrow(DestCtrl);
            return false;
        }

        if (unicode == 38) {
            // Up arrow
            e.preventDefault();
            handleUpArrow(DestCtrl);
            return false;
        }

        if (unicode == 13) {
            // Enter selects the highlighted row
            if (table && table.style.display != "none") {

                e.preventDefault();

                if (!ctrlState[DestCtrl]) {
                    ctrlState[DestCtrl] = {
                        index: -1,
                        rows: []
                    };
                }

                var rows = getClickableRows(table, DestCtrl);
                var index = ctrlState[DestCtrl].index;

                if (rows.length > 0) {
                    if (index < 0 || index >= rows.length) {
                        index = 0;
                    }

                    var link = rows[index].querySelector("a[onclick]");

                    if (link) {
                        link.click();
                    }
                }

                return false;
            }
        }

        if (unicode == 27) {
            e.preventDefault();
            FnOnHide(DestCtrl);
            return false;
        }

        return true;
    }

    /* ================= MAIN KEY HANDLER ================= */
    function txtOnChange(DestCtrl, e) {
        if (!ctrlState[DestCtrl]) {
            ctrlState[DestCtrl] = { index: -1, rows: [] };
        }

        var TxtSearch = document.getElementById(DestCtrl + "_TxtCaption").value;

        if (TxtSearch.length <= 0) {
            document.getElementById(DestCtrl + "_HdnId").value = 0;
        }

        var unicode = e.keyCode || e.charCode;

        if (unicode == 13 ||  // Enter
            unicode == 9 ||  // Tab
            unicode == 27 ||  // Escape
            unicode == 38 ||  // Up
            unicode == 40) {  // Down
            return;
        }

        

        // NORMAL TYPING
        else {

            if (document.getElementById(DestCtrl + "_HdnCount").value == 0) {
                toggleImgDiv(DestCtrl, e);
            } else {
                document.getElementById(DestCtrl + "_TABLE1").style.display = 'block';
                document.getElementById(DestCtrl + "_HdnCount").value = '1';
                WebServiceCall(1, DestCtrl);
            }
        }

    }
    function FnOnHide(DestCtrl) {
        document.getElementById(DestCtrl + "_HdnCount").value = '0';
        document.getElementById(DestCtrl + "_TABLE1").style.display = 'none';
    }
    /* ================= DOWN ARROW ================= */
    function handleDownArrow(DestCtrl) {

        if (!ctrlState[DestCtrl]) {
            ctrlState[DestCtrl] = { index: -1, rows: [] };
        }

        showDropdown(DestCtrl);

        var table = document.getElementById(DestCtrl + "_TABLE1");

        ctrlState[DestCtrl].rows = getClickableRows(table, DestCtrl);
        var rows = ctrlState[DestCtrl].rows;

        if (rows.length > 0) {

            if (ctrlState[DestCtrl].index < 0 || ctrlState[DestCtrl].index >= rows.length) {
                ctrlState[DestCtrl].index = 0;
            } else {
                ctrlState[DestCtrl].index++;
                if (ctrlState[DestCtrl].index >= rows.length) {
                    ctrlState[DestCtrl].index = 0;
                }
            }

            highlightRow(rows[ctrlState[DestCtrl].index], DestCtrl);

            rows[ctrlState[DestCtrl].index].scrollIntoView({
                behavior: 'smooth',
                block: 'center'
            });
        }
    }


    /* ================= UP ARROW ================= */
    function handleUpArrow(DestCtrl) {

        if (!ctrlState[DestCtrl]) {
            ctrlState[DestCtrl] = { index: -1, rows: [] };
        }

        showDropdown(DestCtrl);

        var table = document.getElementById(DestCtrl + "_TABLE1");

        ctrlState[DestCtrl].rows = getClickableRows(table, DestCtrl);
        var rows = ctrlState[DestCtrl].rows;

        if (rows.length > 0) {

            if (ctrlState[DestCtrl].index > 0) {
                ctrlState[DestCtrl].index--;
            } else {
                ctrlState[DestCtrl].index = rows.length - 1;
            }

            highlightRow(rows[ctrlState[DestCtrl].index], DestCtrl);

            rows[ctrlState[DestCtrl].index].scrollIntoView({
                behavior: 'smooth',
                block: 'center'
            });
        }
    }
    function getClickableRows(table, DestCtrl) {

        var rows = [];

        if (!table) return rows;

        var links = table.querySelectorAll('a[onclick]');

        for (var i = 0; i < links.length; i++) {
            var row = links[i].closest('tr');
            if (row && row.style.display !== 'none' && rows.indexOf(row) === -1) {
                rows.push(row);
            }
        }

        return rows;
    }

    /* ================= SHOW DROPDOWN ================= */

    function showDropdown(DestCtrl) {

        var table = document.getElementById(DestCtrl + "_TABLE1");
        var hdnCount = document.getElementById(DestCtrl + "_HdnCount");

        if (!table || !hdnCount) return;

        if (hdnCount.value == "0") {
            table.style.display = "block";
            table.style.position = "absolute";
            hdnCount.value = "1";
            _activeGridCtrl = DestCtrl;

            WebServiceCall(1, DestCtrl);
        } else {
            table.style.display = "block";
        }
    }



    /* ================= HIGHLIGHT ================= */
    function highlightRow(row, DestCtrl) {

        var table = document.getElementById(DestCtrl + "_TABLE1");
        if (!table) return;

        var allRows = table.querySelectorAll('tr.highlight-selected');

        for (var i = 0; i < allRows.length; i++) {
            allRows[i].classList.remove('highlight-selected');
        }

        if (row) {
            row.classList.add('highlight-selected');
        }
    }
    var _activeGridCtrl = null;

    function wireGridClose(ctrlId) {

        var txt = document.getElementById(ctrlId + "_TxtCaption");
        var tbl = document.getElementById(ctrlId + "_TABLE1");
        var btn = document.getElementById(ctrlId + "_DivLst");

        if (!txt || !tbl) return;

        function isInside(e) {
            return txt.contains(e.relatedTarget) ||
                tbl.contains(e.relatedTarget) ||
                (btn && btn.contains(e.relatedTarget));
        }

        function tryClose(e) {
            if (isInside(e)) return;

            tbl.style.display = "none";
            document.getElementById(ctrlId + "_HdnCount").value = "0";
            if (_activeGridCtrl === ctrlId) _activeGridCtrl = null;
        }

        txt.addEventListener("mouseleave", tryClose);
        tbl.addEventListener("mouseleave", tryClose);
        if (btn) btn.addEventListener("mouseleave", tryClose);
    }
    function closeGrid(ctrlId) {
        var tbl = document.getElementById(ctrlId + "_TABLE1");
        var hdn = document.getElementById(ctrlId + "_HdnCount");

        if (tbl) tbl.style.display = "none";
        if (hdn) hdn.value = "0";

        if (_activeGridCtrl === ctrlId) _activeGridCtrl = null;
    }


    function toggleImgDiv(DestCtrlId, e) {
        var unicode = e.charCode ? e.charCode : e.keyCode;
        if ((unicode >= 112 && unicode <= 123) || (unicode >= 44 && unicode <= 46) || (unicode >= 33 && unicode <= 36) || (unicode >= 16 && unicode <= 20) || (unicode >= 144 && unicode <= 145) || (unicode == 91) || (unicode == 93)) { return false; }

        if (_activeGridCtrl && _activeGridCtrl !== DestCtrlId) {
            var oldTbl = document.getElementById(_activeGridCtrl + "_TABLE1");
            var oldHdn = document.getElementById(_activeGridCtrl + "_HdnCount");
            if (oldTbl) oldTbl.style.display = "none";
            if (oldHdn) oldHdn.value = "0";
        }

        var iCount = document.getElementById(DestCtrlId + "_HdnCount").value;
        if (iCount == 0) {
            document.getElementById(DestCtrlId + "_TABLE1").style.display = 'block';
            document.getElementById(DestCtrlId + "_TABLE1").style.position = 'absolute';
            document.getElementById(DestCtrlId + "_HdnCount").value = '1';
            _activeGridCtrl = DestCtrlId;

            //wireGridClose(DestCtrlId);
        }
        else {
            document.getElementById(DestCtrlId + "_HdnCount").value = '0';
            document.getElementById(DestCtrlId + "_TABLE1").style.display = 'none';
        }
        WebServiceCall(1, DestCtrlId);
    }
    function WebServiceCall(pageNo, DestCtrlId) {
        var TxtSearch = document.getElementById(DestCtrlId + "_TxtCaption");
        var hdnAcType = document.getElementById(DestCtrlId + "_HiddenAccType");
        var hdnRowCcunt = document.getElementById(DestCtrlId + "_hdnGridRowCount");

        var HdnFlag = document.getElementById(DestCtrlId + "_HdnFlag").value;
        var HdnCmpId = document.getElementById(DestCtrlId + "_HdnCmpId").value;
        var HdnBrId = document.getElementById(DestCtrlId + "_HdnBrId").value;
        var HdnFaId = document.getElementById(DestCtrlId + "_HdnFaId").value;
        var HdnUsrId = document.getElementById(DestCtrlId + "_HdnUserId").value;
        var HdnParent = document.getElementById(DestCtrlId + "_HdnParent").value;
        var HdnSubParent = document.getElementById(DestCtrlId + "_HdnSubParent").value;
        var HdnDestCtrl = document.getElementById(DestCtrlId + "_HdnDestCtrl").value;
        if (HdnParent.length > 0) {
            HdnParent = document.getElementById(HdnParent).value;
        }
        if (HdnSubParent.length > 0) {
            HdnSubParent = document.getElementById(HdnSubParent).value;
        }
        Web_GridSelection.PopulateGrid(HdnFlag, HdnCmpId, HdnBrId, HdnFaId, HdnUsrId, TxtSearch.value, DestCtrlId, HdnParent, HdnSubParent, hdnRowCcunt.value, pageNo, HdnDestCtrl, Callback);
        if (TxtSearch.value.length <= 0) {
            document.getElementById(DestCtrlId + "_HdnId").value = "0";
            document.getElementById(DestCtrlId + "_TxtCaption").value = "";
        }
    }

    function Callback(result) {

        if (result[0] && result[0].length > 110) {

            ParseData(result[0], result[1]);

            // Reset selection after fresh results arrive
            ctrlState[result[1]] = {
                index: -1,
                rows: []
            };

            var table = document.getElementById(result[1] + "_TABLE1");
            var count = document.getElementById(result[1] + "_HdnCount");

            if (table && count && count.value == "1") {
                table.style.display = "block";
            }
        }
        else {
            document.getElementById(result[1] + "_HdnId").value = "0";
            document.getElementById(result[1] + "_HdnCount").value = "0";
            document.getElementById(result[1] + "_TABLE1").style.display = "none";

            ctrlState[result[1]] = {
                index: -1,
                rows: []
            };
        }
    }

    function ParseData(xml, DestCtrlId) {
        var xmlDoc;
        if (window.DOMParser) {
            var parser = new DOMParser();
            xmlDoc = parser.parseFromString(xml, "text/xml");
        }
        else // Internet Explorer
        {
            xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
            xmlDoc.async = "false";
            xmlDoc.loadXML(xml);
        }
        var Pager = xmlDoc.getElementsByTagName('PAGER')[0];
        var Grid = xmlDoc.getElementsByTagName('GRID')[0];
       /* document.getElementById("ContentPlaceHolder1_TabContainer1_TabPanel1_CtrlGrdPayTerms_TxtHidden").value = GetNodeData(Pager);*/
        document.getElementById(DestCtrlId + "_dvContent").innerHTML = GetNodeData(Grid);
        document.getElementById(DestCtrlId + "_lblPageNumber").innerHTML = GetNodeData(Pager);
        

    }
    function GetNodeData(node) {
        
        return (node.textContent || node.innerText || node.text);
    }
    //=============================================================================== Data Bind
    function FnGetSelectedGeneralListValue(strCtrl, strId, strVal) {
        document.getElementById(strCtrl + "_TxtCaption").value = strVal;
        document.getElementById(strCtrl + "_TABLE1").style.display = 'none';
        document.getElementById(strCtrl + "_HdnCount").value = '0';
        document.getElementById(strCtrl + "_HdnId").value = strId;
        FnClearValues();
        document.getElementById(strCtrl + "_TxtCaption").focus();
    }
    function FnGetSelectedGeneralListControlValue(strCtrl, strId, strVal, strRemarks, strDestCtrl) {
        document.getElementById(strCtrl + "_TxtCaption").value = strVal;
        document.getElementById(strCtrl + "_TABLE1").style.display = 'none';
        document.getElementById(strCtrl + "_HdnCount").value = '0';
        document.getElementById(strCtrl + "_HdnId").value = strId;

        document.getElementById(strDestCtrl).value = strRemarks;
        FnClearValues();
        document.getElementById(strCtrl + "_TxtCaption").focus();
    }
    function FnGetSelectedGeneralItemListControlValue(strCtrl, strId, strVal, strRemarks, strDestCtrl) {

        document.getElementById(strCtrl + "_TxtCaption").value = strVal;
        document.getElementById(strCtrl + "_TABLE1").style.display = 'none';
        document.getElementById(strCtrl + "_HdnCount").value = '0';
        document.getElementById(strCtrl + "_HdnId").value = strId;

        FnClearItemValues();
        document.getElementById(strCtrl + "_TxtCaption").focus();
    }
    function FnGetSelecteItemListValue(strCtrl, strId, strVal, strCode, strUomId, strUom, strTaxId, strTax, strItmGrpId, strItmSubGrpId, strItmBrandId,strAvgWt, strDestCtrl) {
        
        document.getElementById(strCtrl + "_TxtCaption").value = strVal;
        document.getElementById(strCtrl + "_TABLE1").style.display = 'none';
        document.getElementById(strCtrl + "_HdnCount").value = '0';
        document.getElementById(strCtrl + "_HdnId").value = strId;
        document.getElementById(strCtrl + "_HdnCode").value = strCode;

        var strDest = strDestCtrl.split(",");
        document.getElementById(strDest[0]).value = strId;
        document.getElementById(strDest[1]).value = strCode;
        document.getElementById(strDest[2]).value = strUomId;
        document.getElementById(strDest[3]).value = strUom;
        document.getElementById(strDest[4]).value = strTaxId;
        document.getElementById(strDest[5]).value = strTaxId;
        document.getElementById(strCtrl + "_HdnItmGrpId").value = strItmGrpId;
        document.getElementById(strCtrl + "_HdnItmSubGrpId").value = strItmSubGrpId;
        document.getElementById(strCtrl + "_HdnItmBrndId").value = strItmBrandId;
        document.getElementById(strCtrl + "_HdnAvgWt").value = strAvgWt;
        FnClearValues();
        document.getElementById(strCtrl + "_TxtCaption").focus();
    }
    
     //-----------------------------------------ARYA---------------------------------------------------
    function FnGetSelectedItemCodeValueBasic(strCtrl, strId, strVal, strCode, strUomId, strUom, strTaxId, strTaxName, strItmGrpId, strItmSubGrpId, strItmBrandId, strAvgWt, strDestCtrl) {
        document.getElementById(strCtrl + "_TxtCaption").value = strCode;
        document.getElementById(strCtrl + "_TABLE1").style.display = 'none';
        document.getElementById(strCtrl + "_HdnCount").value = '0';
        document.getElementById(strCtrl + "_HdnId").value = strId;
        document.getElementById(strCtrl + "_HdnCode").value = strCode;

        var strDest = strDestCtrl.split(",");
        document.getElementById(strDest[0]).value = strId;
        document.getElementById(strDest[1]).value = strVal;
        document.getElementById(strDest[2]).value = strUomId;
        document.getElementById(strDest[3]).value = strUom;
        document.getElementById(strDest[4]).value = strTaxId;
        document.getElementById(strDest[5]).value = strTaxId;
        document.getElementById(strCtrl + "_HdnItmGrpId").value = strItmGrpId;
        document.getElementById(strCtrl + "_HdnItmSubGrpId").value = strItmSubGrpId;
        document.getElementById(strCtrl + "_HdnItmBrndId").value = strItmBrandId;
        document.getElementById(strCtrl + "_HdnAvgWt").value = strAvgWt;
        FnClearValues();
        document.getElementById(strCtrl + "_TxtCaption").focus();
    }

    function FnGetSelectedCustomerCodeValue(strCtrl, strId, strVal, strCode, strDestCtrl) {

        document.getElementById(strCtrl + "_TxtCaption").value = strCode;
        document.getElementById(strCtrl + "_TABLE1").style.display = 'none';
        document.getElementById(strCtrl + "_HdnCount").value = '0';
        document.getElementById(strCtrl + "_HdnId").value = strId;
        document.getElementById(strCtrl + "_HdnCode").value = strCode;

        var strDest = strDestCtrl.split(",");
        document.getElementById(strDest[0]).value = strId;
        document.getElementById(strDest[1]).value = strVal;

        FnClearValues();
        document.getElementById(strCtrl + "_TxtCaption").focus();
    }
    function FnGetSelectedCustomerGeneralValue(strCtrl, strId, strVal, strCode, strDestCtrl) {
        document.getElementById(strCtrl + "_TxtCaption").value = strVal;
        document.getElementById(strCtrl + "_TABLE1").style.display = 'none';
        document.getElementById(strCtrl + "_HdnCount").value = '0';
        document.getElementById(strCtrl + "_HdnId").value = strId;
        document.getElementById(strCtrl + "_HdnCode").value = strCode;

        var strDest = strDestCtrl.split(",");
        document.getElementById(strDest[0]).value = strId;
        document.getElementById(strDest[1]).value = strCode;

        FnClearValues();
        document.getElementById(strCtrl + "_TxtCaption").focus();
    }
    function FnGetSelectedCustomerGeneralValueAddress(strCtrl, strId, strVal, strCode, strAddress, strDestCtrl) {
        document.getElementById(strCtrl + "_TxtCaption").value = strVal;
        document.getElementById(strCtrl + "_TABLE1").style.display = 'none';
        document.getElementById(strCtrl + "_HdnCount").value = '0';
        document.getElementById(strCtrl + "_HdnId").value = strId;
        document.getElementById(strCtrl + "_HdnCode").value = strCode;

        var strDest = strDestCtrl.split(",");
        document.getElementById(strDest[0]).innerText = strAddress;

        FnClearValues();
        document.getElementById(strCtrl + "_TxtCaption").focus();
    }


   



</script>
<style>
    
.highlight-selected {
    background-color: #3d63be !important;
    color: white !important;
}

.highlight-selected td {
    background-color: #3d63be !important;
    color: white !important;
}

.highlight-selected td a {
    color: white !important;
}

</style>

<table border="0" cellpadding="0" cellspacing="0" style="border-color: black; border-width: 1px;width: 100%; position: relative;">
    <tr>
        <td style="vertical-align: middle; width: 100%;">
            <div class="search-container">
                <asp:TextBox ID="TxtCaption" runat="server" CssClass=""></asp:TextBox>
            </div>
        </td>
        <td>
            <div class="dropdown-srch" id="DivLst" runat="server"><i class="fa fa-angle-down"></i></div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <table id="TABLE1" runat="server" border="0" cellpadding="0" cellspacing="0" class="TblGrdStyle" enableviewstate="false" visible="false" style="right:0px;width: 100%;" >
                <tr>
                    <td colspan="3" valign="top">
                        <div id="devGridList" runat="server" enableviewstate="true" class="grid-container">
                            <div id="dvContent" runat="server" enableviewstate="true">
                                <table id="tblContent" border="0" cellpadding="0" cellspacing="0" class="table table-responsive grid-table" style="width: 100%">
                                    <tr class="rowstyle">
                                        <td></td>
                                    </tr>
                                    <tr class="alternatingrowstyle">
                                        <td></td>
                                    </tr>
                                    <tr class="rowstyle">
                                        <td></td>
                                    </tr>
                                </table>
                            </div>
                            <asp:HiddenField ID="HdnCount" runat="server" Value="0" />
                            <asp:HiddenField ID="HiddenAccType" runat="server" Value="0" />
                            <asp:HiddenField ID="HdnId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnGridRowCount" runat="server" Value="0" />
                            <asp:HiddenField ID="HdnCmpId" runat="server" Value="0" />
                            <asp:HiddenField ID="HdnBrId" runat="server" Value="0" />
                            <asp:HiddenField ID="HdnFaId" runat="server" Value="0" />
                            <asp:HiddenField ID="HdnFlag" runat="server" Value="ACC" />
                            <asp:HiddenField ID="HdnParent" runat="server" Value="" />
                            <asp:HiddenField ID="HdnIsCurrency" runat="server" Value="false" />
                            <asp:HiddenField ID="HdnUserId" runat="server" />
                            <asp:HiddenField ID="HdnDestCtrl" runat="server" />
                            <asp:HiddenField ID="HdnCode" runat="server" />
                            <asp:HiddenField ID="HdnMeterReading" runat="server" />
                            <asp:HiddenField ID="HdnSubParent" runat="server" OnValueChanged="HdnSubParent_ValueChanged" />
                            <asp:HiddenField ID="HdnItmGrpId" runat="server" Value="0" />
                            <asp:HiddenField ID="HdnItmSubGrpId" runat="server" Value="0" />
                            <asp:HiddenField ID="HdnItmBrndId" runat="server" Value="0" />
                            <asp:HiddenField ID="HdnCurrencyId" runat="server" Value="0" />
                            <asp:HiddenField ID="HdnAvgWt" runat="server" Value="0" />
                        </div>
                    </td>
                </tr>
    <tr>                
    <td align="center" colspan="3" class="pagination-container"   valign="middle">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <span class="page-text" id="lblPageNumber" runat="server" style="font-size: 10px;">Page 1</span>
        </ContentTemplate>
    </asp:UpdatePanel>
</td>
    </tr>
</table>
</td>
    </tr>
    </table>