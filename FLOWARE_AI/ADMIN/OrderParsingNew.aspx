<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master"AutoEventWireup="true" StylesheetTheme="SkinFile" Theme="SkinFile" Async="true" CodeBehind="OrderParsingNew.aspx.cs" Inherits="FLOWARE_AI.ADMIN.OrderParsingNew" MaintainScrollPositionOnPostback="true" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/CtrlGrdList.ascx" TagPrefix="uc1" TagName="CtrlGrdList" %>
<%@ Register Src="~/CtrlGridViewList.ascx" TagPrefix="uc2" TagName="CtrlGridViewList" %>
<%@ Register Src="~/CtrlDate.ascx" TagPrefix="uc1" TagName="CtrlDate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%--     <link rel="stylesheet" href="css/export.css">--%>
    <script type="text/javascript" src="Javascript/OrderParsing.js"></script>
     <section class="content-section">
        <div class="container">
            <!-- Export Section -->
            <div class="export-section">
                <div class="row">
                    <asp:HiddenField ID="HdnMailId" runat="server" />
                    <asp:HiddenField ID="HdnMailFrom" runat="server" />
                    <asp:HiddenField ID="HdnType" runat="server" />
                    <asp:HiddenField ID="HdnTokenNo" runat="server" />
                    <asp:HiddenField ID="HdnScrollPos" runat="server" />
                    <asp:HiddenField ID="HdnMailInbIds" runat="server" />
                    <asp:HiddenField ID="HdnMailAccIds" runat="server" />
                    <asp:HiddenField ID="HdnConfirmZeroQty" runat="server" Value="" />
                    <!-- Column 2: Tabs -->
                    <div class="col-lg-4 col-md-12">
                        <div class="export-tabs">
                            <h5><i class="fas fa-file-alt me-2"></i>Emails,Upload Or Paste Orders</h5>
                            <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="ajax-tab-container_export" OnActiveTabChanged="TabContainer1_ActiveTabChanged" AutoPostBack="True">
                                <!-- 🌟 MAIL PANEL -->
                                <ajaxToolkit:TabPanel ID="TabPanel4" runat="server">
                                    <HeaderTemplate>
                                        Mails
                                    </HeaderTemplate>
                                    <ContentTemplate>                                         
                                        <div class="tab-pane-content p-3">
                                            <div id="mailScrollDiv" class="export-section_mail_listing" style="max-height: 82vh;overflow:scroll;">                                              
                                              <asp:GridView ID="GrdVwRecords" SkinID="GrdVwMaster" runat="server" DataKeyNames="Id" Style="width: 100%;" OnSelectedIndexChanged="GrdVwRecords_SelectedIndexChanged" OnRowDataBound="GrdVwRecords_RowDataBound" OnPageIndexChanging="GrdVwRecords_PageIndexChanging"  >
                                                    <SelectedRowStyle BackColor="#f0f0f0" Font-Bold="true" BorderStyle="Solid" BorderWidth="3px" BorderColor="#8e44ad" />
                                                    <Columns>
                                                       <asp:BoundField DataField="SlNo" HeaderText="Sl" ItemStyle-Width="25px" HeaderStyle-Width="25px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                                                       <asp:TemplateField HeaderText="" ItemStyle-Width="25px" HeaderStyle-Width="35px">
                                                            <HeaderTemplate>                                                                
                                                                 <asp:Button ID="BtnDelete" runat="server" CssClass="mini-delete-btn btn btn-delete redcolor fa" Text="&#xf1f8;" OnClick="BtnDelete_Click" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="ChkSelect" runat="server" CssClass="mini-chk" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="From mail/Subject">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="HdnTokenId" runat="server" Value='<%# Eval("TokenId") %>' />
                                                                <asp:HiddenField ID="HdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                                <asp:HiddenField ID="HdnMailInbId" runat="server" Value='<%# Eval("UniqueInboxId") %>' />
                                                                <asp:HiddenField ID="HdnMailAccId" runat="server" Value='<%# Eval("MailAccId") %>' />
                                                                <asp:LinkButton ID="LnkFromMail" runat="server" CommandName="SELECT" Text='<%#FnGetEmailDateTime(Eval("FromMail").ToString(),Eval("MailDate").ToString()) %>' CssClass="text-start email-link" Width="600px" ForeColor="#cd456d" OnClientClick="saveMailScroll();"></asp:LinkButton>
                                                                <asp:LinkButton ID="LnkName" runat="server" CommandName="SELECT" CommandArgument='<%# Eval("Id") %>' CssClass="text-start  email-link"  Text='<%# Eval("SubjectMail") %>' Width="600px" OnClientClick="saveMailScroll();"></asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>                                                      
                                                    </Columns>
                                                    <PagerStyle BackColor="#87B0CC" CssClass="pag_btns" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel ID="TabPanel5" runat="server">
                                    <HeaderTemplate>
                                        Read Mail
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="tab-pane-content p-3" style="max-height: 85vh;overflow:scroll;">
                                            <asp:Label ID="LblSubject" runat="server" Text=""></asp:Label>
                                            <asp:Literal ID="LitMailBody" runat="server"></asp:Literal>
                                            <asp:HyperLink ID="HyLnkDoc" runat="server" CssClass="upload-field-img" Visible="false" Text="Open PDF"  ImageHeight="40" ImageWidth="40"></asp:HyperLink>
                                            <asp:HiddenField ID="HdnPDFMailId" runat="server" />
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <!-- 🌟 TEXT PANEL -->
                                <ajaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>
                                        Text
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="tab-pane-content p-3" style="max-height: 85vh;">
                                           <asp:TextBox runat="server" ID="TxtAreainput" TextMode="MultiLine" height="600px" Style="font-size: 25px;"  placeholder="Enter your text here..."></asp:TextBox>
                                            <asp:LinkButton ID="LnkTxtSubmit" runat="server" CssClass="btn submit-btnsave" Text="<i class='fas fa-paper-plane'></i> AI Convert" OnClick="LnkTxtSubmit_Click"></asp:LinkButton>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>

                                <!-- 🌟 PDF PANEL -->
                                <ajaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>
                                        PDF
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="tab-pane-content p-3" style="max-height: 85vh;">
                                            <ajaxToolkit:AsyncFileUpload ID="FileUploadDoc" runat="server" class="file-input-label" OnUploadedComplete="FileUploadDoc_UploadedComplete" />
                                            <asp:HiddenField ID="HdnFilePath" runat="server" />
                                            <asp:LinkButton ID="LnkPDFSubmit" runat="server" CssClass="btn submit-btnsave" Text="<i class='fas fa-paper-plane'></i> AI Convert" OnClick="LnkPDFSubmit_Click"></asp:LinkButton>
                                            <asp:Label ID="LblResult" runat="server" CssClass="text-danger mt-2"> </asp:Label>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>

                                <!-- 🌟 LINK PANEL -->
                                <ajaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>
                                        Link
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="tab-pane-content p-3" style="max-height: 85vh;">
                                            <asp:TextBox runat="server" ID="TxtLink" Rows="22" placeholder="Enter link or URL here..." TextMode="MultiLine"> </asp:TextBox>
                                            <asp:LinkButton ID="LnkSubmit" runat="server" CssClass="btn submit-btnsave" Text="<i class='fas fa-paper-plane'></i> AI Convert" OnClick="LnkSubmit_Click"></asp:LinkButton>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                            </ajaxToolkit:TabContainer>
                        </div>
                    </div>
                    <!-- Column 3: Table with Filtering -->
                    <div class="col-lg-8 col-md-12">
                        <div class="export-table">
                            <h5 class="d-flex align-items-center justify-content-between flex-wrap gap-2">
                            <span><i class="fas fa-table me-2"></i>Order Details</span>
                            <span class="d-flex align-items-center gap-2">
                                <a class="btn btn-add" runat="server" id="HrefDirect"  href="#">
                                <i class="fa fa-file-export me-1"></i>DIRECT</a>
                                <a class="btn btn-add"  runat="server"  id="HrefExport"  href="#">
                                <i class="fa fa-file-export me-1"></i>Export</a>
                            </span>
                            </h5>
                            <!-- Filter Section -->
                            <div class="filter-section mt-3">
                                <div class="row g-2">
                                    <!-- Column 1 -->
                                    <div class="col-md-4 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                            <label class="fw-semibold mb-0 text-nowrap">PO NO:</label>
                                            <asp:TextBox ID="TxtPONo" runat="server" CssClass="form-control" placeholder="PO No."></asp:TextBox>
                                        </div>
                                    </div>
                                    <!-- Column 2 -->
                                    <div class="col-md-4 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                            <label class="fw-semibold mb-0 text-nowrap">Order Date:</label>
                                            <uc1:CtrlDate runat="server" ID="CtrlOrderDate" />
                                        </div>
                                    </div>
                                    <!-- Column 3 -->
                                    <div class="col-md-4 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                            <label class="fw-semibold mb-0 text-nowrap">Delivery Date:</label>
                                            <uc1:CtrlDate runat="server" ID="CtrlDelDate" />
                                        </div>
                                    </div>
                                </div>
                                <!-- Second Row -->
                                <div class="row g-3 mt-2 mb-1">
                                    <div class="col-md-5 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                            <label for="filter-category" class="fw-semibold mb-0 text-nowrap">Customer:</label>
                                            <asp:Label ID="LblCustName" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-md-1 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                               <asp:Label ID="LblAssignTo" runat="server" Text="Assign:"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-md-3 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                            <uc1:CtrlGrdList runat="server" ID="CtrlGrdCustDtl" GridWidth="500" AccountType="CustomerList" PlaceHoldr="Customer Name" />

                                        </div>
                                    </div>
                                    <div class="col-md-3 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                               <asp:Label ID="LblAddress" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="responsive-input-wrapper1 mt-0">
                                    <div id="inputColumn_6">
                                        <div class="input-cell">
                                            <label for="CtrlGrdItmName">Code</label>
                                            <uc1:CtrlGrdList runat="server" ID="CtrlGrdItemCode" AccountType="ItemCodeList" PlaceHoldr="Item Code" GridWidth="400" />
                                        </div>
                                        <div class="input-cell">
                                            <label for="Code">Name</label>
                                            <uc1:CtrlGrdList runat="server" ID="CtrlGrdItem" AccountType="ItemRegList" PlaceHoldr="Item Name" GridWidth="400" />
                                        </div>
                                        <div class="input-cell">
                                            <label for="Code">Uom</label>
                                            <uc1:CtrlGrdList runat="server" ID="CtrlGrdUom" AccountType="UomList" PlaceHoldr="Uom"  />
                                        </div>
                                        <div class="input-cell">
                                            <label for="Code">Order Qty</label>
                                            <asp:TextBox ID="TxtOrderQty" runat="server" placeholder="Qty"></asp:TextBox>
                                        </div>
                                        <div class="input-cell">
                                            <label for="Code">Rate</label>
                                            <asp:TextBox ID="TxtPrice" runat="server" placeholder="Rate"></asp:TextBox>
                                        </div>
                                        <div class="input-cellCtrlCommand">
                                            <label for="TxtTax"></label>
                                            <asp:LinkButton ID="BtnAddRow" runat="server" CssClass="btn Add-btn" Text="Add+" OnClick="BtnAddRow_Click"></asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- Preview Table -->
                            <div class="preview-table">
                                <div style="height: 55vh; overflow: scroll;">
                                    <asp:GridView ID="GrdVwOrderDtls" SkinID="GrdVwMasterNoPageing" runat="server" PageSize="25" AllowPaging="True" AutoGenerateColumns="true" OnRowDataBound="GrdVwOrderDtls_RowDataBound" OnPreRender="GrdVwOrderDtls_PreRender" OnRowCommand="GrdVwOrderDtls_RowCommand" >
                                        <Columns>
                                            <asp:BoundField DataField="SlNo" HeaderText="Sl" />
                                            <asp:TemplateField HeaderText="Code">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="HdnIds" runat="server" Value='<%# Eval("ID") %>' />
                                                    <asp:HiddenField ID="HdnItemId" runat="server" Value='<%# Eval("ItemId") %>' />
                                                    <asp:HiddenField ID="HdnUomId" runat="server" Value='<%# Eval("UomId") %>' />  
                                                     <asp:HiddenField ID="HdnAmt" runat="server" Value='' />     
                                                    <asp:Label ID="LblCode" runat="server" CommandName="SELECT" Text='<%# Eval("AICode") %>' Width="30px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="LblName" runat="server" CommandName="SELECT" Text='<%# Eval("AIItemName") %>' Width="170px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div style="width: 100px;">
                                                        F Code
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <uc2:CtrlGridViewList runat="server" ID="CtrlGrdViewItemCode" PlaceHoldr="Code" AccountType="ItemCodeList" ClientIDMode="Predictable" SelectedValue='<%# Eval("ItemId") %>' SelectedText='<%# Eval("ItemCode") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div style="width: 180px;">
                                                        Floware Item
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <uc2:CtrlGridViewList runat="server" ID="CtrlGrdViewItemDtl" PlaceHoldr="Item Name" AccountType="ItemRegList" ClientIDMode="Predictable" SelectedValue='<%# Eval("ItemId") %>' SelectedText='<%# Eval("ItemName") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="UOM">
                                                <ItemTemplate>
                                                    <asp:Label ID="LblUom" runat="server" CommandName="SELECT" Text='<%# Eval("UOM") %>' Width="80px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div style="width: 80px;">
                                                       F UOM
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <uc2:CtrlGridViewList runat="server" GridWidth="100" ID="CtrlGrdViewUom" PlaceHoldr="Uom" AccountType="UomList" ClientIDMode="Predictable" SelectedValue='<%# Eval("UomId") %>' SelectedText='<%# Eval("UomName") %>'  />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ConVal">
                                                <ItemTemplate>
                                                    <asp:Label ID="LblConVal" runat="server" CommandName="SELECT" Text='<%# Eval("ConVal") %>' Width="170px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Qty">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="TxtQty" runat="server" Text='<%# Eval("Quantity") %>' Width="50px"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Price">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="TxtPrice" runat="server" Text='<%# Eval("Price") %>' Width="80px"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Remarks">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="TxtRemarks" runat="server" Text='<%# Eval("ItemRemarks") %>' Width="80px"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Delete">
                                                <ItemTemplate>
                                                    <asp:Button ID="BtnDelete" runat="server" CssClass="btn btn-delete w-100 redcolor" Text="Delete" CommandName="DeleteMail" CommandArgument='<%# Eval("Id") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle BackColor="#87B0CC" CssClass="pag_btns" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:GridView>
                                </div>
                            </div>
                            <div class="row mt-2">
                                    <div class="col-md-12 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                            <asp:LinkButton ID="LnkBtnReExtract" runat="server" CssClass="btn submit-btn3" Text="Re-Extract" OnClick="LnkBtnReExtract_Click" />
                                            <asp:Button ID="BtnViewMail" runat="server" Text="View Mail" CssClass="btn btn-add w-100 VwWidth" OnClientClick="return openViewEmailsWinBox();" ></asp:Button>
                                            <asp:Button ID="BtnAllocate" runat="server" CssClass="btn map-item-btn w-100" Text="View Items" OnClientClick="return openMapItemsWinBox();" />
                                            <asp:LinkButton ID="BtnSave" runat="server" CssClass="btn submit-btn" Text="Save" OnClick="BtnSaveData_Click"></asp:LinkButton>
                                            <asp:LinkButton ID="BtnApplyMapping" runat="server" CssClass="btn submit-btn1" Text="Export To Sysware" OnClick="BtnApplyMapping_Click"  />
                                            <asp:LinkButton ID="BtnClear" runat="server" CssClass="btn submit-btn2" Text="Clear" OnClick="BtnClear_Click" />
                                            <asp:TextBox ID="TxtPriceTot" runat="server" Text="0" CssClass="TxtTotPr" Enabled="false" BackColor="#d65576" ForeColor="White" Height="42px" style="font-weight: bold;text-align:right;"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Export section -->
        </div>
    </section>
    <script type="text/javascript">
        function FnCloseAllPopups() {

            // Get all WinBox popup windows
            var popups = document.querySelectorAll(".winbox");

            // Close every popup, including minimized windows
            for (var i = popups.length - 1; i >= 0; i--) {

                var popup = popups[i];

                if (popup && popup.winbox && typeof popup.winbox.close === "function") {
                    popup.winbox.close();
                }
                else if (popup) {
                    popup.remove();
                }
            }
        }

        function openMapItemsWinBox() {
            var mailId = document.getElementById('<%= HdnMailId.ClientID %>').value;
            var Customer = document.getElementById('<%= HdnMailFrom.ClientID %>').value;
            var TokenNo = document.getElementById('<%= HdnTokenNo.ClientID %>').value;
            //alert(Customer);
            if (!mailId) {
                alert("Please select an email first.");
                return false;
            }
            var url = "MapItems.aspx?MailId=" + encodeURIComponent(mailId) + "&From=" + encodeURIComponent(Customer) + "&TokenNo=" + encodeURIComponent(TokenNo);

            PopUpIframe("Floware Allocation", "700", "850", "map-items-frame", url);
            return false;
        }
        function openViewEmailsWinBox() {
            var mailId = document.getElementById('<%= HdnMailId.ClientID %>').value;
            var Customer = document.getElementById('<%= HdnMailFrom.ClientID %>').value;
            var TokenNo = document.getElementById('<%= HdnTokenNo.ClientID %>').value;
            if (!mailId) {
                alert("Please select an email first.");
                return false;
            }
            var url = "ViewEmail.aspx?MailId=" + encodeURIComponent(mailId) + "&From=" + encodeURIComponent(Customer) + "&TokenNo=" + encodeURIComponent(TokenNo);

            PopUpIframeLeft("View Email", "700", "850", "map-items-frame", url);
            return false;
        }
    </script>
    <script>
        function saveMailScroll() {
            var div = document.getElementById('mailScrollDiv');
            if (div) {
                document.getElementById('<%= HdnScrollPos.ClientID %>').value = div.scrollTop;
            }
        }
    </script>
    <script type="text/javascript">
        Sys.Application.add_load(function () {
            var div = document.getElementById('mailScrollDiv');
            var pos = document.getElementById('<%= HdnScrollPos.ClientID %>').value;

        if (div && pos) {
            setTimeout(function () {
                div.scrollTop = parseInt(pos);

                // Scroll to selected row
                var selectedRows = div.querySelectorAll('tr[style*="f0f0f0"], tr[style*="8e44ad"], .selected-row');
                if (selectedRows.length > 0) {
                    selectedRows[0].scrollIntoView({ block: 'center', behavior: 'smooth' });
                }
            }, 100);
        }
    });
</script>
</asp:Content>
