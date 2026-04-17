using Godot;
using System.Text.Json;
using System.Net.WebSockets;
using System;
using System.Text;
using System.Security.Cryptography;

public partial class Connect : Button
{
	WebSocketManager manager;
	PopulateActions root;
	Button button, panelButton;
	Panel popupMenu;
	LineEdit IPInput, PortInput, PasswordInput;

	Uri uri;

	public override void _Ready(){
		manager = GetNode<WebSocketManager>("/root/Control/Manager");
		root = GetNode<PopulateActions>("/root/Control");
		popupMenu = GetNode<Panel>("/root/Control/Panel");
		panelButton = GetNode<Button>("/root/Control/Panel/Button");
		panelButton.ButtonUp += OpenConnection;

		button = GetNode<Button>("/root/Control/ConnectToStreamerBot");
		button.ButtonUp += OpenPopup;
		//GD.Print("Button Setup Complete");


		IPInput = popupMenu.GetNode<LineEdit>("IPInput");
		PortInput = popupMenu.GetNode<LineEdit>("PortInput");
		PasswordInput = popupMenu.GetNode<LineEdit>("PasswordInput");

	}

	public void OpenPopup()
	{
		popupMenu.Visible = true;

	}




	public async void OpenConnection()
	{
		popupMenu.Visible = false;
        uri = new($"ws://{IPInput.Text}:{PortInput.Text}/");
        if (PasswordInput.Text != "")
		{
            //Authenticated Access
            await manager.WebSocketClient.ConnectAsync(uri, default);

            string tmp = await manager.ReceiveData();

			HelloRoot authResponse = JsonSerializer.Deserialize<HelloRoot>(tmp);

			string secret;
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(PasswordInput.Text + authResponse.authentication.salt));
				secret = Convert.ToBase64String(hash);
			}

			string authentication;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret + authResponse.authentication.challenge));
                authentication = Convert.ToBase64String(hash);
            }

			string authRequest = $"{{\"request\": \"Authenticate\", \"id\": \"{Guid.NewGuid().ToString()}\", \"authentication\": \"{authentication}\"}}";

			string AuthConfirmation = await manager.SendRequestAndReceiveResponse(authRequest);
			GD.Print(AuthConfirmation);


        }
		else
		{
			//Unauthenticated Access
            await manager.WebSocketClient.ConnectAsync(uri, default);

            string tmp = await manager.ReceiveData();
        }


		//GD.Print("Button Pressed");
		
		
		//GD.Print(tmp);

		string request = new BasicRequest("GetActions").ToString();

		string responseString = await manager.SendRequestAndReceiveResponse(request);
        GD.Print(responseString);
        GetActionsRoot response = JsonSerializer.Deserialize<GetActionsRoot>(responseString);
		
		//GD.Print($"Actions Returned: {response.count}");
		manager.actions.AddRange(response.actions);
		root.CreateActionButtons();

	}

	public override void _Process(double delta)
	{
		if(manager.WebSocketClient.State == WebSocketState.Open)
		{
			button.AddThemeColorOverride("icon_normal_color", new("64f959"));
		}
		else if (manager.WebSocketClient.State == WebSocketState.Connecting)
		{
			button.AddThemeColorOverride("icon_normal_color", new("eaf215"));
		}
		else
		{
			button.AddThemeColorOverride("icon_normal_color", new("f22f15"));
		}
	}
}
