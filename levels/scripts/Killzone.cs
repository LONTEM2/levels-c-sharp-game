using Godot;
using System;

public partial class Killzone : Area2D
{
	private Timer _timer;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
	}

	public void _on_body_entered(Node2D body)
	{
		GD.Print("You Died!");	
		Engine.TimeScale = 0.5;
		_timer.Start();
		Engine.TimeScale = 1;
		GetTree().ReloadCurrentScene();
	}
}
