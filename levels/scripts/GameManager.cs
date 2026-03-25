using Godot;
using System;

public partial class GameManager : Node
{
	// Deklaracja zmiennej całkowitej (int)
	private int score = 0;
	[Export] public Label scoreLabel;
	// Odpowiednik 'func add_point()'
	public void AddPoint()
	{
		score += 1;
		GD.Print(score);
		scoreLabel.Text = $"{score} coins.";
		if (score == 5)
		{
		GD.Print("Osiągnięto 5 monet! Ukonczono lvl1...");
		var progress = GetNode<ProgressManager>("/root/ProgressManager");
		progress.IsLvl2Unlocked = true;
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
		}
	}

	public override void _Ready()
	{
	
	}

	public override void _Process(double delta)
	{
	}
}
