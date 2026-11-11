<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" StylesheetTheme="SkinFile" Theme="SkinFile" CodeBehind="Products.aspx.cs" Inherits="FLOWARE_AI.ADMIN.Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Content Section -->
    <section class="content-section">
        <div class="container">
            <!-- Page Header -->
            <div class="page-header">
                <h1 class="page-title">Products</h1>
            </div>

            <!-- Search area -->
            <div class="table-controls mb-3">
                <div class="row g-2 align-items-center w-100">
                    <!-- Code -->
                    <div class="col-md-4 col-sm-6">
                       <asp:TextBox ID="TxtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                    </div>

                    <!-- Name -->
                    <div class="col-md-3 col-sm-6">
                         <asp:TextBox ID="TxtCode" runat="server" CssClass="form-control" placeholder="Code"></asp:TextBox>
                    </div>

                    <!-- Search Button -->
                    <div class="col-md-2 col-sm-6">
                        <asp:Button ID="BtnSearch" runat="server" Text="Search" CssClass="btn btn-add w-100" OnClick="BtnSearch_Click"></asp:Button>
                    </div>
                    <div class="col-md-3 col-sm-6">
                        <asp:Button ID="BtnUpdateItems"
                            runat="server"
                            Text="Synchronize Items"
                            CssClass="btn btn-primary"
                            OnClick="BtnUpdateItems_Click"
                            OnClientClick="return confirm('Are you sure you want to update items?');" />
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
                                <asp:Label ID="LnkName" runat="server" CommandName="SELECT" Text='<%# Eval("Name") %>' Width="400px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Code">
                            <ItemTemplate>
                                <asp:Label ID="LnkCode" runat="server" CommandName="SELECT" Text='<%# Eval("Code") %>' Width="75px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Box Rate">
                            <ItemTemplate>
                                <asp:Label ID="LblGrossRate" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("GrossRate") %>' Width="50px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Split Rate">
                            <ItemTemplate>
                                <asp:Label ID="LblSplitRate" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("SplitRate") %>' Width="50px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ConVal">
                            <ItemTemplate>
                                <asp:Label ID="LblConVal" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("ConVal") %>' Width="50px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Box Uom">
                            <ItemTemplate>
                                <asp:HiddenField ID="HdnBoxUomId" runat="server" Value='<%# Eval("BoxUomId") %>'></asp:HiddenField>
                                <asp:Label ID="LblBoxUom" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("BoxUom") %>' Width="50px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Split Uom">
                            <ItemTemplate>
                                <asp:HiddenField ID="HdnSplitUomId" runat="server" Value='<%# Eval("SplitUomId") %>'></asp:HiddenField>
                                <asp:Label ID="LblSplitUom" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("SplitUom") %>' Width="50px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Country">
                            <ItemTemplate>
                                <asp:Label ID="LblCountry" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("Country") %>' Width="150px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle BackColor="#87B0CC" CssClass="pag_btns" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                </asp:GridView>

            </div>
        </div>
    </section>

</asp:Content>
