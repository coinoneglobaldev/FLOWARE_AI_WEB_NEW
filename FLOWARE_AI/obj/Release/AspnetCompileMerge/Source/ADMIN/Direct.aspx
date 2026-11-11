<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="Direct.aspx.cs" StylesheetTheme="SkinFile" Theme="SkinFile" Inherits="FLOWARE_AI.ADMIN.Direct" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/CtrlDate.ascx" TagPrefix="uc1" TagName="CtrlDate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="stylesheet" href="css/export.css">

    <section class="content-section">
        <div class="container">
            <!-- Export Section -->
            <div class="export-section">
                <div class="row">
                    <!-- Column 1: List -->
                    <asp:HiddenField ID="HdnmailIds" runat="server" />
                    <div class="col-lg-12 col-md-12">
                        <div class="export-table">
                           <h5 class="d-flex align-items-center justify-content-between flex-wrap gap-2">
                                <span>
                                    <i class="fas fa-table me-2"></i>DIRECT ORDERS
                                </span>
                                <span class="d-flex align-items-center gap-2 ms-auto">
                                    <a class="btn btn-add" runat="server" id="HrefExport" href="#">
                                        <i class="fa fa-file-export me-1"></i>EXPORT
                                    </a>
                                    <a class="btn btn-add" runat="server" id="HrefAIOrdering" href="#">
                                        <i class="fa fa-robot me-1"></i>AI ORDERING
                                    </a>
                                </span>
                            </h5>
                            <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="floware-tabs">
                                <ajaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>
                                        Pending
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="col-lg-12 col-md-12">
                                            <div class="export-table">
                                                <div class="table-controls mb-3">
                                                    <div class="row g-2 align-items-center w-100">
                                                        <!-- Code -->
                                                        <div class="col-md-2 col-sm-6">
                                                            <asp:Label ID="LblfromDate" runat="server" Text="From Date"></asp:Label>
                                                            <uc1:CtrlDate runat="server" ID="CtrlFromDate" IsVisibleDate="true" />
                                                        </div>
                                                        <!-- Name -->
                                                        <div class="col-md-2 col-sm-6">
                                                            <asp:Label ID="Label1" runat="server" Text="To Date"></asp:Label>
                                                            <uc1:CtrlDate runat="server" ID="CtrlToDate"  />
                                                        </div>
                                                        <div class="col-md-3 col-sm-6">
                                                            <asp:Label ID="Label2" runat="server" Text="Customer"></asp:Label>
                                                            <asp:TextBox ID="TxtCust" runat="server" CssClass="form-control" placeholder="Customer"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-3 col-sm-6">
                                                            <asp:Label ID="Label3" runat="server" Text="Order No"></asp:Label>
                                                            <asp:TextBox ID="TxtOrderNo" runat="server" CssClass="form-control" placeholder="Order No"></asp:TextBox>
                                                        </div>
                                                        <!-- Search Button -->
                                                        <div class="col-md-2 col-sm-6 mt-4">
                                                            <asp:Button ID="BtnSearch" runat="server" Text="🔍 Search" CssClass="btn btn-add w-100" OnClick="BtnSearch_Click" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <!-- Preview Table -->
                                                <div class="preview-table">
                                                    <asp:Label ID="LblMsg" Text="No Records Found" Visible="false" runat="server" CssClass="LblMaroon"></asp:Label>
                                                    <asp:GridView ID="GrdVwMailDtls" SkinID="GrdVwMaster" runat="server" PageSize="25" AllowPaging="True" AutoGenerateColumns="false" OnRowCommand="GrdVwMailDtls_RowCommand" OnRowDataBound="GrdVwMailDtls_RowDataBound" OnPageIndexChanging="GrdVwMailDtls_PageIndexChanging">
                                                        <Columns>
                                                            <asp:BoundField HeaderText="SlNo" DataField="SlNo" />
                                                            <asp:TemplateField HeaderText="Date">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LnkDate" runat="server" CommandName="SELECT" Text='<%# FnDateTime( Eval("FirstOrderDate"),"dd-MMM-yyyy") %>' Width="150px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Customer Name">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="HdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                                    <asp:HiddenField ID="HdnTokenId" runat="server" Value='<%# Eval("TokenId") %>' />
                                                                    <asp:HiddenField ID="HdnPoType" runat="server" Value='<%# Eval("POType") %>' />
                                                                    <asp:Label ID="LnkCode" runat="server" CommandName="SELECT" Text='<%# Eval("CustomerName") %>' Width="250px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Order No">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LnkSubject" runat="server" CommandName="SELECT" Text='<%# Eval("OrderNo") %>' Width="300px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Type">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LnkSubject1" runat="server" CommandName="SELECT" Text='<%# Eval("POType") %>' Width="100px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Map Items">
                                                                <ItemTemplate>
                                                                    <asp:Button ID="BtnMapItems" runat="server" CssClass="btn map-item-btn w-100" Text="Map Items" OnClientClick="return openMapItemsWinBoxes(this);" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                        </Columns>
                                                        <PagerStyle BackColor="#87B0CC" CssClass="pag_btns" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>
                                        Completed
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="col-lg-12 col-md-12">
                                            <div class="export-table">
                                                <div class="table-controls mb-3">
                                                    <div class="row g-2 align-items-center w-100">
                                                        <!-- Code -->
                                                        <div class="col-md-2 col-sm-6">
                                                            <asp:Label ID="Label4" runat="server" Text="From Date"></asp:Label>
                                                            <uc1:CtrlDate runat="server" ID="CtrlFrDate" IsVisibleDate="true" />
                                                        </div>

                                                        <!-- Name -->
                                                        <div class="col-md-2 col-sm-6">
                                                            <asp:Label ID="Label5" runat="server" Text="To Date"></asp:Label>
                                                            <uc1:CtrlDate runat="server" ID="CtrlToDate1"  />
                                                        </div>
                                                        <div class="col-md-3 col-sm-6">
                                                            <asp:Label ID="Label6" runat="server" Text="Customer"></asp:Label>
                                                            <asp:TextBox ID="TxtCust1" runat="server" CssClass="form-control" placeholder="Customer"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-3 col-sm-6">
                                                            <asp:Label ID="Label7" runat="server" Text="Order No"></asp:Label>
                                                            <asp:TextBox ID="TxtOrder1" runat="server" CssClass="form-control" placeholder="Order No"></asp:TextBox>
                                                        </div>

                                                        <!-- Search Button -->
                                                        <div class="col-md-2 col-sm-6 mt-4">

                                                            <asp:Button ID="Button1" runat="server" Text="🔍 Search" CssClass="btn btn-add w-100" OnClick="Button1_Click" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <!-- Preview Table -->
                                                <div class="preview-table">
                                                    <asp:Label ID="LblMsgE" Text="No Records Found" Visible="false" runat="server" CssClass="LblMaroon"></asp:Label>
                                                    <asp:GridView ID="GrdVwRecords" SkinID="GrdVwMaster" runat="server" PageSize="25" AllowPaging="True" AutoGenerateColumns="false" OnRowCommand="GrdVwMailDtls_RowCommand" OnRowDataBound="GrdVwMailDtls_RowDataBound" OnPageIndexChanging="GrdVwMailDtls_PageIndexChanging">
                                                        <Columns>
                                                            <asp:BoundField HeaderText="SlNo" DataField="SlNo" />
                                                            <asp:TemplateField HeaderText="Date">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LnkMail2" runat="server" CommandName="SELECT" Text='<%# FnDateTime(Eval("FirstOrderDate"),"dd-MMM-yyyy") %>' Width="150px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Customer Name">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="HdnIds" runat="server" Value='<%# Eval("Id") %>' />
                                                                    <asp:HiddenField ID="HdnTokenIds" runat="server" Value='<%# Eval("TokenId") %>' />
                                                                    <asp:HiddenField ID="HdnPoTypes" runat="server" Value='<%# Eval("POType") %>' />
                                                                    <asp:Label ID="LnkCode2" runat="server" CommandName="SELECT" Text='<%# Eval("CustomerName") %>' Width="250px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Order No">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LnkSubject12" runat="server" CommandName="SELECT" Text='<%# Eval("OrderNo") %>' Width="300px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Type">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LnkSubject122" runat="server" CommandName="SELECT" Text='<%# Eval("POType") %>' Width="100px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Map Items">
                                                                <ItemTemplate>
                                                                    <asp:Button ID="BtnMapItemsE" runat="server" CssClass="btn map-item-btn w-100" Text="Map Items" OnClientClick="return openMapItemsWinBoxesExp(this);" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                        </Columns>
                                                        <PagerStyle BackColor="#87B0CC" CssClass="pag_btns" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                            </ajaxToolkit:TabContainer>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Export section -->
        </div>
    </section>





    <script>
        function OpenItemMappingModal() {
            var myModal = new bootstrap.Modal(document.getElementById('ItemMappingModal'));
            myModal.show();
        }

        function syncFlowareItem(fCodeDropdown) {
            //alert(fCodeDropdown);
            var row = fCodeDropdown.closest('tr');
            var flowareDropdown = row.querySelector('.floware-item');

            if (flowareDropdown) {
                flowareDropdown.value = fCodeDropdown.value;
            }
        }

    </script>


    <script type="text/javascript">
        function openMapItemsWinBoxes(button) {
            // Find the HiddenField in the same row as the clicked button
            var row = button.parentNode.parentNode; // TemplateField > ItemTemplate > Button's parent row
            var HdnTokenId = row.querySelector('input[id*=\"HdnTokenId\"]').value;
            var HdnType = row.querySelector('input[id*=\"HdnPoType\"]').value;
            var Customer = "test";
            var url = "../ADMIN/MapItemsDirect.aspx?TokenNo=" + encodeURIComponent(HdnTokenId) + "&POType=" + encodeURIComponent(HdnType);

            PopUpIframe("Floware Allocation", "1270", "700", "map-items-frame", url);
            return false;
        }
        function openMapItemsWinBoxesExp(button) {
            // Find the HiddenField in the same row as the clicked button
            var row = button.parentNode.parentNode; // TemplateField > ItemTemplate > Button's parent row
            var HdnTokenId = row.querySelector('input[id*=\"HdnTokenIds\"]').value;
            var HdnType = row.querySelector('input[id*=\"HdnPoTypes\"]').value;
            var Customer = "test";
            var url = "../ADMIN/MapItemsDirect.aspx?TokenNo=" + encodeURIComponent(HdnTokenId) + "&POType=" + encodeURIComponent(HdnType);

            PopUpIframe("Floware Allocation", "1270", "700", "map-items-frame", url);
            return false;
        }

    </script>




</asp:Content>
