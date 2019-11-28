using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Empty, Floor };

public class Tile {

    //Action<int, string, float> someFunction;

  

    private TileType _type = TileType.Empty;
    public TileType Type
    {
        get { return _type; }
        set
        {
            TileType oldType = _type;
            _type = value;
            // Call the callback and let things know we've changed.

            if (cbTileTypeChanged != null && oldType != _type)
                cbTileTypeChanged(this);
        }
    }

    public Furniture furniture
    {
        get; protected set;
    }

    public World world { get; protected set; }

    public int X { get; protected set; }
    public int Y { get; protected set; }

    Action<Tile> cbTileTypeChanged;

    public Tile(World world, int x, int y)
    {
        this.world = world;
        this.X = x;
        this.Y = y;
    }

    TileType type = TileType.Empty;

    LooseObject looseObject;
    InstalledObject installedObject;

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    /// <summary>
    /// Unregister a callback.
    /// </summary>
    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

    public bool PlaceFurniture(Furniture objInstance)
    {
        if (objInstance == null)
        {
            // We are uninstalling whatever was here before.
            furniture = null;
            return true;
        }

        // objInstance isn't null

        if (furniture != null)
        {
            //Debug.LogError("Trying to assign a furniture to a tile that already has one!");
            return false;
        }

        // At this point, everything's fine!

        furniture = objInstance;
        return true;
    }


}
