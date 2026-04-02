using Godot;
using System;

public partial class PuzzleManager : Node
{
	[Export] public NodePath NotificationLabelPath; 
	[Export] public NodePath DoorPath;               

	private Label _notificationLabel;
	private int _slot1 = 0, _slot2 = 0, _slot3 = 0;
	private bool _canClick = true;

	public override void _Ready()
	{
		GD.Print(">>> PUZZLE MANAGER: System aktywny.");
		
		// Sprawdzenie Labela powiadomień
		if (NotificationLabelPath != null && !NotificationLabelPath.IsEmpty)
		{
			_notificationLabel = GetNodeOrNull<Label>(NotificationLabelPath);
		}
		
		if (_notificationLabel == null)
			GD.PrintErr("!!! OSTRZEŻENIE: Nie podpięto Labela w Inspektorze (NotificationLabelPath)!");
	}

	public void HandleClick(int slot)
	{
		if (!_canClick) return;

		switch (slot)
		{
			case 1:
				_slot1 = (_slot1 + 1) % 10;
				GD.Print($"[LOG] Slot 1 -> {_slot1}");
				ShowNotification(_slot1.ToString());
				break;
			case 2:
				_slot2 = (_slot2 + 1) % 10;
				GD.Print($"[LOG] Slot 2 -> {_slot2}");
				ShowNotification(_slot2.ToString());
				break;
			case 3:
				_slot3 = (_slot3 + 1) % 10;
				GD.Print($"[LOG] Slot 3 -> {_slot3}");
				ShowNotification(_slot3.ToString());
				break;
			case 4:
				GD.Print($"[LOG] Sprawdzam wpisany kod: {_slot1}{_slot2}{_slot3}");
				ShowNotification("=");
				CheckResult();
				break;
		}
		
		StartCooldown();
	}

	private async void StartCooldown()
	{
		_canClick = false;
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
		_canClick = true;
	}

	private void CheckResult()
	{
		// Twoja sekwencja to 2-5-6
		if (_slot1 == 2 && _slot2 == 5 && _slot3 == 6)
		{
			GD.Print(">>> KOD POPRAWNY! Próbuję usunąć drzwi...");
			ShowNotification("WIN!");

			// Pobieramy węzeł drzwi
			Node doorNode = GetNodeOrNull(DoorPath);
			
			if (doorNode != null)
			{
				GD.Print($">>> SUKCES: Usuwam węzeł o nazwie: {doorNode.Name}");
				doorNode.QueueFree();
			}
			else
			{
				GD.PrintErr($"!!! BŁĄD: Ścieżka DoorPath ({DoorPath}) jest nieprawidłowa lub obiekt nie istnieje!");
				// Wyświetl listę dostępnych obiektów u rodzica, żeby ułatwić debugowanie
				GD.Print("Dostępne obiekty w tej samej gałęzi:");
				foreach(var child in GetParent().GetChildren()) 
					GD.Print($" - {child.Name}");
			}
		}
		else
		{
			GD.Print($">>> KOD BŁĘDNY! Masz: {_slot1}{_slot2}{_slot3}, wymagane: 256");
			ShowNotification("ERR");
		}
	}

	private async void ShowNotification(string text)
	{
		if (_notificationLabel == null) return;
		
		_notificationLabel.Text = text;
		_notificationLabel.Show();
		
		await ToSignal(GetTree().CreateTimer(0.3), "timeout");
		
		// Czyścimy tylko jeśli w międzyczasie nie wskoczyła inna cyfra
		if (_notificationLabel.Text == text)
		{
			_notificationLabel.Text = "";
		}
	}
}
