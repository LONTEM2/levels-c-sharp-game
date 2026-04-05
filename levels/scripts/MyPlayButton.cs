using Godot;
using System;

public partial class MyPlayButton : Button
{
	// Kolory modulacji (filtrów)
	private Color _colorNormalModulate = new Color(1, 1, 1); // Normalny obrazek (biała modulacja = brak zmian)
	private Color _colorHoverModulate = new Color(0.8f, 0.8f, 0.8f); // Ciutkę ciemniejszy filtr

	private StyleBoxTexture _styleNormal;
	private StyleBoxTexture _styleHover;

	public override void _Ready()
	{
		// 1. Pobieramy aktualny StyleBoxTexture z tematu przycisku
		// Zakładam, że masz tam ustawiony "New StyleBoxTexture" w Inspektorze w Theme Overrides -> Styles -> Normal
		var currentStyle = GetThemeStylebox("normal") as StyleBoxTexture;
		
		if (currentStyle == null)
		{
			GD.PrintErr("Błąd: Przycisk nie ma ustawionego StyleBoxTexture w 'Theme Overrides' -> 'Normal'!");
			return;
		}

		// 2. Tworzymy kopię dla stanu normalnego
		_styleNormal = (StyleBoxTexture)currentStyle.Duplicate();
		_styleNormal.ModulateColor = _colorNormalModulate; // Na start jest normalny biały filtr

		// 3. Tworzymy kopię dla stanu hover
		_styleHover = (StyleBoxTexture)currentStyle.Duplicate();
		_styleHover.ModulateColor = _colorHoverModulate; // Na hover jest ciemniejszy filtr

		// 4. Przypisujemy style (na wszelki wypadek, żeby Godot ich nie zgubił)
		// Godot 4 sam ogarnie zmianę na hover!
		AddThemeStyleboxOverride("normal", _styleNormal);
		AddThemeStyleboxOverride("hover", _styleHover);
	}
}
