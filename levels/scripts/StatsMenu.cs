using Godot;
using System;
using System.Linq; // Wymagane do sortowania poziomów

public partial class StatsMenu : Control
{
	private VBoxContainer _container;

	public override void _Ready()
	{
		// Pobieramy kontener na podstawie struktury z Twojego edytora
		_container = GetNodeOrNull<VBoxContainer>("ScrollContainer/VBoxContainer");

		if (_container != null)
		{
			DisplayStats();
		}
		else
		{
			GD.PrintErr("BŁĄD: Nie znaleziono VBoxContainer w StatsMenu! Sprawdź ścieżkę.");
		}
	}

	private void DisplayStats()
	{
		// Pobieramy StatsManager jako Singleton
		var statsManager = GetNodeOrNull<StatsManager>("/root/StatsManager");
		
		if (statsManager == null)
		{
			GD.PrintErr("BŁĄD: StatsManager nie jest załadowany jako Autoload!");
			return;
		}

		// Wymuszamy wczytanie najświeższych danych z pliku
		statsManager.LoadStats();

		// Czyścimy listę przed dodaniem nowych elementów
		foreach (Node child in _container.GetChildren())
		{
			child.QueueFree();
		}

		// Sprawdzamy, czy są jakiekolwiek dane
		if (statsManager.AllLevels.Count == 0)
		{
			Label emptyLbl = new Label();
			emptyLbl.Text = "\n\nNo stats saved.\nComplete the level or die to save your data!";
			emptyLbl.HorizontalAlignment = HorizontalAlignment.Center;
			_container.AddChild(emptyLbl);
			return;
		}

		// Sortujemy klucze (numery poziomów), aby wyświetlały się po kolei
		var sortedLevelIds = statsManager.AllLevels.Keys.OrderBy(id => id);

		foreach (int levelNum in sortedLevelIds)
		{
			var lv = statsManager.AllLevels[levelNum];
			
			// Tworzymy etykietę dla konkretnego poziomu
			Label lbl = new Label();
			
			lbl.Text = $"--- Level {levelNum} ---\n" +
					   $"Time (Total ⌛) : {lv.TimeToComplete:0.0}s | Attempts 😨: {lv.TotalAttempts}\n" +
					   $"Death (Slime 🟢): {lv.DeathsBySlime} | Death (Jumps 💨): {lv.DeathsByJump}\n" +
					   $"Coins 🟡: {lv.CoinsCollected}/5 | Complited 🎯: {(lv.IsCompleted ? "Yes" : "No")}\n";
			
			// Ustawiamy styl wyświetlania
			lbl.HorizontalAlignment = HorizontalAlignment.Center;
			
			// Dodajemy margines dolny za pomocą pustej linii lub ustawień kontenera
			_container.AddChild(lbl);

			// Opcjonalnie: dodajemy separator (pustą przestrzeń) między poziomami
			Control spacer = new Control();
			spacer.CustomMinimumSize = new Vector2(0, 10);
			_container.AddChild(spacer);
		}
	}

	// Metoda podpięta pod sygnał 'pressed' przycisku HomeButton
	public void _on_back_button_pressed()
	{
		// Powrót do menu głównego
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
	}
}
