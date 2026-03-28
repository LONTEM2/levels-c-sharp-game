using Godot;

public partial class ProgressManager : Node
{
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

	// Możesz tu dodać logikę zapisu do pliku w przyszłości
	private string savePath = "user://savegame.save";

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

public override void _Ready()
{
	//DirAccess.RemoveAbsolute(savePath); //it clears save system
	LoadProgress(); // Wczytaj przy starcie gry
}
}
