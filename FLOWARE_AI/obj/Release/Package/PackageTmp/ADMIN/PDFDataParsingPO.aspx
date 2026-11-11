<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="PDFDataParsingPO.aspx.cs" Inherits="FLOWARE_AI.ADMIN.PDFDataParsingPO" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <section class="content-section">
        <div class="container">
            <!-- Page Header -->
            <div class="page-header">
                <h1 class="page-title">Order Parsing</h1>
            </div>
            </div>
        </section>
    <div style="display:grid; grid-template-columns: 1fr 2fr; gap:30px; max-width:1400px; margin:auto; font-family:Arial;">

        <!-- Column 1: Upload + Text Input -->
        <div style="background:#fdfdfd; padding:15px; border-radius:8px; border:1px solid #ddd;">
            <h3 style="margin-bottom:15px;">Upload PDF or Enter Text</h3>

            <!-- Upload PDF -->
            <div style="border: 2px dashed #ccc; border-radius: 8px; padding: 20px; text-align: center; background: #f9f9f9;">
              <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:FileUpload ID="fileUploadPdf" runat="server" Style="margin-bottom: 10px;" />
                    <br />
                    <asp:Button ID="btnUploadPdf" runat="server" Text="Upload PDF File" CssClass="btn btn-primary"
                        OnClick="btnUploadPdf_Click" />
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnUploadPdf" />
                </Triggers>
              </asp:UpdatePanel>
            </div>
         <%--  --%>

            <!-- 🔹 New Prompt Entry -->
            <div style="margin-bottom:15px; display:none;" >
                <label for="txtPrompt" style="font-weight:bold;" >Enter Prompt Data:</label><br />
                <asp:TextBox ID="txtPrompt" runat="server" TextMode="MultiLine" Rows="4" Width="100%" 
                    Style="border:1px solid #ccc; border-radius:6px; padding:10px;"></asp:TextBox>
            </div>
             <div style="text-align:center; margin:15px 0; font-weight:bold; color:#888;">— OR —</div>

            <!-- 🔹 Chat Data Entry -->
            <div style="margin-bottom:15px;">
                <label for="txtChatData" style="font-weight:bold;">Paste Chat Data (to convert to JSON):</label><br />
                <asp:TextBox ID="txtChatData" runat="server" TextMode="MultiLine" Rows="6" Width="100%" 
                    Style="border:1px solid #ccc; border-radius:6px; padding:10px;"></asp:TextBox>
            </div>

            <!-- Buttons -->
            <div style="text-align:center; margin-top:15px;">
              
                <asp:Button ID="BtnConvertJson" runat="server" Text="Convert and Display" CssClass="btn btn-info" 
                    OnClick="btnConvertJson_Click" />
            </div>


             <!-- 🆕 New Section: URL to JSON -->
            <hr style="margin:20px 0;" />
            <div style="margin-top:10px;">
                <label for="txtUrl" style="font-weight:bold;">Enter URL (to convert to JSON):</label><br />
                <asp:TextBox ID="txtUrl" runat="server" Width="100%" CssClass="form-control"
                    placeholder="Enter webpage URL like https://vendor.marketman.com/..."></asp:TextBox>
            </div>

            <div style="text-align:center; margin-top:15px;">
                <asp:Button ID="BtnConvertUrl" runat="server" Text="Convert URL to JSON" CssClass="btn btn-success"
                    OnClick="BtnConvertUrl_Click" />
            </div>
             <!-- Status Label -->
        <div class="mb-3 text-center">
            <asp:Label ID="lblStatus" runat="server" CssClass="fw-bold"></asp:Label>
        </div>

            <!-- Result -->
            <div style="margin-top:20px;">
                <asp:Label ID="LblResult" runat="server" ForeColor="Green"></asp:Label>
            </div>
        </div>

         <div class="container-fluid mb-2">
             <div class="row align-items-center mb-2">
                 <div class="col-md-4 d-flex align-items-center">
                     <asp:Label ID="lblPONumber" runat="server" Text="PO No: " Font-Bold="true" CssClass="me-4"></asp:Label>
                     <asp:TextBox ID="txtPONumber" runat="server" CssClass="form-control"  Style="width: 150px;"></asp:TextBox>
                 </div>
                 <div class="col-md-4 d-flex align-items-center">
                     <asp:Label ID="lblCreatedDate" runat="server" Text="Created Date: " Font-Bold="true" CssClass="me-2"></asp:Label>
                     <asp:TextBox ID="txtCreatedDate" runat="server" CssClass="form-control"  Style="width: 150px;"></asp:TextBox>
                 </div>
                 <div class="col-md-4 d-flex align-items-center">
                     <asp:Label ID="lblDeliveryDate" runat="server" Text="Delivery Date: " Font-Bold="true" CssClass="me-2"></asp:Label>
                     <asp:TextBox ID="txtDeliveryDate" runat="server" CssClass="form-control"  Style="width: 150px;"></asp:TextBox>
                 </div>
             </div>
             <asp:GridView ID="GridView1" runat="server"
                 AutoGenerateColumns="true"
                 CssClass="table table-bordered table-striped"
                 GridLines="Both" CellPadding="5"
                 OnRowDataBound="GridView1_RowDataBound"
                 OnRowEditing="GridView1_RowEditing"
                 OnRowCancelingEdit="GridView1_RowCancelingEdit"
                 OnRowUpdating="GridView1_RowUpdating"
                 Width="100%">

                 <Columns>
                     <asp:CommandField ShowEditButton="True"
                         ButtonType="Button"
                         EditText="Edit">

                         <HeaderStyle Width="180px" />
                         <%-- width for header cell --%>
                         <ItemStyle Width="180px" HorizontalAlign="Center" />
                         <%-- width for data cell --%>
                     </asp:CommandField>
                 </Columns>
             </asp:GridView>
         </div>
        </div>



</asp:Content>

