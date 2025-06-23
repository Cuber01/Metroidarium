using System;
using System.Collections.Generic;
using Godot;
using Metroidarium.Menu;

namespace Metroidarium;

public class InventoryComponent : Component
{
    public Dictionary<String, InventoryItem> Inventory { get; private set; }
    private InventoryMenu menu;
    private SnakeHead player;
    
    public InventoryComponent(SnakeHead player)
    {
        this.player = player;
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
            Inventory.Add(item.GameName, item);    
        }
    }

    public void Equip(String itemGameName, int slotIndex)
    {
        SnakeTail slot = (SnakeTail)player.snakeParts[slotIndex];
        Charm charm = (Charm)Activator.CreateInstance(Type.GetType(itemGameName)!, player, slot);
        if (charm is GunCharm gun)
        {
            GunItem data = (GunItem)Inventory[itemGameName];
            gun.MaxDelay = data.MaxDelay;
            charm.Equip(new Dictionary<Directions, Node2D> {
                 		{Directions.Left, data.ShootLeft ? (Node2D)slot.GetNode("Left") : null},
                 		{Directions.Right, data.ShootRight ? (Node2D)slot.GetNode("Right") : null},
                 		{Directions.Up, data.ShootUp ? (Node2D)slot.GetNode("Up") : null},
                 		{Directions.Down, data.ShootDown ? (Node2D)slot.GetNode("Down") : null}
                 	});
        }
        else
        {
            charm!.Equip();
        }

        if (player.charms[slotIndex] != null)
        {
            player.charms[slotIndex].Unequip();
        }
        
        player.charms[slotIndex] = charm;
    }

    public void Unequip(int slotIndex)
    {
        player.charms[slotIndex].Unequip();
        player.charms[slotIndex] = null;
    }

    public void Open()
    {
        menu.Init(this);
        menu.Show();
        // TODO pause breaks this
        // player.GetTree().Paused = true;
    }

    public void Close()
    {
        menu.Hide();
        player.GetTree().Paused = false;
    }
}