using Godot;
using System;

public partial class Coin : Area2D
{
	private GameManager _gameManager;
	private AudioStreamPlayer2D _coinSound;
	private bool _collected = false;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("%GameManager");
		_coinSound = GetNode<AudioStreamPlayer2D>("CoinSound");
	}

	public void _on_body_entered(Node2D body)
	{
		if (_collected) return;
		
		_collected = true;

		// 1. Odtwórz dźwięk
		if (_coinSound != null)
		{
			_coinSound.Play();
		}

		// 2. Dodaj punkt w GameManagerze
		if (_gameManager != null)
		{
			_gameManager.AddPoint();
		}

		// 3. Wizualne ukrycie monety
		Hide(); 

		// NAPRAWA BŁĘDU: Używamy SetDeferred zamiast zwykłego Monitoring = false;
		// Zapobiega to błędowi "Function blocked during in/out signal"
		SetDeferred(PropertyName.Monitoring, false);

		// 4. Usuwamy obiekt z pamięci po krótkiej chwili (żeby dźwięk zdążył wybrzmieć)
		GetTree().CreateTimer(0.5f).Timeout += () => QueueFree();
	}
}
