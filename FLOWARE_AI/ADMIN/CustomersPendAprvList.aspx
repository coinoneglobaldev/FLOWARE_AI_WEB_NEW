<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" StylesheetTheme="SkinFile" Theme="SkinFile" CodeBehind="CustomersPendAprvList.aspx.cs" Inherits="FLOWARE_AI.ADMIN.CustomersPendAprvList" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
   

    <div class="container body">
        <div class="tab_container">
            <div class="main_container">
                <div class="page-header">
                    <h1 class="page-title">Customers Pending Approved List</h1>
                </div>
                <div class="tab-content" id="myTabContent">
                    <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="1" BorderColor="White" BorderStyle="Solid" BorderWidth="0px">
                        <ajaxToolkit:TabPanel runat="server" HeaderText="TabPanel1" ID="TabPanel1">
                            <HeaderTemplate>
                                Customer Pending List            
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="tab-pane fade show active" id="Div1" tabindex="1" runat="server">
                                    <div class="col-md-12 p-2">
                                        <div class="p-0">
                                            <div class="x_content mt-0">
                                                <div class="form-horizontal form-label-left">
                                                    <div class="table-container">
                                                        <asp:GridView ID="GrdVwRecords" SkinID="GrdVwMaster" runat="server" PageSize="25" AllowPaging="True" AutoGenerateColumns="False">

                                                            <Columns>
                                                                <asp:BoundField DataField="SlNo" HeaderText="SlNo" />
                                                                <asp:TemplateField HeaderText="Name">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="Hdn" runat="server" Value='<%# Eval("Id") %>'></asp:HiddenField>
                                                                        <asp:Label ID="LnkName" runat="server" CommandName="SELECT" Text='<%# Eval("Name") %>' Width="175px"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Code">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="LnkCode" runat="server" CommandName="SELECT" Text='<%# Eval("Code") %>' Width="175px"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Parent Group">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="LblgrpName" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("Parent") %>' Width="150px"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Remarks">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="LblRmrk" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("Remarks") %>' Width="150px"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Status">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblStatus" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("Status") %>' Width="100px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Approve">
                                                                    <ItemTemplate>
                                                                        <asp:Button ID="BtnApprove" runat="server" CommandName="DELETE" Text="Approve" CssClass="Approve" Width="70px" Height="30px"></asp:Button>
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
                                </div>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="TabPanel1" ID="TabPanel2">
                            <HeaderTemplate>
                                Customer Approved List              
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="tab-pane" id="lists">
                                    <div class="col-md-12 p-0">
                                        <div class="x_panel p-0" style="border: none;">
                                            <div class="x_content mt-0">
                                                <div class="table-container">
                                                    <asp:GridView ID="GrdVwRecords1" SkinID="GrdVwMaster" runat="server" PageSize="25" AllowPaGRDging="True" AutoGenerateColumns="False">

                                                        <Columns>
                                                            <asp:BoundField DataField="SlNo" HeaderText="SlNo" />
                                                            <asp:TemplateField HeaderText="Name">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="Hdn" runat="server" Value='<%# Eval("Id") %>'></asp:HiddenField>
                                                                    <asp:Label ID="LnkName" runat="server" CommandName="SELECT" Text='<%# Eval("Name") %>' Width="175px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LnkCode" runat="server" CommandName="SELECT" Text='<%# Eval("Code") %>' Width="175px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Parent Group">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblgrpName" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("Parent") %>' Width="150px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Remarks">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblRmrk" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("Remarks") %>' Width="150px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Status">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LblStatus" runat="server" SkinID="LblGrdMaster" Text='<%# Eval("Status") %>' Width="100px"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Reject">
                                                                    <ItemTemplate>
                                                                        <asp:Button ID="BtnRej" runat="server" CommandName="DELETE" Text="Reject" CssClass="Reject" Width="70px" Height="30px"></asp:Button>
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
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </div>
            </div>
            <!-- End Form Layout -->
        </div>
        <!-- End Form Layout -->
    </div>
</asp:Content>
