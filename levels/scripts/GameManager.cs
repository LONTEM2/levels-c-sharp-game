using Godot;
using System;

public partial class GameManager : Node
{
<<<<<<< Updated upstream
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

	// DODANE: Funkcja do obsługi paska ładowania
	public void ChangeLevelWithLoading(string targetLvlPath)
	{
		LoadingScreen.NextScenePath = targetLvlPath;
		GetTree().ChangeSceneToFile("res://loading_screen.tscn");
	}
>>>>>>> Stashed changes

	public override void _Ready()
	{
		_remainingTime = timeLimit;
		
		// Inicjalizacja tekstu punktów
		if (scoreLabel != null)
			scoreLabel.Text = $"🟡 {score}";
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======

		// WAŻNE: Wczytujemy checkpoint po tym, jak cała scena się załaduje
		CallDeferred(nameof(LoadCheckpoint));
>>>>>>> Stashed changes
	}

	if (score == 5)
	{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
		UnlockNextLevel();
=======
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
			// Formatowanie do postaci 00:00
			timeLabel.Text = string.Format("⌛ {0:00}:{1:00}", minutes, seconds);
			
			// Opcjonalnie: Zmień kolor na czerwony, gdy zostanie mało czasu
			if (_remainingTime < 10)
				timeLabel.Modulate = new Color(1, 0, 0);
				timeLabel.Text = string.Format("⏳ {0:00}:{1:00}", minutes, seconds);
=======
			
			// Standardowy wygląd
			timeLabel.Text = string.Format("⌛ {0:00}:{1:00}", minutes, seconds);
			
			// Zmiana na czerwoną klepsydrę, gdy zostanie mało czasu
			if (_remainingTime < 10)
			{
				timeLabel.Modulate = new Color(1, 0, 0);
				timeLabel.Text = string.Format("⏳ {0:00}:{1:00}", minutes, seconds);
			}
			else
			{
				timeLabel.Modulate = new Color(1, 1, 1); // Biały kolor
			}
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
	public float GetRemainingTime() => (float)_remainingTime;

>>>>>>> Stashed changes
	private void OnTimeOut()
	{
		_isLevelEnded = true;
		GD.Print("Koniec czasu!");
<<<<<<< Updated upstream
		// Restart poziomu po przegranej
		GetTree().ReloadCurrentScene();
	}

	private void UnlockNextLevel()
	{
		_isLevelEnded = true;
=======
		// Reload spowoduje ponowne wywołanie _Ready i LoadCheckpoint
		GetTree().ReloadCurrentScene();
	}

	private void LoadCheckpoint()
	{
		string savePath = $"user://checkpoint_lvl{currentLevelNum}.dat";
		string key = "Checkpoint_Secret_456"; // Może być inne hasło niż do postępu gry

		if (FileAccess.FileExists(savePath))
		{
			using var file = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Read, key);
			
			if (file == null) return; // Plik uszkodzony lub zły klucz

			try 
			{
				float posX = float.Parse(file.GetLine());
				float posY = float.Parse(file.GetLine());
				float savedTime = float.Parse(file.GetLine());

				var player = GetTree().CurrentScene.FindChild("Player", true, false) as CharacterBody2D;
				if (player != null)
				{
					player.GlobalPosition = new Vector2(posX, posY);
					_remainingTime = savedTime;
				}
			}
			catch (Exception e) { GD.PrintErr("Błąd odczytu zaszyfrowanego checkpointu: " + e.Message); }
		}
	}

	private void UnlockNextLevel()
	{
		_isLevelEnded = true;

		// Usuwamy checkpoint po ukończeniu poziomu, żeby nie zaczynać od środka przy replayu
		string savePath = $"user://checkpoint_lvl{currentLevelNum}.dat";
		if (FileAccess.FileExists(savePath))
		{
			DirAccess.RemoveAbsolute(savePath);
		}

>>>>>>> Stashed changes
		var progress = GetNodeOrNull<ProgressManager>("/root/ProgressManager");
		
		if (progress == null)
		{
			GD.PrintErr("BŁĄD: Nie znaleziono ProgressManager w Autoload!");
			return;
		}

<<<<<<< Updated upstream
		// Logika odblokowywania poziomów
=======
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
		if (currentLevelNum < 10)
		{
			GD.Print($"Ukończono Lvl {currentLevelNum} -> Odblokowano Lvl {currentLevelNum + 1}");
		}

		progress.SaveProgress();
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
>>>>>>> Stashed changes
=======
		progress.SaveProgress();
		// ZMIENIONE: Teraz ładuje menu przez ekran ładowania
		ChangeLevelWithLoading("res://scenes/menu.tscn");
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
