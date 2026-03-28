using Godot;
using System;

public partial class GameManager : Node
{
	private int score = 4;
	[Export] public Label scoreLabel;
	
	// NOWOŚĆ: Wpisz w Inspektorze Godota, który to poziom (1, 2, lub 3)
	[Export] public int currentLevelNum = 1; 

	public void AddPoint()
{
	score += 1;
	GD.Print("Punkty: " + score);

	// Sprawdzamy, czy Label w ogóle istnieje
	
	if (scoreLabel != null)
	{
		scoreLabel.Text = $"{score} coins.";
	}

	if (score == 5)
	{
		UnlockNextLevel();
	}
}

	private void UnlockNextLevel()
{
	var progress = GetNode<ProgressManager>("/root/ProgressManager");

	// Logika odblokowywania "krok po kroku"
	switch (currentLevelNum)
{
	case 1: progress.IsLvl2Unlocked = true; break;
	case 2: progress.IsLvl3Unlocked = true; break;
	case 3: progress.IsLvl4Unlocked = true; break;
	case 4: progress.IsLvl5Unlocked = true; break;
	case 5: progress.IsLvl6Unlocked = true; break;
	case 6: progress.IsLvl7Unlocked = true; break;
	case 7: progress.IsLvl8Unlocked = true; break;
	case 8: progress.IsLvl9Unlocked = true; break;
	case 9: progress.IsLvl10Unlocked = true; break;
	case 10: GD.Print("Gratulacje! Ukończyłeś całą grę!"); break;
}

if (currentLevelNum < 10)
{
	GD.Print($"Ukończono Lvl {currentLevelNum} -> Odblokowano Lvl {currentLevelNum + 1}");
}

	// KONIECZNIE zapisz stan do pliku zaraz po zmianie na true
	progress.SaveProgress();

	// Powrót do menu, gdzie przyciski się odświeżą
	GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
}
}
