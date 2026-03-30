using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 120f;
	public const float JumpVelocity = -300.0f;
	
	private AnimatedSprite2D _sprite;

	// --- LOGIKA KLĄTWY ---
	// 0 = brak, 1 = Lewo/Prawo, 2 = Chaos
	private int _activeCurseMode = 0; 
	private float _curseTimer = 0;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Ta funkcja jest wywoływana przez Area2D (pamiętaj o body.Call w Area2D)
	public void ApplyCurse(int mode, float duration)
	{
		_activeCurseMode = mode;
		_curseTimer = duration;

		// Zmiana koloru dla feedbacku: Fioletowy (Tryb 1), Czerwony (Tryb 2)
		if (mode == 1) Modulate = new Color(0.7f, 0.3f, 1.0f);
		else if (mode == 2) Modulate = new Color(1.0f, 0.3f, 0.3f);
		
		GD.Print($"AKTYWOWANO KLĄTWĘ TRYB: {mode} NA {duration}s");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Obsługa timera klątwy
		if (_activeCurseMode > 0)
		{
			_curseTimer -= (float)delta;
			if (_curseTimer <= 0)
			{
				_activeCurseMode = 0;
				Modulate = new Color(1, 1, 1); // Powrót do normalnego koloru
				GD.Print("Klątwa minęła!");
			}
		}

		Vector2 velocity = Velocity;

		// Grawitacja
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Pobieranie surowego wektora kierunku
		// Zakładam mapowanie: lewo=A, prawo=D, jump=W, ui_down=S
		Vector2 rawInput = Input.GetVector("left", "right", "jump", "ui_down");
		Vector2 finalDirection = rawInput;

		// --- LOGIKA TRYBÓW STEROWANIA ---
		if (_activeCurseMode == 1) 
		{
			// Tryb 1: Tylko Lewo <-> Prawo
			finalDirection.X = -rawInput.X;
		}
		else if (_activeCurseMode == 2) 
		{
			// Tryb 2: Chaos (A->S, D->W, W->D, S->A)
			// Mapowanie wektora:
			// X (prawo/lewo) bierze wartość z osi Y
			// Y (góra/dół) bierze wartość z osi X
			finalDirection.X = -rawInput.Y; 
			finalDirection.Y = rawInput.X;  
		}

		// Skok (Używamy finalDirection.Y, bo w Chaosie 'D' może wywołać skok)
		// Jeśli chcesz, żeby skok na spację/W działał zawsze mimo klątwy, 
		// użyj Input.IsActionJustPressed("jump") zamiast finalDirection.
		if ((Input.IsActionJustPressed("jump") || (_activeCurseMode == 2 && finalDirection.Y > 0.5f)) && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// --- LOGIKA ANIMACJI I RUCHU ---
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

		// Obracanie sprite'a
		if (finalDirection.X > 0) _sprite.FlipH = false;
		else if (finalDirection.X < 0) _sprite.FlipH = true;

		// Ruch poziomy
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
