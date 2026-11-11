<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" StylesheetTheme="SkinFile" Theme="SkinFile" AutoEventWireup="true" CodeBehind="BlockedMails.aspx.cs" Inherits="FLOWARE_AI.ADMIN.BlockedMails" %>

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
                            <h5><i class="fas fa-table me-2"></i>Blocked Emails</h5>
                            <!-- Filter Section -->
                            <div class="table-controls mb-3">
                                <div class="row g-2 align-items-center w-100">                                    
                                    <div class="col-md-3 col-sm-6">
                                        <asp:Label ID="Label2" runat="server" Text="Mail"></asp:Label>
                                        <asp:TextBox ID="TxtMail" runat="server" CssClass="form-control" placeholder="Mail Address"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3 col-sm-6">
                                        <asp:Label ID="Label3" runat="server" Text="Customer"></asp:Label>
                                        <asp:TextBox ID="TxtCustomer" runat="server" CssClass="form-control" placeholder="Customer"></asp:TextBox>
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
                                    AutoGenerateColumns="false" OnRowCommand="GrdVwMailDtls_RowCommand" 
                                    OnPageIndexChanging="GrdVwMailDtls_PageIndexChanging">
                                    <Columns>
                                        <asp:BoundField HeaderText="SlNo" DataField="SlNo" />
                                        <asp:TemplateField HeaderText="Blocked Date">
                                            <ItemTemplate>
                                                <asp:Label ID="LnkMail" runat="server" CommandName="SELECT" Text='<%# FnDateTime( Eval("BlockedDate"),"dd-MM-yyyy hh:mm:tt") %>' Width="150px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                       
                                        <asp:TemplateField HeaderText="EMail Address">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="HdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:Label ID="LnkFrmMail" runat="server" CommandName="SELECT" Text='<%# Eval("EmailId") %>' Width="250px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Customer">
                                            <ItemTemplate>
                                                <asp:Label ID="LnkSubject" runat="server" CommandName="SELECT" Text='<%# Eval("Remarks") %>' Width="350px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Un Block">
                                            <ItemTemplate>
                                                <asp:Button ID="BtnUnBlock" runat="server" CssClass="btn btn-warning w-100 blkbutton" Text="UnBlock" CommandName="UNBLOCK" CommandArgument='<%# Eval("Id") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
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

    </script>




</asp:Content>