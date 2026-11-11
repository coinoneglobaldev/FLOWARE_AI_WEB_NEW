<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="DashboardNew.aspx.cs" Inherits="FLOWARE_AI.DashBoard.DashboardNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Content Section -->
    <section class="content-section">
        <div class="container">
            <!-- Page Header -->
            <div class="page-header">
                <h1 class="page-title">Dashboard</h1>
            </div>

            <!-- Dashboard Stats -->
            <div class="stats-container" >
                <a id="lnkTotalOrders" runat="server">
                    <div class="stat-card orders">
                        <div class="stat-icon">
                            <i class="fas fa-shopping-cart"></i>
                        </div>
                        <div class="stat-label">Total Orders</div>
                        <div class="stat-value">
                            <asp:Label ID="LblTotOrder" runat="server" Text="0"></asp:Label>
                        </div>
                        <div class="stat-note">Updated today</div>
                    </div>
                </a>
                <a id="LnkExpPend" runat="server">
                    <div class="stat-card expend">
                        <div class="stat-icon">
                            <i class="fas fa-clock"></i>
                        </div>
                        <div class="stat-label">Export Pending</div>
                        <div class="stat-value">
                            <asp:Label ID="LblExpPend" runat="server" Text="0"></asp:Label>
                        </div>
                        <div class="stat-note">Updated today</div>
                    </div>
                </a>
                <a id="LnkMapPend" runat="server">
                    <div class="stat-card mapend">
                        <div class="stat-icon">
                            <i class="fas fa-hourglass-end"></i>
                        </div>
                        <div class="stat-label">Mapping Pending</div>
                        <div class="stat-value">
                            <asp:Label ID="LblMappPend" runat="server" Text="0"></asp:Label>
                        </div>
                        <div class="stat-note">Updated today</div>
                    </div>
                </a>
            </div>
            <div class="stats-container" >
                <a id="LnkExpComp" runat="server">
                    <div class="stat-card expcomp">
                        <div class="stat-icon">
                            <i class="fas fa-check-circle"></i>
                        </div>
                        <div class="stat-label">Export Completed</div>
                        <div class="stat-value">
                            <asp:Label ID="LblExpComp" runat="server" Text="0"></asp:Label>
                        </div>
                        <div class="stat-note">Updated today</div>
                    </div>
                </a>
                <a id="LnkTotPrdct" runat="server"> 
                    <div class="stat-card products">
                        <div class="stat-icon">
                            <i class="fas fa-box"></i>
                        </div>
                        <div class="stat-label">Total Products</div>
                        <div class="stat-value">
                            <asp:Label ID="LblTotProducts" runat="server" Text="0"></asp:Label>
                        </div>
                        <div class="stat-note">Updated today</div>
                    </div>
                </a>

                <a id="LnkTotCust" runat="server">
                    <div class="stat-card customers">
                        <div class="stat-icon">
                            <i class="fas fa-users"></i>
                        </div>
                        <div class="stat-label">Total Customers</div>
                        <div class="stat-value">
                            <asp:Label ID="LblTotCust" runat="server" Text="0"></asp:Label>
                        </div>
                        <div class="stat-note">Updated today</div>
                    </div>
                </a>
            </div>
        </div>
    </section>
</asp:Content>
