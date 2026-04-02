using Godot;
using System;

public partial class GameManager : Node
{
	private int score = 0;
	
	[ExportGroup("UI References")]
	[Export] public Label scoreLabel;
	[Export] public Label timeLabel;
	
	[ExportGroup("Level Settings")]
	[Export] public int currentLevelNum = 1;
	[Export] public float timeLimit = 60.0f; 

	private double _remainingTime;
	private bool _isLevelEnded = false;

	public override void _Ready()
	{
		_remainingTime = timeLimit;
		
		if (scoreLabel != null)
			scoreLabel.Text = $"🟡 {score}";

		CallDeferred(nameof(LoadCheckpoint));
	}

	public override void _Process(double delta)
	{
		if (_isLevelEnded) return;

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
			
			timeLabel.Text = string.Format("⌛ {0:00}:{1:00}", minutes, seconds);
			
			if (_remainingTime < 10)
			{
				timeLabel.Modulate = new Color(1, 0, 0);
				timeLabel.Text = string.Format("⏳ {0:00}:{1:00}", minutes, seconds);
			}
			else
			{
				timeLabel.Modulate = new Color(1, 1, 1);
			}
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
	public float GetRemainingTime() => (float)_remainingTime;

	private void OnTimeOut()
	{
		_isLevelEnded = true;
		GD.Print("Koniec czasu!");
		GetTree().ReloadCurrentScene();
	}

	private void LoadCheckpoint()
	{
		string savePath = $"user://checkpoint_lvl{currentLevelNum}.dat";
		string key = "Checkpoint_Secret_456";

		if (FileAccess.FileExists(savePath))
		{
			using var file = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Read, key);
			if (file == null) return;

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
			catch (Exception e) { GD.PrintErr("Błąd odczytu checkpointu: " + e.Message); }
		}
	}

	private void UnlockNextLevel()
	{
		_isLevelEnded = true;

		string savePath = $"user://checkpoint_lvl{currentLevelNum}.dat";
		if (FileAccess.FileExists(savePath))
		{
			DirAccess.RemoveAbsolute(savePath);
		}

		var progress = GetNodeOrNull<ProgressManager>("/root/ProgressManager");
		
		if (progress != null)
		{
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
			progress.SaveProgress();
		}

		// BEZPOŚREDNIA ZMIANA SCENY (BEZ LOADING SCREEN)
		// Upewnij się, że ścieżka poniżej jest poprawna!
		string menuPath = "res://scenes/menu.tscn"; 
		CallDeferred(nameof(DeferredChangeScene), menuPath);
	}

	// Metoda pomocnicza do bezpiecznej zmiany sceny
	private void DeferredChangeScene(string path)
	{
		if (FileAccess.FileExists(path))
		{
			GetTree().ChangeSceneToFile(path);
		}
		else
		{
			GD.PrintErr("BŁĄD: Nie znaleziono sceny: " + path);
		}
	}
}
