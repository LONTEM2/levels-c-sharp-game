using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 120f;
	[Export] public float JumpVelocity = -300.0f;
	[Export] public float Acceleration = 800f; // Prędkość dochodzenia do celu podczas klątwy

	private AnimatedSprite2D _sprite;
	private AudioStreamPlayer2D _jumpSound;

	private bool _cLeftRight = false;
	private bool _cChaos = false;
	private bool _cFlip180 = false;
	private float _curseTimer = 0;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_jumpSound = GetNode<AudioStreamPlayer2D>("JumpSound");
	}

	public void ApplyCurseMulti(bool lr, bool chaos, bool flip, float duration)
	{
		_cLeftRight = lr;
		_cChaos = chaos;
		_cFlip180 = flip;
		_curseTimer = duration;
		
		Modulate = (lr || chaos || flip) ? new Color(0.8f, 0.4f, 1.0f) : new Color(1, 1, 1);
	}

	public override void _PhysicsProcess(double delta)
	{
		// Obsługa timera klątwy
		if (_curseTimer > 0)
		{
			_curseTimer -= (float)delta;
			if (_curseTimer <= 0)
			{
				_cLeftRight = _cChaos = _cFlip180 = false;
				Modulate = new Color(1, 1, 1);
			}
		}

		Vector2 velocity = Velocity;

		// Grawitacja
		if (!IsOnFloor()) 
			velocity += GetGravity() * (float)delta;

		// 1. Pobieranie Inputu (GetAxis rozwiązuje konflikt lewo+prawo)
		float moveInput = Input.GetAxis("left", "right");
		float verticalInput = Input.GetAxis("ui_up", "ui_down");

		// 2. Logika kierunku z uwzględnieniem klątw
		float directionX = moveInput;

		if (_cLeftRight) directionX = -directionX;
		if (_cFlip180)   directionX = -directionX;

		// Chaos zamienia osie (Góra/Dół steruje teraz lewo/prawo)
		if (_cChaos) 
		{
			directionX = -verticalInput; 
		}

		// 3. Obliczanie prędkości poziomej
		float targetX = directionX * Speed;

		if (_cLeftRight || _cChaos || _cFlip180)
		{
			// Podczas klątwy: Płynne przyspieszenie (MoveToward jest bardziej responsywny niż Lerp)
			velocity.X = Mathf.MoveToward(velocity.X, targetX, Acceleration * (float)delta);
		}
		else
		{
			// Tryb normalny: Natychmiastowe zatrzymanie i start
			velocity.X = targetX;
		}

		// 4. Skok
		bool jumpPressed = Input.IsActionJustPressed("jump");
		
		// Warunek skoku: przycisk skoku LUB (Chaos i wychylenie "w górę" na osi Y)
		if ((jumpPressed || (_cChaos && verticalInput < -0.5f)) && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			_jumpSound?.Play();
		}

		// Zastosowanie ruchu
		Velocity = velocity;
		MoveAndSlide();
		
		// Animacje na podstawie wektora kierunku
		UpdateAnimations(new Vector2(directionX, 0));
	}

	private void UpdateAnimations(Vector2 direction)
	{
		if (!IsOnFloor()) 
			_sprite.Play("jump");
		else if (Mathf.Abs(Velocity.X) > 5.0f) 
			_sprite.Play("run");
		else 
			_sprite.Play("idle");

		if (direction.X > 0) 
			_sprite.FlipH = false;
		else if (direction.X < 0) 
			_sprite.FlipH = true;
	}
}
