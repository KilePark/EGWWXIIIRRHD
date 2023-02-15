using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

//Code documented by Andy Jia and Kyle Park
//Whoever tf actually wrote this please for the love of god comment ur goddamn code lmfao
public sealed class Board : MonoBehaviour
{
    public static Board instance { get; private set; } //This variable basically represents the game board

    public Row[] rows; //This is a variable that holds the rows of the board



    public Tile[,] tiles { get; private set;} //Tiles i think? (GUYS FKIN COMMENT UR CODE JEEZ)

    public int Width => tiles.GetLength(dimension:0); //Width of the board
    public int Height => tiles.GetLength(dimension:1); //Height of the board

    private readonly List<Tile> _selection = new List<Tile>(); //When a player clicks a tile, it is added to this list to be compared (check for matches)
    private void Awake() => instance = this; //Initializes the game board (I think)

    //Initializes the game board and placing tiles in the 2D list/array
    private void Start()
    {
        tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length]; //makes new tile i think  
        
        //This for loop goes over every element within the 2D list "tiles" and assigns a random orb to each tile
        for (var y = 0; y < Height; y++)
        {    
            for(var x = 0; x < Width; x++)
            {   
                var tile = rows[y].tiles[x];


                tile.x = x;
                tile.y = y;

                tile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
                
                tiles[x, y] = tile;
            }
        }

    }

    /// <summary>
    /// This function selects tiles in order for them to be compared for matches.
    /// </summary>
    /// <param name="tile"></param>
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

        //If we found a match, then we make the match 3 thingy happen
        //⚠️⚠️⚠️⚠️⚠️ This is where we can insert the thingies for making the characters attack and other shid idfk
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

    /// <summary>
    /// Swaps the positions of the selected tiles (orbs), then check for matches.
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    public void Swap(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        
        icon2Transform.SetParent(tile1.transform, false); //Swaps tiles instead of dotween
        icon1Transform.SetParent(tile2.transform, false); //swaps tiles

        //If found a match, do the thanos snap orb thing idk mayb add stuff here for characters
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

    /// <summary>
    /// Says it all in the name. Returns true if a possible match is found
    /// </summary>
    /// <returns></returns>
    private bool CanPop()
    {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                if (tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2) 
                    return true;

        return false;
    }
    
    /// <summary>
    /// Performs thanos snapping the orbs. Self-explanatory
    /// </summary>
    private void Pop()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = tiles[x, y];

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
