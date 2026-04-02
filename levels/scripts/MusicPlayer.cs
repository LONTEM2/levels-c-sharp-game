using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MusicPlayer : AudioStreamPlayer
{
	[Export] public string MusicFolderPath = "res://audio/music/"; // Ścieżka do Twoich piosenek
	private List<string> _playlist = new List<string>();
	private string _lastTrack = "";

	public override void _Ready()
	{
		ApplySavedVolume();
		LoadPlaylist();
		PlayRandomTrack();

		// Łączymy sygnał zakończenia utworu, aby puścić następny
		Finished += OnTrackFinished;
		
	}
	// W MusicPlayer.cs
public void PlaySfx(string soundPath)
{
	// Tworzymy tymczasowy odtwarzacz, który sam się usunie po zagraniu
	AudioStreamPlayer sfxPlayer = new AudioStreamPlayer();
	AddChild(sfxPlayer);
	
	sfxPlayer.Stream = GD.Load<AudioStream>(soundPath);
	sfxPlayer.Bus = "SFX"; // Ustawiamy szynę SFX, żeby suwak działał
	sfxPlayer.Play();
	
	// Usuwamy węzeł z pamięci automatycznie, gdy dźwięk się skończy
	sfxPlayer.Finished += () => sfxPlayer.QueueFree();
}
	private void ApplySavedVolume()
{
	ConfigFile config = new ConfigFile();
	if (config.Load("user://settings.cfg") == Error.Ok)
	{
		// Lista szyn do wczytania
		string[] buses = { "Music", "SFX" };

		foreach (string busName in buses)
		{
			float vol = (float)config.GetValue("Audio", busName, 0.8f);
			int idx = AudioServer.GetBusIndex(busName);
			
			if (idx != -1)
			{
				AudioServer.SetBusVolumeDb(idx, (float)Mathf.LinearToDb(vol));
				AudioServer.SetBusMute(idx, vol <= 0);
			}
		}
		GD.Print("Ustawienia Audio (Music & SFX) wczytane.");
	}
}

	private void LoadPlaylist()
	{
		using var dir = DirAccess.Open(MusicFolderPath);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();

			while (fileName != "")
			{
				// Dodajemy tylko pliki audio (importowane przez Godota)
				// W wyeksportowanej grze szukamy plików .import lub sprawdzamy rozszerzenia
				if (!dir.CurrentIsDir() && (fileName.EndsWith(".mp3") || fileName.EndsWith(".ogg") || fileName.EndsWith(".wav")))
				{
					_playlist.Add(MusicFolderPath + fileName);
				}
				fileName = dir.GetNext();
			}
		}
		else
		{
			GD.PrintErr($"BŁĄD: Nie znaleziono folderu z muzyką: {MusicFolderPath}");
		}
	}

	public void PlayRandomTrack()
	{
		if (_playlist.Count == 0) return;

		// Losujemy utwór (staramy się, żeby nie był ten sam co przed chwilą)
		string nextTrack;
		do {
			int index = GD.RandRange(0, _playlist.Count - 1);
			nextTrack = _playlist[index];
		} while (_playlist.Count > 1 && nextTrack == _lastTrack);

		_lastTrack = nextTrack;
		
		// Ładowanie i odtwarzanie
		AudioStream stream = GD.Load<AudioStream>(nextTrack);
		Stream = stream;
		Play();
		
		GD.Print($"Gram teraz: {nextTrack}");
	}

	private void OnTrackFinished()
	{
		PlayRandomTrack();
	}
}
