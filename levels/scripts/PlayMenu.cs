using Godot;
using System;

public partial class PlayMenu : Control
{
	public void _on_play_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
	}
	
}
