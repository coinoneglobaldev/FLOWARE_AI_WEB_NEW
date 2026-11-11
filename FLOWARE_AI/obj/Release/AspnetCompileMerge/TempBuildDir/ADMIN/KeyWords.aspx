<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" StylesheetTheme="SkinFile" Theme="SkinFile" AutoEventWireup="true" CodeBehind="KeyWords.aspx.cs" Inherits="FLOWARE_AI.ADMIN.KeyWords" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



    <!-- Content Section -->
    <section class="content-section">
        <div class="container">
            <!-- Page Header -->
            <div class="page-header">
                <h1 class="page-title">Customer List-Keywords</h1>
            </div>

            <!-- Search area -->
            <div class="table-controls mb-3">
                <div class="row g-2 align-items-center w-100">
                    <!-- Code -->
                    <div class="col-md-4 col-sm-6">
                       <asp:TextBox ID="TxtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                    </div>

                    <!-- Name -->
                    <div class="col-md-4 col-sm-6">
                         <asp:TextBox ID="TxtCode" runat="server" CssClass="form-control" placeholder="Code"></asp:TextBox>
                    </div>
                    <div class="col-md-2 col-sm-6">
                    <asp:LinkButton runat="server" ID="BtnSave" Width="115px" Text="<i class='fa fa-check-circle-o'></i> SAVE"  OnClick="BtnSave_Click" CssClass="btn btn-success w-100" Height="42px" ></asp:LinkButton>
                        </div>
                    <!-- Search Button -->
                    <div class="col-md-2 col-sm-6">
                        <asp:Button ID="BtnSearch" runat="server" Text="Search" CssClass="btn btn-add w-100" OnClick="BtnSearch_Click"></asp:Button>
                    </div>
                </div>
            </div>



            <!-- Product Table -->
            <div class="table-container">

                <asp:GridView ID="GrdVwRecords" SkinID="GrdVwMaster" runat="server" PageSize="2" AllowPaging="True" AutoGenerateColumns="False" OnPageIndexChanging="GrdVwRecords_PageIndexChanging">

                    <Columns>
                        <asp:BoundField DataField="SlNo" HeaderText="SlNo" />                        
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:HiddenField ID="Hdn" runat="server" Value='<%# Eval("Id") %>'></asp:HiddenField>
                                <asp:Label ID="LnkName" runat="server" CommandName="SELECT" Text='<%# Eval("Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Code">
                            <ItemTemplate>
                                <asp:Label ID="LnkCode" runat="server" CommandName="SELECT" Text='<%# Eval("Code") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>                        
                    </Columns>
                    <PagerStyle BackColor="#87B0CC" CssClass="pag_btns" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                </asp:GridView>

            </div>
        </div>
    </section>

</asp:Content>
