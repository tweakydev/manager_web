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

                transition:
                    background 0.2s ease,
                    box-shadow 0.2s ease;
            }

            .status-indicator.online {
                background: #35d07f;
                box-shadow: 0 0 12px rgba(53, 208, 127, 0.5);
            }

            .status-text {
                display: flex;
                flex-direction: column;
                gap: 2px;
                flex: 1;
            }

            .status-title {
                font-size: 15px;
                font-weight: 600;
            }

            .status-description {
                font-size: 13px;
                color: #9299a6;
            }

            /* Small refresh button */

            .refresh-button {
                width: 38px;
                height: 38px;

                display: flex;
                align-items: center;
                justify-content: center;

                border: 1px solid #343b46;
                border-radius: 10px;

                background: #242a33;
                color: #b8bec8;

                font-size: 20px;
                line-height: 1;

                cursor: pointer;
                user-select: none;
                -webkit-user-select: none;
                -webkit-tap-highlight-color: transparent;

                transition:
                    background 0.15s ease,
                    transform 0.08s ease,
                    color 0.15s ease;
            }

            .refresh-button:hover {
                background: #2c333e;
                color: #ffffff;
            }

            .refresh-button:active {
                transform: scale(0.92);
            }

            .refresh-button:disabled {
                opacity: 0.5;
                cursor: not-allowed;
            }

            .refresh-button.spinning {
                animation: spin 0.7s linear infinite;
            }

            @keyframes spin {
                from {
                    transform: rotate(0deg);
                }

                to {
                    transform: rotate(360deg);
                }
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
                    opacity 0.15s ease,
                    filter 0.15s ease;
            }

            .control-button:hover {
                background: #2c333e;
            }

            .control-button:active {
                transform: scale(0.97);
            }

            .control-button:disabled {
                opacity: 0.35;
                cursor: not-allowed;
                filter: grayscale(0.7);
            }

            .control-button:disabled:hover {
                background: inherit;
            }

            .open-button {
                background: #176b45;
            }

            .open-button:hover {
                background: #1b7b50;
            }

            .open-button:disabled {
                background: #176b45;
            }

            .close-button {
                background: #8a2929;
            }

            .close-button:hover {
                background: #9e3030;
            }

            .close-button:disabled {
                background: #8a2929;
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
                <div id="statusIndicator" class="status-indicator"></div>

                <div class="status-text">
                    <span id="statusTitle" class="status-title">
                        Checking connection...
                    </span>

                    <span id="statusDescription" class="status-description">
                        Checking WebSocket connection
                    </span>
                </div>

                <button
                    id="refreshButton"
                    class="refresh-button"
                    onclick="reconnect()"
                    title="Reconnect"
                    aria-label="Reconnect"
                >
                    ↻
                </button>
            </section>

            <section class="controls">

                <div class="control-card">
                    <h2 class="control-title">Door</h2>

                    <button
                        id="openButton"
                        class="control-button open-button"
                        onclick="clickOpen()"
                        disabled
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
                        disabled
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

            const statusIndicator =
                document.getElementById("statusIndicator");

            const statusTitle =
                document.getElementById("statusTitle");

            const statusDescription =
                document.getElementById("statusDescription");

            const refreshButton =
                document.getElementById("refreshButton");

            const message =
                document.getElementById("message");


            let websocketConnected = false;


            function updateConnectionStatus(connected) {
                websocketConnected = connected;

                openButton.disabled = !connected;
                closeButton.disabled = !connected;

                if (connected) {
                    statusIndicator.classList.add("online");

                    statusTitle.textContent = "System Online";

                    statusDescription.textContent =
                        "Dashboard is connected";
                }
                else {
                    statusIndicator.classList.remove("online");

                    statusTitle.textContent = "System Offline";

                    statusDescription.textContent =
                        "WebSocket is not connected";
                }
            }


            async function checkConnection() {
                try {
                    const response = await fetch(
                        "status",
                        {
                            cache: "no-store"
                        }
                    );

                    if (!response.ok) {
                        updateConnectionStatus(false);
                        return;
                    }

                    const data = await response.json();

                    updateConnectionStatus(data.connected);
                }
                catch (error) {
                    updateConnectionStatus(false);
                    console.error(error);
                }
            }


            async function reconnect() {
                refreshButton.disabled = true;
                refreshButton.classList.add("spinning");

                message.textContent = "Reconnecting...";

                try {
                    const response = await fetch(
                        "reconnect",
                        {
                            method: "POST"
                        }
                    );

                    if (!response.ok) {
                        throw new Error(
                            "Server returned " + response.status
                        );
                    }

                    // Give the server a moment to establish
                    // the new WebSocket connection.
                    await new Promise(resolve =>
                        setTimeout(resolve, 500)
                    );

                    await checkConnection();

                    if (websocketConnected) {
                        message.textContent =
                            "Connection restored.";
                    }
                    else {
                        message.textContent =
                            "Unable to connect.";
                    }
                }
                catch (error) {
                    updateConnectionStatus(false);

                    message.textContent =
                        "Failed to reconnect.";

                    console.error(error);
                }
                finally {
                    refreshButton.disabled = false;
                    refreshButton.classList.remove("spinning");
                }
            }


            async function sendCommand(url) {
                if (!websocketConnected) {
                    message.textContent =
                        "WebSocket is not connected.";

                    return;
                }

                openButton.disabled = true;
                closeButton.disabled = true;

                message.textContent = "Sending command...";

                try {
                    const response = await fetch(url);

                    if (!response.ok) {
                        throw new Error(
                            "Server returned " + response.status
                        );
                    }

                    message.textContent =
                        "Command sent successfully.";
                }
                catch (error) {
                    message.textContent =
                        "Failed to send command.";

                    console.error(error);

                    // Immediately check if the WebSocket
                    // has gone down.
                    await checkConnection();
                }
                finally {
                    // checkConnection() normally sets these,
                    // but make sure the state is correct.
                    openButton.disabled = !websocketConnected;
                    closeButton.disabled = !websocketConnected;
                }
            }


            async function clickOpen() {
                await sendCommand("buttonOpen");
            }


            async function clickClose() {
                await sendCommand("buttonClose");
            }


            // Check immediately when the page loads.
            checkConnection();

            // Keep the UI reactive to the actual server connection.
            setInterval(checkConnection, 2000);
        </script>
    </body>
    </html>
    """, "text/html"));


app.MapGet("/buttonOpen", () =>
{
    if (!WebsocketManager.SendToServer("send door open"))
    {
        return Results.StatusCode(503);
    }

    return Results.Ok();
});


app.MapGet("/buttonClose", () =>
{
    if (!WebsocketManager.SendToServer("send door close"))
    {
        return Results.StatusCode(503);
    }

    return Results.Ok();
});


app.MapGet("/status", () =>
{
    return Results.Ok(new
    {
        connected = WebsocketManager.IsConnected
    });
});


app.MapPost("/reconnect", () =>
{
    var connected = WebsocketManager.Start();

    return connected
        ? Results.Ok()
        : Results.StatusCode(503);
});


WebsocketManager.Start();

app.Run("http://localhost:5000");


public static class WebsocketManager
{
    private static WebSocket? ws;

    private static readonly object lockObject = new();


    public static bool IsConnected
    {
        get
        {
            lock (lockObject)
            {
                return ws != null &&
                       ws.ReadyState == WebSocketState.Open;
            }
        }
    }


    public static bool Start()
    {
        lock (lockObject)
        {
            try
            {
                // Close the previous connection if one exists.
                if (ws != null)
                {
                    try
                    {
                        if (ws.ReadyState == WebSocketState.Open)
                        {
                            ws.Close();
                        }
                    }
                    catch
                    {
                        // Ignore errors closing the old connection.
                    }

                    ws = null;
                }


                var newWs =
                    new WebSocket("ws://127.0.0.1:6969");


                newWs.Connect();

                newWs.Send("name web");

                ws = newWs;

                Console.WriteLine(
                    "Connected to WebSocket server as web"
                );

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Failed to connect to WebSocket server: {ex.Message}"
                );

                ws = null;

                return false;
            }
        }
    }


    public static bool SendToServer(string msg)
    {
        lock (lockObject)
        {
            if (ws == null ||
                ws.ReadyState != WebSocketState.Open)
            {
                Console.WriteLine(
                    "WebSocket is not connected."
                );

                return false;
            }


            try
            {
                ws.Send(msg);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Failed to send WebSocket message: {ex.Message}"
                );

                return false;
            }
        }
    }
}