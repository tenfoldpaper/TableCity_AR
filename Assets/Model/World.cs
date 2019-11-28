using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

    Tile[,] tiles;
    int width;
    public int Width
    {
        get
        {
            return width;
        }
    }

    int height;
    public int Height
    {
        get
        {
            return height;
        }
    }
    public World(int width = 100, int height = 100)
    {
        this.width = width;
        this.height = height;

        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }
        Debug.Log("World created with " + (width * height) + "tiles.");
    }

    public void RandomizeTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y].Type = Tile.TileType.Floor;
            }
        }
    }
    void CreateRoad(Vector3 start, Vector3 end)
    {
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
        if (x >= width || x < 0 || y >= height || y < 0)
        {
            //Debug.LogError("Tile (" + x + ", " + y + ") is out of range\n");
            return null;
        }
        return tiles[x, y];
    }
        //This might allow loading to take place dynamically, but probably just instantiating all of it right away is better

    public int GetHeight()
    {
        return height;
    }

    public int GetWidth()
    {
        return width;
    }
}
