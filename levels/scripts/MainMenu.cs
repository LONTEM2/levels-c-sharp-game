using Godot;

public partial class MainMenu : Node
{
	[Export] public Button lvl2Button;
	[Export] public Button lvl3Button;

	public override void _Ready()
	{
		// 1. Pobieramy dostęp do Twojego globalnego skryptu (Autoload)
		var progress = GetNode<ProgressManager>("/root/ProgressManager");

		// 2. Jeśli poziom NIE JEST odblokowany, wyłączamy przycisk
		// ! oznacza "zaprzeczenie" (jeśli false, to Disabled = true)
		if (lvl2Button != null)
		{
			lvl2Button.Disabled = !progress.IsLvl2Unlocked;
		}
		if (lvl3Button != null)
		{
			lvl3Button.Disabled = !progress.IsLvl3Unlocked;
		}
	}

	public void _on_lvl_1_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/lvl_1.tscn");
	}

	public void _on_lvl_2_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/lvl_2.tscn");
	}
	public void _on_lvl_3_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/lvl_3.tscn");
	}
}
