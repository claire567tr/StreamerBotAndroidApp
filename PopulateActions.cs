using Godot;
using System;

public partial class PopulateActions : Node
{
    // Called when the node enters the scene tree for the first time.
    WebSocketManager manager;
    int HorizontalOffset = 1;
    int VerticalOffset = 0;
    int MaxHorizontalOffset;
    public override void _Ready()
    {
        manager = GetNode<WebSocketManager>("/root/Control/Manager");
        float offset = GetWindow().Size.X / 136;
        MaxHorizontalOffset = (int)MathF.Floor(offset);
    }

   public void CreateActionButtons()
    {
        foreach(ActionData action in manager.actions)
        { 
            Button newButton = new();
            AddChild(newButton);

            DoActionRequestRoot request = new(action);

            Label label = new();
            newButton.AddChild(label);
            label.AutowrapMode = TextServer.AutowrapMode.Word;
            label.Text = action.name;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Size = new Vector2(136, 136);
            //GD.Print(label.Size.X);


            //label.Position = new Vector2((136 - label.Size.X) / 2, 0);
            newButton.Icon = GD.Load<Texture2D>("res://icon.svg");


            ButtonData buttonData = new();
            newButton.AddChild(buttonData);
            ulong id = buttonData.GetInstanceId();
            buttonData.SetScript(GD.Load<CSharpScript>("res://ButtonData.cs"));
            ButtonData button = (ButtonData)InstanceFromId(id);
            
            button.request = request.ToString();
            //GD.Print(button.request);
            newButton.ButtonUp += button.DoAction;


            newButton.Position = newButton.Position + new Vector2(136 * HorizontalOffset, VerticalOffset);
            HorizontalOffset += 1;
            if(HorizontalOffset == MaxHorizontalOffset)
            {
                HorizontalOffset = 0;
                VerticalOffset += 136;
            }
            //tmp.ButtonUp += button.DoAction;
            
        }
    }
}
