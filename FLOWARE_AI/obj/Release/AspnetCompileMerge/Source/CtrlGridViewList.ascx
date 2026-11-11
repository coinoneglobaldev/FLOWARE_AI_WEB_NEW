<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CtrlGridViewList.ascx.cs" Inherits="FLOWARE_AI.CtrlGridViewList" %>

<style>

    .grid-wrapper {
position: relative;
display: inline-block;
width: 100%;
}

.input-wrapper { position: relative; }
.grid-input {
width: 100%; height: 34px; padding: 6px 35px 6px 10px;
border: 1px solid #ddd; border-radius: 4px; font-size: 14px;
transition: border-color 0.2s, box-shadow 0.2s;
}
.grid-input:focus {
border-color: #1f6ea5; box-shadow: 0 0 0 2px rgba(31,110,165,0.1);
outline: none;
}
.dropdown-srch1 {
position: absolute; top: 0; right: 0; width: 34px; height: 37px !important;
background: linear-gradient(145deg, #f8f9fa, #e9ecef);
border: 1px solid #ddd; border-left: 0;
border-top-right-radius: 4px; border-bottom-right-radius: 4px;
cursor: pointer; display: flex; align-items: center; justify-content: center;
transition: background 0.2s;
}
.dropdown-srch1:hover { background: #1f6ea5; }
.dropdown-srch1:hover i { color: white; }
.dropdown-srch1 i { font-size: 14px; color: #666; }

.dropdown-footer {
text-align: center !important;
padding: 10px !important;
background: #f8f9fa !important;
font-size: 12px !important;
color: #666 !important;
border-top: 1px solid #e1e1e1 !important;
}


/* Popup container (unchanged) */
.grid-dropdown {
    position: absolute !important;
    min-width: 380px !important;
    min-height: 150px !important;
    max-height: 350px !important;
    background: #fff !important;
    border: 1px solid #ddd !important;
    border-radius: 8px !important;
    box-shadow: 0 12px 40px rgba(0,0,0,0.2) !important;
    z-index: 999999 !important;
    display: none !important;
    overflow: hidden !important;
    font-size: 13px;
}

/* Scrollable body */
.dropdown-content {
    max-height: 260px !important;
    overflow-y: auto !important;
    min-height: 80px !important;
}

/* Table base */
.dropdown-content .general-grid1 {
    width: 100%;
    border-collapse: collapse;
    table-layout: fixed;
}

/* HEADER – blue bar exactly like first image */
.dropdown-content .general-grid1 thead tr {
    background: #1f6ea5;
    color: #fff;
}
.dropdown-content .general-grid1 thead td {
    font-weight: 600;
    font-size: 12px;
    padding: 8px 10px;
    border-bottom: 1px solid #0f3b5c;
}

/* Equal width columns (header + rows) */
.dropdown-content .general-grid1 td:nth-child(1),
.dropdown-content .general-grid1 thead td:nth-child(1) {
    width: 20% !important;
}
.dropdown-content .general-grid1 td:nth-child(2),
.dropdown-content .general-grid1 thead td:nth-child(2) {
    width: 50%;
}

/* Row styles (light lines, like original) */
.dropdown-content .general-grid1 tbody tr:nth-child(odd) {
    background-color: #ffffff;
}
.dropdown-content .general-grid1 tbody tr:nth-child(even),
.dropdown-content .general-grid1 tbody tr.alternatingrowstyle {
    background-color: #f9fbff;
}
.dropdown-content .general-grid1 tbody tr:hover {
    background-color: #e7f1ff;
}

/* Cells */
.dropdown-content .general-grid1 tbody td {
    padding: 6px 10px;
    border-bottom: 1px solid #f0f0f0;
    font-size: 12px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}
.dropdown-content .general-grid1 td.rowcell a {
    display: block;
    color: #333;
    text-decoration: none;
    font-weight: bold;
    font-size: 13px;
    font-family: sans-serif;
}
.dropdown-content .general-grid1 tbody tr.active-row,
.dropdown-content .general-grid1 tbody tr.active-row:hover {
    background-color: #bcd9f5 !important;
}
</style>
    <script>

        (function () {

            window.GridViewControls = window.GridViewControls || {};
            function getCtrl(wrapperId, suffix) {
                    var wrapper = document.getElementById(wrapperId);
                    if (!wrapper) return null;
                    return wrapper.querySelector(
                    '[id*="' + suffix + '"], [name*="' + suffix + '"], #' + suffix);
            } 
            function getVal(wrapperId, suffix) {
                var e = getCtrl(wrapperId, suffix);
                return e ? e.value : '';
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

                tbl.style.cssText = `
                    min-height:150px !important;
                    max-height:350px !important;
                    position:absolute !important;
                    display:block !important;
                    background:white !important;
                    border-radius:8px !important;
                    box-shadow:0 12px 40px rgba(0,0,0,0.2) !important;
                    overflow:hidden !important;
                    z-index:999999 !important;
                `;

                var closeTimer;
                var hoverTimeout = 150;
                var startCloseTimer = function () {
                    closeTimer = setTimeout(function () {
                        cnt.value = '0';
                        tbl.style.display = 'none';
                        }, hoverTimeout);
                    };

                    var clearCloseTimer = function () {
                        if (closeTimer) {
                            clearTimeout(closeTimer);
                            closeTimer = null;
                        }
                    };
                    wrapper.addEventListener('mouseleave', startCloseTimer);
                    tbl.addEventListener('mouseenter', clearCloseTimer);
                    tbl.addEventListener('mouseleave', startCloseTimer);
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
                var HdnCID = document.getElementById(wrapperId, 'HdnCmpId');

        if (HdnParent && HdnParent.length > 0) {
            var parentCtrl = document.getElementById(HdnParent);
            if (parentCtrl)
            HdnParent = parentCtrl.value;
        }
        WebGridSelection_Grid.PopulateGrid_GridView(HdnFlag, HdnCmpId, HdnBrId, HdnFaId, HdnUsrId, TxtSearch, wrapperId, HdnParent, HdnSubParent, parseInt(hdnRowCount, 10) || 25, pageNo, HdnDestCtrl,
                    function (result) {
                                handleGridResult(result);
                    },
            function (err) {
                    console.error('Service error:', err);
                }
            );
        }
        function handleGridResult(result) {
            var wrapperId = result[1];
            var table1 = getCtrl(wrapperId, 'TABLE1');
            if (!table1) return;
            var w = document.getElementById(wrapperId);
            if (w) w.dataset.activeIdx = -1;
            var parser = new DOMParser();
            var doc = parser.parseFromString(result[0], 'text/xml');
            var gridNode = doc.getElementsByTagName('GRID')[0];
            var pagerNode = doc.getElementsByTagName('PAGER')[0];
            var gridHtml = gridNode ? gridNode.textContent : '';
            var pagerHtml = pagerNode ? pagerNode.textContent : '';
            var dvContent = table1.querySelector('.dropdown-content');
            var lblPageNumber = table1.querySelector('.dropdown-footer');
            if (dvContent) dvContent.innerHTML = gridHtml;
            if (lblPageNumber) lblPageNumber.innerHTML = pagerHtml;

            if (dvContent) {
                dvContent.querySelectorAll('a[data-id]').forEach(function (a) {
                    a.onclick = function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        var text = this.getAttribute('data-text') || this.textContent.trim();
                        var id = this.getAttribute('data-id') || '';
                        var txtCaption = getCtrl(wrapperId, 'TxtCaption');
                        var hdnId = getCtrl(wrapperId, 'HdnId');
                        var hdnCount = getCtrl(wrapperId, 'HdnCount');
                        if (txtCaption) txtCaption.value = text;
                        if (hdnId) hdnId.value = id;
                        if (hdnCount) hdnCount.value = '0';
                        table1.style.display = 'none';
                    };
                });
        }
            if (lblPageNumber) {
                lblPageNumber.querySelectorAll('a').forEach(function (a) {
                    var onclickAttr = a.getAttribute('onclick') || '';
                    var match = onclickAttr.match(/WebServiceCallGrid\((\d+)/);
                    if (!match) return;
                    var pageNo = parseInt(match[1], 10);
                    a.removeAttribute('onclick');
                    a.onclick = function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        WebServiceCallGrid(pageNo, wrapperId);
                    };
                });
        }
            }
            function isOpen(wrapperId) {
                var t = getCtrl(wrapperId, 'TABLE1');
                return !!t && window.getComputedStyle(t).display !== 'none';
            }

            function getRows(wrapperId) {
                var t = getCtrl(wrapperId, 'TABLE1');
                return t ? t.querySelectorAll('.dropdown-content tbody tr') : [];
            }

            function setActive(wrapperId, idx) {
                var wrapper = document.getElementById(wrapperId);
                var rows = getRows(wrapperId);
                if (!wrapper || rows.length === 0) return;

                if (idx < 0) idx = 0;
                if (idx > rows.length - 1) idx = rows.length - 1;

                for (var i = 0; i < rows.length; i++) rows[i].classList.remove('active-row');
                var row = rows[idx];
                row.classList.add('active-row');
                wrapper.dataset.activeIdx = idx;

                // ---- scroll the active row into view ----
                var box = getCtrl(wrapperId, 'TABLE1').querySelector('.dropdown-content');
                var thead = box.querySelector('thead');
                var headH = thead ? thead.offsetHeight : 0;

                var rb = row.getBoundingClientRect();
                var bb = box.getBoundingClientRect();
                var rowTop = rb.top - bb.top + box.scrollTop;   // row position inside scroll content
                var rowBottom = rowTop + rb.height;

                if (idx === 0) {
                    box.scrollTop = 0;                          // show header again
                } else if (rowTop - headH < box.scrollTop) {
                    box.scrollTop = rowTop - headH;             // scroll up
                } else if (rowBottom > box.scrollTop + box.clientHeight) {
                    box.scrollTop = rowBottom - box.clientHeight; // scroll down
                }
            }

            function closeDropdown(wrapperId) {
                var cnt = getCtrl(wrapperId, 'HdnCount');
                var tbl = getCtrl(wrapperId, 'TABLE1');
                var wrapper = document.getElementById(wrapperId);
                if (tbl) tbl.style.display = 'none';
                if (cnt) cnt.value = '0';
                if (wrapper) wrapper.dataset.activeIdx = -1;
            }


            window.initGridViewControl = function (wrapperId) {
                var input = getCtrl(wrapperId, 'TxtCaption');
                var arrow = document.querySelector('#' + wrapperId + ' .dropdown-srch1');

                if (input) {
                    // ---- NEW: keyboard navigation ----
                    input.onkeydown = function (e) {
                        var wrapper = this.closest('.grid-wrapper');
                        if (!wrapper || !wrapper.id) return;
                        var id = wrapper.id;
                        var idx = parseInt(wrapper.dataset.activeIdx || '-1', 10);

                        switch (e.key) {
                            case 'ArrowDown':
                                e.preventDefault();
                                if (!isOpen(id)) toggleImgDivGrid(id);
                                else setActive(id, idx + 1);
                                break;

                            case 'ArrowUp':
                                e.preventDefault();
                                if (isOpen(id)) setActive(id, idx - 1);
                                break;

                            case 'Enter':
                                if (isOpen(id) && idx >= 0) {
                                    e.preventDefault();          // stop form submit / postback
                                    var rows = getRows(id);
                                    var link = rows[idx] && rows[idx].querySelector('a');
                                    if (link) link.click();      // runs your inline onclick
                                    closeDropdown(id);
                                }
                                break;

                            case 'Escape':
                            case 'Tab':
                                closeDropdown(id);
                                break;
                        }
                    };

                    // ---- UPDATED: ignore navigation keys ----
                    input.onkeyup = function (e) {
                        if ([13, 38, 40, 9, 37, 39, 16, 17, 18].indexOf(e.keyCode) > -1) return;

                        var wrapper = this.closest('.grid-wrapper');
                        if (!wrapper || !wrapper.id) return;
                        var wrapperId = wrapper.id;
                        var cnt = getCtrl(wrapperId, 'HdnCount');
                        var table1 = getCtrl(wrapperId, 'TABLE1');

                        if (this.value.trim() === '' || e.keyCode === 27) {
                            closeDropdown(wrapperId);
                            return;
                        }
                        if (cnt && cnt.value === '0') toggleImgDivGrid(wrapperId);
                        else WebServiceCallGrid(1, wrapperId);
                    };
                }

                if (arrow) {
                    arrow.onclick = function (e) { toggleFromArrow(this, e); };
                }
            };
            function initAll() {
                var wrappers = document.querySelectorAll('.grid-wrapper');
            wrappers.forEach(function (wrapper, index) {
            if (!wrapper.id)
            wrapper.id = 'gridWrapper_' + index;
            initGridViewControl(wrapper.id);
        });
            console.log('✓ All ' + wrappers.length + ' controls initialized');
    }
            initAll();
            if (typeof (Sys) !== 'undefined') {
                Sys.WebForms.PageRequestManager
                    .getInstance()
                    .add_endRequest(initAll);
            }

        })();
    </script>

<div class="grid-wrapper" runat="server" id="gridWrapper">
    <div class="input-wrapper">
        <asp:TextBox ID="TxtCaption" runat="server" CssClass="grid-input" autocomplete="off" />
        <span class="dropdown-srch1"><i class="fa fa-angle-down"></i></span>
    </div>
    
    <!-- Hidden Fields -->
    <asp:HiddenField ID="HdnCount" runat="server" Value="0" />
    <asp:HiddenField ID="HdnId" runat="server" Value="0" />
    <asp:HiddenField ID="hdnGridRowCount" runat="server" Value="25" />
    <asp:HiddenField ID="HdnCmpId" runat="server" />
    <asp:HiddenField ID="HdnBrId" runat="server" />
    <asp:HiddenField ID="HdnFaId" runat="server" />
    <asp:HiddenField ID="HdnUserId" runat="server" />
    <asp:HiddenField ID="HdnFlag" runat="server" />
    <asp:HiddenField ID="HdnParent" runat="server" />
    <asp:HiddenField ID="HdnSubParent" runat="server" />
    <asp:HiddenField ID="HdnDestCtrl" runat="server" />
    
    <!-- DROPDOWN TABLE -->
  <!-- ✅ REPLACE your TABLE1 completely -->
<div id="TABLE1" runat="server" class="grid-dropdown" style="display:none;">
    <%--<div class="dropdown-header">
        <%--<span>Name</span>
        <span>Code</span>--%
    </div>--%>
    <div id="dvContent" runat="server" class="dropdown-content">
        <div style="padding: 30px; text-align: center; color: #999;">
            
        </div>
    </div>
    <div id="lblPageNumber" runat="server" class="dropdown-footer">
        Page 1
    </div>
</div>


</div>


