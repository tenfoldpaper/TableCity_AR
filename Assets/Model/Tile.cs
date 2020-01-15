using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Empty, Floor };

public class Tile {

    public enum TileType { Empty, Floor, Road, Residential, Industrial, Entertainment, Water, Electricity };
                           //0      1      2        3           4             5          6          7
    //Action<int, string, float> someFunction;
    LooseObject looseObject;
    InstalledObject installedObject;
    public Furniture furniture{ get; protected set; }
    public World world { get; protected set; }
    public int X { get; protected set; }
    public int Y { get; protected set; }

    public GameObject gameObject;
    
    public bool electricity { get; set; }
    public bool water { get; set; }
    public int level;
    public int happiness { get; set; }

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
    

    public Tile(World world, int x, int y)
    {
        this.world = world;
        this.X = x;
        this.Y = y;
        this.happiness = 0;
    }
    public bool isOccupied()
    {
        if(Type == TileType.Empty)
        {
            return false;
        }
        else
        {
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

    public bool AdjacencyCheck()
    {
        // This type of object requires a road to be present at one of its four
        // adjacent blocks. Otherwise, the build will fail.
        Tile tile = this;
        Tile t1, t2, t3, t4;
        int x = tile.X;
        int y = tile.Y;
        if(x > 0)
        {
            t4 = tile.world.GetTileAt(x - 1, y);
        }
        else
        {
            t4 = this;
        }
        if(y > 0)
        {
            t3 = tile.world.GetTileAt(x, y - 1);
        }
        else
        {
            t3 = this;
        }
        if (x < WorldController.Instance.worldX - 1)
        {
            t2 = tile.world.GetTileAt(x + 1, y);
        }
        else
        {
            t2 = this;
        }
        if (y < WorldController.Instance.worldY - 1)
        {
            t1 = tile.world.GetTileAt(x, y + 1);
        }
        else
        {
            t1 = this;
        }
        if (t1.furniture != null)
        {
            if (t1.furniture.objectType == "Road")
            {
                return true;
            }
        }
        if (t2.furniture != null)
        {
            if (t2.furniture.objectType == "Road")
            {
                return true;
            }
        }
        if (t3.furniture != null)
        {
            if (t3.furniture.objectType == "Road")
            {
                return true;
            }
        }
        if (t4.furniture != null)
        {
            if (t4.furniture.objectType == "Road")
            {
                return true;
            }
        }

        return false;
    }


}
