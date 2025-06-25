using Godot;

namespace Metroidarium;

public partial class PickupableItem : Area2D
{
    [Export] private InventoryItem item = null;

    public override void _Ready()
    {
        GetNode<Sprite2D>("Sprite2D").Texture = item.Image;
    }

    private void _onBodyEntered(Node body)
    {
        if (body is SnakeHead player)
        {
            player.pickup(item);
            CallDeferred("queue_free");
        }
    }
}