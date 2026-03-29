using Godot;
using System;

public partial class Area2d : Area2D
{
<<<<<<< Updated upstream
	[Export] public NodePath MonetaPath = "../Coin5";
=======
	[Export] public NodePath MonetaPath = "";
>>>>>>> Stashed changes
	[Export] public Vector2 TeleportTarget = new Vector2(725, -50);

	// Ta funkcja musi być PUBLICZNA
	public void _on_tp_body_entered(Node2D body)
	{
		GD.Print(">>> KOLIZJA WYKRYTA Z: " + body.Name);

		if (body.Name.ToString().ToLower().Contains("player"))
		{
			GD.Print(">>> TELEPORTACJA!");
			
			var moneta = GetNodeOrNull<Node2D>(MonetaPath);
			if (moneta != null) moneta.Show();

			body.GlobalPosition = TeleportTarget;
		}
	}
}
