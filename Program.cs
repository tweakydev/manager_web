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
                const text = await response.text();

                alert(text);
            }
            async function clickClose() {
                const response = await fetch('/buttonClose');
                const text = await response.text();

                alert(text);
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
        using var ws = new WebSocket("ws://gjrnx.site");

        ws.OnMessage += (s, e) => {};

        ws.Connect();

        ws.Send(msg);
    }
}