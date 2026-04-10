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

	private void OnResetConfirmed()
{
	// 1. Resetowanie postępu poziomów (to co miałeś)
	var progress = GetNodeOrNull<ProgressManager>("/root/ProgressManager");
	if (progress != null)
	{
		progress.ClearSave();
		GD.Print("Save wyczyszczony po potwierdzeniu.");
	}

	// 2. NOWE: Resetowanie statystyk (czas, zgony, monety)
	var stats = GetNodeOrNull<StatsManager>("/root/StatsManager");
	if (stats != null)
	{
		stats.ResetAllStats(); // Upewnij się, że dodałeś tę metodę do StatsManager.cs
		GD.Print("Statystyki wyczyszczone po potwierdzeniu.");
	}

	// 3. Przeładowanie sceny, aby odświeżyć UI menu
	GetTree().ReloadCurrentScene();
}
}
