using Godot;
using System.Text.Json;
using System.Net.WebSockets;
using System;
using System.Text;
using System.Security.Cryptography;


namespace StreamerBotApp
{
	public partial class Connect : Button
	{
		WebSocketManager _WebsocketManager;
		PopulateActions _NodeRoot;
		Button _MainConnectionButton, _PanelButton;
		Panel _ConnectionMenu;
		LineEdit _IPInput, _PortInput, _PasswordInput;

		Uri uri;

		public override void _Ready()
		{

			//Define Local Connections to Global Instances
			_WebsocketManager = GetNode<WebSocketManager>("/root/Control/Manager");
			_NodeRoot = GetNode<PopulateActions>("/root/Control");
			_ConnectionMenu = GetNode<Panel>("/root/Control/Panel");
			_PanelButton = GetNode<Button>("/root/Control/Panel/Button");
			_MainConnectionButton = GetNode<Button>("/root/Control/ConnectToStreamerBot");
			_IPInput = _ConnectionMenu.GetNode<LineEdit>("IPInput");
			_PortInput = _ConnectionMenu.GetNode<LineEdit>("PortInput");
			_PasswordInput = _ConnectionMenu.GetNode<LineEdit>("PasswordInput");

			//Connect Buttons to their desired action
			_PanelButton.ButtonUp += OpenConnection;
			_MainConnectionButton.ButtonUp += OpenPopup;
			//GD.Print("Button Setup Complete");




		}

		public void OpenPopup()
		{
			_ConnectionMenu.Visible = true;

		}




		public async void OpenConnection()
		{
			//Close Connection Menu
			_ConnectionMenu.Visible = false;



			uri = new($"ws://{_IPInput.Text}:{_PortInput.Text}/");

			//Check if a password was provided. If it was, use the encrypted authenticated connection. Else, Connect Insecure
			if (_PasswordInput.Text != "")
			{
				//Authenticated Access
				await _WebsocketManager.WebSocketClient.ConnectAsync(uri, default);

				string tmp = await _WebsocketManager.ReceiveData();

				HelloRoot authResponse = JsonSerializer.Deserialize<HelloRoot>(tmp);

				string secret;
				using (SHA256 sha256 = SHA256.Create())
				{
					byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(_PasswordInput.Text + authResponse.authentication.salt));
					secret = Convert.ToBase64String(hash);
				}

				string authentication;
				using (SHA256 sha256 = SHA256.Create())
				{
					byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret + authResponse.authentication.challenge));
					authentication = Convert.ToBase64String(hash);
				}

				string authRequest = $"{{\"request\": \"Authenticate\", \"id\": \"{Guid.NewGuid().ToString()}\", \"authentication\": \"{authentication}\"}}";

				string AuthConfirmation = await _WebsocketManager.SendRequestAndReceiveResponse(authRequest);


			}
			else
			{
				//Unauthenticated Access
				await _WebsocketManager.WebSocketClient.ConnectAsync(uri, default);

				string tmp = await _WebsocketManager.ReceiveData();

			}


			//Request All Actions from Streamer.Bot and Convert to a usable object
			string request = new BasicRequest("GetActions").ToString();

			string responseString = await _WebsocketManager.SendRequestAndReceiveResponse(request);

			GetActionsRoot response = JsonSerializer.Deserialize<GetActionsRoot>(responseString);
			//Store all actions somewhere they can be globally accessed and begin creating buttons
			_WebsocketManager.actions.AddRange(response.actions);
			_NodeRoot.CreateActionButtons();

		}

		public override void _Process(double delta)
		{
			if (_WebsocketManager.WebSocketClient.State == WebSocketState.Open)
			{
				_MainConnectionButton.AddThemeColorOverride("icon_normal_color", new("64f959"));
			}
			else if (_WebsocketManager.WebSocketClient.State == WebSocketState.Connecting)
			{
				_MainConnectionButton.AddThemeColorOverride("icon_normal_color", new("eaf215"));
			}
			else
			{
				_MainConnectionButton.AddThemeColorOverride("icon_normal_color", new("f22f15"));
			}
		}
	}
}