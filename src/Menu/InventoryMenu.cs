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
        InventoryItem dash = ResourceLoader.Load<InventoryItem>("res://assets/item_data/dash_charm.tres");
        inventory.Add(dash.FullName, dash);
        
        InventoryItem gun = ResourceLoader.Load<InventoryItem>("res://assets/item_data/double_cannon.tres");
        inventory.Add(gun.FullName, gun);
        
        setupEquippedNeighbors();
        setupInventoryContents();
    }

    private void setupEquippedNeighbors()
    {
        Node2D equipped = (Node2D)GetNode("Equipped");
        int i = equipped.GetChildCount();
        for (int j = 0; j < i; j++)
        {
            TextureButton button = equipped.GetNode<TextureButton>("Slot" + j);
            button.SetFocusNeighbor(Side.Left, j > 0 ? "../Slot" + (j - 1) : null);
            button.SetFocusNeighbor(Side.Right, j + 1 < i ? "../Slot" + (j + 1) : null);
        }
    }

    private void setupInventoryContents()
    {
        if(inventory.Count == 0) return;
        
        TextureButton[,] buttonField = new TextureButton[20,20];
        int i = 0, j = 0;
        
        Node2D collection = GetNode<Node2D>("Collection");
        float startX = 16;
        float maximumX = 200;
        Vector2 buttonPos = new Vector2(startX, 144);
        Vector2 offset = new Vector2(40, 40);
        
        foreach (KeyValuePair<String, InventoryItem> item in inventory)
        {
            TextureButton newButton = (TextureButton)itemButton.Instantiate();
            newButton.GetNode<Sprite2D>("Image")
                .Texture = item.Value.Image;
            
            newButton.GetNode<RichTextLabel>("AmountLabel")
                .Text = item.Value.Amount.ToString();
            
            if (buttonPos.X > maximumX)
            {
                buttonPos.X = startX;
                buttonPos.Y += offset.Y;
                j++;
            }
            newButton.Position = buttonPos;
            buttonPos += new Vector2(offset.X, 0);

            newButton.Name = item.Key;
            
            buttonField[i,j] = newButton;
            i++;
            
            collection.AddChild(newButton);
        }

        for (int y = 0; y <= i; y++)
        {
            for (int x = 0; x <= j; x++)
            {
                TextureButton btn = buttonField[y,x];
                
                if (x != 0) {
                    btn.SetFocusNeighbor(Side.Left, "Collection/" + buttonField[y,x-1].Name);
                }

                if (x != j) {
                    btn.SetFocusNeighbor(Side.Right, "Collection/" + buttonField[y,x+1].Name);
                }

                if (y != 0) {
                    btn.SetFocusNeighbor(Side.Top, "Collection/" + buttonField[y-1,x].Name);
                }
                
                if (y != i) {
                    btn.SetFocusNeighbor(Side.Top, "Collection/" + buttonField[y+1,x].Name);
                }
            }
        }

        buttonField[0,0].GrabFocus();

    }
}