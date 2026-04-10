using Godot;
using System;

public partial class Slime : Node2D
{
	// Zamieniamy 'const' na '[Export]'. 
	// Teraz zobaczysz suwak/pole w Inspektorze Godota.
	[Export] public float Speed = 60.0f; 

	private int _direction = 1;

	private RayCast2D _rayCastRight;
	private RayCast2D _rayCastLeft;
	private AnimatedSprite2D _sprite;

	public override void _Ready()
	{
		_rayCastRight = GetNode<RayCast2D>("RayCastRight");
		_rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _Process(double delta)
	{
		// Sprawdzanie kolizji po prawej
		if (_rayCastRight.IsColliding())
		{
			var collider = _rayCastRight.GetCollider();
			if (!(collider is CharacterBody2D)) 
			{
				_direction = -1;
				if (_sprite != null) _sprite.FlipH = true;
			}
		}
		
		// Sprawdzanie kolizji po lewej
		if (_rayCastLeft.IsColliding())
		{
			var collider = _rayCastLeft.GetCollider();
			if (!(collider is CharacterBody2D)) 
			{
				_direction = 1;
				if (_sprite != null) _sprite.FlipH = false;
			}
		}

		Vector2 currentPos = Position;
		// Używamy zmiennej Speed zamiast stałej
		currentPos.X += (float)(_direction * Speed * delta);
		Position = currentPos;
	}
}
