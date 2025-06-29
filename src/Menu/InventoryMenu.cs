using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium.Menu;

public partial class InventoryMenu : Node2D
{
    private readonly Texture2D emptySlotImage = ResourceLoader.Load<Texture2D>("res://assets/img/no-item.png");
    private readonly Texture2D snakeBodyImage = ResourceLoader.Load<Texture2D>("res://assets/img/snake-body.png");
    private readonly PackedScene itemButton = GD.Load<PackedScene>("res://assets/scenes/menus/ItemInventoryBox.tscn");
    
    private Dictionary<String, InventoryItem> inventory;
    private List<InventoryItem> equipped = new List<InventoryItem>(new InventoryItem[10]);
    
    private InventoryComponent inventoryComponent;
    private InventoryItem selected;
    private TextureButton selectedButton;
    private TextureButton firstSlot;
    private TextureButton firstItem;
    
    // TODO this is a weird workaround for CallDeferred being deferred. Is there another way?
    private bool blockInput = true;

    #region SETUP

    public void Init(InventoryComponent inventoryComponent, int slotsAmount)
    {
        this.inventoryComponent = inventoryComponent;
        inventory = inventoryComponent.CharmInventory;
        
        Control equipped = GetNode<Control>("Equipped");
        firstSlot = equipped.GetNode<TextureButton>("Slot0");
        
        setupInventoryContents();
        setupEquipped(equipped, slotsAmount);
        firstItem.CallDeferred("grab_focus");
    }
    
    private void setupEquipped(Control equipped, int slotsAmount)
    {
        int i = equipped.GetChildCount();
        firstSlot = equipped.GetNode<TextureButton>("Slot0");
        for (int j = 0; j < i; j++)
        {
            TextureButton button = equipped.GetNode<TextureButton>("Slot" + j);

            if (j <= slotsAmount)
            {
                button.Visible = true;
                //button.GetNode<Sprite2D>("Sprite").Visible = true;
                button.SetMeta("SlotIndex", j);
                button.SetFocusNeighbor(Side.Left, "../../Collection/" + firstItem.Name);
                button.Pressed += () => _onSlotPressed(button, (int)button.GetMeta("SlotIndex"));    
            }
            else
            {
                button.Visible = false;
                //button.GetNode<Sprite2D>("Sprite").Visible = false;
            }
            
        }
    }

    private void setupInventoryContents()
    {
        TextureButton[,] buttonField = new TextureButton[10,10];
        int i = 0, j = 0;
        
        Control collection = GetNode<Control>("Collection");
        float startX = 16;
        float maximumX = 200;
        Vector2 buttonPos = new Vector2(startX, 32);
        Vector2 offset = new Vector2(40, 40);
        
        
        foreach (InventoryItem item in inventoryComponent.AllCharms)
        {
            TextureButton newButton = (TextureButton)itemButton.Instantiate();
            // Item was unlocked and is available
            if (inventory.TryGetValue(item.FullName, out InventoryItem inventoryItem) && inventoryItem.Amount > 0)
            {
                newButton.TextureNormal = snakeBodyImage;
                
                newButton.GetNode<Sprite2D>("Image")
                    .Texture = inventoryItem.Image;    
                
                newButton.GetNode<RichTextLabel>("AmountLabel")
                    .Text = inventoryItem.Amount.ToString();
            }
            // Item is not available
            else
            {
                newButton.TextureNormal = null;
                
                newButton.GetNode<Sprite2D>("Image")
                    .Texture = emptySlotImage;    
            }
            // TODO: Item was unlocked but is not available
            
            if (buttonPos.X > maximumX)
            {
                buttonPos.X = startX;
                buttonPos.Y += offset.Y;
                j++;
            }
            newButton.Position = buttonPos;
            buttonPos += new Vector2(offset.X, 0);

            newButton.SetMeta("GameName", item.FullName);
            newButton.Pressed += () => _onEquipmentPressed(newButton, (String)newButton.GetMeta("GameName"));
            
            buttonField[j,i] = newButton;
            i++;
            
            collection.AddChild(newButton);
        }
        

        for (int y = 0; y <= j; y++)
        {
            int x = i - 1;
            buttonField[y,x].SetFocusNeighbor(Side.Right, "../../Equipped/" + firstSlot.Name);
        }

        firstItem = buttonField[0,0];
    }

    #endregion
    
    public override void _Process(double delta)
    {
        blockInput = false;
        if (Input.IsActionPressed("ui_cancel"))
        {
            inventoryComponent.CloseMenu();   
        }
    }
    
    private void _onEquipmentPressed(TextureButton btn, string name)
    {
        if(blockInput) return;
        if(!inventory.TryGetValue(name, out var item)) return;
        
        selected = item;
        selectedButton = btn;
        btn.GetNode<Sprite2D>("SelectedSprite").Visible = true;
        firstSlot.CallDeferred("grab_focus");
        blockInput = true;
    }
    
    private void _onSlotPressed(TextureButton btn, int index)
    {
        if(blockInput) return;
        
        if (selected != null)
        {
            btn.TextureNormal = selected.Image;
            equipped[index] = selected;
            selected = null;   
            selectedButton.GetNode<Sprite2D>("SelectedSprite").Visible = false;
        }
        else if(btn.TextureNormal != null)
        {
            btn.TextureNormal = null;
            equipped[index] = null;
        }
        blockInput = true;
        firstItem.CallDeferred("grab_focus");
    }
    
    public void Close()
    {
        for (int i = 0; i < equipped.Count; i++)
        {
            if (equipped[i] != null)
            {
                inventoryComponent.EquipItem(equipped[i], i);    
            }
            else
            {
                inventoryComponent.Unequip(i);
            }
        }
        
        selected = null;
    }
    
}