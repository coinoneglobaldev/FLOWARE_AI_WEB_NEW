<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" StylesheetTheme="SkinFile" Theme="SkinFile" CodeBehind="OrderParsing.aspx.cs" Inherits="FLOWARE_AI.ADMIN.OrderParsing" Async="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/CtrlGrdList.ascx" TagPrefix="uc1" TagName="CtrlGrdList" %>
<%@ Register Src="~/CtrlGridViewList.ascx" TagPrefix="uc2" TagName="CtrlGridViewList" %>
<%@ Register Src="~/CtrlDate.ascx" TagPrefix="uc1" TagName="CtrlDate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="stylesheet" href="css/export.css">
    <section class="content-section">
        <div class="container">
            <!-- Export Section -->
            <div class="export-section">
                <div class="row">
                    <asp:HiddenField ID="HdnmailId" runat="server" />
                    <asp:HiddenField ID="HdnMailFrom" runat="server" />
                    <asp:HiddenField ID="HdnType" runat="server" />
                    <!-- Column 2: Tabs -->
                    <div class="col-lg-5 col-md-12">
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
                                            <div class="export-section_mail_listing" style="max-height: 82vh;overflow:scroll;">                                                
                                                <asp:HiddenField ID="HdnSelectedRowIndex" runat="server" />
                                                <asp:HiddenField ID="HiddenField1" runat="server" />
                                                <asp:GridView ID="GrdVwRecords" SkinID="GrdVwMaster" runat="server" DataKeyNames="Id" Style="width: 100%;" OnSelectedIndexChanged="GrdVwRecords_SelectedIndexChanged" OnRowDataBound="GrdVwRecords_RowDataBound" OnRowCommand="GrdVwRecords_RowCommand">
                                                    <SelectedRowStyle BackColor="#f0f0f0" Font-Bold="true" BorderStyle="Solid" BorderWidth="3px" BorderColor="#8e44ad" />
                                                    <Columns>
                                                        <asp:BoundField DataField="SlNo" HeaderText="Sl" />
                                                        <asp:TemplateField  HeaderText="Date">
                                                            <ItemTemplate>
                                                                  <!-- <asp:HiddenField ID="HdnFrMail" runat="server" Value='<%# Eval("FromMail") %>' />
                                                                 <asp:LinkButton ID="LnkFromMail" runat="server" CommandName="SELECT" Text='<%# Eval("FromMail") %>' Width="150px"></asp:LinkButton> -->
                                                                <asp:LinkButton ID="Lbldate" runat="server" CommandName="SELECT" Text='<%# Eval("MailDate") %>' Width="100px"></asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Subject">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="Hdn" runat="server" Value='<%# Eval("Id") %>' />
                                                                <asp:LinkButton ID="LnkName" runat="server" CommandName="SELECT" CommandArgument='<%# Eval("Id") %>' CssClass="text-start w-100 email-link"
                                                                    Text='<%# Eval("SubjectMail") %>' Width="400px">
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle BackColor="#87B0CC" CssClass="pag_btns" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>

                                <!-- 🌟 TEXT PANEL -->
                                <ajaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>
                                        Text
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="tab-pane-content p-3">
                                            <asp:TextBox runat="server" ID="TxtAreainput" TextMode="MultiLine" Rows="22" placeholder="Enter your text heree..."></asp:TextBox>
                                            <asp:LinkButton ID="BtnTxtSubmit" runat="server" CssClass="btn submit-btn" Text="<i class='fas fa-paper-plane'></i> AI Convert" OnClick="BtnTxtSubmit_Click1"></asp:LinkButton>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>

                                <!-- 🌟 PDF PANEL -->
                                <ajaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>
                                        PDF
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="tab-pane-content p-3">
                                            <ajaxToolkit:AsyncFileUpload ID="FileUploadDoc" runat="server" class="file-input-label" OnUploadedComplete="FileUploadDoc_UploadedComplete" />
                                            <asp:HiddenField ID="HdnFilePath" runat="server" />
                                            <asp:LinkButton ID="BtnPDFSubmit" runat="server" CssClass="btn submit-btn" Text="<i class='fas fa-paper-plane'></i> AI Convert" OnClick="BtnPDFSubmit_Click1"></asp:LinkButton>
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
                                        <div class="tab-pane-content p-3">
                                            <asp:TextBox runat="server" ID="TxtLink" Rows="22" placeholder="Enter link or URL here..." TextMode="MultiLine"> </asp:TextBox>
                                            <asp:LinkButton ID="BtnLinkSubmit" runat="server" CssClass="btn submit-btn" Text="<i class='fas fa-paper-plane'></i> AI Convert" OnClick="BtnLinkSubmit_Click1"></asp:LinkButton>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                            </ajaxToolkit:TabContainer>
                        </div>
                    </div>
                    <!-- Column 3: Table with Filtering -->
                    <div class="col-lg-7 col-md-12">
                        <div class="export-table">
                            <h5><i class="fas fa-table me-2"></i>Order Details</h5>
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
                                    <div class="col-md-2 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                               <asp:Button ID="BtnAssignMap" runat="server" Text="Assign" CssClass="btn btn-secondary w-100 asignmap" OnClick="BtnAssignMap_Click"></asp:Button>
                                        </div>
                                    </div>
                                    <div class="col-md-5 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                            <uc1:CtrlGrdList runat="server" ID="CtrlGrdCustDtl" AccountType="CustomerList" PlaceHoldr="Customer Name" />

                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-12 col-12">
                                        <div class="d-flex align-items-center gap-2">
                                            <asp:Button ID="BtnViewMail" runat="server" Text="View Mail" CssClass="btn btn-add w-100" OnClientClick="return openViewEmailsWinBox();"></asp:Button>
                                            <asp:Button ID="Button1" runat="server" CssClass="btn map-item-btn w-100" Text="Allocate Items" OnClientClick="return openMapItemsWinBox();" />
                                        </div>
                                    </div>
                                </div>
                                <div class="responsive-input-wrapper1 mt-0">
                                    <div id="inputColumn_5">
                                        <div class="input-cell">
                                            <label for="CtrlGrdItmName">Code</label>
                                            <uc1:CtrlGrdList runat="server" ID="CtrlGrdItemCode" AccountType="ItemCodeList" PlaceHoldr="Item Code" GridWidth="400" />
                                        </div>
                                        <div class="input-cell">
                                            <label for="Code">Name</label>
                                            <uc1:CtrlGrdList runat="server" ID="CtrlGrdItem" AccountType="ItemRegList" PlaceHoldr="Item Name" GridWidth="400" />
                                        </div>
                                        <div class="input-cell text_down">
                                            <label for="TxtQty">UOM</label>
                                            <asp:DropDownList ID="DdlUomAdd" runat="server" SkinID="DdlNormal"></asp:DropDownList>
                                        </div>
                                        <div class="input-cell">
                                            <label for="Code">Order Qty</label>
                                            <asp:TextBox ID="TxtOrderQty" runat="server" placeholder="Qty"></asp:TextBox>
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
                                <div style="height: 44vh; overflow: scroll;">
                                    <asp:GridView ID="GrdVwOrderDtls" SkinID="GrdVwMasterNoPageing" runat="server" PageSize="25" AllowPaging="True" AutoGenerateColumns="true" OnRowDataBound="GrdVwOrderDtls_RowDataBound" OnPreRender="GrdVwOrderDtls_PreRender" OnRowCommand="GrdVwOrderDtls_RowCommand">
                                        <Columns>
                                            <asp:BoundField DataField="SlNo" HeaderText="SlNo" />
                                            <asp:TemplateField HeaderText="Code">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="HdnId" runat="server" Value='<%# Eval("ID") %>' />
                                                    <asp:HiddenField ID="HdnItemId" runat="server" Value='<%# Eval("ItemId") %>' />
                                                    <asp:HiddenField ID="HdnUomId" runat="server" Value='<%# Eval("UomId") %>' />
                                                    <asp:Label ID="LblCode" runat="server" CommandName="SELECT" Text='<%# Eval("AICode") %>' Width="60px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="LblName" runat="server" CommandName="SELECT" Text='<%# Eval("AIItemName") %>' Width="190px"></asp:Label>
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
                                                    <div style="width: 200px;">
                                                        Floware Item
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <uc2:CtrlGridViewList runat="server" ID="CtrlGrdViewItemDtl" PlaceHoldr="Item Name" AccountType="ItemRegList" ClientIDMode="Predictable" SelectedValue='<%# Eval("ItemId") %>' SelectedText='<%# Eval("ItemName") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="UOM Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="LblUom" runat="server" CommandName="SELECT" Text='<%# Eval("UOM") %>' Width="100px"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div style="width: 100px;">
                                                       UOM
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <uc2:CtrlGridViewList runat="server" ID="CtrlGrdViewUom" PlaceHoldr="Uom" AccountType="UomList" ClientIDMode="Predictable" SelectedValue='<%# Eval("UomId") %>' SelectedText='<%# Eval("UomName") %>'  />
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
                            <asp:LinkButton ID="BtnExportData" runat="server" CssClass="btn submit-btn" Text="<i class='fas fa-paper-plane'></i> SAVE" OnClick="BtnExportData_Click1"></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Export section -->
        </div>
    </section>

    <script type="text/javascript">
        function openMapItemsWinBox() {
            var mailId = document.getElementById('<%= HdnmailId.ClientID %>').value;
            var Customer = document.getElementById('<%= HdnMailFrom.ClientID %>').value;
            //alert(Customer);
            if (!mailId) {
                alert("Please select an email first.");
                return false;
            }
            var url = "MapItems.aspx?MailId=" + encodeURIComponent(mailId) + "&From=" + encodeURIComponent(Customer);

            PopUpIframe("Floware Allocation", "1270", "700", "map-items-frame", url);
            return false;
        }
        function openViewEmailsWinBox() {
            var mailId = document.getElementById('<%= HdnmailId.ClientID %>').value;
            var Customer = document.getElementById('<%= HdnMailFrom.ClientID %>').value;
            if (!mailId) {
                alert("Please select an email first.");
                return false;
            }
            var url = "ViewEmail.aspx?MailId=" + encodeURIComponent(mailId) + "&From=" + encodeURIComponent(Customer);

            PopUpIframeLeft("View Email", "800", "850", "map-items-frame", url);
            return false;
        }


        function setFocusRow(rowIndex) {
            var grid = document.getElementById('<%= GrdVwRecords.ClientID %>');
            if (grid) {
                var row = grid.rows[rowIndex + 1]; // Adjust for header row
                if (row) {
                    row.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    row.style.backgroundColor = "#FFC0FF"; // Highlight color
                    row.focus(); // Set focus
                }
            }
        }
        function setScrollPosition(rowIndex) {
            document.getElementById('<%= HdnSelectedRowIndex.ClientID %>').value = rowIndex;
        }
        function restoreScroll() {
            var rowIndex = document.getElementById('<%= HdnSelectedRowIndex.ClientID %>').value;
            if (rowIndex === "") return;

            var rows = document.querySelectorAll('#<%= GrdVwRecords.ClientID %> tr');
            if (rows.length > rowIndex) {
                rows[rowIndex].scrollIntoView({
                    behavior: 'smooth',
                    block: 'center'
                });
                rows[rowIndex].focus();
            }
        }
        window.onload = restoreScroll;



    </script>

</asp:Content>
