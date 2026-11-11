<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="DirectNews.aspx.cs" Inherits="FLOWARE_AI.ADMIN.DirectNews" StylesheetTheme="SkinFile" Theme="SkinFile" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/CtrlDate.ascx" TagPrefix="uc1" TagName="CtrlDate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="content-section">
        <div class="container">
            <!-- Export Section -->
            <div class="export-section">
                <div class="row">
                    <asp:HiddenField ID="HdnmailIds" runat="server" />
                    <!-- Column 2: Tabs -->
                    <div class="col-lg-4 col-md-12">
                        <div class="export-tabs">
                           <h5><i class="fas fa-table me-2"></i>DIRECT ORDERS</h5>
                            <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="ajax-tab-container_export"  AutoPostBack="True">
                                <ajaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>
                                        Text
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="col-lg-12 col-md-12">
                        <div class="export-table">
                            
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
                                        <uc1:CtrlDate runat="server" ID="CtrlToDate" IsVisibleDate="true" />
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
                                        <asp:Button ID="BtnSearch1" runat="server" Text="Search" CssClass="btn btn-add w-100" OnClick="BtnSearch1_Click" />
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
                                                <asp:Label ID="LnkMail" runat="server" CommandName="SELECT" Text='<%# FnDateTime( Eval("FirstOrderDate"),"dd-MM-yyyy hh:mm:tt") %>' Width="150px"></asp:Label>
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
                                                <asp:Label ID="LnkSubject" runat="server" CommandName="SELECT" Text='<%# Eval("POType") %>' Width="100px"></asp:Label>
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

                                <!-- 🌟 PDF PANEL -->
                                <ajaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>
                                        PDF
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="tab-pane-content p-3" style="max-height: 85vh;">
                                            <%--<ajaxToolkit:AsyncFileUpload ID="FileUploadDoc" runat="server" class="file-input-label" OnUploadedComplete="FileUploadDoc_UploadedComplete" />--%>
                                            <asp:HiddenField ID="HdnFilePath" runat="server" />
                                            <%--<asp:LinkButton ID="LnkPDFSubmit" runat="server" CssClass="btn submit-btnsave" Text="<i class='fas fa-paper-plane'></i> AI Convert" OnClick="LnkPDFSubmit_Click"></asp:LinkButton>--%>
                                            <asp:Label ID="LblResult" runat="server" CssClass="text-danger mt-2"> </asp:Label>
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
</asp:Content>
