using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class ItemDatabase
{
    public static Item[] Items { get; private set; }
    public static List<string> elements { get; private set;}

    //Loads assets before scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Initialiez Items list with possible items
        Items = Resources.LoadAll<Item>(path:"Items/");
        
        // Initialiez elements list with possible elements
        List<string> holder = new List<string>();
        foreach (Item x in Items)
        {
            holder.Add(x.element);
        }
        elements = holder;
    }
}