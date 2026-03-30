using Godot;
using System;

public partial class OrangeCheckpoint : Area2D
{
	[Export] public int LevelNum = 1;
	// Użyj tego samego hasła, które masz w GameManagerze do checkpointów!
	private const string EncryptionKey = "Checkpoint_Secret_456"; 

	// Podepnij ten sygnał w Inspektorze (Node -> body_entered)
	public void _on_orange_body_entered(Node2D body)
	{
		// Sprawdzamy czy to gracz (lepiej użyć grup lub typu niż nazwy)
		if (body.IsInGroup("player") || body.Name.ToString().ToLower().Contains("player"))
		{
			SaveCheckpoint(body.GlobalPosition);
			
			// Ukrywamy pomarańczę, żeby nie można było zebrać jej dwa razy
			Visible = false;
			SetDeferred(Area2D.PropertyName.Monitoring, false); 
			GD.Print($">>> CHECKPOINT LVL {LevelNum} ZAPISANY SZYFROWANIEM");
		}
	}

	private void SaveCheckpoint(Vector2 pos)
	{
		var gm = GetTree().CurrentScene.GetNodeOrNull<GameManager>("GameManager");
		if (gm == null) { GD.PrintErr("BŁĄD: Nie znaleziono GameManager!"); return; }

		float timeLeft = gm.GetRemainingTime();

		// Zmieniamy rozszerzenie na .dat, żeby pasowało do nowego systemu
		string path = $"user://checkpoint_lvl{LevelNum}.dat"; 
		
		// UŻYWAMY SZYFROWANIA - to jest kluczowe!
		using var file = FileAccess.OpenEncryptedWithPass(path, FileAccess.ModeFlags.Write, EncryptionKey);
		
		if (file != null)
		{
			file.StoreLine(pos.X.ToString());
			file.StoreLine(pos.Y.ToString());
			file.StoreLine(timeLeft.ToString());
		}
		else
		{
			GD.PrintErr("BŁĄD: Nie udało się utworzyć zaszyfrowanego pliku checkpointu!");
		}
	}
}
