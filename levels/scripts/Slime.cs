using Godot;
using System;

public partial class Slime : Node2D
{
	private const float Speed = 60.0f;
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
			// Jeśli to, co wykryliśmy, NIE jest Graczem (zakładając, że Gracz to np. CharacterBody2D)
			// Możesz tu sprawdzić nazwę: collider.GetName() == "Player" 
			// Albo klasę, jak poniżej:
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
		currentPos.X += (float)(_direction * Speed * delta);
		Position = currentPos;
	}
}
