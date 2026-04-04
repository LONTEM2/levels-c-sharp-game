using Godot;
using System;

public partial class MusicMenu : Control
{
	public void _on_home_button_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}
}
