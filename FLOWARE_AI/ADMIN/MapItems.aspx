
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPageEmpty.Master" AutoEventWireup="true" CodeBehind="MapItems.aspx.cs" Inherits="FLOWARE_AI.ADMIN.MapItems" Async="true" StylesheetTheme="SkinFile" Theme="SkinFile" %>
<%@ Register Src="~/CtrlGrdList.ascx" TagPrefix="uc1" TagName="CtrlGrdList" %>
<%@ Register Src="~/CtrlGridViewList.ascx" TagPrefix="uc2" TagName="CtrlGridViewList" %>
<%@ Register Src="~/CtrlDate.ascx" TagPrefix="uc1" TagName="CtrlDate" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <div class="winbox-content p-4">
            <!-- Header filters (same as modal) -->
            <div class="filter-section mt-3">
                <div class="row g-2">
                    <div class="col-md-3 col-12">
                        <div class="d-flex align-items-center gap-2">
                            <label class="fw-semibold mb-0 text-nowrap">PO NO:</label>
                            <asp:TextBox ID="TxtPOp" runat="server" CssClass="form-control" placeholder="PO No."></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3 col-12">
                        <div class="d-flex align-items-center gap-2">
                            <label class="fw-semibold mb-0 text-nowrap">Order Date:</label>
                            <uc1:CtrlDate runat="server" id="CtrlOrderDate" />
                        </div>
                    </div>
                    <div class="col-md-3 col-12">
                        <div class="d-flex align-items-center gap-2">
                            <label class="fw-semibold mb-0 text-nowrap">Delivery Date:</label>
                            <uc1:CtrlDate runat="server" id="CtrlDelDate" />
                        </div>
                    </div>
                    <div class="col-md-3 col-12">
                        <div class="d-flex align-items-center gap-2">                            
                            <asp:LinkButton ID="BtnSave" runat="server" CssClass="btn Add-btn1" Text="Save Only" OnClick="BtnSave_Click" />
                        </div>
                    </div>
                </div>
                <div class="row g-3 mt-2">
                    <div class="col-md-5 col-12">
                        <div class="d-flex align-items-center gap-2">
                            <label class="fw-semibold mb-0 text-nowrap">Customer:</label>
                            <asp:Label ID="LblCustName" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                     <div class="col-md-1 col-12">
                        <div class="d-flex align-items-center gap-2">                            
                            <uc1:CtrlGrdList runat="server" ID="CtrlGrdCustCode" PlaceHoldr="Code" AccountType="CustomerCodeList" GridWidth="400" />
                        </div>
                    </div>
                    <div class="col-md-3 col-12">
                        <div class="d-flex align-items-center gap-2">                            
                            <uc1:CtrlGrdList runat="server" ID="CtrlGrdCustomer" PlaceHoldr="Customer" AccountType="CustomerList" GridWidth="400" />
                        </div>
                    </div>
                    
                    <div class="col-md-3 col-12">
                        <div class="d-flex align-items-center gap-2">                            
                            <asp:LinkButton ID="BtnApplyMapping" runat="server" CssClass="btn Add-btn" Text="Save & Export To Sysware" OnClick="BtnApplyMapping_Click1" />
                        </div>
                        <asp:HiddenField ID="HdnConfirmZeroQty" runat="server" Value="" />
                    </div>
                </div>
            </div>
            <div style="height: 67vh; overflow: scroll;">
            <!-- Mapping Grid -->
            <asp:GridView ID="GrdMapItems" SkinID="GrdVwMasterNoPageing" runat="server" AutoGenerateColumns="False"  OnPreRender="GrdMapItems_PreRender" OnRowDataBound="GrdMapItems_RowDataBound" OnRowCommand="GrdMapItems_RowCommand" >
                <Columns>
                    <asp:BoundField HeaderText="Sl No" DataField="SlNo" />
                    <asp:TemplateField HeaderText="Code">
                        <ItemTemplate>
                            <asp:HiddenField ID="HdnId" runat="server" Value='<%# Eval("Id") %>' />
                             <asp:HiddenField ID="HdnItemId" runat="server" Value='<%# Eval("ItemId") %>' />
                            <asp:HiddenField ID="HdnUomId" runat="server" Value='<%# Eval("UomId") %>' />
                            <asp:HiddenField ID="HdnAmt" runat="server" Value='' /> 
                            <asp:Label ID="LblCode" runat="server" Text='<%# Eval("AICode") %>' Width="60px"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label ID="LblName" runat="server" Text='<%# Eval("AIItemName") %>' Width="200px"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle Width="60px" />
                        <HeaderTemplate>
                            <div style="width: 60px;">
                                F Code
                            </div>
                        </HeaderTemplate>
                        <ItemStyle Width="60px" />
                        <ItemTemplate>
                            <uc2:CtrlGridViewList runat="server" ID="CtrlGrdViewItemCode" PlaceHoldr="Item Code" AccountType="ItemCodeList" ClientIDMode="Predictable" SelectedValue='<%# Eval("ItemId") %>' SelectedText='<%# Eval("ItemCode") %>'  />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Floware Item">
                        <ItemTemplate>
                            <uc2:CtrlGridViewList runat="server" ID="CtrlGrdViewItemDtl" PlaceHoldr="Item Name" AccountType="ItemRegList" ClientIDMode="Predictable" SelectedValue='<%# Eval("ItemId") %>' SelectedText='<%# Eval("ItemName") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="UOM">
                        <ItemTemplate>
                            <asp:Label ID="LblUom" runat="server" Text='<%# Eval("UOM") %>' Width="50px"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle Width="70px" />
                        <HeaderTemplate>
                            <div style="width: 70px;">
                                F UOM
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <uc2:CtrlGridViewList runat="server" ID="CtrlGrdViewUom" PlaceHoldr="Uom" AccountType="UomList" ClientIDMode="Predictable" SelectedValue='<%# Eval("UomId") %>' SelectedText='<%# Eval("UomName") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Qty">
                        <ItemTemplate>
                            <asp:TextBox ID="TxtQty" runat="server" Text='<%# Eval("Quantity") %>' Width="50px"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price">
                        <ItemTemplate>
                            <asp:TextBox ID="TxtPrice" runat="server" Text='<%# Eval("Price") %>' Width="90px"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Remarks">
                        <ItemTemplate>
                            <asp:TextBox ID="TxtRemarks" runat="server" Text='<%# Eval("ItemRemarks") %>' Width="80px"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Delete">
                        <ItemTemplate>
                            <asp:Button ID="BtnDelete" runat="server" CssClass="btn btn-delete w-100 redcolor" Text="Delete" CommandName="DeleteMail" CommandArgument='<%# Eval("Id") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
                </div>
            <!-- Save Button -->           
        </div>  

    <script>
        // Access WinBox instance from parent window
        var winbox = window.parent ? window.parent.winboxInstance : null;
    </script>
   
</asp:Content>

