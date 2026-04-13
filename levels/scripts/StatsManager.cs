using Godot;
using System;
using System.Collections.Generic;

public partial class StatsManager : CanvasLayer
{
	public class LevelStats
	{
		public float TimeToComplete = 0;
		public int DeathsBySlime = 0;
		public int DeathsByJump = 0;
		public int TotalAttempts = 1;
		public int CoinsCollected = 0;
		public bool IsCompleted = false;
	}

	public Dictionary<int, LevelStats> AllLevels = new Dictionary<int, LevelStats>();
	private Label _statsLabel;
	private const string EncryptionKey = "TrollGame_Stats_Secure_997";

	public override void _Ready()
	{
		LoadStats();
		
		// Szukamy labela, ale domyślnie go ukrywamy
		_statsLabel = GetNodeOrNull<Label>("Control/StatsLabel");
		if (_statsLabel == null) _statsLabel = GetNodeOrNull<Label>("%StatsLabel");
		
		// Podczas normalnej gry StatsManager ma być niewidoczny
		this.Visible = false;
	}

	public LevelStats GetLevel(int lvNum)
	{
		if (!AllLevels.ContainsKey(lvNum)) AllLevels[lvNum] = new LevelStats();
		return AllLevels[lvNum];
	}

	public override void _Process(double delta)
	{
		Node scene = GetTree()?.CurrentScene;
		if (scene == null) return;

		GameManager gm = scene.GetNodeOrNull<GameManager>("GameManager");
		if (gm == null) gm = scene.FindChild("GameManager", true, false) as GameManager;

		// LICZENIE CZASU: Działa w tle, gdy jesteśmy w poziomie
		if (gm != null && !gm.IsLevelEnded())
		{
			var currentLv = GetLevel(gm.currentLevelNum);
			currentLv.TimeToComplete += (float)delta;
		}

		// WYŚWIETLANIE: Singleton jest widoczny tylko jeśli scena to nie poziom 
		// (możesz to też ręcznie włączać/wyłączać)
		this.Visible = false; 
	}

	public void AddDeath(int lvNum, string type)
	{
		var lv = GetLevel(lvNum);
		if (type == "slime") lv.DeathsBySlime++; else lv.DeathsByJump++;
		lv.TotalAttempts++;
		SaveStats();
	}

	public void MarkCompleted(int lvNum, int coins)
	{
		var lv = GetLevel(lvNum);
		lv.IsCompleted = true;
		lv.CoinsCollected = Math.Max(lv.CoinsCollected, coins);
		SaveStats();
	}

	public void SaveStats()
	{
		using var file = FileAccess.OpenEncryptedWithPass("user://detailed_stats.dat", FileAccess.ModeFlags.Write, EncryptionKey);
		if (file == null) return;

		foreach (var entry in AllLevels)
		{
			file.StoreLine($"LV:{entry.Key}");
			file.StoreLine(entry.Value.TimeToComplete.ToString());
			file.StoreLine(entry.Value.DeathsBySlime.ToString());
			file.StoreLine(entry.Value.DeathsByJump.ToString());
			file.StoreLine(entry.Value.TotalAttempts.ToString());
			file.StoreLine(entry.Value.CoinsCollected.ToString());
			file.StoreLine(entry.Value.IsCompleted.ToString());
		}
	}

	public void LoadStats()
	{
		string path = "user://detailed_stats.dat";
		if (!FileAccess.FileExists(path)) return;
		using var file = FileAccess.OpenEncryptedWithPass(path, FileAccess.ModeFlags.Read, EncryptionKey);
		if (file == null) return;

		try 
		{
			while (file.GetPosition() < file.GetLength())
			{
				string line = file.GetLine();
				if (line.StartsWith("LV:"))
				{
					int lvNum = int.Parse(line.Replace("LV:", ""));
					var lv = GetLevel(lvNum);
					lv.TimeToComplete = float.Parse(file.GetLine());
					lv.DeathsBySlime = int.Parse(file.GetLine());
					lv.DeathsByJump = int.Parse(file.GetLine());
					lv.TotalAttempts = int.Parse(file.GetLine());
					lv.CoinsCollected = int.Parse(file.GetLine());
					lv.IsCompleted = bool.Parse(file.GetLine());
				}
			}
		}
		catch { }
	}
	public void ResetAllStats()
{
	// Czyścimy dane w pamięci RAM
	AllLevels.Clear();
	
	// Usuwamy plik z dysku, aby po restarcie gry staty nie wróciły
	string path = "user://detailed_stats.dat";
	if (FileAccess.FileExists(path))
	{
		DirAccess.RemoveAbsolute(path);
	}
	
	GD.Print("STATS: Statystyki zostały pomyślnie zresetowane.");
}
}
