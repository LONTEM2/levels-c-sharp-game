using Godot;

public partial class ProgressManager : Node
{
	// Zmienna trzymająca informację o odblokowanych poziomach
	public bool IsLvl2Unlocked = true;

	// Możesz tu dodać logikę zapisu do pliku w przyszłości
	private string savePath = "user://savegame.save";

public void SaveProgress()
{
	using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
	file.StoreLine(IsLvl2Unlocked.ToString().ToLower());
}

public void LoadProgress()
{
	if (FileAccess.FileExists(savePath))
	{
		using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
		string content = file.GetLine();
		IsLvl2Unlocked = content == "";
	}
}

public override void _Ready()
{
	LoadProgress(); // Wczytaj przy starcie gry
}
}
