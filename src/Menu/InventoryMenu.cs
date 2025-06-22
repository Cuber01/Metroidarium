using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium.Menu;

public partial class InventoryMenu : Node2D
{
    private Dictionary<String, InventoryItem> inventory = new Dictionary<String, InventoryItem>();
    private InventoryItem selected;
    private TextureButton firstSlot;
    private TextureButton firstItem;
    private readonly PackedScene itemButton = GD.Load<PackedScene>("res://assets/scenes/ItemInventoryBox.tscn");
    
    public override void _Ready()
    {
        InventoryItem dash = ResourceLoader.Load<InventoryItem>("res://assets/item_data/dash_charm.tres");
        inventory.Add(dash.FullName, dash);
        
        InventoryItem gun = ResourceLoader.Load<InventoryItem>("res://assets/item_data/double_cannon.tres");
        inventory.Add(gun.FullName, gun);
        
        setupEquipped();
        setupInventoryContents();
    }

    private void setupEquipped()
    {
        Node2D equipped = (Node2D)GetNode("Equipped");
        int i = equipped.GetChildCount()-1;
        firstSlot = equipped.GetNode<TextureButton>("Slot0");
        for (int j = 0; j < i; j++)
        {
            TextureButton button = equipped.GetNode<TextureButton>("Slot" + j);
            button.SetMeta("SlotIndex", j);
            button.SetFocusNeighbor(Side.Left, j > 0 ? "../Slot" + (j - 1) : null);
            button.SetFocusNeighbor(Side.Right, j + 1 < i ? "../Slot" + (j + 1) : null);
            button.Pressed += () => _onSlotPressed(button, (int)button.GetMeta("SlotIndex"));
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
            newButton.Pressed += () => _onEquipmentPressed(newButton.Name);
            
            buttonField[j,i] = newButton;
            i++;
            
            collection.AddChild(newButton);
        }
        
        int m = i - 1;
        for (int y = 0; y <= j; y++)
        {
            for (int x = 0; x <= m; x++)
            {
                TextureButton btn = buttonField[y,x];
                
                if (x != 0) {
                    btn.SetFocusNeighbor(Side.Left, "../" + buttonField[y,x-1].Name);
                }

                if (x != m) {
                    btn.SetFocusNeighbor(Side.Right, "../" + buttonField[y,x+1].Name);
                }

                if (y != 0) {
                    btn.SetFocusNeighbor(Side.Top, "../" + buttonField[y-1,x].Name);
                }
                
                if (y != j) {
                    btn.SetFocusNeighbor(Side.Top, "../" + buttonField[y+1,x].Name);
                }
            }
        }

        firstItem = buttonField[0,0];
        firstItem.GrabFocus();
    }

    private void _onEquipmentPressed(string name)
    {
        selected = inventory[name];
        firstSlot.GrabFocus();
    }
    
    private void _onSlotPressed(TextureButton btn, int index)
    {
        btn.TextureNormal = selected.Image;
        firstItem.GrabFocus();
    }
    

}