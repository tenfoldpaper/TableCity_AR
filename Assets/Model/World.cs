using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World
{

    Tile[,] tiles;

    //Dictionary<string, Furniture> furniturePrototypes;
    Dictionary<string, Object> objectPrototypes;

    // The tile width of the world.
    public int Width { get; protected set; }

    // The tile height of the world
    public int Height { get; protected set; }

    Action<Object> cbObjectCreated;

    public World(int width = 100, int height = 100)
    {
        Width = width;
        Height = height;

        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (Width * Height) + " tiles.");

        CreateObjectPrototypes();
    }

    void CreateObjectPrototypes()
    {
        objectPrototypes = new Dictionary<string, Object>();

        objectPrototypes.Add("Building",
            Object.CreatePrototype(
                                "Building",
                                1,  // Width
                                1,  // Height
                                false // Links to neighbours and "sort of" becomes part of a large object
                            )
        );

        objectPrototypes.Add("Road",
            Object.CreatePrototype(
                                "Road",
                                1,  // Width
                                1,  // Height
                                true // Links to neighbours and "sort of" becomes part of a large object
                            )
        );

    }

    public void RandomizeTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y].Type = Tile.TileType.Floor;
                tiles[x, y].gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
            }
        }
    }
    public bool Occupied(int tile_x, int tile_y, int w, int h)
    {
        for (int x = tile_x; x < w; x++)
        {
            for (int y = tile_y; y < h; y++)
            {
                Tile t = GetTileAt(x, y);
                if (t != null)
                    if (t.isOccupied())
                        return true;
            }
        }
        return false;
    }
    public Tile GetTileAt(int x, int y)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
        {
            return null;
        }
        return tiles[x, y];
    }

    public void PlaceObject(string objectType, Tile t)
    {
        Object obj = Object.PlaceInstance(objectPrototypes[objectType], t);

        if (obj == null)
        {
            // Failed to place object -- most likely there was already something there.
            return;
        }

        if (cbObjectCreated != null)
        {
            cbObjectCreated(obj);
        }
    }

    public int GetHeight()
    {
        return Height;
    }

    public int GetWidth()
    {
        return Width;
    }

    public void RegisterObjectCreated(Action<Object> callbackfunc)
    {
        cbObjectCreated += callbackfunc;
    }

    public void UnregisterObjectCreated(Action<Object> callbackfunc)
    {
        cbObjectCreated -= callbackfunc;
    }
}
