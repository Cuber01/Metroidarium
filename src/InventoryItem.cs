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
    [Export] public int Amount = 1;
}