const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/ticketHub")
    .build();

hubConnection.on("ReceiveCalledTicket", function (ticketId, servicePoint) {

   
    $("#calledTicketSection").html(`<p>Called Ticket Number: ${ticketId}</p>
    <p>Service Point: ${servicePoint}</p>
    `);

    speakTicketAnnouncement(ticketId, servicePoint); 
});

hubConnection.start();

function speakTicketAnnouncement(ticketId, servicePoint) {

    const textToSpeak = `Ticket number ${ticketId} please proceed to ${servicePoint}`;
    const utterance = new SpeechSynthesisUtterance(textToSpeak);

    utterance.rate = 0.8;

    window.speechSynthesis.speak(utterance);
}