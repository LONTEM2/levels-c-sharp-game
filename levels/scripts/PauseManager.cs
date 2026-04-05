using Godot;
using System;

public partial class PauseManager : CanvasLayer
{
	private Control _panel;

	public override void _Ready()
	{
		_panel = GetNodeOrNull<Control>("Panel");
		Visible = false; 
		ProcessMode = ProcessModeEnum.Always;
		Layer = 128; 
	}

	public override void _Input(InputEvent @event)
	{
		// 1. Obsługa Pauzy (ESC)
		if (@event.IsActionPressed("ui_cancel"))
		{
			TogglePause();
		}

		// 2. Obsługa Fullscreen (F11 lub F)
		// Używamy KeyCode, aby nie musieć definiować akcji w Input Map
		if (@event is InputEventKey eventKey && eventKey.Pressed)
		{
			if (eventKey.Keycode == Key.F11 || eventKey.Keycode == Key.F)
			{
				ToggleFullscreen();
			}
		}
	}

	private void ToggleFullscreen()
	{
		if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			GD.Print("Tryb okienkowy");
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			GD.Print("Tryb pełnoekranowy");
		}
	}

	public void TogglePause()
	{
		GetTree().Paused = !GetTree().Paused;
		Visible = GetTree().Paused;

		if (Visible)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;

			if (_panel != null)
			{
				_panel.Show();
				_panel.Modulate = new Color(1, 1, 1, 1);
				_panel.Scale = new Vector2(1, 1);
				_panel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
				_panel.CustomMinimumSize = new Vector2(400, 300); 
			}
		}
	}
	
	// --- Sygnały dla przycisków ---
	
	public void _on_continue_button_pressed() => TogglePause();

	public void _on_retry_pressed()
	{
		TogglePause();
		GetTree().ReloadCurrentScene();
	}

	public void _on_levels_menu_pressed()
	{
		TogglePause();
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
	}
}
