using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MainButtons : Node2D
{
	[Export] Godot.Label[] buttons;
	[Export] Node2D iconsBar;
	[Export] Node2D buttonsParent;
	[Export] Node2D selectedPos;
	[Export] Node2D iconsBarParent;
	[Export] Node2D[] icons;
	[Export] AnimationPlayer anim;
	[Export] Node2D[] buttons2;
	public bool selected;
	public bool canX = true;
	Vector2 firstPos;
	int index = 0;
	Tween tween;
	Tween[] tweens;
	Tween tween2;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (this.Name == "MainButtons")
		{
			firstPos = iconsBar.GlobalPosition;	
		}
		else
		{
			SetProcess(false);
		}
		tweens = new Tween[buttons.Count()];
		for (int i = 0; i < buttons.Count(); i++)
		{
			tween = CreateTween();
			tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
			tweens[i] = tween;
		}
		UpdateButton();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Down") && !selected)
		{
			if (index == buttons.Length - 1)
			{
				index = 0;
			}
			else
			{
				index ++;
			}
			UpdateButton();
		}
		if (Input.IsActionJustPressed("Up") && !selected)
		{
			if (index == 0)
			{
				index = buttons.Length - 1;
			}
			else
			{
				index --;
			}
			UpdateButton();
		}
		if (this.Name == "MainButtons")
		{
			UpdateIcons(delta);
		}
		foreach (Godot.Label button in buttons)
		{
			if (button == buttons[index])
			{
				button.Modulate = button.Modulate.Lerp(Colors.Yellow, 5 * (float)delta);
			}
			else
			{
				button.Modulate = button.Modulate.Lerp(Colors.White, 5 * (float)delta);
			}
		}
		if (Input.IsActionJustPressed("Z") && !selected)
		{
			if (buttons[index].Name == "Play" || buttons[index].Name == "Settings" || buttons[index].Name == "Quit")
			{
				selected = true;	
			}
			if (buttons[index].Name == "no")
			{
				if (GetParent().GetParent() is MainButtons parent)
				{
					parent.selected = false;
				}
			}
			if (buttons[index].Name == "yes")
			{
				GetTree().Quit();
			}
		}
		if (selected)
		{
			if (this.Name == "MainButtons")
			{
				iconsBarParent.GlobalPosition = iconsBarParent.GlobalPosition.Lerp(selectedPos.GlobalPosition, 7 * (float)delta);
				if (!buttons2[index].IsProcessing())
				{
					buttons2[index].SetProcess(true);
				}
				foreach (Node2D icon in icons)
				{
					if (icon != icons[index])
					{
						icon.Modulate = icon.Modulate.Lerp(new Color(0,0,0,0) , 10 * (float)delta);
					}
				}
				if (buttonsParent.Modulate.A < 0.1)
				{
					buttons2[index].Modulate = buttons2[index].Modulate.Lerp(new Color(1,1,1,1) , 10 * (float)delta);
				}	
			}
			
			buttonsParent.Modulate = buttonsParent.Modulate.Lerp(new Color(0,0,0,0) , 10 * (float)delta);
			if (Input.IsActionJustPressed("X") && canX)
			{
				selected = false;
			}
		}
		else
		{
			if (this.Name == "MainButtons")
			{
				foreach (Node2D button2 in buttons2)
				{
					button2.Modulate = button2.Modulate.Lerp(new Color(0,0,0,0) , 10 * (float)delta);	
					if (button2.IsProcessing())
					{
						button2.SetProcess(false);
					}
				}	
				if (buttons2[index].Modulate.A < 0.1)
				{
					buttonsParent.Modulate = buttonsParent.Modulate.Lerp(new Color(1,1,1,1) , 10 * (float)delta);
				}
				iconsBarParent.GlobalPosition = iconsBarParent.GlobalPosition.Lerp(Vector2.Zero, 7 * (float)delta);
			}
		}
	}

	void UpdateButton()
	{
		for (int i = 0; i < buttons.Count(); i++)
		{
			tweens[i].Kill();
			tween = CreateTween();
			tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
			tweens[i] = tween;
		}
		int inx = 0;
		foreach(Godot.Label button in buttons)
		{
			if (button == buttons[index])
			{
				tweens[index].TweenProperty(button, "scale", new Vector2(1.3f, 1.3f), 0.8f);	
			}
			else
			{
				tweens[inx].TweenProperty(button, "scale", new Vector2(1f,1f), 0.8f);
			}
			inx ++;
		}

		if (this.Name == "MainButtons")
		{
			if (tween2 != null)
			{
				tween2.Kill();
			}
			tween2 = CreateTween();
			tween2.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
			tween2.TweenProperty(iconsBar, "position", new Vector2(firstPos.X + (index * -480), iconsBar.Position.Y), 0.4f);
			if (index == 2)
			{
				anim.Play("TurningEyes");
			}
			else
			{
				anim.Pause();
			}
		}
	}

	void UpdateIcons(double delta)
	{
		int a = 0;
		foreach (Node2D icon in icons)
		{
			if (icon == icons[index])
			{
				icon.Scale = icon.Scale.Lerp(new Vector2(1,1), 10 * (float)delta);
				icon.Modulate = icon.Modulate.Lerp(new Color(1,1,1,1), 12 * (float)delta);
				if (index == 1)
				{
					icon.GlobalRotationDegrees += 75 * (float)delta;
				}
			}
			else if (!selected)
			{
				icon.Scale = icon.Scale.Lerp(new Vector2(0.5f / Mathf.Abs(index - a), 0.5f / Mathf.Abs(index - a)), 10 * (float)delta); 
				icon.Modulate = icon.Modulate.Lerp(new Color(1,1,1,0.3f), 12 * (float)delta);
			}
			a++;
		}
	}
}
