using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium.Menu;

public partial class InventoryMenu : Node2D
{
    private readonly PackedScene itemButton = GD.Load<PackedScene>("res://assets/scenes/menus/ItemInventoryBox.tscn");
    private Dictionary<String, InventoryItem> inventory;
    private InventoryComponent inventoryComponent;
    private InventoryItem selected;
    private TextureButton firstSlot;
    private TextureButton firstItem;
    private bool blockInput = true;

    public void Init(InventoryComponent inventoryComponent)
    {
        this.inventoryComponent = inventoryComponent;
        inventory = inventoryComponent.Inventory;
        
        setupEquipped();
        setupInventoryContents();
        firstItem.CallDeferred("grab_focus");
    }

    public void Close()
    {
        selected = null;
    }

    public override void _Process(double delta)
    {
        blockInput = false;
        if (Input.IsActionPressed("ui_cancel"))
        {
            inventoryComponent.CloseMenu();   
        }
    }
    
    private void setupEquipped()
    {
        Control equipped = GetNode<Control>("Equipped");
        int i = equipped.GetChildCount();
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
        
        TextureButton[,] buttonField = new TextureButton[inventory.Count,inventory.Count];
        int i = 0, j = 0;
        
        Control collection = GetNode<Control>("Collection");
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

            newButton.SetMeta("GameName", item.Key);
            newButton.Pressed += () => _onEquipmentPressed((String)newButton.GetMeta("GameName"));
            
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
                else
                {
                    btn.SetFocusNeighbor(Side.Top, "../../Equipped/" + firstSlot.Name);
                }
                
                if (y != j) {
                    btn.SetFocusNeighbor(Side.Top, "../" + buttonField[y+1,x].Name);
                }
            }
        }

        firstItem = buttonField[0,0];
    }

    private void _onEquipmentPressed(string name)
    {
        if(blockInput) return;
        
        selected = inventory[name];
        firstSlot.CallDeferred("grab_focus");
        blockInput = true;
    }
    
    private void _onSlotPressed(TextureButton btn, int index)
    {
        if(blockInput) return;
        
        if (selected != null)
        {
            GD.Print("Equip");
            btn.TextureNormal = selected.Image;
            inventoryComponent.Equip(selected.GameName, index);
            selected = null;    
        }
        else if(btn.TextureNormal != null)
        {
            GD.Print("Unequip");
            btn.TextureNormal = null;
            inventoryComponent.Unequip(index);
        }
        blockInput = true;
        firstItem.CallDeferred("grab_focus");
    }
    

}