using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 120f;
	public const float JumpVelocity = -300.0f;
	
	private AnimatedSprite2D _sprite;
	// KROK 3: Zmienna dla dźwięku skoku
	private AudioStreamPlayer2D _jumpSound;

	// --- LOGIKA KLĄTWY ---
	private int _activeCurseMode = 0; 
	private float _curseTimer = 0;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		// KROK 3: Pobranie węzła dźwięku przy starcie
		_jumpSound = GetNode<AudioStreamPlayer2D>("JumpSound");
	}

	public void ApplyCurse(int mode, float duration)
	{
		_activeCurseMode = mode;
		_curseTimer = duration;

		if (mode == 1) Modulate = new Color(0.7f, 0.3f, 1.0f);
		else if (mode == 2) Modulate = new Color(1.0f, 0.3f, 0.3f);
		
		GD.Print($"AKTYWOWANO KLĄTWĘ TRYB: {mode} NA {duration}s");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_activeCurseMode > 0)
		{
			_curseTimer -= (float)delta;
			if (_curseTimer <= 0)
			{
				_activeCurseMode = 0;
				Modulate = new Color(1, 1, 1);
				GD.Print("Klątwa minęła!");
			}
		}

		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		Vector2 rawInput = Input.GetVector("left", "right", "jump", "ui_down");
		Vector2 finalDirection = rawInput;

		if (_activeCurseMode == 1) 
		{
			finalDirection.X = -rawInput.X;
		}
		else if (_activeCurseMode == 2) 
		{
			finalDirection.X = -rawInput.Y; 
			finalDirection.Y = rawInput.X;  
		}

		// KROK 3: Odtwarzanie dźwięku przy skoku
		if ((Input.IsActionJustPressed("jump") || (_activeCurseMode == 2 && finalDirection.Y > 0.5f)) && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			_jumpSound.Play(); // <--- Odtwórz dżwięk jump.wav
		}

		if (!IsOnFloor())
		{
			_sprite.Play("jump");
		}
		else if (finalDirection.X != 0)
		{
			_sprite.Play("run");
		}
		else
		{
			_sprite.Play("idle");
		}

		if (finalDirection.X > 0) _sprite.FlipH = false;
		else if (finalDirection.X < 0) _sprite.FlipH = true;

		if (finalDirection.X != 0)
		{
			velocity.X = finalDirection.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
