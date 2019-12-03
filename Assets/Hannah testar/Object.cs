using UnityEngine;
using System.Collections;
using System;

public class Object
{
    public Tile tile {get; protected set;}

    public string objectType {get; protected set;}

    int width;
    int height;

    public bool linksToNeighbour {get; protected set;}

    Action<Object> cbOnChanged;

    static public Object CreatePrototype(string objectType, int width = 1, int height = 1, bool linksToNeighbour = false)
    {
        Object obj = new Object();

        obj.objectType = objectType;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linksToNeighbour;

        return obj;
    }

    static public Object PlaceInstance(Object proto, Tile tile)
    {
        Object obj = new Object();

        obj.objectType = proto.objectType;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbour = proto.linksToNeighbour;

        obj.tile = tile;

        if (tile.PlaceObject(obj) == false)
        {
            return null;
        }

        if (obj.linksToNeighbour)
        {
            Tile t;
            int x = tile.X;
            int y = tile.Y;

            t = tile.world.GetTileAt(x, y + 1);
            if (t != null && t.objects != null && t.objects.objectType == obj.objectType)
            {
                // We have a Northern Neighbour with the same object type as us, so
                // tell it that it has changed by firing is callback.
                t.objects.cbOnChanged(t.objects);
            }
            t = tile.world.GetTileAt(x + 1, y);
            if (t != null && t.objects != null && t.objects.objectType == obj.objectType)
            {
                t.objects.cbOnChanged(t.objects);
            }
            t = tile.world.GetTileAt(x, y - 1);
            if (t != null && t.objects != null && t.objects.objectType == obj.objectType)
            {
                t.objects.cbOnChanged(t.objects);
            }
            t = tile.world.GetTileAt(x - 1, y);
            if (t != null && t.objects != null && t.objects.objectType == obj.objectType)
            {
                t.objects.cbOnChanged(t.objects);
            }
        }
        return obj;
    }
    public void RegisterOnChangedCallback(Action<Object> callbackFunc)
    {
        cbOnChanged += callbackFunc;
    }
    public void UnregisterOnChangedCallback(Action<Object> callbackFunc)
    {
        cbOnChanged -= callbackFunc;
    }
}