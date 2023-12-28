<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="usersignup.aspx.cs" Inherits="QueueManagementSystem.usersignup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
  <div class="row">
    <div class="col-md-8 mx-auto">
      <div class="card">
        <div class="card-body">
          <div class="row">
            <div class="col">
              <center>
                <img src="images/generaluser.png" width="100px" />
              </center>
            </div>
          </div>
          <div class="row">
            <div class="col">
              <center>
                <h4>User Registration</h4>
              </center>
            </div>
          </div>

          <div class="row">
            <div class="col">
              <hr />
            </div>
          </div>

          <div class="row">
            <div class="col-md-6">
              <label>Member ID</label>
              <div class="form-group">
                <asp:TextBox
                  ID="TextBox1"
                  class="form-control"
                  placeholder="Full Name"
                  runat="server"
                >
                </asp:TextBox>
              </div>
            </div>

            <div class="col-md-6">
              <label>Date of Birth</label>
              <div class="form-group">
                <asp:TextBox
                  ID="TextBox3"
                  class="form-control"
                  placeholder="Date of Birth"
                  TextMode="Date"
                  runat="server"
                >
                </asp:TextBox>
              </div>
            </div>
          </div>

          <div class="row">
            <div class="col-md-6">
              <label>Contact No</label>
              <div class="form-group">
                <asp:TextBox
                  ID="TextBox4"
                  class="form-control"
                  placeholder="Contact Number"
                  TextMode="Number"
                  runat="server"
                >
                </asp:TextBox>
              </div>
            </div>

            <div class="col-md-6">
              <label>Email</label>
              <div class="form-group">
                <asp:TextBox
                  ID="TextBox5"
                  class="form-control"
                  placeholder="Email"
                  TextMode="Email"
                  runat="server"
                >
                </asp:TextBox>
              </div>
            </div>
          </div>

          <div class="row">
            <div class="col-md-4">
              <label>County</label>
              <div class="form-group">
                <asp:DropDownList
                  ID="DropDownList1"
                  CssClass="form-control"
                  runat="server"
                >
                  <asp:ListItem Text="Select" Value="Select" />
                  <asp:ListItem Text="Baringo" Value="Baringo" />
                  <asp:ListItem Text="Bomet" Value="Bomet" />
                  <asp:ListItem Text="Bungoma" Value="Bungoma" />
                  <asp:ListItem Text="Busia" Value="Busia" />
                  <asp:ListItem
                    Text="Elgeyo-Marakwet"
                    Value="Elgeyo-Marakwet"
                  />
                  <asp:ListItem Text="Embu" Value="Embu" />
                  <asp:ListItem Text="Garissa" Value="Garissa" />
                  <asp:ListItem Text="Homa Bay" Value="Homa Bay" />
                  <asp:ListItem Text="Isiolo" Value="Isiolo" />
                  <asp:ListItem Text="Kajiado" Value="Kajiado" />
                  <asp:ListItem Text="Kakamega" Value="Kakamega" />
                  <asp:ListItem Text="Kericho" Value="Kericho" />
                  <asp:ListItem Text="Kiambu" Value="Kiambu" />
                  <asp:ListItem Text="Kilifi" Value="Kilifi" />
                  <asp:ListItem Text="Kirinyaga" Value="Kirinyaga" />
                  <asp:ListItem Text="Kisii" Value="Kisii" />
                  <asp:ListItem Text="Kisumu" Value="Kisumu" />
                  <asp:ListItem Text="Kitui" Value="Kitui" />
                  <asp:ListItem Text="Kwale" Value="Kwale" />
                  <asp:ListItem Text="Laikipia" Value="Laikipia" />
                  <asp:ListItem Text="Lamu" Value="Lamu" />
                  <asp:ListItem Text="Machakos" Value="Machakos" />
                  <asp:ListItem Text="Makueni" Value="Makueni" />
                  <asp:ListItem Text="Mandera" Value="Mandera" />
                  <asp:ListItem Text="Marsabit" Value="Marsabit" />
                  <asp:ListItem Text="Meru" Value="Meru" />
                  <asp:ListItem Text="Migori" Value="Migori" />
                  <asp:ListItem Text="Mombasa" Value="Mombasa" />
                  <asp:ListItem Text="Murang'a" Value="Murang'a" />
                  <asp:ListItem Text="Nairobi" Value="Nairobi" />
                  <asp:ListItem Text="Nakuru" Value="Nakuru" />
                  <asp:ListItem Text="Nandi" Value="Nandi" />
                  <asp:ListItem Text="Narok" Value="Narok" />
                  <asp:ListItem Text="Nyamira" Value="Nyamira" />
                  <asp:ListItem Text="Nyandarua" Value="Nyandarua" />
                  <asp:ListItem Text="Nyeri" Value="Nyeri" />
                  <asp:ListItem Text="Samburu" Value="Samburu" />
                  <asp:ListItem Text="Siaya" Value="Siaya" />
                  <asp:ListItem Text="Taita-Taveta" Value="Taita-Taveta" />
                  <asp:ListItem Text="Tana River" Value="Tana River" />
                  <asp:ListItem Text="Tharaka-Nithi" Value="Tharaka-Nithi" />
                  <asp:ListItem Text="Trans Nzoia" Value="Trans Nzoia" />
                  <asp:ListItem Text="Turkana" Value="Turkana" />
                  <asp:ListItem Text="Uasin Gishu" Value="Uasin Gishu" />
                  <asp:ListItem Text="Vihiga" Value="Vihiga" />
                  <asp:ListItem Text="Wajir" Value="Wajir" />
                  <asp:ListItem Text="West Pokot" Value="West Pokot" />
                </asp:DropDownList>
              </div>
            </div>

            <div class="col-md-4">
              <label>City</label>
              <div class="form-group">
                <asp:TextBox
                  ID="TextBox7"
                  class="form-control"
                  placeholder="City"
                  runat="server"
                >
                </asp:TextBox>
              </div>
            </div>

            <div class="col-md-4">
              <label>Pincode</label>
              <div class="form-group">
                <asp:TextBox
                  ID="TextBox2"
                  class="form-control"
                  placeholder="Pincode"
                  TextMode="Number"
                  runat="server"
                >
                </asp:TextBox>
              </div>
            </div>
          </div>

          <div class="row">
            <div class="col-12">
              <label>Email</label>
              <div class="form-group">
                <asp:TextBox
                  ID="TextBox6"
                  class="form-control"
                  placeholder="Full Address"
                  TextMode="MultiLine"
                  Rows="2"
                  runat="server"
                >
                </asp:TextBox>
              </div>
            </div>
          </div>

          <div class="row">
            <div class="col-12 text-center">
              <span class="badge badge-pill badge-info text-center"
                >Login Credentials</span
              >
            </div>
          </div>

          <div class="row">
            <div class="col-md-6">
              <label>Username</label>
              <div class="form-group">
                <asp:TextBox
                  ID="TextBox8"
                  class="form-control"
                  placeholder="User Name"
                  runat="server"
                >
                </asp:TextBox>
              </div>
            </div>

            <div class="col-md-6">
              <label>Password</label>
              <div class="form-group">
                <asp:TextBox
                  ID="TextBox9"
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
              <div class="col-md-8 col-lg-6 d-flex justify-content-center mx-auto">
                  <div class="form-group">
                      <a href="usersignup.aspx" class="text-center mx-auto">
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

      <a href="homepage.aspx"><< Back to Home</a><br />
      <br />
    </div>
  </div>
</div>

</asp:Content>
