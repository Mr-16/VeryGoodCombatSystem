using Godot;
using System;
using System.Xml;

public partial class PlayerUi : CanvasLayer
{
	[Export]
    private Label nameLabel;

    [Export]
    private ProgressBar healthBar;
    [Export]
    private Label healthLabel;

    [Export]
    private ProgressBar expBar;
    [Export]
    private Label levelLabel;
    [Export]
    private Label expLabel;

    public override void _Ready()
	{
		nameLabel.Text = "史泰杰";
    }

	public override void _Process(double delta)
	{
	}

	public void UpdateHealth(int newCurHealth, int newMaxHealth)
	{
		healthBar.MaxValue = newMaxHealth;
		healthBar.Value = newCurHealth;
		healthLabel.Text = $"{newCurHealth} / {newMaxHealth}";
    }

	public void UpdateExpAndLevel(int curExp, float maxExp, int newLevel)
	{
        expBar.MaxValue = maxExp;
        expBar.Value = curExp;
        expLabel.Text = $"{curExp} / {maxExp}";
        levelLabel.Text = $"Lv.{newLevel}";
    }
}
