using Godot;
using System;
using System.Collections.Generic;

public partial class MusicPlayer : AudioStreamPlayer
{
	[Export] public string MusicFolderPath = "res://audio/music/";
	private List<string> _playlist = new List<string>();
	private string _lastTrack = "";
	private bool _isCurseActive = false;

	public override void _Ready()
	{
		ApplySavedVolume();
		LoadPlaylist();
		PlayRandomTrack();
		Finished += OnTrackFinished;
	}

	// Wywołaj to przy śmierci gracza!
	public void ForceReset()
	{
		if (!_isCurseActive) return;
		_isCurseActive = false;
		PlayRandomTrack();
	}

	public void StartCurseMusic(string path)
	{
		_isCurseActive = true;
		AudioStream stream = GD.Load<AudioStream>(path);
		if (stream != null)
		{
			Stream = stream;
			Play();
		}
	}

	public void StopCurseMusic()
	{
		if (!_isCurseActive) return;
		_isCurseActive = false;
		PlayRandomTrack(); 
	}

	public void PlayRandomTrack()
	{
		if (_playlist.Count == 0 || _isCurseActive) return;

		string nextTrack;
		do {
			int index = (int)(GD.Randi() % _playlist.Count);
			nextTrack = _playlist[index];
		} while (_playlist.Count > 1 && nextTrack == _lastTrack);

		_lastTrack = nextTrack;
		Stream = GD.Load<AudioStream>(nextTrack);
		Play();
	}

	private void OnTrackFinished()
	{
		if (!_isCurseActive) PlayRandomTrack();
	}

	public void PlaySfx(string soundPath)
	{
		AudioStreamPlayer sfxPlayer = new AudioStreamPlayer();
		AddChild(sfxPlayer);
		sfxPlayer.Stream = GD.Load<AudioStream>(soundPath);
		sfxPlayer.Bus = "SFX";
		sfxPlayer.Play();
		sfxPlayer.Finished += () => sfxPlayer.QueueFree();
	}

	private void ApplySavedVolume()
	{
		ConfigFile config = new ConfigFile();
		if (config.Load("user://settings.cfg") == Error.Ok)
		{
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
				if (!dir.CurrentIsDir() && (fileName.EndsWith(".mp3") || fileName.EndsWith(".ogg") || fileName.EndsWith(".wav")))
				{
					_playlist.Add(MusicFolderPath + fileName);
				}
				fileName = dir.GetNext();
			}
		}
	}
}
