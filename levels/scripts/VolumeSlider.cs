using Godot;
using System;

public partial class VolumeSlider : HSlider
{
	[Export] public string BusName = "Music";
	private int _busIndex;
	private string _savePath = "user://settings.cfg";

	public override void _Ready()
	{
		_busIndex = AudioServer.GetBusIndex(BusName);
		
		if (_busIndex == -1) return;

		// 1. Wczytaj zapisane ustawienie przy starcie
		LoadVolume();

		ValueChanged += OnVolumeChanged;
		// 2. Zapisz, gdy użytkownik skończy przesuwać (Drag Ended)
		DragEnded += (bool valueChanged) => SaveVolume();
	}

	private void OnVolumeChanged(double value)
	{
		float dbValue = (float)Mathf.LinearToDb(value);
		AudioServer.SetBusVolumeDb(_busIndex, dbValue);
		AudioServer.SetBusMute(_busIndex, value <= 0);
	}

	private void SaveVolume()
	{
		ConfigFile config = new ConfigFile();
		// Ładujemy istniejący plik (żeby nie skasować innych ustawień)
		config.Load(_savePath);
		
		// Zapisujemy wartość suwaka (0.0 - 1.0)
		config.SetValue("Audio", BusName, Value);
		config.Save(_savePath);
		GD.Print("Głośność zapisana!");
	}

	private void LoadVolume()
	{
		ConfigFile config = new ConfigFile();
		Error err = config.Load(_savePath);

		if (err == Error.Ok)
		{
			// Pobierz wartość, a jeśli jej nie ma, ustaw 0.8 jako domyślną
			float savedValue = (float)config.GetValue("Audio", BusName, 0.8f);
			Value = savedValue;
			
			// Od razu zastosuj głośność do miksera
			OnVolumeChanged(savedValue);
		}
	}
}
