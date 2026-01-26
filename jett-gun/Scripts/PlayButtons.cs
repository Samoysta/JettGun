using Godot;
using System;
using System.Linq;

public partial class PlayButtons : Node2D
{
	[Export] Godot.Label[] buttons;
	[Export] Godot.Label[] userInfos;
	[Export] Node2D actionButtons;
	[Export] Godot.Label[] buttons2;
	[Export] MainButtons Parent;
	int index;
	int index2;
	Tween tween;
	Tween[] tweens;
	Tween[] tweens2;
	bool selected;
	PlayerData pd;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		actionButtons.Modulate = new Color(0,0,0,0);
		tweens = new Tween[buttons.Count()];
		tweens2 = new Tween[buttons2.Count()];
		pd = GetNode<PlayerData>("/root/PlayerData");
		updateInfos();
		UpdateButton();
		UpdateButton2();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Down") && selected)
		{
			if (index2 == buttons2.Length - 1)
			{
				index2 = 0;
			}
			else
			{
				index2 ++;
			}
			UpdateButton2();
		}
		if (Input.IsActionJustPressed("Up") && selected)
		{
			if (index2 == 0)
			{
				index2 = buttons2.Length - 1;
			}
			else
			{
				index2 --;
			}
			UpdateButton2();
		}
		if (Input.IsActionJustPressed("Right") && !selected)
		{
			if (index == buttons.Length - 1)
			{
				index = 0;
			}
			else
			{
				index++;
			}
			UpdateButton();
		}
		if (Input.IsActionJustPressed("Left") && !selected)
		{
			if (index == 0)
			{
				index = buttons.Length - 1;
			}
			else
			{
				index--;
			}
			UpdateButton();
		}

		foreach (Godot.Label button in buttons)
		{
			if (button == buttons[index] && !selected)
			{
				button.Modulate = button.Modulate.Lerp(Colors.Yellow, 5 * (float)delta);
			}
			else
			{
				button.Modulate = button.Modulate.Lerp(Colors.White, 5 * (float)delta);
			}
		}
		foreach (Godot.Label button2 in buttons2)
		{
			if (button2 == buttons2[index2])
			{
				button2.Modulate = button2.Modulate.Lerp(Colors.Yellow, 5 * (float)delta);
			}
			else
			{
				button2.Modulate = button2.Modulate.Lerp(Colors.White, 5 * (float)delta);
			}
		}
		if (selected)
		{
			actionButtons.GlobalPosition = userInfos[index].GlobalPosition;
			actionButtons.Modulate = actionButtons.Modulate.Lerp(new Color(1,1,1,1), 5 * (float)delta);
			if (Input.IsActionJustPressed("X"))
			{
				Parent.canX = true;
				selected = false;
			}
		}
		else
		{
			actionButtons.Modulate = actionButtons.Modulate.Lerp(new Color(0,0,0,0), 8 * (float)delta);
		}
		if (Input.IsActionJustPressed("Z"))
		{
			if (!selected)
			{
				index2 = 0;
				UpdateButton2();
				Parent.canX = false;
				selected = true;
			}
			else
			{
				if (buttons2[index2].Name == "Play")
				{
					pd.saveIndex = index;
					pd.Load();
					GetTree().ChangeSceneToFile("res://Scenes/Levels/game.tscn");		
				}
				else if (buttons2[index2].Name == "Delete")
				{
					pd.saveIndex = index;
					pd.DeleteSave();
					updateInfos();
				}
			}
		}
	}

	void UpdateButton()
	{
		for (int i = 0; i < buttons.Count(); i++)
		{
			if (tweens[i] != null)
			{
				tweens[i].Kill();	
			}
			tween = CreateTween();
			tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
			tweens[i] = tween;
		}
		int inx = 0;
		foreach(Godot.Label button in buttons)
		{
			if (button == buttons[index] && !selected)
			{
				tweens[index].TweenProperty(button, "scale", new Vector2(1.3f, 1.3f), 0.8f);	
			}
			else
			{
				tweens[inx].TweenProperty(button, "scale", new Vector2(1f,1f), 0.8f);
			}
			inx ++;
		}
	}

	void UpdateButton2()
	{
		for (int i = 0; i < buttons2.Count(); i++)
		{
			if (tweens2[i] != null)
			{
				tweens2[i].Kill();
			}
			tween = CreateTween();
			tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
			tweens2[i] = tween;
		}
		int inx = 0;
		foreach(Godot.Label button2 in buttons2)
		{
			if (button2 == buttons2[index2])
			{
				tweens2[index2].TweenProperty(button2, "scale", new Vector2(1.3f, 1.3f), 0.8f);	
			}
			else
			{
				tweens2[inx].TweenProperty(button2, "scale", new Vector2(1f,1f), 0.8f);
			}
			inx ++;
		}
	}

	void updateInfos()
	{
		for (int i = 0; i < userInfos.Count(); i++)
		{
			pd.saveIndex = i;
			if (!pd.Load())
			{
				userInfos[i].Text = "Empty";
				userInfos[i].Modulate = new Color(1,1,1,1);
			}
			else
			{
				userInfos[i].Text = "% " + pd.percentage;
				userInfos[i].Modulate = new Color(0,1.3f,0,1);
			}
		}
	}

}
