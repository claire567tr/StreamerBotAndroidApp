using Godot;
using System;

namespace StreamerBotApp
{
    public partial class PopulateActions : Node
    {
        WebSocketManager _WebsocketManager;
        int _HorizontalOffset = 1;
        int _VerticalOffset = 0;
        int _MaxHorizontalOffset;
        public override void _Ready()
        {
            _WebsocketManager = GetNode<WebSocketManager>("/root/Control/Manager");

            //Calculate how many buttons can fit across the screen at most
            //TODO: Implement vertical with scrolling for more buttons
            _MaxHorizontalOffset = (int)MathF.Floor(GetWindow().Size.X / 136);
        }

        public void CreateActionButtons()
        {
            foreach (ActionData action in _WebsocketManager.actions)
            {

                //Create a new button as a child of the Root Node.
                Button newButton = new();
                AddChild(newButton);

                //Create a Label as a Child of the New Button and set it up
                Label label = new();
                newButton.AddChild(label);
                label.AutowrapMode = TextServer.AutowrapMode.Word;
                label.Text = action.name;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.Size = new Vector2(136, 136);
                newButton.Icon = GD.Load<Texture2D>("res://icon.svg");

                //Create an empty node with the ButtonData Script attached as a Child of the New Button
                ButtonData buttonData = new();
                newButton.AddChild(buttonData);
                ulong id = buttonData.GetInstanceId();
                buttonData.SetScript(GD.Load<CSharpScript>("res://ButtonData.cs"));
                ButtonData button = (ButtonData)InstanceFromId(id);

                //Set the JSON Request string in the newly create ButtonData based on the Action Details from Streamer.Bot
                //and set the button to call the action when pressed.
                DoActionRequestRoot request = new(action);
                button.Request = request.ToString();
                newButton.ButtonUp += button.DoAction;

                //Move the new button to the next available spot on the grid
                //Determined based on the assumed size of the button being 136px x 136px
                //TODO: Make work dynamically with icon size
                newButton.Position = newButton.Position + new Vector2(136 * _HorizontalOffset, _VerticalOffset);
                _HorizontalOffset += 1;
                if (_HorizontalOffset == _MaxHorizontalOffset)
                {
                    _HorizontalOffset = 0;
                    _VerticalOffset += 136;
                }

            }
        }
    }
}
