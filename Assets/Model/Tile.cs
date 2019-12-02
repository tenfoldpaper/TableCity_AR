using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Empty, Floor };

public class Tile {

    public enum TileType { Empty, Floor, Road };
    //Action<int, string, float> someFunction;
    public GameObject gameObject;
    public bool electricity;
    public bool water;
    bool highlightedValue = false;
    public bool highlighted
    {
        get
        {
            return highlightedValue;
        }
        set
        {
            bool old = highlightedValue;
            highlightedValue = value;
            if (old != highlightedValue)
            {
                if (!old)
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 1f, 1f);
                else
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
    bool occupiedHighlightedValue = false;
    public bool occupiedHighlighted
    {
        get
        {
            return occupiedHighlightedValue;
        }
        set
        {
            bool old = occupiedHighlightedValue;
            occupiedHighlightedValue = value;
            if (old != occupiedHighlightedValue)
            {
                if (!old)
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f, 1f);
                else
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
    bool transparentValue = false;
    public bool transparent
    {
        get
        {
            return transparentValue;
        }
        set
        {
            bool old = transparentValue;
            transparentValue = value;
            if (old != transparentValue)
            {
                if(!old)
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .5f);
                else
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
    Action<Tile> cbTileTypeChanged;

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
    LooseObject looseObject;
    InstalledObject installedObject;
    public Furniture furniture
    {
        get; protected set;
    }

    public World world { get; protected set; }

    public int X { get; protected set; }
    public int Y { get; protected set; }

    public Tile(World world, int x, int y)
    {
        this.world = world;
        this.X = x;
        this.Y = y;
    }
    public bool isOccupied()
    {
        switch (Type)
        {
            case TileType.Floor:
            case TileType.Empty:
                return false;
            default:
                return true;
        }
    }



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
