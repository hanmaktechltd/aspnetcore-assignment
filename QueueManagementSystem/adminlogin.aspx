<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="adminlogin.aspx.cs" Inherits="QueueManagementSystem.adminlogin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="row">
            <div class="col-md-6 mx-auto">
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col">
                                <center>
                                    <img src="images/adminuser.jpg" width="150px" />
                                </center>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col">
                                <center>
                                    <h3>Admin Login</h3>
                                </center>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col">
                               <hr />
                            </div>
                        </div>
                        <div class="row">
                                <div class="form-group">
                                    <asp:TextBox ID="TextBox1" class="form-control" placeholder="Admin ID" runat="server">

                                    </asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <asp:TextBox ID="TextBox2" class="form-control" placeholder="Password" TextMode="Password" runat="server">

                                    </asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button1" runat="server" Text="Login" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <a href="homepage.aspx" ><< Back to Home</a><br /> <br />
            </div>
        </div>
    </div>
</asp:Content>
