using Godot;
using System;

public partial class EnemyUi : Control
{
    [Export]
    private ProgressBar healthBar;
    [Export]
    private Label healthLabel;
    [Export]
    private Label nameLabel;
    [Export]
    private Label levelLabel;

    public override void _Ready()
	{
        nameLabel.Text = "马嘉祺";
    }

	public override void _Process(double delta)
	{
	}

    public void UpdateHealth(float newCurHealth, float newMaxHealth)
    {
        healthBar.MaxValue = newMaxHealth;
        healthBar.Value = newCurHealth;
        healthLabel.Text = $"{newCurHealth} / {newMaxHealth}";
    }

    public void UpdateLevel(int newLevel)
    {
        levelLabel.Text = $"Lv.{newLevel}";
    }

}
