using Godot;
using System;

public partial class PlayMenu : Control
{
	private ConfirmationDialog _confirmDialog;

	public override void _Ready()
	{
		// Pobieramy referencję do okna dialogowego
		_confirmDialog = GetNode<ConfirmationDialog>("ConfirmResetDialog");
		
		// Łączymy sygnał potwierdzenia ("OK") z naszą funkcją resetującą
		_confirmDialog.Confirmed += OnResetConfirmed;
	}

	public void _on_play_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
	}

	// Funkcja podpięta pod przycisk Resetu/Ustawień w menu
	public void _on_settings_pressed()
	{
		// Zamiast usuwać, tylko pokazujemy okno pytania
		_confirmDialog.PopupCentered(); 
	}
	public void _on_sound_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/musicMenu.tscn");
	}

	// Ta funkcja wykona się TYLKO, gdy gracz kliknie "Tak" w okienku
	private void OnResetConfirmed()
	{
		var progress = GetNodeOrNull<ProgressManager>("/root/ProgressManager");
		if (progress != null)
		{
			progress.ClearSave();
			GD.Print("Save wyczyszczony po potwierdzeniu.");
			
			// Przeładowujemy scenę, żeby przyciski poziomów się zaktualizowały
			GetTree().ReloadCurrentScene();
		}
	}
}
