const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/ticketHub")
    .build();

hubConnection.on("ReceiveCalledTicket", function (ticketId, servicePoint) {
    $("#calledTicketSection").html(`<p>Called Ticket Number: ${ticketId}</p>
    <p>Service Point: ${servicePoint}</p>
    `);
});

hubConnection.start();
