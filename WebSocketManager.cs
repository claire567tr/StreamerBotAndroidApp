using Godot;
using System.Buffers;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public partial class WebSocketManager : Node
{
	
	public ClientWebSocket WebSocketClient { get {return _webSocketClient;} }
	ClientWebSocket _webSocketClient = new();
	public List<ActionData> actions = new();
	
	/// <summary>
	/// Receive Data Without a request being sent (ie. After connecting)
	/// </summary>
	/// <returns>Json Formated Response String</returns>
	public async Task<string> ReceiveData()
	{
		byte[] responseBuffer = ArrayPool<byte>.Shared.Rent(4096);
		string msg = "";
		while (_webSocketClient.State == WebSocketState.Open)
		{
			WebSocketReceiveResult response = await _webSocketClient.ReceiveAsync(responseBuffer, new());
			msg = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
			if (response.EndOfMessage == true)
			{
				break;
			}
		}
		ArrayPool<byte>.Shared.Return(responseBuffer);
		return msg;
	}
	/// <summary>
	/// Send a Request to the Websocket Server and receive a response
	/// </summary>
	/// <param name="request">JSON Formatted Request Data</param>
	/// <returns>JSON Formatted Response String</returns>
	public async Task<string> SendRequestAndReceiveResponse(string request)
	{
		byte[] requestBytes = Encoding.UTF8.GetBytes(request.ToCharArray());


		await _webSocketClient.SendAsync(requestBytes, WebSocketMessageType.Text, true, new());

		byte[] responseBuffer = ArrayPool<byte>.Shared.Rent(4096);


		string msg = "";
		while (_webSocketClient.State == WebSocketState.Open)
		{

			WebSocketReceiveResult response = await _webSocketClient.ReceiveAsync(responseBuffer, new());


			msg += Encoding.UTF8.GetString(responseBuffer, 0, response.Count);

			//Console.WriteLine("Response: " + msg);

			if (response.EndOfMessage == true)
			{
				break;
			}
		}
		ArrayPool<byte>.Shared.Return(responseBuffer);

		return msg;
	}
}
