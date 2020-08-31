using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private int x;
    private int y;
    private bool open;
    private bool isPartOfRoom;
    // 0x1 -> wall up
    // 0x2 -> wall right
    // 0x4 -> wall down
    // 0x8 -> wall left
    private byte walls;

    public Tile(int xInput, int yInput, bool openInput, byte wallsInput) {
        x = xInput;
        y = yInput;
        open = openInput;
        walls = wallsInput;

    }

    // ======================
    // Getters

    public byte getWalls() { return walls; }
    public int getX() { return x; }
    public int getY() { return y; }
    public bool isOpen() { return open; }


    // ======================
    // Setters

    public void setWalls(byte sides) { walls = sides; }
    public void closeWallOnSides(byte sides) { walls = (byte)(walls | sides); }
    public void openWallOnSides(byte sides) { if (walls >= sides) walls -= sides; }
    public void setOpen() { open = true; }
    public void setClosed() { open = false;  }

    public byte[,] getMap() {
        byte[,] map = new byte[3, 3];

        if (open)
            // set centre tile to a piece of unobstructed floor
            map[1, 1] = 0x1;
        
            // check for walls at the top
            byte wallCode = (byte)(((walls & 0x1) > 0) ? 0x3 : map[1, 1]);
            for (int i = 0; i < 3; i++)
                map[2, i] = (byte)(map[2, i] | wallCode);

            // check for walls to the right
            wallCode = (byte)(((walls & 0x2) > 0) ? 0x3 : map[1, 1]);
            for (int i = 0; i < 3; i++)
                map[i, 2] = (byte)(map[i, 2] | wallCode);

            // check for walls to the bottom
            wallCode = (byte)(((walls & 0x4) > 0) ? 0x3 : map[1, 1]);
            for (int i = 0; i < 3; i++)
                map[0, i] = (byte)(map[0, i] | wallCode);

            // check for walls to the left
            wallCode = (byte)(((walls & 0x8) > 0) ? 0x3 : map[1, 1]);
            for (int i = 0; i < 3; i++)
                map[i, 0] = (byte)(map[i, 0] | wallCode);
        
        return map;
    }
}
