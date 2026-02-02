using Godot;
using System;

public partial class SettingsData : Node
{
	string SAVE_PATH;
	//settings dataları
	public bool VSync = true;
	public int MaxFps = 60;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SAVE_PATH = $"user://settings.json";
		Load();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Save()
    {
        var data = new Godot.Collections.Dictionary
        {
            {"VSync", VSync},
			{"MaxFps", MaxFps},
        };

        using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Write);
        file.StoreString(Json.Stringify(data));

        GD.Print("SETTINGS DATA SAVED");
    }

	public void Load()
    {
        if (!FileAccess.FileExists(SAVE_PATH))
        {
            return; 
        }

        using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
        var data = Json.ParseString(file.GetAsText()).AsGodotDictionary();

		VSync = (bool)data["VSync"];
		MaxFps = (int)data["MaxFps"];

        GD.Print("SETTINGS DATA LOADED");
		ApplySettings();
    }

	void ApplySettings()
	{
		if (VSync)
		{
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		}
		else
		{
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
		}

		Engine.MaxFps = MaxFps;
	}
}
