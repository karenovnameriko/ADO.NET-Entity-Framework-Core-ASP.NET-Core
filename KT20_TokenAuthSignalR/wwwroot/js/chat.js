
const token = sessionStorage.getItem("jwtToken");
const userName = sessionStorage.getItem("userName");

if (!token) {
    window.location.replace("/Account/Login");
} else {
    document.getElementById("currentUser").textContent = userName;

    const connectionStatus = document.getElementById("connectionStatus");
    const messagesList = document.getElementById("messagesList");
    const messageForm = document.getElementById("messageForm");
    const messageInput = document.getElementById("messageInput");
    const logoutButton = document.getElementById("logoutButton");

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub", {
            accessTokenFactory: () => sessionStorage.getItem("jwtToken")
        })
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveMessage", (userName, message) => {
        const item = document.createElement("li");
        item.className = "list-group-item";

        const user = document.createElement("strong");
        user.textContent = userName + ": ";

        item.appendChild(user);
        item.appendChild(document.createTextNode(message));

        messagesList.appendChild(item);
    });

    async function startConnection() {
        try {
            await connection.start();

            connectionStatus.textContent = "Подключено";
            connectionStatus.className = "text-success mb-3";
        } catch (error) {
            console.error(error);

            connectionStatus.textContent = "Ошибка подключения";
            connectionStatus.className = "text-danger mb-3";

            if (error.statusCode === 401 || error.statusCode === 403) {
                sessionStorage.clear();
                window.location.replace("/Account/Login");
                return;
            }

            setTimeout(startConnection, 5000);
        }
    }

    messageForm.addEventListener("submit", async (event) => {
        event.preventDefault();

        const message = messageInput.value.trim();

        if (!message ||
            connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

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

    connection.onclose(() => {
        connectionStatus.textContent = "Соединение закрыто";
        connectionStatus.className = "text-danger mb-3";
    });

    logoutButton.addEventListener("click", async () => {
        await connection.stop();

        sessionStorage.clear();
        window.location.replace("/Account/Login");
    });

    startConnection();
}
