<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="userlogin.aspx.cs" Inherits="QueueManagementSystem.userlogin" %>
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
                    <img src="images/generaluser.png" width="150px" />
                  </center>
                </div>
              </div>
              <div class="row">
                <div class="col">
                  <center>
                    <h3>Member Login</h3>
                  </center>
                </div>
              </div>
              <div class="row">
                <div class="col">
                  <hr />
                </div>
              </div>
              <div class="row">
                <div class="col">
                  <label>Member ID</label>
                  <div class="form-group">
                    <asp:TextBox
                      ID="TextBox1"
                      class="form-control"
                      placeholder="Member ID"
                      runat="server"
                    >
                    </asp:TextBox>
                  </div>

                  <label>Member Password</label>
                  <div class="form-group">
                    <asp:TextBox
                      ID="TextBox2"
                      class="form-control"
                      placeholder="Password"
                      TextMode="Password"
                      runat="server"
                    >
                    </asp:TextBox>
                  </div>
                </div>
              </div>

                <div class="row pt-3">
                    <div class="col-md-8 col-lg-6 d-flex mx-auto justify-content-center">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                  <asp:Button
                                    class="btn btn-primary btn-block"           
                                    ID="Button1"
                                    runat="server"
                                    Text="Login"
                                  />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                  <a href="usersignup.aspx">
                                    <input
                                      type="button"
                                      class="btn btn-info btn-block"
                                      id="Button2"
                                      name="signup"
                                      value="Sign Up"
                                    />
                                  </a>
                                </div>
                            </div>
                        </div>
                        
                    </div>
                </div>
            </div>
          </div>

          <a href="homepage.aspx"><< Back to Home</a><br />
          <br />
        </div>
      </div>
    </div>

</asp:Content>
