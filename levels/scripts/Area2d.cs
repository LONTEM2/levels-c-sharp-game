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

			// Teleportacja
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

			// Aktywacja mechanik
			player.ApplyCurseMulti(CurseLeftRight, CurseChaos, CurseFlip180, ReverseDuration);
			ApplyVisualAndAudioCurses(player, musicPlayer);

			var moneta = GetNodeOrNull<Node2D>(MonetaPath);
			if (moneta != null) moneta.Show();
		}
	}

	private void ApplyVisualAndAudioCurses(Player player, MusicPlayer musicPlayer)
{
	var camera = player.GetNodeOrNull<Camera2D>("Camera2D");

	// 1. Muzyka
	if (CurseLeftRight || CurseChaos || CurseFlip180 || CurseInvert)
	{
		musicPlayer?.StartCurseMusic(CurseMusicPath);
		
		// Zamiast prostego timera, sprawdzamy stan gracza przy kończeniu
		GetTree().CreateTimer(ReverseDuration).Timeout += () => {
			// Sprawdzamy, czy u gracza timer klątwy faktycznie dobiegł końca
			// Musisz dodać publiczny dostęp do _curseTimer lub sprawdzić Modulate
			if (IsInstanceValid(player) && player.Get("_curseTimer").AsSingle() <= 0.1f)
			{
				musicPlayer?.StopCurseMusic();
			}
		};
	}

	// 2. Kamera 180
	if (CurseFlip180 && camera != null)
	{
		camera.RotationDegrees = 180;
		GetTree().CreateTimer(ReverseDuration).Timeout += () => {
			// Tutaj też sprawdzamy, czy nie nałożono nowej klątwy obrotu
			if (IsInstanceValid(player) && player.Get("_cFlip180").AsBool() == false)
			{
				if (IsInstanceValid(camera)) camera.RotationDegrees = 0;
			}
		};
	}

	// 3. Shader Negatywu (InvertEffect)
	if (CurseInvert)
	{
		var shaderNode = GetTree().Root.FindChild("InvertEffect", true, false) as CanvasItem;
		if (shaderNode != null)
		{
			shaderNode.Show();
			GetTree().CreateTimer(ReverseDuration).Timeout += () => {
				// Sprawdzamy, czy gracz nadal powinien mieć negatyw
				if (IsInstanceValid(player) && player.Get("_curseTimer").AsSingle() <= 0.1f)
				{
					if (IsInstanceValid(shaderNode)) shaderNode.Hide();
				}
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
		var slimes = GetTree().GetNodesInGroup("slimes");
		foreach (Node node in slimes)
		{
			if (node is CharacterBody2D slime)
			{
				Variant speed = slime.Get("Speed");
				if (speed.VariantType != Variant.Type.Nil)
					slime.Set("Speed", speed.AsSingle() + SlimeSpeedBoost);
			}
		}
	}
}
