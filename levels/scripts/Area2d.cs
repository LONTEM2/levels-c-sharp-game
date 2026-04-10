using Godot;
using System;

public partial class Area2d : Area2D
{
	[ExportGroup("Standard Settings")]
	[Export] public NodePath MonetaPath = "";
	[Export] public Vector2 TeleportTarget = new Vector2(725, -50);
	
	[ExportGroup("Curse Settings")]
	[Export] public float ReverseDuration = 10.0f;
	[Export] public bool CurseLeftRight = false;
	[Export] public bool CurseChaos = false;
	[Export] public bool CurseFlip180 = false;
	[Export] public bool CurseFlip90 = false; // NOWOŚĆ: Obrót o 90 stopni
	[Export] public bool CurseInvert = false;
	[Export] public string CurseMusicPath = "res://audio/music/CurseTheme.ogg";

	[ExportGroup("Troll/Trap Settings")]
	[Export] public bool IsTrollTP = false;
	[Export] public int TrollLimit = 3;
	[Export] public Vector2 TrollTarget = new Vector2(0, 0);
	[Export] public float ShakeIntensityPerTroll = 5.0f;
	[Export] public float SlimeSpeedBoost = 20.0f;

	private int _teleportCount = 0;

	public void _on_tp_body_entered(Node2D body)
	{
		if (body is Player player)
		{
			var musicPlayer = GetNodeOrNull<MusicPlayer>("/root/MusicPlayer");
			musicPlayer?.PlaySfx("res://audio/sfx/Die.wav");

			if (IsTrollTP && _teleportCount < TrollLimit)
			{
				_teleportCount++;
				player.GlobalPosition = TrollTarget;
				ApplyCameraShake(player);
				BoostAllSlimes();
			}
			else
			{
				player.GlobalPosition = TeleportTarget;
				ResetCameraEffect(player);
			}

			// Aktywacja mechanik w Playerze
			player.ApplyCurseMulti(CurseLeftRight, CurseChaos, CurseFlip180, ReverseDuration);
			// Przekazujemy Flip90 jako Meta-dane lub przez nową metodę, jeśli ją dopiszesz w Playerze
			if (CurseFlip90) player.SetMeta("cFlip90", true);

			ApplyVisualAndAudioCurses(player, musicPlayer);

			var moneta = GetNodeOrNull<Node2D>(MonetaPath);
			if (moneta != null) moneta.Show();
		}
	}

	private void ApplyVisualAndAudioCurses(Player player, MusicPlayer musicPlayer)
	{
		var camera = player.GetNodeOrNull<Camera2D>("Camera2D");

		// 1. Muzyka
		if (CurseLeftRight || CurseChaos || CurseFlip180 || CurseFlip90 || CurseInvert)
		{
			musicPlayer?.StartCurseMusic(CurseMusicPath);
			GetTree().CreateTimer(ReverseDuration).Timeout += () => {
				if (IsInstanceValid(player) && player.Get("_curseTimer").AsSingle() <= 0.1f)
					musicPlayer?.StopCurseMusic();
			};
		}

		// 2. Kamera (Rotacja)
		if (camera != null)
		{
			if (CurseFlip180) camera.RotationDegrees = 180;
			else if (CurseFlip90) camera.RotationDegrees = 90;

			GetTree().CreateTimer(ReverseDuration).Timeout += () => {
				if (IsInstanceValid(player))
				{
					bool stillFlipped = player.Get("_cFlip180").AsBool() || 
									   (player.HasMeta("cFlip90") && player.GetMeta("cFlip90").AsBool());
					if (!stillFlipped && IsInstanceValid(camera)) camera.RotationDegrees = 0;
				}
			};
		}

		// 3. Invert
		if (CurseInvert)
		{
			var shaderNode = GetTree().Root.FindChild("InvertEffect", true, false) as CanvasItem;
			if (shaderNode != null)
			{
				shaderNode.Show();
				GetTree().CreateTimer(ReverseDuration).Timeout += () => {
					if (IsInstanceValid(player) && player.Get("_curseTimer").AsSingle() <= 0.1f)
						if (IsInstanceValid(shaderNode)) shaderNode.Hide();
				};
			}
		}
	}

	private void ApplyCameraShake(Node2D player)
	{
		var camera = player.GetNodeOrNull<Camera2D>("Camera2D");
		if (camera != null && camera.HasMethod("AddShake"))
			camera.Call("AddShake", _teleportCount * ShakeIntensityPerTroll);
	}

	private void ResetCameraEffect(Node2D player)
	{
		var camera = player.GetNodeOrNull<Camera2D>("Camera2D");
		if (camera != null && camera.HasMethod("ResetShake")) camera.Call("ResetShake");
	}

	private void BoostAllSlimes()
	{
		foreach (Node node in GetTree().GetNodesInGroup("slimes"))
		{
			if (node is CharacterBody2D slime)
			{
				float speed = slime.Get("Speed").AsSingle();
				slime.Set("Speed", speed + SlimeSpeedBoost);
			}
		}
	}
}
