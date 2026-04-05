using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node
{
	// Zmieniamy score na public i dodajemy listę ID zebranych monet
	public int score = 0;
	public List<int> collectedCoins = new List<int>();
	
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
		UpdateScoreDisplay();

		// Wczytujemy checkpoint (w tym listę monet)
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

	private void UpdateScoreDisplay()
	{
		if (scoreLabel != null)
		{
			scoreLabel.Text = $"🟡 {score}";
		}
	}

	private void UpdateTimerDisplay()
	{
		if (timeLabel != null)
		{
			int totalSeconds = (int)Mathf.Max(_remainingTime, 0);
			int minutes = totalSeconds / 60;
			int seconds = totalSeconds % 60;
			
			if (_remainingTime < 10)
			{
				timeLabel.Modulate = new Color(1, 0, 0);
				timeLabel.Text = string.Format("⏳ {0:00}:{1:00}", minutes, seconds);
			}
			else
			{
				timeLabel.Modulate = new Color(1, 1, 1);
				timeLabel.Text = string.Format("⌛ {0:00}:{1:00}", minutes, seconds);
			}
		}
	}

	// NAPRAWIONA METODA: Teraz przyjmuje ID monety
	public void AddPoint(int id)
	{
		if (_isLevelEnded) return;

		// Jeśli to ID już jest na liście, ignorujemy (zabezpieczenie przed podwójnym naliczeniem)
		if (collectedCoins.Contains(id)) return;

		collectedCoins.Add(id);
		score = collectedCoins.Count; // Wynik to liczba unikalnych zebranych monet
		
		GD.Print($"Zabrano monetę ID: {id}. Razem: {score}");
		UpdateScoreDisplay();

		if (score >= 5)
		{
			UnlockNextLevel();
		}
	}

	public float GetRemainingTime() => (float)_remainingTime;

	private void OnTimeOut()
	{
		if (_isLevelEnded) return;
		_isLevelEnded = true;
		
		var musicPlayer = GetNodeOrNull<MusicPlayer>("/root/MusicPlayer");
		musicPlayer?.ForceReset();

		GetTree().ReloadCurrentScene();
	}

	private void LoadCheckpoint()
	{
		string savePath = $"user://checkpoint_lvl{currentLevelNum}.dat";
		string key = "Checkpoint_Secret_456";

		if (!FileAccess.FileExists(savePath)) return;

		using var file = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Read, key);
		if (file == null) return;

		try 
		{
			float posX = float.Parse(file.GetLine());
			float posY = float.Parse(file.GetLine());
			float savedTime = float.Parse(file.GetLine());

			// ODCZYT LISTY MONET (4. linia)
			if (file.GetPosition() < file.GetLength())
			{
				string coinData = file.GetLine();
				if (!string.IsNullOrEmpty(coinData))
				{
					// Zamieniamy tekst "1,2,5" z powrotem na listę liczb
					collectedCoins = coinData.Split(',')
											 .Select(int.Parse)
											 .ToList();
					score = collectedCoins.Count;
					UpdateScoreDisplay();
				}
			}

			var player = GetTree().CurrentScene.FindChild("Player", true, false) as CharacterBody2D;
			if (player != null)
			{
				player.GlobalPosition = new Vector2(posX, posY);
				_remainingTime = savedTime;
			}

			// Po wczytaniu usuwamy ze świata te monety, które już są na liście
			RemoveCollectedCoinsFromScene();
		}
		catch (Exception e) { GD.PrintErr("Błąd odczytu checkpointu: " + e.Message); }
	}

	private void RemoveCollectedCoinsFromScene()
	{
		// Szukamy wszystkich monet w grupie "coins"
		var allCoins = GetTree().GetNodesInGroup("coins");
		foreach (Node node in allCoins)
		{
			// Pobieramy ID z monety (musi mieć zmienną CoinId)
			int id = (int)node.Get("CoinId");
			if (collectedCoins.Contains(id))
			{
				node.QueueFree(); // Usuwamy, bo już zebrana
			}
		}
	}

	private void UnlockNextLevel()
	{
		if (_isLevelEnded) return;
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
			}
			progress.SaveProgress();
		}

		CallDeferred(nameof(DeferredChangeScene), "res://scenes/menu.tscn");
	}

	private void DeferredChangeScene(string path)
	{
		if (FileAccess.FileExists(path))
		{
			GetTree().ChangeSceneToFile(path);
		}
	}
}
