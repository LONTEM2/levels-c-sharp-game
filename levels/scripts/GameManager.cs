using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node
{
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

	// Metoda dla StatsManagera, aby wiedział czy zatrzymać stoper
	public bool IsLevelEnded() => _isLevelEnded;

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

	public void AddPoint(int id)
	{
		if (_isLevelEnded) return;
		if (collectedCoins.Contains(id)) return;

		collectedCoins.Add(id);
		score = collectedCoins.Count;
		
		// Statystyki nie są już globalne w tym miejscu, zajmiemy się nimi przy MarkCompleted
		// lub możesz zostawić stats?.AddCoin() jeśli StatsManager ma taką metodę.

		UpdateScoreDisplay();

		if (score >= 5)
		{
			UnlockNextLevel();
		}
	}

	private void OnTimeOut()
	{
		if (_isLevelEnded) return;
		_isLevelEnded = true;
		
		// Statystyka śmierci przez czas (traktujemy jako "skok"/środowiskowe)
		var stats = GetNodeOrNull<StatsManager>("/root/StatsManager");
		stats?.AddDeath(currentLevelNum, "jump");

		var musicPlayer = GetNodeOrNull<MusicPlayer>("/root/MusicPlayer");
		musicPlayer?.ForceReset();

		GetTree().ReloadCurrentScene();
	}

	private void LoadCheckpoint()
	{
		string savePath = $"user://checkpoint_lvl{currentLevelNum}.dat";
		if (!FileAccess.FileExists(savePath)) return;

		using var file = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Read, "Checkpoint_Secret_456");
		if (file == null) return;

		try 
		{
			float posX = float.Parse(file.GetLine());
			float posY = float.Parse(file.GetLine());
			float savedTime = float.Parse(file.GetLine());

			if (file.GetPosition() < file.GetLength())
			{
				string coinData = file.GetLine();
				if (!string.IsNullOrEmpty(coinData))
				{
					collectedCoins = coinData.Split(',').Select(int.Parse).ToList();
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

			RemoveCollectedCoinsFromScene();
		}
		catch (Exception e) { GD.PrintErr("Błąd odczytu checkpointu: " + e.Message); }
	}

	private void RemoveCollectedCoinsFromScene()
	{
		foreach (Node node in GetTree().GetNodesInGroup("coins"))
		{
			int id = (int)node.Get("CoinId");
			if (collectedCoins.Contains(id)) node.QueueFree();
		}
	}

	private void UnlockNextLevel()
	{
		if (_isLevelEnded) return;
		_isLevelEnded = true;

		// 1. Zapisujemy statystyki ukończenia poziomu
		var stats = GetNodeOrNull<StatsManager>("/root/StatsManager");
		stats?.MarkCompleted(currentLevelNum, collectedCoins.Count);

		// 2. Usuwamy checkpoint (poziom ukończony)
		string savePath = $"user://checkpoint_lvl{currentLevelNum}.dat";
		if (FileAccess.FileExists(savePath)) DirAccess.RemoveAbsolute(savePath);

		// 3. Odblokowanie postępu
		var progress = GetNodeOrNull<ProgressManager>("/root/ProgressManager");
		if (progress != null)
		{
			if (currentLevelNum == 8) progress.IsLvl9Unlocked = true;
			else progress.Set($"IsLvl{currentLevelNum + 1}Unlocked", true);
			progress.SaveProgress();
		}

		CallDeferred(nameof(DeferredChangeScene), "res://scenes/menu.tscn");
	}
	
	public float GetRemainingTime()
{
	return (float)_remainingTime;
}
	
	private void DeferredChangeScene(string path)
	{
		if (FileAccess.FileExists(path)) GetTree().ChangeSceneToFile(path);
	}
}
