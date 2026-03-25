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
		GD.Print("+1 coin!");
		// Wywołujemy metodę, którą stworzyliśmy wcześniej
		_gameManager.AddPoint();
		// Usuwamy monetę
		QueueFree();
	}
}
