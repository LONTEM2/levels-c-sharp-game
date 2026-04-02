using Godot;
using System;

public partial class PuzzleBlock : Area2D
{
	[Export] public int SlotNumber = 1;

	public override void _Ready()
	{
		// Łączymy sygnał body_entered
		BodyEntered += OnBodyEntered;
		GD.Print($"[SYSTEM] Blok {SlotNumber} gotowy. Czekam na kolizję...");
	}

	private void OnBodyEntered(Node2D body)
	{
		// LOG: To pokaże się zawsze, gdy cokolwiek dotknie bloku
		GD.Print($">>> KOLIZJA: Blok {SlotNumber} dotknięty przez: {body.Name}");

		// Szukamy managera u rodzica
		var manager = GetParentOrNull<PuzzleManager>();
		
		if (manager != null)
		{
			manager.HandleClick(SlotNumber);
			
			// Mały efekt wizualny (opcjonalnie) - blok mignie gdy go dotkniesz
			Modulate = new Color(2, 2, 2); // Rozjaśnienie
			GetTree().CreateTimer(0.1).Timeout += () => Modulate = new Color(1, 1, 1);
		}
		else
		{
			GD.PrintErr($"[BŁĄD] Blok {SlotNumber} nie widzi PuzzleManager! Sprawdź czy Panel to rodzic.");
		}
	}
}
