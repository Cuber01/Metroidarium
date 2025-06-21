using Godot;

namespace Metroidarium;

[GlobalClass]
public partial class GunItem : InventoryItem
{
    [Export] public float MaxDelay;
    [ExportGroup("Sides")] [Export] public bool ShootRight;
    [Export] public bool ShootLeft;
    [Export] public bool ShootDown;
    [Export] public bool ShootUp;
}
