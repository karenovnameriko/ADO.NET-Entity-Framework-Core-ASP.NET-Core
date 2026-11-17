
const loginForm = document.getElementById("loginForm");
const registerForm = document.getElementById("registerForm");
const message = document.getElementById("authMessage");

function showMessage(text, isError = true) {
    message.textContent = text;
    message.className = isError
        ? "text-danger mb-3"
        : "text-success mb-3";
}

// Регистрация
if (registerForm) {
    registerForm.addEventListener("submit", async (event) => {
        event.preventDefault();

        const userName = document.getElementById("userName").value;
        const password = document.getElementById("password").value;
        const confirmPassword =
            document.getElementById("confirmPassword").value;

        if (password !== confirmPassword) {
            showMessage("Пароли не совпадают.");
            return;
        }

        try {
            const response = await fetch("/api/auth/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    userName,
                    password,
                    confirmPassword
                })
            });

            const data = await response.json();

            if (!response.ok) {
                showMessage(data.message || "Ошибка регистрации.");
                return;
            }

            showMessage("Регистрация успешна! Перенаправление на вход...", false);

            setTimeout(() => {
                window.location.href = "/Account/Login";
            }, 1000);
        } catch {
            showMessage("Не удалось подключиться к серверу.");
        }
    });
}

// Вход
if (loginForm) {
    loginForm.addEventListener("submit", async (event) => {
        event.preventDefault();

        const userName = document.getElementById("userName").value;
        const password = document.getElementById("password").value;

        try {
            const response = await fetch("/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    userName,
                    password
                })
            });

            if (!response.ok) {
                showMessage("Неверный логин или пароль.");
                return;
            }

            const data = await response.json();

            sessionStorage.setItem("jwtToken", data.token);
            sessionStorage.setItem("userName", data.userName);

            window.location.href = "/Chat";
        } catch {
            showMessage("Не удалось подключиться к серверу.");
        }
    });
}
