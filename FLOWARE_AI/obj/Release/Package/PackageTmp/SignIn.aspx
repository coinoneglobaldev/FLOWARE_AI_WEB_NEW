<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="FLOWARE_AI.SignIn" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login</title>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="css/login.css" rel="stylesheet" />
</head>
<body>
   <form runat="server">
     <div class="container">
         <asp:ScriptManager ID="ScriptManager1" runat="server">
                          </asp:ScriptManager>
         <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
     <div class="login-container">
         <div class="row g-0">
             <div class="col-md-5 d-none d-md-flex login-image">
                 <div class="text-center">
                     <i class="fas fa-user-circle"></i>
                     <h3 class="mt-4">Welcome Back!</h3>
                     <p class="mt-2">Enter your credentials to access your account</p>
                 </div>
             </div>
             <div class="col-md-7">
                 <div class="login-form">
                     <h2 class="mb-4 text-center">Sign In</h2>
                     <form id="loginForm">
                         <div class="mb-3">
                             <label for="email" class="form-label">Email Address</label>
                             <div class="input-group">
                                 <span class="input-group-text"><i class="fas fa-user"></i></span>
                                 <asp:TextBox runat="server" class="form-control" required="uname" ID="TxtUserName" placeholder="Enter Username"></asp:TextBox>
                             </div>
                         </div>
                         <div class="mb-3">
                             <label for="password" class="form-label">Password</label>
                             <div class="input-group">
                                 <span class="input-group-text"><i class="fas fa-lock"></i></span>
                                 <asp:TextBox runat="server" class="form-control" required="pwd" ID="TxtPwd" placeholder="Enter Password" TextMode="Password"></asp:TextBox>
                            <i class="fa fa-eye-slash" id="tooglepswd" style="position: absolute; right: 10px; top: 40%;"></i>
                             </div>
                         </div>
                         <div class="mb-3 form-check">
                             <input type="checkbox" class="form-check-input" id="remember">
                             <label class="form-check-label" for="remember">Remember me</label>
                             <a href="#" class="float-end text-decoration-none">Forgot Password?</a>
                         </div>
                         

                             <asp:Button runat="server" ID="BtnSignIn" class="btn btn-primary btn-login w-100 mb-3" Style="margin-left: 4px;" Text="Sign In" OnClick="BtnSignIn_Click" />
                         </a>
                     </form>

                     <p class="text-center mb-0">
                         Don't have an account? <a href="dashboard.aspx" class="text-decoration-none fw-bold">Sign Up</a>
                     </p>
                 </div>
             </div>
         </div>
     </div>
                        </ContentTemplate>
                        </asp:UpdatePanel>
 </div>
       </form>
    <script src="JS/bootstrap.bundle.min.js"></script>
</body>
</html>
