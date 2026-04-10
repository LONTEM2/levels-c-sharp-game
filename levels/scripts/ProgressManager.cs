using Godot;
using System;

public partial class ProgressManager : Node
{
	// --- USTAWIENIA ZABEZPIECZEŃ ---
	// Zmień to hasło na własne, unikalne!
	private const string EncryptionKey = "!@!^#@$%!(@#$JHF@#$%!GSD!&%$@%A{!@#@!#}!#!)$}}1#@!#!adhaahg432kot321423";
	private string savePath = "user://savegame.dat"; 

	// --- STAN GRY ---
	public bool IsLvl2Unlocked = false;
	public bool IsLvl3Unlocked = false;
	public bool IsLvl4Unlocked = false;
	public bool IsLvl5Unlocked = false;
	public bool IsLvl6Unlocked = false;
	public bool IsLvl7Unlocked = false;
	public bool IsLvl8Unlocked = false;
	public bool IsLvl9Unlocked = false;
	public bool IsLvl10Unlocked = true;

	public override void _Ready()
	{
		// Wczytujemy postęp przy starcie gry
		LoadProgress();
	}

	/// <summary>
	/// Zapisuje postęp do zaszyfrowanego pliku binarnego.
	/// </summary>
	public void SaveProgress()
	{
		// Otwieramy plik z szyfrowaniem AES-256
		using var file = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Write, EncryptionKey);
		
		if (file == null)
		{
			GD.PrintErr("BŁĄD: Nie można utworzyć zaszyfrowanego pliku zapisu!");
			return;
		}

		// Zapisujemy wartości jako stringi "true"/"false"
		file.StoreLine(IsLvl2Unlocked.ToString().ToLower());
		file.StoreLine(IsLvl3Unlocked.ToString().ToLower());
		file.StoreLine(IsLvl4Unlocked.ToString().ToLower());
		file.StoreLine(IsLvl5Unlocked.ToString().ToLower());
		file.StoreLine(IsLvl6Unlocked.ToString().ToLower());
		file.StoreLine(IsLvl7Unlocked.ToString().ToLower());
		file.StoreLine(IsLvl8Unlocked.ToString().ToLower());
		file.StoreLine(IsLvl9Unlocked.ToString().ToLower());
		file.StoreLine(IsLvl10Unlocked.ToString().ToLower());
		
		GD.Print(">>> Postęp gry został bezpiecznie zapisany.");
	}

	/// <summary>
	/// Wczytuje i deszyfruje postęp. Jeśli plik jest uszkodzony, resetuje zapis.
	/// </summary>
	public void LoadProgress()
	{
		if (!FileAccess.FileExists(savePath))
		{
			GD.Print("Brak pliku zapisu. Tworzę nowy...");
			SaveProgress();
			return;
		}

		using var file = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Read, EncryptionKey);
		
		// Jeśli file jest null, oznacza to zły klucz lub uszkodzony/zmodyfikowany plik
		if (file == null)
		{
			GD.PrintErr("BŁĄD DESZYFROWANIA: Plik zapisu jest uszkodzony lub zmodyfikowany! Resetuję postęp.");
			ClearSave();
			return;
		}

		try
		{
			IsLvl2Unlocked = file.GetLine() == "true";
			IsLvl3Unlocked = file.GetLine() == "true";
			IsLvl4Unlocked = file.GetLine() == "true";
			IsLvl5Unlocked = file.GetLine() == "true";
			IsLvl6Unlocked = file.GetLine() == "true";
			IsLvl7Unlocked = file.GetLine() == "true";
			IsLvl8Unlocked = file.GetLine() == "true";
			IsLvl9Unlocked = file.GetLine() == "true";
			IsLvl10Unlocked = file.GetLine() == "true";
			GD.Print(">>> Postęp gry wczytany pomyślnie.");
		}
		catch (Exception e)
		{
			GD.PrintErr("Błąd podczas przetwarzania pliku zapisu: " + e.Message);
			ClearSave();
		}
	}

	/// <summary>
	/// Resetuje wszystkie postępy i usuwa plik z dysku.
	/// </summary>
	public void ClearSave()
	{
		IsLvl2Unlocked = false;
		IsLvl3Unlocked = false;
		IsLvl4Unlocked = false;
		IsLvl5Unlocked = false;
		IsLvl6Unlocked = false;
		IsLvl7Unlocked = false;
		IsLvl8Unlocked = false;
		IsLvl9Unlocked = false;
		IsLvl10Unlocked = false;

		if (FileAccess.FileExists(savePath))
		{
			DirAccess.RemoveAbsolute(savePath);
			GD.Print("Plik zapisu został usunięty z dysku.");
		}

		// Zapisujemy "czysty" stan gry
		SaveProgress();
		GD.Print(">>> Postęp gry został zresetowany do zera.");
	}
}
