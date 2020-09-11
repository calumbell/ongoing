using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon
{
    private byte[,] map;
    private int width, height;

    private TileMap tileMap;

    int startRoom, endRoom;

    public Dungeon(int x, int y)
    {

        tileMap = new TileMap(x, y);

        // get array dims
        width = 4 * tileMap.getWidth() + 1;
        height = 4 * tileMap.getHeight() + 1;

        InitialiseDungeon();

    }

    // ===================
    // Getters

    public byte getByte(int x, int y) { return map[y, x]; }
    public int getWidth() { return width; }
    public int getHeight() { return height; }
    public Room getStartRoom() { return tileMap.getRoom(startRoom); }
    public int getStartCoordX()
    {
        return 4 * (getStartRoom().getX() + getStartRoom().getWidth() / 2);
    }

    public int getStartCoordY()
    {
        return 4 * (getStartRoom().getY() + getStartRoom().getHeight() / 2);
    }

    // ===================
    // Init Methods

    private void InitialiseDungeon()
    {
        // create new map of level
        map = new byte[height, width];

        // decalre var to store the current tile
        Tile currentTile;

        // declare var to the tile as a 3x3 array of bytes
        byte[,] tileInBytes;

        // iterate for all of our tiles
        for (int y = 0; y < tileMap.getHeight(); y++) {
            for (int x = 0; x < tileMap.getWidth(); x++) {

                currentTile = tileMap.getTile(x, y);

                // if the current tile isn't null, convert to a 3x3 byte array
                if (currentTile != null) {

                    tileInBytes = currentTile.getMap();

                    // add walls to obtuse (270º) corner of our tileMap
                    GenerateObtuseCorners(tileInBytes, x, y);

                    // add our tile to the map
                    GenerateMapFromTile(tileInBytes, x, y);
                }
            }
        }

        // add walls & floors to buffer btwn tiles to connect the map
        ConnectMapTiles();

        startRoom = tileMap.getStartRoomIndex();
        endRoom = tileMap.getEndRoomIndex();
    }

    // ===================
    // Helper Methods

    private void ConnectMapTiles()
    {
        // fill in rows between tiles, connect everything up nicely
        for (int y = 4; y < map.GetLength(1) - 1; y += 4)
            for (int x = 1; x < map.GetLength(0) - 1; x++)
            {

                // if there are two floors either side of a space, connect them
                if (map[y - 1, x] == 0x1 & map[y + 1, x] == 0x1)

                    map[y, x] = (byte)(map[y, x] | 0x1);

                // if there is a wall N+S AND either floor NE+SE OR floor NW+SW
                // then this is the edge of a vertical passage, make a wall.
                else if (map[y - 1, x] == 0x3 & map[y + 1, x] == 0x3
                    & ((map[y - 1, x + 1] == 0x1 & map[y + 1, x + 1] == 0x1)
                    | ((map[y - 1, x - 1] == 0x1 & map[y + 1, x - 1] == 0x1))))

                    map[y, x] = (byte)(map[y, x] | (byte)0x3);
            }
       

        // fill in columns between tiles, connect everything up
        for (int x = 4; x < map.GetLength(0) - 1; x += 4)
            for (int y = 1; y < map.GetLength(1) - 1; y++)
            {
                // if there are two floors either side of a space, connect them
                if (map[y, x - 1] == 0x1 & map[y, x + 1] == 0x1)

                    map[y, x] = (byte)(map[y, x] | 0x1);

                // if there is a wall W+E AND either floor SW+SE OR floor NW+NE
                // then this is the edge of a horizontal passage, make a wall
                else if (map[y, x - 1] == 0x3 & map[y, x + 1] == 0x3
                    & ((map[y - 1, x - 1] == 0x1 & map[y - 1, x + 1] == 0x1)
                    | ((map[y + 1, x - 1] == 0x1 & map[y + 1, x + 1] == 0x1))))

                    map[y, x] = (byte)(map[y, x] | (byte)0x3);
            }
        
    }


    private void GenerateMapFromTile(byte[,] tileInBytes, int x, int y)
    {
        // iterate over the byte array and add each on to the map
        for (int i = 0; i < 3; i++) 
            for (int j = 0; j < 3; j++)
                map[(4 * y) + i + 1, (4 * x) + j + 1] =
                      (byte)(map[(4 * y) + i + 1, (4 * x) + j + 1] | tileInBytes[i, j]);
            
        
    }

    private void GenerateObtuseCorners(byte[,] tileInBytes, int x, int y)
    {

        // if above 0, and  there is an open path down from current tileMap
        if (y > 0 & tileInBytes[0,1] == 0x1)
        {

            // if current tile has no left wall, but tile below does
            if ((tileMap.getTile(x, y).getWalls() & 0x8) == 0
            & (tileMap.getTile(x, y-1).getWalls() & 0x8) > 0) 
                // add an obtuse corner to btm-L of tileMap
                tileInBytes[0, 0] = 0x3;
            
            
            // if current tile has no right wall, but tile below does
            if ((tileMap.getTile(x, y).getWalls() & 0x2) == 0
            & (tileMap.getTile(x, y-1).getWalls() & 0x2) > 0)
                // add an obtuse corner to btm-R of tileMap
                tileInBytes[0, 2] = 0x3;
        }

        // if below tilesHeight, and there is an open path down from current tileMap
        if (y < tileMap.getHeight() - 1 & tileInBytes[2,1] == 0x1)
        {
            // if current tile has no left wall, but tile above does
            if ((tileMap.getTile(x, y).getWalls() & 0x8) == 0
            & (tileMap.getTile(x, y+1).getWalls() & 0x8) > 0) 
                // add an obtuse corner to top-L of tileMap
                tileInBytes[2, 0] = 0x3;

            // if current tile has no right wall, but tile above does
            if ((tileMap.getTile(x, y).getWalls() & 0x2) == 0
            & (tileMap.getTile(x, y+1).getWalls() & 0x2) > 0) 
                // add an obtuse corner to top-R of tileMap
                tileInBytes[2, 2] = 0x3;
        }

        // if right of 0, and there is an open path left from current tileMap
        if (x > 0 & tileInBytes[1,0] == 0x1)
        {

            // if current tile has no top wall, but tile to left does
            if ((tileMap.getTile(x, y).getWalls() & 0x1) == 0
            & (tileMap.getTile(x-1, y).getWalls() & 0x1) > 0)
                // add an obtuse corner to top-L of tileMap
                tileInBytes[2, 0] = 0x3;
            
            
            // if current tile has no bottom wall, but tile to left does
            if ((tileMap.getTile(x, y).getWalls() & 0x4) == 0
            & (tileMap.getTile(x-1, y).getWalls() & 0x4) > 0)
                // add an obtuse corner to btm-L of tileMap
                tileInBytes[0, 0] = 0x3;
        }

        // if left of tilesWidth, & there is an open path right from current tileMap
        if (x < tileMap.getWidth() - 1 & tileInBytes[1,2] == 0x1)
        {

            // if current tile has no top wall, but tile to right does
            if ((tileMap.getTile(x, y).getWalls() & 0x1) == 0
            & (tileMap.getTile(x+1, y).getWalls() & 0x1) > 0)
                // add an obtuse corner to top-R of tileMap
                tileInBytes[2, 2] = 0x3;

            // if current tile has no bottom wall, but tile to right does
            if ((tileMap.getTile(x, y).getWalls() & 0x4) == 0
            & (tileMap.getTile(x+1, y).getWalls() & 0x4) > 0)
                // add an obtuse corner to btm-R of tileMap
                tileInBytes[0, 2] = 0x3;
        }
    }
    
}

