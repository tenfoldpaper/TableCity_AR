using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

    public enum TileType { Empty, Floor, Road};
    //Action<int, string, float> someFunction;
    public bool highlighted = false;
    Action<Tile> cbTileTypeChanged;

    public TileType Type
    {
        get
        {
            return type;
        }
        set
        {
            TileType oldType = type;
            type = value;
            // Call the callback and let things know the tile has been changed
            // Action Delegate! A variable that holds the reference to a function in C#
            if(cbTileTypeChanged != null && oldType != type)
            {
                cbTileTypeChanged(this);
            }

        }
    }
    
    TileType type = TileType.Empty;

    LooseObject looseObject;
    InstalledObject installedObject;

    World world;
    public int X { get; protected set;}
    
    public int Y { get; protected set; }

    public Tile( World world, int x, int y ){

        this.world = world;
        this.X = x;
        this.Y = y;

    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
        //This Action is an odd type of function, so something like
        //cbTileTypeChanged += callback; is perfectly legal, and each of those additional funcitons will be run. 
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }


}
