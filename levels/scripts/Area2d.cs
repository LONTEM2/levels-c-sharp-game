using Godot;
using System;
using System.Collections.Generic;

public partial class Area2d : Area2D
{
	public enum CurseType { None, LeftRight, Chaos }

	[ExportGroup("Standard Settings")]
	[Export] public NodePath MonetaPath = "";
	[Export] public Vector2 TeleportTarget = new Vector2(725, -50);
	
	[ExportGroup("Curse Settings")]
	[Export] public CurseType SelectedCurse = CurseType.None; 
	[Export] public float ReverseDuration = 120.0f;

	[ExportGroup("Troll/Trap Settings")]
	[Export] public bool IsTrollTP = false;
	[Export] public int TrollLimit = 3;
	[Export] public Vector2 TrollTarget = new Vector2(0, 0);
	[Export] public float ShakeIntensityPerTroll = 5.0f;
	[Export] public float SlimeSpeedBoost = 20.0f;

	[ExportGroup("Troll Delete Settings")]
	[Export] public NodePath NodeToDeletePath = ""; // Tu w Inspektorze wybierz co ma zniknąć
	[Export] public int DeleteAtTrollCount = 1;      // Przy którym przeteleportowaniu ma usunąć obiekt?

	private int _teleportCount = 0;

	public void _on_tp_body_entered(Node2D body)
	{
		if (body.Name.ToString().ToLower().Contains("player"))
		{
			if (IsTrollTP && _teleportCount < TrollLimit)
			{
				_teleportCount++;
				body.GlobalPosition = TrollTarget;

				// USUWANIE OBIEKTU (Troll Delete)
				if (_teleportCount == DeleteAtTrollCount)
				{
					var nodeToDelete = GetNodeOrNull<Node2D>(NodeToDeletePath);
					if (nodeToDelete != null)
					{
						GD.Print($">>> TROLL: Usuwam obiekt {nodeToDelete.Name}!");
						nodeToDelete.QueueFree();
					}
				}

				ApplyCameraShake(body);
				BoostAllSlimes();

				GD.Print($">>> TROLL {_teleportCount}/{TrollLimit}! Speed Up & Shake!");
			}
			else
			{
				body.GlobalPosition = TeleportTarget;
				ResetCameraEffect(body);
			}

			if (SelectedCurse != CurseType.None && body.HasMethod("ApplyCurse"))
			{
				body.Call("ApplyCurse", (int)SelectedCurse, ReverseDuration);
			}

			var moneta = GetNodeOrNull<Node2D>(MonetaPath);
			if (moneta != null) moneta.Show();
		}
	}

	private void ApplyCameraShake(Node2D player)
	{
		var camera = player.GetNodeOrNull<Camera2D>("Camera2D");
		if (camera != null && camera.HasMethod("AddShake"))
		{
			camera.Call("AddShake", _teleportCount * ShakeIntensityPerTroll);
		}
	}

	private void BoostAllSlimes()
	{
		var slimes = GetTree().GetNodesInGroup("slimes");
		foreach (Node node in slimes)
		{
			if (node is Slime slime)
			{
				slime.Speed += SlimeSpeedBoost;
			}
		}
	}

	private void ResetCameraEffect(Node2D player)
	{
		var camera = player.GetNodeOrNull<Camera2D>("Camera2D");
		if (camera != null && camera.HasMethod("ResetShake")) camera.Call("ResetShake");
	}
}
