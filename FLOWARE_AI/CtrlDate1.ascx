<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CtrlDate1.ascx.cs" Inherits="FLOWARE_AI.CtrlDate" %>
<table border="0" cellpadding="0" cellspacing="0" style="width:100%">
    <tr>
        <td>
            <div class="date-input-container">
                <asp:TextBox ID="TxtDate" runat="server" placeholder="dd/mmm/yyyy" CssClass="datepicker-crm" />
                <i class="fa fa-calendar calendar-icon" aria-hidden="true"></i>
            </div>
        </td>
    </tr>
</table>
<asp:PlaceHolder ID="CDdlr" runat="server"></asp:PlaceHolder>