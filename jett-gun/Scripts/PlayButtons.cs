using Godot;
using System;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;

public partial class PlayButtons : Node2D
{
	[Export] Godot.Label[] buttons;
	[Export] Godot.Label[] userInfos;
	[Export] Node2D actionButtons;
	[Export] Godot.Label[] buttons2;
	[Export] Godot.Label[] buttons3;
	[Export] MainButtons Parent;
	int index;
	int index2;
	Tween tween;
	Tween tween2;
	Tween tween3;
	Tween[] tweens;
	Tween[] tweens2;
	Tween[] tweens3;
	bool selected;
	PlayerData pd;
	string currentLevel;
	bool canX = true;
	[Export] Node2D deleteMenu;
	[Export] PackedScene deleteEffect;
	[Export] Node2D blackScreen;
	[Export] float gameStartTime;
	bool playPressed;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		deleteMenu.Scale = Vector2.Zero;
		actionButtons.Modulate = new Color(0,0,0,0);
		tweens = new Tween[buttons.Count()];
		tweens2 = new Tween[buttons2.Count()];
		tweens3 = new Tween[buttons3.Count()];
		pd = GetNode<PlayerData>("/root/PlayerData");
		currentLevel = pd.currentLevel;
		updateInfos();
		UpdateButton();
		UpdateButton2();
		UpdateButton3();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (playPressed)
		{
			gameStartTime -= (float)delta;
			if (gameStartTime <= 0)
			{
				GetTree().ChangeSceneToFile($"res://Scenes/Levels/{currentLevel}");
			}
		}
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
			UpdateButton3();
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
			UpdateButton3();
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
		foreach (Godot.Label button3 in buttons3)
		{
			if (button3 == buttons3[index2])
			{
				button3.Modulate = button3.Modulate.Lerp(Colors.Yellow, 5 * (float)delta);
			}
			else
			{
				button3.Modulate = button3.Modulate.Lerp(Colors.White, 5 * (float)delta);
			}
		}
		if (selected)
		{
			actionButtons.GlobalPosition = userInfos[index].GlobalPosition;
			actionButtons.Modulate = actionButtons.Modulate.Lerp(new Color(1,1,1,1), 5 * (float)delta);
			if (Input.IsActionJustPressed("X"))
			{
				if (canX)
				{
					Parent.canX = true;
					selected = false;	
				}
				else
				{
					if (tween3 != null)
					{
						tween3.Kill();
					}
					tween3 = CreateTween();
					tween3.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Linear);
					tween3.TweenProperty(blackScreen, "modulate", new Color(0,0,0,0), 0.5f);
					if (tween2 != null)
					{
						tween2.Kill();
					}
					tween2 = CreateTween();
					tween2.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
					tween2.TweenProperty(deleteMenu, "scale", new Vector2(0,0), 0.8f);
					UpdateButton2();
					canX = true;
				}
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
				if (buttons2[index2].Name == "Play" && canX)
				{
					pd.saveIndex = index;
					pd.Load();
					if (tween3 != null)
					{
						tween3.Kill();
					}
					tween3 = CreateTween();
					tween3.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
					tween3.TweenProperty(blackScreen, "modulate", new Color(1,1,1,1), 1);
					playPressed = true;		
				}
				else if (buttons2[index2].Name == "Delete" && canX)
				{
					if (userInfos[index].Text != "Empty")
					{
						canX = false;
						if (tween2 != null)
						{
							tween2.Kill();
						}
						tween2 = CreateTween();
						tween2.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
						tween2.TweenProperty(deleteMenu, "scale", new Vector2(1,1), 0.8f);
						if (tween3 != null)
						{
							tween3.Kill();
						}
						tween3 = CreateTween();
						tween3.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
						tween3.TweenProperty(blackScreen, "modulate", new Color(1,1,1,1), 0.2f);
					}
				}
				else if (buttons3[index2].Name == "Yes")
				{
					if (!canX)
					{
						DeleteSave();
						canX = true;
						if (tween3 != null)
						{
							tween3.Kill();
						}
						tween3 = CreateTween();
						tween3.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Linear);
						tween3.TweenProperty(blackScreen, "modulate", new Color(0,0,0,0), 0.5f);
						if (tween2 != null)
						{
							tween2.Kill();
						}
						tween2 = CreateTween();
						tween2.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
						tween2.TweenProperty(deleteMenu, "scale", new Vector2(0,0), 0.8f);
						UpdateButton2();
					}
				}
				else if (buttons3[index2].Name == "No")
				{
					if (!canX)
					{
						canX = true;
						if (tween3 != null)
						{
							tween3.Kill();
						}
						tween3 = CreateTween();
						tween3.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Linear);
						tween3.TweenProperty(blackScreen, "modulate", new Color(0,0,0,0), 0.5f);
						if (tween2 != null)
						{
							tween2.Kill();
						}
						tween2 = CreateTween();
						tween2.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
						tween2.TweenProperty(deleteMenu, "scale", new Vector2(0,0), 0.8f);
						UpdateButton2();	
					}
				}
			}
		}
	}

	void DeleteSave()
	{
		CpuParticles2D effect = (CpuParticles2D)deleteEffect.Instantiate();
		effect.GlobalPosition = userInfos[index].GlobalPosition + new Vector2(62,32);
		effect.Emitting = true;
		GetParent().AddChild(effect);
		pd.saveIndex = index;
		pd.DeleteSave();
		updateInfos();	
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

	void UpdateButton3()
	{
		for (int i = 0; i < buttons3.Count(); i++)
		{
			if (tweens3[i] != null)
			{
				tweens3[i].Kill();
			}
			tween = CreateTween();
			tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
			tweens3[i] = tween;
		}
		int inx = 0;
		foreach(Godot.Label button3 in buttons3)
		{
			if (button3 == buttons3[index2])
			{
				tweens3[index2].TweenProperty(button3, "scale", new Vector2(1.3f, 1.3f), 0.8f);	
			}
			else
			{
				tweens3[inx].TweenProperty(button3, "scale", new Vector2(1f,1f), 0.8f);
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
