using Godot;
using System;

// Zmieniamy MyCamera na Camera2D, aby pasowało do nazwy pliku
public partial class Camera2D : Godot.Camera2D 
{
	private float _shakeAmount = 0.0f;
	private Random _random = new Random();

	public override void _Process(double delta)
	{
		if (_shakeAmount > 0)
		{
			Offset = new Vector2(
				(float)_random.NextDouble() * _shakeAmount - _shakeAmount / 2,
				(float)_random.NextDouble() * _shakeAmount - _shakeAmount / 2
			);
		}
		else
		{
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
	}
}
