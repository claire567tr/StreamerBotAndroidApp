using Godot;
using System;

public partial class ButtonData : Node
{
    public string request = "";
    WebSocketManager manager;

    public override void _Ready()
    {
        manager = GetNode<WebSocketManager>("/root/Control/Manager");
    }

    public async void DoAction()
    {
        await manager.SendRequestAndReceiveResponse(request);

    }
}
