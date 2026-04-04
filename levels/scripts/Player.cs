using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 120f;
	[Export] public float JumpVelocity = -300.0f;
	
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

		if (!IsOnFloor()) velocity += GetGravity() * (float)delta;

		Vector2 rawInput = Input.GetVector("left", "right", "ui_up", "ui_down");
		Vector2 finalDirection = rawInput;

		if (_cLeftRight) finalDirection.X = -finalDirection.X;
		if (_cFlip180)   finalDirection.X = -finalDirection.X;

		if (_cChaos) 
		{
			float oldX = finalDirection.X;
			finalDirection.X = -finalDirection.Y;
			finalDirection.Y = oldX;
		}

		// Skok (95% wysokości przy combo klątw)
		float currentJumpPower = (_cChaos && _cLeftRight) ? JumpVelocity * 0.95f : JumpVelocity;
		bool jumpPressed = Input.IsActionJustPressed("jump");
		
		if ((jumpPressed || (_cChaos && finalDirection.Y > 0.5f)) && IsOnFloor())
		{
			velocity.Y = currentJumpPower;
			_jumpSound?.Play();
		}

		// Ruch: Lerp tylko podczas klątwy dla trudności, inaczej 1:1 responsywność
		float targetX = finalDirection.X * Speed;
		if (_cLeftRight || _cChaos || _cFlip180)
			velocity.X = Mathf.Lerp(velocity.X, targetX, 0.15f);
		else
			velocity.X = targetX;

		UpdateAnimations(finalDirection);
		Velocity = velocity;
		MoveAndSlide();
	}

	private void UpdateAnimations(Vector2 direction)
	{
		if (!IsOnFloor()) _sprite.Play("jump");
		else if (Mathf.Abs(Velocity.X) > 5.0f) _sprite.Play("run");
		else _sprite.Play("idle");

		if (direction.X > 0) _sprite.FlipH = false;
		else if (direction.X < 0) _sprite.FlipH = true;
	}
}
