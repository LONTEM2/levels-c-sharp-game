using Godot;
using System;

public partial class Slime : Node2D
{
	private const float Speed = 60.0f;
	private int _direction = 1;

	// Odpowiedniki @onready
	private RayCast2D _rayCastRight;
	private RayCast2D _rayCastLeft;
	private AnimatedSprite2D _sprite; // Opcjonalnie do obracania grafiki

	public override void _Ready()
	{
		_rayCastRight = GetNode<RayCast2D>("RayCastRight");
		_rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _Process(double delta)
	{
		// Sprawdzanie kolizji
		if (_rayCastRight.IsColliding())
		{
			_direction = -1;
			if (_sprite != null) _sprite.FlipH = true; // Obraca grafikę w lewo
		}
		
		if (_rayCastLeft.IsColliding())
		{
			_direction = 1;
			if (_sprite != null) _sprite.FlipH = false; // Obraca grafikę w prawo
		}
		Vector2 currentPos = Position;
		currentPos.X += (float)(_direction * Speed * delta);
		Position = currentPos;
	}
}
