using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script documented by Andy @!rmfandyplayz
//As always, whoever originally wrote this code, it would be cool if u document this urself

//This class represents a tile in the board
public sealed class Tile : MonoBehaviour
{
    //these x y variables store the coords of 
    public int x;
    public int y;

    private Item _item; //Stores "item" (I still dont understand what "item" is supposed to be) on the tile

    //Allows access to _item variable and sets the Image for the tile (I think)
    public Item Item
    {
        get => _item;

        set
        {
            if (_item == value) return;

            _item = value;

            icon.sprite = _item.sprite;

        }
    }

    public Image icon; //The sprite for each orb
    public Button button; //A button is placed on each orb so operations can be performed

    //These next 4 variables represent the neighbors of the tile a player picks
    public Tile Left => x > 0 ? Board.instance.tiles[x - 1 ,y] : null;
    public Tile Top => y > 0 ? Board.instance.tiles[x, y - 1] : null;
    public Tile Right => x < Board.instance.width - 1 ? Board.instance.tiles[x + 1, y] : null;
    public Tile Bottom => y < Board.instance.height - 1 ? Board.instance.tiles[x, y + 1] : null;

    //Returns an array of all neighboring tiles
    public Tile[] Neighbours => new[]
    {
        Left,
        Top,
        Right,
        Bottom,
    };

    //Basically saying that if a button is clicked, run the Select() function from Board script, passing in this script as the parameter (aka the tile)
    private void Start() => button.onClick.AddListener(() => Board.instance.Select(this));
    
    /// <summary>
    /// Returns a list of all tiles that are connected to the current tile and are of the same type (same color)
    /// </summary>
    /// <param name="exclude"></param>
    /// <returns></returns>
    public List<Tile> GetConnectedTiles(List<Tile> exclude = null)
    {
        var result = new List<Tile> { this, }; //list that contains the current tile


        if (exclude == null)
        {
            exclude = new List<Tile> { this, };
        }
        else
        {
            exclude.Add(item:this);
        }

        foreach (var neighbour in Neighbours)
        {
            if (neighbour == null || exclude.Contains(neighbour) || neighbour.Item != Item) continue;

            result.AddRange(neighbour.GetConnectedTiles(exclude));

        }
        
        return result;
    }
}
