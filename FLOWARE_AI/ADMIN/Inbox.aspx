<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="Inbox.aspx.cs" Inherits="FLOWARE_AI.ADMIN.Inbox" StylesheetTheme="SkinFile" Theme="SkinFile" %>

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
                            <h5><i class="fas fa-table me-2"></i>INBOX</h5>
                            <!-- Filter Section -->
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
                                        <uc1:CtrlDate runat="server" ID="CtrlToDate" />
                                    </div>
                                    <div class="col-md-3 col-sm-6">
                                        <asp:Label ID="Label2" runat="server" Text="Mail"></asp:Label>
                                        <asp:TextBox ID="TxtMail" runat="server" CssClass="form-control" placeholder="Mail Address"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3 col-sm-6">
                                        <asp:Label ID="Label3" runat="server" Text="Subject"></asp:Label>
                                        <asp:TextBox ID="TxtSubject" runat="server" CssClass="form-control" placeholder="Subject"></asp:TextBox>
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
                                <asp:GridView ID="GrdVwMailDtls" SkinID="GrdVwMaster" runat="server" PageSize="25" EnableViewState="true" AllowPaging="True"
                                    AutoGenerateColumns="false" OnRowCommand="GrdVwMailDtls_RowCommand" OnRowDataBound="GrdVwMailDtls_RowDataBound"
                                    OnPageIndexChanging="GrdVwMailDtls_PageIndexChanging" AllowSorting="True" OnSorting="GrdVwMailDtls_Sorting">
                                    <Columns>
                                        <asp:BoundField HeaderText="SlNo" DataField="SlNo" SortExpression="SlNo" />
                                        <asp:TemplateField HeaderText="Date" SortExpression="MailDate">
                                            <ItemTemplate>
                                                <asp:Label ID="LnkMail" runat="server" CommandName="SELECT" Text='<%# FnDateTime( Eval("MailDate"),"dd-MM-yyyy hh:mm:tt") %>' Width="150px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" ItemStyle-Width="25px" HeaderStyle-Width="35px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="ChkSelectAll" runat="server" CssClass="mini-chk" />
                                                <asp:Button ID="BtnDelete" runat="server" CssClass="mini-delete-btn btn btn-delete redcolor fa pl-2" Text="&#xf1f8;" OnClick="BtnDelete_Click" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ChkSelect" runat="server" CssClass="mini-chk" Width="75px" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="From Mail" SortExpression="FromMail">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="HdnTokenId" runat="server" Value='<%# Eval("TokenId") %>' />
                                                <asp:HiddenField ID="HdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField ID="HdnFrMail" runat="server" Value='<%# Eval("FromMail") %>' />
                                                <asp:HiddenField ID="HdnIxExport" runat="server" Value='<%# Eval("IsExport") %>' />
                                                <asp:HiddenField ID="HdncUniqueId" runat="server" Value='<%# Eval("UniqueMailId") %>' />
                                                <asp:Label ID="LnkFrmMail" runat="server" CommandName="SELECT" Text='<%# Eval("FromMail") %>' Width="250px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Subject Mail" SortExpression="SubjectMail">
                                            <ItemTemplate>
                                                <asp:Label ID="LnkSubject" runat="server" CommandName="SELECT" Text='<%# Eval("SubjectMail") %>' Width="500px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Delivery Date" SortExpression="DeliveryDate">
                                            <ItemTemplate>
                                                <asp:Label ID="LnkDate" runat="server" CommandName="SELECT" Text='<%# FnDateTime( Eval("DeliveryDate"),"dd-MM-yyyy") %>' Width="130px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Map Items">
                                            <ItemTemplate>
                                                <asp:Button ID="BtnMapItems" runat="server" CssClass="btn map-item-btn w-100" Text="Map Items" OnClientClick="return openMapItemsWinBoxes(this);" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="View Email">
                                            <ItemTemplate>
                                                <asp:Button ID="BtnView" runat="server" CssClass="btn map-item-btn w-100 ViewButton" Text="View Email" OnClientClick="return openViewEmailWinBoxes(this);" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Delete">
                                            <ItemTemplate>
                                                <asp:Button ID="BtnDelete" runat="server" CssClass="btn btn-delete w-100 redcolor" Text="Delete" CommandName="DeleteMail" CommandArgument='<%# Eval("Id") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" ItemStyle-Width="25px"  ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="35px">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="ChkSelectAllB" runat="server" CssClass="mini-chk" />
                                                <asp:LinkButton ID="LnkBlock" runat="server" CssClass="mini-delete-btn btn redcolor1 fa pl-2" Text="&#xf05e;" OnClick="LnkBlock_Click" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ChkSelectB" runat="server" CssClass="mini-chk" Width="75px" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:TemplateField HeaderText="Block">
                                            <ItemTemplate>
                                                <asp:Button ID="BtnBlock" runat="server" CssClass="btn btn-warning w-100 blkbutton" Text="Block" CommandName="BLOCK" CommandArgument='<%# Eval("Id") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                    </Columns>
                                    <PagerStyle BackColor="#87B0CC" CssClass="pag_btns" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:GridView>
                            </div>
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
            var hdnId = row.querySelector('input[id*=\"HdnId\"]').value;
            var HdnFrMail = row.querySelector('input[id*=\"HdnFrMail\"]').value;
            var HdnToken = row.querySelector('input[id*=\"HdnTokenId\"]').value;
            var Customer = "test";
            var url = "MapItems.aspx?MailId=" + encodeURIComponent(hdnId) + "&From=" + encodeURIComponent(HdnFrMail) + "&TokenNo=" + encodeURIComponent(HdnToken);

            PopUpIframe("Floware Allocation", "1300", "700", "map-items-frame", url);
            return false;
        }
        function openViewEmailWinBoxes(button) {
            // Find the HiddenField in the same row as the clicked button
            var row = button.parentNode.parentNode; // TemplateField > ItemTemplate > Button's parent row
            var hdnId = row.querySelector('input[id*=\"HdnId\"]').value;
            var HdnFrMail = row.querySelector('input[id*=\"HdnFrMail\"]').value;
            var HdnToken = row.querySelector('input[id*=\"HdnTokenId\"]').value;
            var Customer = "test";
            var url = "ViewEmail.aspx?MailId=" + encodeURIComponent(hdnId) + "&From=" + encodeURIComponent(HdnFrMail) + "&TokenNo=" + encodeURIComponent(HdnToken);;

            PopUpIframe("View Email", "1300", "700", "map-items-frame", url);
            return false;
        }
        function GridChkSelectAll() {
            var grid = document.getElementById('ContentPlaceHolder1_GrdVwMailDtls');
            var ChkAll = document.getElementById('ContentPlaceHolder1_GrdVwMailDtls_ChkSelectAll');
            if (grid && grid.rows.length > 0) {
                for (var i = 0; i < grid.rows.length; i++) {

                    var chk = document.getElementById('ContentPlaceHolder1_GrdVwMailDtls_ChkSelect_' + i);
                    if (chk != null) {
                        chk.checked = ChkAll.checked;
                    }
                   
                }
            }
            
        }
        function GridChkSelectAllB() {
            var grid = document.getElementById('ContentPlaceHolder1_GrdVwMailDtls');
            var ChkAll = document.getElementById('ContentPlaceHolder1_GrdVwMailDtls_ChkSelectAllB');
            if (grid && grid.rows.length > 0) {
                for (var i = 0; i < grid.rows.length; i++) {

                    var chk = document.getElementById('ContentPlaceHolder1_GrdVwMailDtls_ChkSelectB_' + i);
                    if (chk != null) {
                        chk.checked = ChkAll.checked;
                    }

                }
            }

        }
    </script>




</asp:Content>

