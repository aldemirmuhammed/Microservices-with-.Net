
var connection = new signalR.HubConnectionBuilder().withUrl("http://localhost:5020/NotificationHub", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
}).build();

if (connection.state !== HubConnectionState.Connected || connection.state !== HubConnectionState.Connecting) {
    connection.start().then(() => {
        console.log("Connection state : " + connection.state);

    });
}
else {
    console.log("Connection already established");
}

connection.on("ReceiveNotificationAll", message => {
    let msg = JSON.parse(message);
    let defaultdiv = document.getElementById("defaultDiv");
    if (defaultdiv != null) {
        defaultdiv.remove()
    }

    $('#listName').append(`<div class="notificationContainer">
                                                            <div class="notificationCard unread">
                                                                        <img alt="photo" src="~/notification.png" />
                                                                <div class="description">
                                                                    <p>${msg.Title}</p>
                                                                    <p>${msg.Message}</p>
                                                                    <p id="notif-time">${msg.CreatedAt}</p>
                                                                </div>
                                                            </div>
                                                        </div>`);






    //$('#listName').append(`<li class="list-group-item"> ${message}</li>`);
    //$("#messages").append(`${message}<br>`);
});