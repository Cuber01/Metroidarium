using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium.Menu;

public partial class InventoryMenu : Node2D
{
    private Dictionary<String, InventoryItem> inventory = new Dictionary<String, InventoryItem>();
    private readonly PackedScene itemButton = GD.Load<PackedScene>("res://assets/scenes/ItemInventoryBox.tscn");
    
    public override void _Ready()
    {
        TextureButton a = (TextureButton)GetNode("Collection/Buttons/DoubleCannon");
        a.GrabFocus();
        setupEquippedNeighbors();
    }

    private void setupEquippedNeighbors()
    {
        Node2D equipped = (Node2D)GetNode("Equipped");
        int i = equipped.GetChildCount();
        for (int j = 0; j < i; j++)
        {
            TextureButton button = (TextureButton)equipped.GetNode("Slot" + j);
            button.SetFocusNeighbor(Side.Left, j > 0 ? "../Slot" + (j - 1) : null);
            button.SetFocusNeighbor(Side.Right, j + 1 < i ? "../Slot" + (j + 1) : null);
        }
    }

    private void setupInventoryContents()
    {
        foreach (KeyValuePair<String, InventoryItem> item in inventory)
        {
            TextureButton newButton = (TextureButton)itemButton.Instantiate();

        }
    }
}