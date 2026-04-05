using Godot;

public partial class MainMenu : Node
{
	[Export] public Button lvl2Button;
	[Export] public Button lvl3Button;
	[Export] public Button lvl4Button;
	[Export] public Button lvl5Button;
	[Export] public Button lvl6Button;
	[Export] public Button lvl7Button;
	[Export] public Button lvl8Button;
	[Export] public Button lvl9Button;
	[Export] public Button lvl10Button;

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
		if (lvl4Button != null)
		{
			lvl4Button.Disabled = !progress.IsLvl4Unlocked;
		}
		if (lvl5Button != null)
		{
			lvl5Button.Disabled = !progress.IsLvl5Unlocked;
		}
		if (lvl6Button != null)
		{
			lvl6Button.Disabled = !progress.IsLvl6Unlocked;
		}
		if (lvl7Button != null)
		{
			lvl7Button.Disabled = !progress.IsLvl7Unlocked;
		}
		if (lvl8Button != null)
		{
			lvl8Button.Disabled = !progress.IsLvl8Unlocked;
		}
		if (lvl9Button != null)
		{
			lvl9Button.Disabled = !progress.IsLvl9Unlocked;
		}
		if (lvl10Button != null)
		{
			lvl10Button.Disabled = !progress.IsLvl10Unlocked;
		}
	}
	
	public void _on_home_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
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
	public void _on_lvl_4_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/lvl_4.tscn");
	}
	public void _on_lvl_5_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/lvl_5.tscn");
	}
	public void _on_lvl_6_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/lvl_6.tscn");
	}
	public void _on_lvl_7_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/lvl_7.tscn");
	}
	public void _on_lvl_8_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/demoEnd.tscn");
	}
	public void _on_lvl_9_pressed()
	{
	}
	public void _on_lvl_10_pressed()
	{
	}
}
