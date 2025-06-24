using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Metroidarium.Menu;

namespace Metroidarium;

public class InventoryComponent : Component
{
    public Dictionary<String, InventoryItem> Inventory { get; private set; }
    public List<InventoryItem> AllItems { get; private set; }
    private InventoryMenu menu;
    private SnakeHead player;
    
    public InventoryComponent(SnakeHead player, List<InventoryItem> allItems)
    {
        this.player = player;
        AllItems = allItems;
        menu = player.GetNode<InventoryMenu>("../../InventoryMenu");
        Inventory = new Dictionary<String, InventoryItem>();
    }
    
    public void AddItem(InventoryItem item)
    {
        if (Inventory.Keys.Contains(item.FullName))
        {
            Inventory[item.FullName].Amount += item.Amount;
        }
        else
        {
            Inventory.Add(item.FullName, item);    
        }
    }

    public void EquipItem(InventoryItem item, int slotIndex)
    {
        SnakeTail slot = (SnakeTail)player.snakeParts[slotIndex];
        Charm charm = (Charm)Activator.CreateInstance(Type.GetType(item.GameName)!, player, slot);
        
        // Init charm
        if (charm is GunCharm gun)
        {
            GunItem data = (GunItem)Inventory[item.FullName];
            gun.MaxDelay = data.MaxDelay;
            charm.EquipCharm(new Dictionary<Directions, Node2D> {
                 		{Directions.Left, data.ShootLeft ? (Node2D)slot.GetNode("Left") : null},
                 		{Directions.Right, data.ShootRight ? (Node2D)slot.GetNode("Right") : null},
                 		{Directions.Up, data.ShootUp ? (Node2D)slot.GetNode("Up") : null},
                 		{Directions.Down, data.ShootDown ? (Node2D)slot.GetNode("Down") : null}
                 	});
        }
        else
        {
            charm!.EquipCharm();
        }

        // Set image
        slot.GetNode<Sprite2D>("CharmMarker").SetTexture(item.Image);
        
        // Swap charms
        if (player.charms[slotIndex] != null)
        {
            player.charms[slotIndex].Unequip();
        }
        
        player.charms[slotIndex] = charm;
    }

    public void Unequip(int slotIndex)
    {
        player.snakeParts[slotIndex].
            GetNode<Sprite2D>("CharmMarker").SetTexture(null);
        
        player.charms[slotIndex].Unequip();
        player.charms[slotIndex] = null;
    }

    public void OpenMenu()
    {
        menu.Init(this);
        menu.Show();
        // TODO pause breaks this
        // player.GetTree().Paused = true;
    }

    public void CloseMenu()
    {
        menu.Close();
        menu.Hide();
        player.GetTree().Paused = false;
    }
}