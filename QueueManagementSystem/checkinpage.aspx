<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="checkinpage.aspx.cs" Inherits="QueueManagementSystem.checkinpage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Portfolio Grid-->
    <section class="page-section bg-light" id="portfolio">
      <div class="container">
        <div class="text-center">
          <h2 class="section-heading text-uppercase">Check In Page</h2>
          <h3 class="section-subheading text-muted">
            Lorem ipsum dolor sit amet consectetur.
          </h3>
        </div>
        <div class="row">
          <div class="col-lg-4 col-sm-6 mb-4">
            <div class="portfolio-item">
              <a
                class="portfolio-link"
                data-toggle="modal"
                href="#portfolioModal1"
              >
                <div class="portfolio-hover">
                  <div class="portfolio-hover-content">
                    <i class="fas fa-plus fa-3x"></i>
                  </div>
                </div>
                <img
                  class="img-fluid bg-white"
                  src="images/cash%20%20withdrawal-2.png"
                  alt=""
                />
              </a>
              <div class="portfolio-caption">
                <div class="portfolio-caption-heading">Withdrawal</div>
                <div class="portfolio-caption-subheading text-muted">
                  Cut a cash withdrawal ticket
                </div>
              </div>
            </div>
          </div>
          <div class="col-lg-4 col-sm-6 mb-4">
            <div class="portfolio-item">
              <a
                class="portfolio-link"
                data-toggle="modal"
                href="#portfolioModal2"
              >
                <div class="portfolio-hover">
                  <div class="portfolio-hover-content">
                    <i class="fas fa-plus fa-3x"></i>
                  </div>
                </div>
                <img
                  class="img-fluid"
                  src="images/cash%20deposit.jpg"
                  alt=""
                />
              </a>
              <div class="portfolio-caption">
                <div class="portfolio-caption-heading">Deposit</div>
                <div class="portfolio-caption-subheading text-muted">
                  Cut a cash deposit ticket
                </div>
              </div>
            </div>
          </div>
          <div class="col-lg-4 col-sm-6 mb-4">
            <div class="portfolio-item">
              <a
                class="portfolio-link"
                data-toggle="modal"
                href="#portfolioModal3"
              >
                <div class="portfolio-hover">
                  <div class="portfolio-hover-content">
                    <i class="fas fa-plus fa-3x"></i>
                  </div>
                </div>
                <img
                  class="img-fluid bg-white"
                  src="images/transaction.png"
                  alt=""
                />
              </a>
              <div class="portfolio-caption">
                <div class="portfolio-caption-heading">Transaction</div>
                <div class="portfolio-caption-subheading text-muted">
                  Cut a transaction ticket
                </div>
              </div>
            </div>
          </div>
          <div class="col-lg-4 col-sm-6 mb-4 mb-lg-0">
            <div class="portfolio-item">
              <a
                class="portfolio-link"
                data-toggle="modal"
                href="#portfolioModal4"
              >
                <div class="portfolio-hover">
                  <div class="portfolio-hover-content">
                    <i class="fas fa-plus fa-3x"></i>
                  </div>
                </div>
                <img
                  class="img-fluid"
                  src="images/buy%20inventory.jpeg"
                  alt=""
                />
              </a>
              <div class="portfolio-caption">
                <div class="portfolio-caption-heading">Buy Inventory</div>
                <div class="portfolio-caption-subheading text-muted">
                  Cut a ticket to buy inventory
                </div>
              </div>
            </div>
          </div>
          <div class="col-lg-4 col-sm-6 mb-4 mb-sm-0">
            <div class="portfolio-item">
              <a
                class="portfolio-link"
                data-toggle="modal"
                href="#portfolioModal5"
              >
                <div class="portfolio-hover">
                  <div class="portfolio-hover-content">
                    <i class="fas fa-plus fa-3x"></i>
                  </div>
                </div>
                <img
                  class="img-fluid"
                  src="images/sell%20inventory.jpg"
                  alt=""
                />
              </a>
              <div class="portfolio-caption">
                <div class="portfolio-caption-heading">Sell Inventory</div>
                <div class="portfolio-caption-subheading text-muted">
                   Cut a ticket to sell inventory
                </div>
              </div>
            </div>
          </div>
          <div class="col-lg-4 col-sm-6">
            <div class="portfolio-item">
              <a
                class="portfolio-link"
                data-toggle="modal"
                href="#portfolioModal6"
              >
                <div class="portfolio-hover">
                  <div class="portfolio-hover-content">
                    <i class="fas fa-plus fa-3x"></i>
                  </div>
                </div>
                <img
                  class="img-fluid"
                  src="images/manage%20stocks.jpg"
                  alt=""
                />
              </a>
              <div class="portfolio-caption">
                <div class="portfolio-caption-heading">Manege Stocks</div>
                <div class="portfolio-caption-subheading text-muted">
                  Cut a ticket for your stock management
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
</asp:Content>
