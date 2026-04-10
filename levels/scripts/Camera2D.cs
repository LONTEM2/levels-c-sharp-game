using Godot;
using System;

public partial class Camera2D : Godot.Camera2D 
{
	private float _shakeAmount = 0.0f;
	private Random _random = new Random();
	
	[Export] public float ShakeDecay = 5.0f;

	public override void _Ready()
	{
		// Kluczowe: pozwala kamerze się obracać o 180 stopni
		IgnoreRotation = false; 
	}

	public override void _Process(double delta)
	{
		if (_shakeAmount > 0)
		{
			Offset = new Vector2(
				(float)_random.NextDouble() * _shakeAmount - _shakeAmount / 2,
				(float)_random.NextDouble() * _shakeAmount - _shakeAmount / 2
			);
			_shakeAmount -= (float)delta * ShakeDecay;
		}
		else
		{
			_shakeAmount = 0;
			Offset = Vector2.Zero;
		}
	}

	public void AddShake(float amount)
	{
		_shakeAmount = amount;
	}

	public void ResetShake()
	{
		_shakeAmount = 0;
		Offset = Vector2.Zero;
		RotationDegrees = 0;
	}
}
