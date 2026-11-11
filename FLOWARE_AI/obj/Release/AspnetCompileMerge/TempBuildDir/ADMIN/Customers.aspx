<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" StylesheetTheme="SkinFile" Theme="SkinFile" CodeBehind="Customers.aspx.cs" Inherits="FLOWARE_AI.ADMIN.Customers" Async="true" %>

<%@ Register Src="~/CtrlGrdList.ascx" TagPrefix="uc1" TagName="CtrlGrdList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="content-section">
        <div class="container">
            <div class="page-header">
                <h1 class="page-title">Customers</h1>
            </div>
            <div class="table-controls mb-3">
                <div class="row g-2 align-items-center w-100">
                    <div class="col-md-4 col-sm-6">
                        <asp:TextBox ID="TxtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <asp:TextBox ID="TxtCode" runat="server" CssClass="form-control" placeholder="Code"></asp:TextBox>
                    </div>
                    <div class="col-md-2 col-sm-6">
                        <asp:Button ID="BtnSearch" runat="server" Text="Search" CssClass="btn btn-add w-100" OnClick="BtnSearch_Click"></asp:Button>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <asp:Button ID="BtnUpdateCust"
                            runat="server"
                            Text="Synchronize Customers"
                            CssClass="btn btn-primary"
                            OnClick="BtnUpdateCust_Click"
                            OnClientClick="return confirm('Are you sure you want to update Customers?');" />
                    </div>
                </div>
            </div>
            <div class="table-container">
                <asp:GridView ID="GrdVwRecords" SkinID="GrdVwMaster" runat="server" PageSize="25" AllowPaging="True" AutoGenerateColumns="False" OnPageIndexChanging="GrdVwRecords_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="SlNo" HeaderText="SlNo" />
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:HiddenField ID="Hdn" runat="server" Value='<%# Eval("Id") %>'></asp:HiddenField>
                                <asp:Label ID="LnkName" runat="server" CommandName="SELECT" Text='<%# Eval("Name") %>' Width="700px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Code">
                            <ItemTemplate>
                                <asp:Label ID="LnkCode" runat="server" CommandName="SELECT" Text='<%# Eval("Code") %>' Width="300px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>                      
                    </Columns>
                    <PagerStyle BackColor="#87B0CC" CssClass="pag_btns" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                </asp:GridView>
            </div>
        </div>
    </section>
</asp:Content>
