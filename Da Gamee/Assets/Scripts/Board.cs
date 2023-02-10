using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    public Row[] rows;



    public Tile[,] Tiles { get; private set;}

    public int Width => Tiles.GetLength(dimension:0);
    public int Height => Tiles.GetLength(dimension:1);

    private readonly List<Tile> _selection = new List<Tile>();
    private void Awake() => Instance = this;

    private void Start()
    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];
        
        for (var y = 0; y < Height; y++)
        {    
            for(var x = 0; x < Width; x++)
            {   
                var tile = rows[y].tiles[x];


                tile.x = x;
                tile.y = y;

                tile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
                
                Tiles[x, y] = tile;
            }
        }

    }

    public void Select(Tile tile)
    {
        if (!_selection.Contains(tile)) 
        {
            if (_selection.Count> 0)
            {
                if (Array.IndexOf(_selection[0].Neighbours, tile) != -1)
                {
                    _selection.Add(tile);
                }
            }
            else
            {
                _selection.Add(tile);
            }
        }

        if (_selection.Count < 2) return;
        
        Debug.Log(message:$"Selected tiles at ({_selection[0].x}, {_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");

        Swap(_selection[0], _selection[1]);

        if (CanPop())
        {
            Pop();
        }
        else
        {
            Swap(_selection[0], _selection[1]);
        }

        _selection.Clear();
    }

    public void Swap(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        
        icon2Transform.SetParent(tile1.transform, false); //Swaps tiles instead of dotween
        icon1Transform.SetParent(tile2.transform, false); //swaps tiles


        if (CanPop())
        {
            Pop();
        }
        else
        {
            icon2Transform.SetParent(tile1.transform, false);
            icon1Transform.SetParent(tile2.transform, false);
        }


        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1Item = tile1.Item;

        tile1.Item = tile2.Item;
        tile2.Item = tile1Item;
    }

    private bool CanPop()
    {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                if (Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2) 
                    return true;

        return false;
    }
 
    private void Pop()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];

                var connectedTiles = tile.GetConnectedTiles();

                if (connectedTiles.Skip(1).Count() < 2) continue;

                foreach (var connectedTile in connectedTiles)
                {
                    connectedTile.icon.rectTransform.localScale = Vector3.zero;
                }

                ScoreCounter.Instance.score += tile.Item.value * connectedTiles.Count();

                foreach (var connectedTile in connectedTiles)
                {
                    connectedTile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
                    connectedTile.icon.rectTransform.localScale = Vector3.one;
                }

                x = 0;
                y = 0;
                
            }
        }
    }
}
