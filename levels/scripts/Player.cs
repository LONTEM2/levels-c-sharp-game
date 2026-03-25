using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 120f;
	public const float JumpVelocity = -300.0f;
	
	private AnimatedSprite2D _sprite;

	public override void _Ready()
	{
		// Musisz zainicjalizować sprite'a, inaczej wywali błąd (null reference)
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Grawitacja
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Skok
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Pobieranie kierunku (X to lewo/prawo)
		Vector2 direction = Input.GetVector("left", "right", "jump", "ui_down");

		// --- LOGIKA ANIMACJI ---
		if (!IsOnFloor())
		{
			_sprite.Play("jump");
		}
		else if (direction.X != 0)
		{
			_sprite.Play("run");
		}
		else
		{
			_sprite.Play("idle");
		}

		// --- OBRACANIE (FLIP) ---
		if (direction.X > 0)
		{
			_sprite.FlipH = false;
		}
		else if (direction.X < 0)
		{
			_sprite.FlipH = true;
		}

		// --- RUCH ---
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
