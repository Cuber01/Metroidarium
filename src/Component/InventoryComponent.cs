using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Metroidarium.Menu;

namespace Metroidarium;

public class InventoryComponent : Component
{
    public Dictionary<String, InventoryItem> CharmInventory { get; private set; }
    public Dictionary<String, InventoryItem> UpgradeInventory { get; private set; }
    public int ApplesCollected = 0;
    
    public List<InventoryItem> AllCharms { get; private set; }
    private InventoryMenu menu;
    private SnakeHead player;
    
    public InventoryComponent(SnakeHead player, List<InventoryItem> allCharms)
    {
        this.player = player;
        AllCharms = allCharms;
        menu = player.GetNode<InventoryMenu>("../../InventoryMenu");
        CharmInventory = new Dictionary<String, InventoryItem>();
        UpgradeInventory = new Dictionary<String, InventoryItem>();
    }
    
    public void AddItem(InventoryItem item)
    {
        switch (item.Type)
        {
            case ItemType.Charm:
                if (CharmInventory.Keys.Contains(item.FullName))
                {
                    CharmInventory[item.FullName].Amount += item.Amount;
                }
                else
                {
                    CharmInventory.Add(item.FullName, item);    
                }
                break;
            case ItemType.Upgrade:
                UpgradeInventory.Add(item.FullName, item);
                EquipUpgrade(item);
                break;
            case ItemType.Apple:
                ApplesCollected++;
                player.AmountOfTail++;
                player.growTail(player.AmountOfTail - player.CurrentAmountOfTail);
                break;
        }
        
    }

    public void EquipItem(InventoryItem item, int slotIndex)
    {
        SnakeTail slot = (SnakeTail)player.SnakeParts[slotIndex];
        Charm charm = (Charm)Activator.CreateInstance(Type.GetType(item.GameName)!, player, slot);
        
        // Init charm
        if (charm is GunCharm gun)
        {
            GunItem data = (GunItem)CharmInventory[item.FullName];
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

    private void EquipUpgrade(InventoryItem item)
    {
        Charm upgrade = (Charm)Activator.CreateInstance(Type.GetType(item.GameName)!, player, null);
        upgrade!.EquipCharm();
        player.permanentCharms.Add(upgrade);
    }
    
    public void Unequip(int slotIndex)
    {
        // This method may be called by InventoryMenu, because InventoryMenu does not make a distinction between an empty slot and a slot to unequip
        if (player.charms.Count <= slotIndex)
            return;
        if (player.charms[slotIndex] == null)
            return;
        
        player.SnakeParts[slotIndex].
            GetNode<Sprite2D>("CharmMarker").SetTexture(null);
        
        player.charms[slotIndex].Unequip();
        player.charms[slotIndex] = null;
    }

    public void OpenMenu()
    {
        menu.Init(this, player.AmountOfTail);
        menu.Show();
        player.GetTree().Paused = true;
    }

    public void CloseMenu()
    {
        menu.Close();
        menu.Hide();
        player.GetTree().Paused = false;
    }
}