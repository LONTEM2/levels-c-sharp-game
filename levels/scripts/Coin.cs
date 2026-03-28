using Godot;
using System;

public partial class Coin : Area2D
{
	// Deklarujemy zmienną dla managera
	private GameManager _gameManager;

	public override void _Ready()
	{
		// Pobieramy GameManager po unikalnej nazwie (%)
		// Musisz upewnić się, że w edytorze Godot węzeł GameManager 
		// ma zaznaczoną opcję "Access as Unique Name"
		_gameManager = GetNode<GameManager>("%GameManager");
	}

	public void _on_body_entered(Node2D body)
{
	// 1. Najpierw usuwamy monetę z ekranu
	QueueFree(); 
	
	// 2. Potem próbujemy dodać punkt
	if (_gameManager != null)
	{
		_gameManager.AddPoint();
	}
	else 
	{
		GD.PrintErr("BŁĄD: Moneta nie widzi GameManagera!");
	}
}
}
