using Godot;
using System;

public partial class GameManager : Node
{
<<<<<<< Updated upstream
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
=======
	private int score = 0;
	
	[ExportGroup("UI References")]
	[Export] public Label scoreLabel;
	[Export] public Label timeLabel;
	
	[ExportGroup("Level Settings")]
	[Export] public int currentLevelNum = 1;
	[Export] public float timeLimit = 60.0f; // Czas w sekundach na poziom

	private double _remainingTime;
	private bool _isLevelEnded = false;

	public override void _Ready()
	{
		_remainingTime = timeLimit;
		
		// Inicjalizacja tekstu punktów
		if (scoreLabel != null)
			scoreLabel.Text = $"🟡 {score}";
>>>>>>> Stashed changes
	}

	if (score == 5)
	{
<<<<<<< Updated upstream
		UnlockNextLevel();
=======
		if (_isLevelEnded) return;

		// Odliczanie czasu
		_remainingTime -= delta;

		if (_remainingTime <= 0)
		{
			_remainingTime = 0;
			OnTimeOut();
		}

		UpdateTimerDisplay();
	}

	private void UpdateTimerDisplay()
	{
		if (timeLabel != null)
		{
			int minutes = (int)_remainingTime / 60;
			int seconds = (int)_remainingTime % 60;
			// Formatowanie do postaci 00:00
			timeLabel.Text = string.Format("⌛ {0:00}:{1:00}", minutes, seconds);
			
			// Opcjonalnie: Zmień kolor na czerwony, gdy zostanie mało czasu
			if (_remainingTime < 10)
				timeLabel.Modulate = new Color(1, 0, 0);
				timeLabel.Text = string.Format("⏳ {0:00}:{1:00}", minutes, seconds);
		}
	}

	public void AddPoint()
	{
		if (_isLevelEnded) return;

		score += 1;
		GD.Print("🟡: " + score);

		if (scoreLabel != null)
		{
			scoreLabel.Text = $"🟡 {score}";
		}

		if (score == 5)
		{
			UnlockNextLevel();
		}
	}

	private void OnTimeOut()
	{
		_isLevelEnded = true;
		GD.Print("Koniec czasu!");
		// Restart poziomu po przegranej
		GetTree().ReloadCurrentScene();
	}

	private void UnlockNextLevel()
	{
		_isLevelEnded = true;
		var progress = GetNodeOrNull<ProgressManager>("/root/ProgressManager");
		
		if (progress == null)
		{
			GD.PrintErr("BŁĄD: Nie znaleziono ProgressManager w Autoload!");
			return;
		}

		// Logika odblokowywania poziomów
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

		progress.SaveProgress();
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
>>>>>>> Stashed changes
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
