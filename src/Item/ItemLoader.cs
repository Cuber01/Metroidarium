using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Metroidarium;

public class ItemLoader
{
    public List<InventoryItem> AllCharms;
    
    public ItemLoader(string dir)
    {
        AllCharms = loadItemsFromDirectory(dir);
    }
    
    private List<InventoryItem> loadItemsFromDirectory(string path)
    {
        return DirAccess.Open(path).GetFiles()
            .Select(file => (InventoryItem)ResourceLoader.Load($"{path}/{file}")).ToList();
    }
}