using Godot;
using System;

namespace StreamerBotApp
{
    public partial class ButtonData : Node
    {
        /// <summary>
        /// The JSON Formatted request to be sent to Streamer.Bot for the action.
        /// </summary>
        public string Request
        {
            get
            {
                return _Request;
            }
            set
            {
                _Request = value;
            }
        }

        private string _Request = "";
        WebSocketManager _WebsocketManager;

        public override void _Ready()
        {
            _WebsocketManager = GetNode<WebSocketManager>("/root/Control/Manager");
        }

        public async void DoAction()
        {
            await _WebsocketManager.SendRequestAndReceiveResponse(Request);

        }
    }
}