﻿public class Tile
{
    // x and y indicies in the tiles array
    private int x, y;

    // open controls whether this tile is open, can be traversed, has a floor
    private bool open;

    // Walls variable controls which walls will be generated around this tile
    // 0x1 = top wall, 0x2 = right wall, 0x4 = bottom wall, 0x8 = left wall
    private byte walls;

    private string stairs;

    public Tile(int xInput, int yInput, bool openInput, byte wallsInput) {
        x = xInput;
        y = yInput;
        open = openInput;
        walls = wallsInput;
        stairs = null;
    }

    // ======================
    // Getters

    public byte getWalls() { return walls; }
    public int getX() { return x; }
    public int getY() { return y; }
    public bool isOpen() { return open; }


    /*
     * getNumWalls
     * Returns the number of walls that are around this tile. checks the walls
     * variable and for each wall increments n before returning it
     */
    public int getNumWalls()
    {
        int n = 0;

        n = (walls & 0x1) > 0 ? n + 1 : n;
        n = (walls & 0x2) > 0 ? n + 1 : n;
        n = (walls & 0x4) > 0 ? n + 1 : n;
        n = (walls & 0x8) > 0 ? n + 1 : n;

        return n;
    }

    // ======================
    // Setters

    public void setWalls(byte sides) { walls = sides; }
    public void closeWallOnSides(byte sides) { walls = (byte)(walls | sides); }
    public void openWallOnSides(byte sides) { walls = (byte)(walls & ~sides) ; }

    public void setOpen() { open = true; }
    public void setClosed() { open = false;  }

    public void setStairs(string dir) { stairs = dir; }


    /*
     * getMap()
     * Returns a tile as a 3x3 array of bytes that tell DungeonGenerator
     * exactly where to place floor, walls, etc.
     */

    public byte[,] getMap()
    {

        byte[,] map = new byte[3, 3];

        if (open)
        {

            // generate a staircase is Tile isStair, else make unblocked floor
            if (stairs == "up")
                map[1, 1] = 0x5;
            else if (stairs == "down")
                map[1, 1] = 0x4;
            else
                map[1, 1] = 0x1;

            // check for walls at the top
            byte wallCode = (byte)(((walls & 0x1) > 0) ? 0x3 : 0x1);
            for (int i = 0; i < 3; i++)
                map[2, i] = (byte)(map[2, i] | wallCode);

            // check for walls to the right
            wallCode = (byte)(((walls & 0x2) > 0) ? 0x3 : 0x1);
            for (int i = 0; i < 3; i++)
                map[i, 2] = (byte)(map[i, 2] | wallCode);

            // check for walls to the bottom
            wallCode = (byte)(((walls & 0x4) > 0) ? 0x3 : 0x1);
            for (int i = 0; i < 3; i++)
                map[0, i] = (byte)(map[0, i] | wallCode);

            // check for walls to the left
            wallCode = (byte)(((walls & 0x8) > 0) ? 0x3 : 0x1);
            for (int i = 0; i < 3; i++)
                map[i, 0] = (byte)(map[i, 0] | wallCode);
        }

        else
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    map[y, x] = 0;

        return map;
    }
}
