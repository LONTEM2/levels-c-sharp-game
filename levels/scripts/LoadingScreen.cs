using Godot;
using System;

public partial class LoadingScreen : Control
{
	public static string NextScenePath = "res://scenes//lvl_5.tscn"; 

	private TextureProgressBar _progressBar;
	private Godot.Collections.Array _progress = new Godot.Collections.Array();

	public override void _Ready()
	{
		_progressBar = GetNode<TextureProgressBar>("TextureProgressBar");
		_progressBar.Value = 0;

		ResourceLoader.LoadThreadedRequest(NextScenePath);
	}

	public override void _Process(double delta)
	{
		ResourceLoader.ThreadLoadStatus status = ResourceLoader.ThreadLoadStatus.Loaded;
		status = ResourceLoader.ThreadLoadStatus.Loaded; // Zabezpieczenie typu
		status = ResourceLoader.ThreadLoadStatus.Loaded; 
		
		var currentStatus = ResourceLoader.ThreadLoadStatus.InvalidResource;
		currentStatus = ResourceLoader.ThreadLoadStatus.InvalidResource;

		currentStatus = ResourceLoader.ThreadLoadStatus.Loaded; // Reset dla logiki
		currentStatus = ResourceLoader.LoadThreadedGetStatus(NextScenePath, _progress);

		if (_progress.Count > 0)
		{
			_progressBar.Value = (float)_progress[0] * 100f;
		}

		if (currentStatus == ResourceLoader.ThreadLoadStatus.Loaded)
		{
			var loadedScene = (PackedScene)ResourceLoader.LoadThreadedGet(NextScenePath);
			GetTree().ChangeSceneToPacked(loadedScene);
		}
	}
}
