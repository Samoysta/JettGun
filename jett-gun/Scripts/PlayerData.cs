using Godot;
using System;

public partial class PlayerData : Node
{
    public int saveIndex;
    string SAVE_PATH;
    // Player Datas (Oyuncu Dataları)
    public int percentage;
    public string currentLevel = "game.tscn";
    public bool canFire;
    public bool canDash;
    public bool hasJettPack;

    public override void _Ready()
    {
        
    }

    public void Save()
    {
        SAVE_PATH = $"user://save_{saveIndex}.json";
        var data = new Godot.Collections.Dictionary
        {
            { "canFire", canFire },
            { "canDash", canDash },
            { "hasJettPack", hasJettPack },
            { "percentage", percentage},
            { "currentLevel", currentLevel}
        };

        using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Write);
        file.StoreString(Json.Stringify(data));

        GD.Print("PLAYER DATA SAVED");
    }

    public bool Load()
    {
        SAVE_PATH = $"user://save_{saveIndex}.json";
        if (!FileAccess.FileExists(SAVE_PATH))
        {
            ResetData();
            return false;  
        }

        using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
        var data = Json.ParseString(file.GetAsText()).AsGodotDictionary();

        canFire = (bool)data["canFire"];
        canDash = (bool)data["canDash"];
        hasJettPack = (bool)data["hasJettPack"];
        percentage = (int)data["percentage"];
        currentLevel = (string)data["currentLevel"];

        GD.Print("PLAYER DATA LOADED");
        return true;
    }

    public void ResetData()
    {
        percentage = 0;
        canFire = false;
        canDash = false;
        hasJettPack = false;
        currentLevel = "game.tscn";
    }

    public void DeleteSave()
    {
        SAVE_PATH = $"user://save_{saveIndex}.json";

        if (FileAccess.FileExists(SAVE_PATH))
        {
            DirAccess.RemoveAbsolute(SAVE_PATH);
        }
    }
}
