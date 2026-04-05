using Godot;
using System;

public partial class Killzone : Area2D
{
	public void _on_body_entered_kz(Node2D body)
	{
		if (body is Player player)
		{
			GD.Print("Gracz zginął!");

			// 1. Dźwięk - poprawiona wielka litera 'D'
			var musicPlayer = GetNodeOrNull<MusicPlayer>("/root/MusicPlayer");
			if (musicPlayer != null)
			{
				// Upewnij się, że w folderze plik to Die.wav a nie die.wav
				musicPlayer.PlaySfx("res://audio/sfx/Die.wav"); 
			}

			// 2. Bezpieczny restart sceny
			// CallDeferred sprawi, że scena zrestartuje się na początku następnej klatki,
			// kiedy silnik fizyki już nie będzie "zablokowany".
			CallDeferred(MethodName.RestartLevel);
		}
	}

	private void RestartLevel()
	{
		Engine.TimeScale = 1.0f;
		GetNode<MusicPlayer>("/root/MusicPlayer").ForceReset();
		GetTree().ReloadCurrentScene();
	}
}
