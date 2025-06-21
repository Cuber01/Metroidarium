using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public class InventoryComponent : Component
{
    private Dictionary<String, InventoryItem> inventory = new Dictionary<String, InventoryItem>();
    private SnakeHead player;
    
    public InventoryComponent(SnakeHead player)
    {
        this.player = player;
    }
    
    public void AddItem(InventoryItem item) => inventory.Add(item.GameName, item);

    public void Equip(String itemGameName, int slotIndex)
    {
        SnakeTail slot = (SnakeTail)player.snakeParts[slotIndex];
        Charm charm = (Charm)Activator.CreateInstance(Type.GetType(itemGameName)!, player, slot);
        if (charm is GunCharm gun)
        {
            GunItem data = (GunItem)inventory[itemGameName];
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
    }

    public void Unequip(int slotIndex)
    {
        player.charms[slotIndex].Unequip();
        player.charms[slotIndex] = null;
    }
}