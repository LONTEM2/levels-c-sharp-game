using Godot;
using System;
using System.Collections.Generic;

public partial class OrangeCheckpoint : Area2D
{
	[Export] public int LevelNum = 1;
	private const string EncryptionKey = "Checkpoint_Secret_456"; 

	public void _on_orange_body_entered(Node2D body)
	{
		// Sprawdzamy czy to gracz
		if (body.IsInGroup("player") || body.Name.ToString().ToLower().Contains("player"))
		{
			SaveCheckpoint(body.GlobalPosition);
			
			// Efekt zebrania checkpointu
			Visible = false;
			SetDeferred(PropertyName.Monitoring, false); 
			GD.Print($">>> CHECKPOINT LVL {LevelNum} ZAPISANY PEŁNY STAN");
		}
	}

	private void SaveCheckpoint(Vector2 pos)
	{
		var gm = GetTree().CurrentScene.GetNodeOrNull<GameManager>("GameManager");
		if (gm == null) { GD.PrintErr("BŁĄD: Nie znaleziono GameManager!"); return; }

		// 1. Pobieramy pozostały czas
		float timeLeft = gm.GetRemainingTime();
		
		// 2. Pobieramy listę zebranych monet z Managera i łączymy je w string "1,2,5"
		// Używamy publicznej listy 'collectedCoins' z nowego GameManagera
		string coinsData = "";
		if (gm.collectedCoins != null && gm.collectedCoins.Count > 0)
		{
			coinsData = string.Join(",", gm.collectedCoins);
		}

		string path = $"user://checkpoint_lvl{LevelNum}.dat"; 
		
		// 3. Zapisujemy zaszyfrowany plik
		using var file = FileAccess.OpenEncryptedWithPass(path, FileAccess.ModeFlags.Write, EncryptionKey);
		
		if (file != null)
		{
			file.StoreLine(pos.X.ToString());      // Linia 1: Pozycja X
			file.StoreLine(pos.Y.ToString());      // Linia 2: Pozycja Y
			file.StoreLine(timeLeft.ToString());   // Linia 3: Czas
			file.StoreLine(coinsData);             // Linia 4: LISTA ID MONET (np. "1,2,3")
			
			GD.Print($"[Zapis] Poz: {pos}, Czas: {timeLeft}, Monety ID: {coinsData}");
		}
		else
		{
			GD.PrintErr("BŁĄD: Nie udało się utworzyć pliku zapisu!");
		}
	}
}
