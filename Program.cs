using WebSocketSharp;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Content("""
    <!DOCTYPE html>
    <html>
    <body>
        <button onclick="clickOpen()">
            Open Door
        </button>
        
        <button onclick="clickClose()">
            Close Door
        </button>

        <script>
            async function clickOpen() {
                const response = await fetch('/buttonOpen');
            }
            async function clickClose() {
                const response = await fetch('/buttonClose');
            }
        </script>
    </body>
    </html>
    """, "text/html"));


app.MapGet("/buttonOpen", () =>
{
    WebsocketManager.SendToServer("send door open");
});
app.MapGet("/buttonClose", () =>
{
    WebsocketManager.SendToServer("send door close");
});


app.Run("http://localhost:5000");

public static class WebsocketManager
{

    public static void SendToServer(string msg) {
        using var ws = new WebSocket("wss://gjrnx.site");

        ws.SslConfiguration.EnabledSslProtocols =
            System.Security.Authentication.SslProtocols.Tls12;

        ws.Connect();

        ws.Send("name web");

        ws.Send(msg);
    }
}