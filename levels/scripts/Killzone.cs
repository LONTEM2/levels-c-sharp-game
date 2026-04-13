using Godot;
using System;

public partial class Killzone : Area2D
{
	private bool _isDead = false;

	public void _on_body_entered_kz(Node2D body)
	{
		// Jeśli już trwa proces śmierci, ignoruj kolejne kolizje
		if (_isDead) return;

		// Sprawdzamy czy to gracz (używamy IsInGroup dla pewności)
		if (body is Player || body.IsInGroup("player") || body.IsInGroup("Player"))
		{
			_isDead = true;
			GD.Print("Gracz zginął!");

			// 1. Pobieramy managery
			var musicPlayer = GetNodeOrNull<MusicPlayer>("/root/MusicPlayer");
			var stats = GetNodeOrNull<StatsManager>("/root/StatsManager");
			var gm = GetTree().CurrentScene.GetNodeOrNull<GameManager>("GameManager");

			// 2. Dźwięk śmierci
			musicPlayer?.PlaySfx("res://audio/sfx/Die.wav");

			// 3. Statystyki: Rozpoznawanie typu śmierci
			if (stats != null && gm != null)
			{
				string deathType = "jump"; // Domyślnie skok w przepaść

				// Sprawdzamy, czy Killzone jest częścią Slime'a na podstawie skryptu lub grupy
				if (GetParent() is Slime || GetParent().IsInGroup("slimes")) 
				{
					deathType = "slime";
				}

				// TYLKO JEDNO WYWOŁANIE (naprawione powtórzenie)
				stats.AddDeath(gm.currentLevelNum, deathType);
			}

			// 4. Bezpieczny restart sceny
			CallDeferred(MethodName.RestartLevel);
		}
	}

	private void RestartLevel()
	{
		Engine.TimeScale = 1.0f;
		
		var musicPlayer = GetNodeOrNull<MusicPlayer>("/root/MusicPlayer");
		musicPlayer?.ForceReset();

		// Reload sceny zresetuje flagę _isDead automatycznie
		GetTree().ReloadCurrentScene();
	}
}
