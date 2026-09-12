using WebSocketSharp;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Content("""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">

        <meta
            name="viewport"
            content="width=device-width, initial-scale=1.0, viewport-fit=cover"
        >

        <title>Home Dashboard</title>

        <style>
            * {
                box-sizing: border-box;
            }

            html,
            body {
                margin: 0;
                padding: 0;
                min-height: 100%;
                font-family:
                    -apple-system,
                    BlinkMacSystemFont,
                    "Segoe UI",
                    Roboto,
                    Helvetica,
                    Arial,
                    sans-serif;
                background: #101318;
                color: #ffffff;
            }

            body {
                min-height: 100vh;
                min-height: 100dvh;
                padding:
                    max(20px, env(safe-area-inset-top))
                    max(20px, env(safe-area-inset-right))
                    max(20px, env(safe-area-inset-bottom))
                    max(20px, env(safe-area-inset-left));
            }

            .dashboard {
                width: 100%;
                max-width: 900px;
                margin: 0 auto;
            }

            .header {
                margin-bottom: 24px;
            }

            .header h1 {
                margin: 0;
                font-size: clamp(28px, 6vw, 42px);
                font-weight: 700;
                letter-spacing: -0.5px;
            }

            .header p {
                margin: 6px 0 0;
                color: #9299a6;
                font-size: 16px;
            }

            .status-card {
                display: flex;
                align-items: center;
                gap: 12px;

                background: #191d24;
                border: 1px solid #292f38;
                border-radius: 16px;

                padding: 16px 18px;
                margin-bottom: 20px;
            }

            .status-indicator {
                width: 12px;
                height: 12px;
                border-radius: 50%;
                background: #555d68;
                flex-shrink: 0;
            }

            .status-indicator.online {
                background: #35d07f;
                box-shadow: 0 0 12px rgba(53, 208, 127, 0.5);
            }

            .status-text {
                display: flex;
                flex-direction: column;
                gap: 2px;
            }

            .status-title {
                font-size: 15px;
                font-weight: 600;
            }

            .status-description {
                font-size: 13px;
                color: #9299a6;
            }

            .controls {
                display: grid;
                grid-template-columns: repeat(2, 1fr);
                gap: 16px;
            }

            .control-card {
                background: #191d24;
                border: 1px solid #292f38;
                border-radius: 20px;
                padding: 16px;
            }

            .control-title {
                margin: 0 0 12px;
                font-size: 15px;
                font-weight: 600;
                color: #b8bec8;
            }

            .control-button {
                width: 100%;
                min-height: 150px;

                border: none;
                border-radius: 16px;

                font-size: clamp(20px, 4vw, 26px);
                font-weight: 700;

                color: #ffffff;
                background: #242a33;

                cursor: pointer;
                user-select: none;
                -webkit-user-select: none;
                -webkit-tap-highlight-color: transparent;

                transition:
                    transform 0.08s ease,
                    background 0.15s ease,
                    opacity 0.15s ease;
            }

            .control-button:hover {
                background: #2c333e;
            }

            .control-button:active {
                transform: scale(0.97);
            }

            .control-button:disabled {
                opacity: 0.5;
                cursor: not-allowed;
            }

            .open-button {
                background: #176b45;
            }

            .open-button:hover {
                background: #1b7b50;
            }

            .close-button {
                background: #8a2929;
            }

            .close-button:hover {
                background: #9e3030;
            }

            .message {
                min-height: 24px;
                margin-top: 20px;

                text-align: center;
                font-size: 14px;
                color: #9299a6;
            }

            @media (max-width: 600px) {
                body {
                    padding:
                        max(14px, env(safe-area-inset-top))
                        max(14px, env(safe-area-inset-right))
                        max(14px, env(safe-area-inset-bottom))
                        max(14px, env(safe-area-inset-left));
                }

                .header {
                    margin-bottom: 18px;
                }

                .status-card {
                    margin-bottom: 14px;
                }

                .controls {
                    grid-template-columns: 1fr;
                    gap: 12px;
                }

                .control-card {
                    padding: 12px;
                }

                .control-button {
                    min-height: 130px;
                }
            }

            @media (max-width: 380px) {
                .control-button {
                    min-height: 110px;
                }
            }
        </style>
    </head>

    <body>
        <main class="dashboard">

            <header class="header">
                <h1>Home Dashboard</h1>
                <p>Control your connected devices</p>
            </header>

            <section class="status-card">
                <div id="statusIndicator" class="status-indicator online"></div>

                <div class="status-text">
                    <span class="status-title">System Online</span>
                    <span id="statusDescription" class="status-description">
                        Dashboard is connected
                    </span>
                </div>
            </section>

            <section class="controls">

                <div class="control-card">
                    <h2 class="control-title">Door</h2>

                    <button
                        id="openButton"
                        class="control-button open-button"
                        onclick="clickOpen()"
                    >
                        Open Door
                    </button>
                </div>

                <div class="control-card">
                    <h2 class="control-title">Door</h2>

                    <button
                        id="closeButton"
                        class="control-button close-button"
                        onclick="clickClose()"
                    >
                        Close Door
                    </button>
                </div>

            </section>

            <div id="message" class="message"></div>

        </main>

        <script>
            const openButton = document.getElementById("openButton");
            const closeButton = document.getElementById("closeButton");
            const message = document.getElementById("message");

            async function sendCommand(url, button) {
                openButton.disabled = true;
                closeButton.disabled = true;

                message.textContent = "Sending command...";

                try {
                    const response = await fetch(url);

                    if (!response.ok) {
                        throw new Error("Server returned " + response.status);
                    }

                    message.textContent = "Command sent successfully.";
                }
                catch (error) {
                    message.textContent = "Failed to send command.";
                    console.error(error);
                }
                finally {
                    openButton.disabled = false;
                    closeButton.disabled = false;
                }
            }

            async function clickOpen() {
                await sendCommand("buttonOpen", openButton);
            }

            async function clickClose() {
                await sendCommand("buttonClose", closeButton);
            }
        </script>
    </body>
    </html>
    """, "text/html"));

app.MapGet("/buttonOpen", () =>
{
    WebsocketManager.SendToServer("send door open");

    return Results.Ok();
});

app.MapGet("/buttonClose", () =>
{
    WebsocketManager.SendToServer("send door close");

    return Results.Ok();
});

app.Run("http://localhost:5000");


public static class WebsocketManager
{
    public static void SendToServer(string msg)
    {
        using var ws = new WebSocket("ws://127.0.0.1:6969");

        ws.Connect();

        ws.Send("name web");
        ws.Send(msg);
    }
}