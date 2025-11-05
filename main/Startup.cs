using Godot;
using System;

public partial class Startup : Control
{
    [Export]
    public Button StartGameBtn;

    [Export]
    public Button ExitGameBtn;

    [Export] public PackedScene mainScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        StartGameBtn.Pressed += StartGameBtn_Pressed;
        ExitGameBtn.Pressed += ExitGameBtn_Pressed;
	}

    private void StartGameBtn_Pressed()
    {
        GD.Print("StartGameBtn_Pressed");
        GetTree().ChangeSceneToPacked(mainScene);
    }

    private void ExitGameBtn_Pressed()
    {
        GD.Print("ExitGameBtn_Pressed");
        GetTree().Quit();
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
