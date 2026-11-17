const connectionStatus = document.getElementById("connectionStatus");
const messagesList = document.getElementById("messagesList");
const messageForm = document.getElementById("messageForm");
const messageInput = document.getElementById("messageInput");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveMessage", (userName, message) => {
    const item = document.createElement("li");
    item.className = "list-group-item";

    const user = document.createElement("strong");
    user.textContent = userName + ": ";

    const text = document.createTextNode(message);

    item.appendChild(user);
    item.appendChild(text);

    messagesList.appendChild(item);
});

async function startConnection() {
    try {
        await connection.start();
        connectionStatus.textContent = "Подключено";
        connectionStatus.className = "text-success mb-3";
    } catch (error) {
        connectionStatus.textContent = "Ошибка подключения";
        connectionStatus.className = "text-danger mb-3";

        console.error(error);
        setTimeout(startConnection, 5000);
    }
}

messageForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    const message = messageInput.value.trim();

    if (!message || connection.state !== signalR.HubConnectionState.Connected)
        return;

    try {
        await connection.invoke("SendMessage", message);
        messageInput.value = "";
    } catch (error) {
        console.error(error);
    }
});

connection.onreconnecting(() => {
    connectionStatus.textContent = "Переподключение...";
    connectionStatus.className = "text-warning mb-3";
});

connection.onreconnected(() => {
    connectionStatus.textContent = "Подключено";
    connectionStatus.className = "text-success mb-3";
});

startConnection();