using System;
using System.ComponentModel;
using Godot;

namespace Metroidarium;

[GlobalClass]
public partial class InventoryItem : Resource
{
    [Export] public String FullName;
    [Export(PropertyHint.MultilineText)] public String Description;
    [Export] public Texture2D Image;
    [Export] public String GameName;
}

public partial class GunItem : InventoryItem
{
    [Export] public float MaxDelay;
    [ExportGroup("Sides")]
    [Export] public bool ShootRight;
    [Export] public bool ShootLeft;
    [Export] public bool ShootDown;
    [Export] public bool ShootUp;
}