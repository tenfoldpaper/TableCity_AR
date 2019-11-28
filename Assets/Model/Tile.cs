using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    public bool isOccupied()
    {
        switch (type)
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
        //This Action is an odd type of function, so something like
        //cbTileTypeChanged += callback; is perfectly legal, and each of those additional funcitons will be run. 
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }


}
