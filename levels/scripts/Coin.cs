using Godot;
using System;

public partial class Coin : Area2D
{
	// Unikalne ID dla każdej monety na poziomie (ustaw w Inspektorze!)
	[Export] public int CoinId = 0; 

	private GameManager _gameManager;
	private AudioStreamPlayer2D _coinSound;
	private bool _collected = false;

	public override void _Ready()
	{
		// 1. Rejestracja monety w grupie (ważne dla systemu checkpointów)
		AddToGroup("coins");

		// 2. Jeśli zapomniałeś nadać ID w starym levelu, moneta dostanie losowe, 
		// żeby przynajmniej dało się ją podnieść (choć lepiej nadać ręcznie 1, 2, 3...)
		if (CoinId == 0) 
		{
			CoinId = (int)GD.Randi(); 
		}

		// 3. Pobieranie komponentów
		_coinSound = GetNodeOrNull<AudioStreamPlayer2D>("CoinSound");
		
		// Próbujemy znaleźć GameManager w korzeniu sceny
		_gameManager = GetTree().CurrentScene.GetNodeOrNull<GameManager>("GameManager");
		
		// Jeśli nadal null, spróbujmy znaleźć go gdziekolwiek w scenie (wolniejsze, ale pewne)
		if (_gameManager == null)
		{
			_gameManager = GetTree().CurrentScene.FindChild("GameManager", true, false) as GameManager;
		}
	}

	public void _on_body_entered(Node2D body)
	{
		// Zabezpieczenie przed podwójnym zebraniem
		if (_collected) return;

		// Sprawdzamy czy to gracz (szukamy grupy "player" lub nazwy)
		bool isPlayer = body.IsInGroup("player") || body.Name.ToString().ToLower().Contains("player");
		
		if (isPlayer)
		{
			CollectCoin();
		}
	}

	private void CollectCoin()
	{
		_collected = true;

		// 1. Powiadomienie GameManagera (przekazujemy ID)
		if (_gameManager != null)
		{
			_gameManager.AddPoint(CoinId);
		}
		else
		{
			GD.PrintErr($"BŁĄD: Moneta {Name} nie znalazła GameManagera! Sprawdź nazwę w drzewie sceny.");
		}

		// 2. Efekty dźwiękowe
		if (_coinSound != null)
		{
			_coinSound.Play();
		}

		// 3. Efekty wizualne
		Hide(); 
		
		// Wyłączamy kolizje natychmiast (SetDeferred zapobiega błędom fizyki)
		SetDeferred(PropertyName.Monitoring, false);

		// 4. Sprzątanie: usuwamy monetę z gry dopiero gdy dźwięk wybrzmi (0.5s)
		GetTree().CreateTimer(0.5f).Timeout += () => QueueFree();
	}
}
