using Godot;
using System.Buffers;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace StreamerBotApp
{
	public partial class WebSocketManager : Node
	{

		public ClientWebSocket WebSocketClient { get { return _webSocketClient; } }
		ClientWebSocket _webSocketClient = new();
		public List<ActionData> actions = new();

		/// <summary>
		/// Receive Data Without a request being sent (ie. After connecting)
		/// </summary>
		/// <returns>Json Formated Response String</returns>
		public async Task<string> ReceiveData()
		{
			//Allocate 4096 bytes to be used as the response buffer and prepear the output variable
			byte[] responseBuffer = ArrayPool<byte>.Shared.Rent(4096);
			string msg = "";
			while (_webSocketClient.State == WebSocketState.Open)
			{
				//While the websocket is open, receive the response in 4096 byte sections, decoding and adding them
				//to the output
				WebSocketReceiveResult response = await _webSocketClient.ReceiveAsync(responseBuffer, new());
				msg = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);

				//If the message contains that it is the final transmision, terminate the loop
				if (response.EndOfMessage == true)
				{
					break;
				}
			}

			//Release the allocated memory back to the memory stack and return the complete response
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
			//Convert the JSON request to a byte array for use with the .NET websocket system
			byte[] requestBytes = Encoding.UTF8.GetBytes(request.ToCharArray());

			//Asynchronously send the request to Streamer.Bot 
			await _webSocketClient.SendAsync(requestBytes, WebSocketMessageType.Text, true, new());

			//Allocate 4096 bytes to be used as the response buffer and prepear the output variable
			byte[] responseBuffer = ArrayPool<byte>.Shared.Rent(4096);
			string output = "";
			while (_webSocketClient.State == WebSocketState.Open)
			{
				//While the websocket is open, receive the response in 4096 byte sections, decoding and adding them
				//to the output
				WebSocketReceiveResult response = await _webSocketClient.ReceiveAsync(responseBuffer, new());
				output += Encoding.UTF8.GetString(responseBuffer, 0, response.Count);

				//If the message contains that it is the final transmision, terminate the loop
				if (response.EndOfMessage == true)
				{
					break;
				}
			}

			//Release the allocated memory back to the memory stack and return the complete response
			ArrayPool<byte>.Shared.Return(responseBuffer);
			return output;
		}
	}
}