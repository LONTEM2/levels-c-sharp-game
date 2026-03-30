using Godot;
using System;

public partial class ProgressManager : Node
{
<<<<<<< Updated upstream
	// Zmienna trzymająca informację o odblokowanych poziomach
	public bool IsLvl2Unlocked = false;
	public bool IsLvl3Unlocked = false;
	public bool IsLvl4Unlocked = false;
	public bool IsLvl5Unlocked = false;
	public bool IsLvl6Unlocked = false;
	public bool IsLvl7Unlocked = false;
	public bool IsLvl8Unlocked = false;
	public bool IsLvl9Unlocked = false;
	public bool IsLvl10Unlocked = false;
=======
	// --- USTAWIENIA ZABEZPIECZEŃ ---
	// Zmień to hasło na własne, unikalne!
	private const string EncryptionKey = "!@!^#@$%!(@#$JHF@#$%!GSD!&%$@%A{!@#@!#}!#!)$}}1#@!#!adhaahg432kot321423";
	private string savePath = "user://savegame.dat"; 
>>>>>>> Stashed changes

	// --- STAN GRY ---
	public bool IsLvl2Unlocked = false;
	public bool IsLvl3Unlocked = false;
	public bool IsLvl4Unlocked = false;
	public bool IsLvl5Unlocked = false;
	public bool IsLvl6Unlocked = false;
	public bool IsLvl7Unlocked = false;
	public bool IsLvl8Unlocked = false;
	public bool IsLvl9Unlocked = false;
	public bool IsLvl10Unlocked = false;

<<<<<<< Updated upstream
// W ProgressManager.cs
public void SaveProgress()
{
	using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
	file.StoreLine(IsLvl2Unlocked.ToString().ToLower()); // Linia 1
	file.StoreLine(IsLvl3Unlocked.ToString().ToLower()); // Linia 2
	file.StoreLine(IsLvl4Unlocked.ToString().ToLower()); // Linia 3
	file.StoreLine(IsLvl5Unlocked.ToString().ToLower()); // Linia 4
	file.StoreLine(IsLvl6Unlocked.ToString().ToLower()); // Linia 5
	file.StoreLine(IsLvl7Unlocked.ToString().ToLower()); // Linia 6
	file.StoreLine(IsLvl8Unlocked.ToString().ToLower()); // Linia 7
	file.StoreLine(IsLvl9Unlocked.ToString().ToLower()); // Linia 8
	file.StoreLine(IsLvl10Unlocked.ToString().ToLower()); // Linia 9
}

public void LoadProgress()
{
	if (FileAccess.FileExists(savePath))
	{
		using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
		// Czytamy kolejno linie
		IsLvl2Unlocked = file.GetLine() == "true";
		IsLvl3Unlocked = file.GetLine() == "true";
		IsLvl4Unlocked = file.GetLine() == "true";
		IsLvl5Unlocked = file.GetLine() == "true";
		IsLvl6Unlocked = file.GetLine() == "true";
		IsLvl7Unlocked = file.GetLine() == "true";
		IsLvl8Unlocked = file.GetLine() == "true";
		IsLvl9Unlocked = file.GetLine() == "true";
		IsLvl10Unlocked = file.GetLine() == "true";
	}
}

public void ClearSave()
{
	// 1. Resetujemy wszystkie zmienne w pamięci do false
	IsLvl2Unlocked = false;
	IsLvl3Unlocked = false;
	IsLvl4Unlocked = false;
	IsLvl5Unlocked = false;
	IsLvl6Unlocked = false;
	IsLvl7Unlocked = false;
	IsLvl8Unlocked = false;
	IsLvl9Unlocked = false;
	IsLvl10Unlocked = false;

	// 2. Usuwamy plik zapisu z dysku (jeśli istnieje)
	if (FileAccess.FileExists(savePath))
	{
		DirAccess.RemoveAbsolute(savePath);
		GD.Print("Plik zapisu został usunięty.");
	}

	// 3. Opcjonalnie zapisujemy "pusty" stan, aby upewnić się, że plik jest czysty
	SaveProgress();
	
	GD.Print("Postęp gry został zresetowany!");
}

public override void _Ready()
{
	//DirAccess.RemoveAbsolute(savePath); //it clears save system
	LoadProgress(); // Wczytaj przy starcie gry
}
}
=======
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
>>>>>>> Stashed changes
