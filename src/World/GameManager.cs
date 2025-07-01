using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Metroidarium;

public partial class GameManager : Node2D
{
    public readonly Dictionary<String, PackedScene> Levels = new Dictionary<String, PackedScene>();
    private const string LevelDirectory = "res://assets/scenes/levels/";
    
    public override void _Ready()
    {
        string[] paths = DirAccess.Open(LevelDirectory).GetFiles();
        foreach (String path in paths)
        {
            Levels.Add(path.GetBaseName(), ResourceLoader.Load<PackedScene>(LevelDirectory+path));
        }
    }
    
}