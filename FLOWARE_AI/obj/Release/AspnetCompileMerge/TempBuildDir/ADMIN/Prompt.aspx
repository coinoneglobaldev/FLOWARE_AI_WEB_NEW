<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="Prompt.aspx.cs" Inherits="FLOWARE_AI.ADMIN.Prompt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="stylesheet" href="css/export.css">
    <section class="content-section">
        <div class="container">
            <!-- Export Section -->
            <div class="export-section">
                <div class="row">                    
                    <div class="col-lg-12 col-md-12">
                        <div class="export-tabs">
                            
                            <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="ajax-tab-container_export" AutoPostBack="True">
                                <!-- 🌟 TEXT PANEL -->
                                <ajaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>
                                        Text
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="tab-pane-content p-3">
                                            <asp:HiddenField ID="HdnText" runat="server" />
                                            <asp:TextBox runat="server" ID="TxtTextInput" TextMode="MultiLine" Rows="15" placeholder="Enter your text heree..."></asp:TextBox>
                                            <asp:LinkButton ID="BtnTxtSubmit" runat="server" CssClass="btn submit-btn" Text="<i class='fa fa-check-circle-o'></i> UPDATE" OnClick="BtnTxtSubmit_Click"></asp:LinkButton>
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
                                            <asp:HiddenField ID="HdnPDF" runat="server" />
                                            <asp:TextBox runat="server" ID="TxtPDFInput" TextMode="MultiLine" Rows="22" placeholder="Enter your text heree..."></asp:TextBox>
                                            <asp:LinkButton ID="BtnPDFSubmit" runat="server" CssClass="btn submit-btn" Text="<i class='fa fa-check-circle-o'></i> UPDATE" OnClick="BtnPDFSubmit_Click"></asp:LinkButton>
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
                                            <asp:HiddenField ID="HdnLink" runat="server" />
                                            <asp:TextBox runat="server" ID="TxtLink" Rows="22" placeholder="Enter link or URL here..." TextMode="MultiLine"> </asp:TextBox>
                                            <asp:LinkButton ID="BtnLinkSubmit" runat="server" CssClass="btn submit-btn" Text="<i class='fa fa-check-circle-o'></i> UPDATE" OnClick="BtnLinkSubmit_Click"></asp:LinkButton>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                 <!-- 🌟 MAIL BODY PANEL -->
                                <ajaxToolkit:TabPanel ID="TabPanel4" runat="server">
                                    <HeaderTemplate>
                                        Mail Body
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="tab-pane-content p-3">
                                            <asp:HiddenField ID="HdnMBody" runat="server" />
                                            <asp:TextBox runat="server" ID="TxtMailBody" TextMode="MultiLine" Rows="22" placeholder="Enter your Mail Body here..."></asp:TextBox>
                                            <asp:LinkButton ID="LnkBtnMBody" runat="server" CssClass="btn submit-btn" Text="<i class='fa fa-check-circle-o'></i> UPDATE" OnClick="LnkBtnMBody_Click"></asp:LinkButton>
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
