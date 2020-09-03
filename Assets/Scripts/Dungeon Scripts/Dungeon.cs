using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon {
    private byte[,] map;
    private int width;
    private int height;

    private Tile[,] tiles;
    private int tilesWidth;
    private int tilesHeight;


    private Room[] rooms;


    public Dungeon(Tile[,] tilesInput, Room[] roomsInput) {

        tiles = tilesInput;
        rooms = roomsInput;

        // get array dims
        tilesWidth = tiles.GetLength(1);
        tilesHeight = tiles.GetLength(0);
        width = 4 * tilesWidth + 1;
        height = 4 * tilesHeight + 1;

        InitialiseDungeon();

    }

    // ===================
    // Getters

    public byte getByte(int x, int y) { return map[y, x]; }
    public int getWidth() { return width; }
    public int getHeight() { return height; }

    // ===================
    // Init Methods

    private void InitialiseDungeon() {

        ConnectRoomsToMaze();

        removeDeadEnds(10);

        // create new map of level
        map = new byte[height, width];

        // declare var to store the map of each tile
        byte[,] tileMap;

        // iterate for all of our tiles
        for (int y = 0; y < tilesHeight; y++) {
            for (int x = 0; x < tilesWidth; x++) {

                // if the current tile isn't null, convert to a 3x3 byte array
                if (tiles[y, x] != null) {

                    tileMap = tiles[y, x].getMap();

                    // add walls to obtuse (270º) corner of our tileMap
                    GenerateObtuseCorners(tileMap, x, y);

                    // add our tile to the map
                    GenerateMapFromTile(tileMap, x, y);
                }
            }
        }

        // add walls & floors to buffer btwn tiles to connect the map
        ConnectMapTiles();
    }

    // ===================
    // Helper Methods

    private void ConnectMapTiles() {
        // fill in rows between tiles, connect everything up nicely
        for (int y = 4; y < map.GetLength(1) - 1; y += 4) {
            for (int x = 1; x < map.GetLength(0) - 1; x++) {

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
        }

        // fill in columns between tiles, connect everything up
        for (int x = 4; x < map.GetLength(0) - 1; x += 4) {
            for (int y = 1; y < map.GetLength(1) - 1; y++) {
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
    }


    private void ConnectRoomsToMaze() {
        byte[] edges = { 0x1, 0x2, 0x4, 0x8 };
        byte edge, edgeBitmask;
        int n, x, y, roomX, roomY, roomWidth, roomHeight;

        // iterate over rooms in dungeon
        foreach (Room room in rooms) {

            if (room == null)
                continue;

            // n controls how many exits to attempt to make
            n = Random.Range(3, 4);

            roomX = room.getX();
            roomY = room.getY();
            roomWidth = room.getWidth();
            roomHeight = room.getHeight();

            edgeBitmask = 0x0;

            for (int i = 0; i < n; i++) {

                // pick an edge to create the exit on
                edge = edges[Random.Range(0, 4)];

                if (edge == 0x1 & (roomY + roomHeight) < tilesHeight
                    & (0x1 & edgeBitmask) == 0)
                {

                    edgeBitmask = (byte)(edgeBitmask | 0x1);

                    x = Random.Range(roomX, roomX + roomWidth);
                    y = roomY + roomHeight - 1;

                    if (tiles[y + 1, x].isOpen())
                    {
                        tiles[y + 1, x].openWallOnSides(0x4);
                        tiles[y, x].openWallOnSides(0x1);
                    }
                }

                else if (edge == 0x4 & roomY > 0 & (0x4 & edgeBitmask) == 0)
                {
                    x = Random.Range(roomX, roomX + roomWidth);
                    y = roomY;

                    edgeBitmask = (byte)(edgeBitmask | 0x4);

                    if (tiles[y - 1, x].isOpen())
                    {
                        tiles[y - 1, x].openWallOnSides(0x1);
                        tiles[y, x].openWallOnSides(0x4);
                    }
                }

                else if (edge == 0x2 & (roomX + roomWidth) < tilesWidth
                    & (0x2 & edgeBitmask) == 0)
                {

                    edgeBitmask = (byte)(edgeBitmask | 0x2);

                    x = roomX + roomWidth - 1;
                    y = Random.Range(roomY, roomY + roomHeight);

                    if (tiles[y, x + 1].isOpen())
                    {
                        tiles[y, x + 1].openWallOnSides(0x8);
                        tiles[y, x].openWallOnSides(0x2);
                    }
                }

                else if (edge == 0x8 & roomX > 0 & (0x8 & edgeBitmask) == 0)
                {
                    edgeBitmask = (byte)(edgeBitmask | 0x8);

                    x = roomX;
                    y = Random.Range(roomY, roomY + roomHeight);

                    if (tiles[y, x - 1].isOpen())
                    {
                        tiles[y, x - 1].openWallOnSides(0x2);
                        tiles[y, x].openWallOnSides(0x8);
                    }
                }

                else
                    i--;
            }
        }
    }


    private void GenerateMapFromTile(byte[,] tileMap, int x, int y) {
        // iterate over the byte array and add each on to the map
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                map[(4 * y) + i + 1, (4 * x) + j + 1] =
                      (byte)(map[(4 * y) + i + 1, (4 * x) + j + 1] | tileMap[i, j]);
            }
        }
    }

    private void GenerateObtuseCorners(byte[,] tileMap, int x, int y) {

        // if above 0, and  there is an open path down from current tileMap
        if (y > 0 & tileMap[0,1] == 0x1) {

            // if current tile has no left wall, but tile below does
            if ((tiles[y, x].getWalls() & 0x8) == 0
            & (tiles[y - 1, x].getWalls() & 0x8) > 0) {
                // add an obtuse corner to btm-L of tileMap
                tileMap[0, 0] = 0x3;
            }
            
            // if current tile has no right wall, but tile below does
            if ((tiles[y, x].getWalls() & 0x2) == 0
            & (tiles[y - 1, x].getWalls() & 0x2) > 0)
                // add an obtuse corner to btm-R of tileMap
                tileMap[0, 2] = 0x3;
        }

        // if below tilesHeight, and there is an open path down from current tileMap
        if (y < tilesHeight-1 & tileMap[2,1] == 0x1) {

            // if current tile has no left wall, but tile above does
            if ((tiles[y, x].getWalls() & 0x8) == 0
            & (tiles[y+1, x].getWalls() & 0x8) > 0) {
                // add an obtuse corner to top-L of tileMap
                tileMap[2, 0] = 0x3;
            }

            // if current tile has no right wall, but tile above does
            if ((tiles[y, x].getWalls() & 0x2) == 0
            & (tiles[y + 1, x].getWalls() & 0x2) > 0) {
                // add an obtuse corner to top-R of tileMap
                tileMap[2, 2] = 0x3;
            }

        }

        // if right of 0, and there is an open path left from current tileMap
        if (x > 0 & tileMap[1,0] == 0x1) {

            // if current tile has no top wall, but tile to left does
            if ((tiles[y, x].getWalls() & 0x1) == 0
            & (tiles[y, x-1].getWalls() & 0x1) > 0)
                // add an obtuse corner to top-L of tileMap
                tileMap[2, 0] = 0x3;
            
            
            // if current tile has no bottom wall, but tile to left does
            if ((tiles[y, x].getWalls() & 0x4) == 0
            & (tiles[y, x-1].getWalls() & 0x4) > 0)
                // add an obtuse corner to btm-L of tileMap
                tileMap[0, 0] = 0x3;
        }

        // if left of tilesWidth, & there is an open path right from current tileMap
        if (x < tilesWidth-1 & tileMap[1,2] == 0x1) {

            // if current tile has no top wall, but tile to right does
            if ((tiles[y, x].getWalls() & 0x1) == 0
            & (tiles[y, x+1].getWalls() & 0x1) > 0) {
                // add an obtuse corner to top-R of tileMap
                tileMap[2, 2] = 0x3;
            }

            // if current tile has no bottom wall, but tile to right does
            if ((tiles[y, x].getWalls() & 0x4) == 0
            & (tiles[y, x+1].getWalls() & 0x4) > 0) {
                // add an obtuse corner to btm-R of tileMap
                tileMap[0, 2] = 0x3;
            }
        }
    }

    private void removeDeadEnds(int n) {
        // create a list of tiles to store dead ends
        var deadEnds = new List<Tile>();

        // find all dead ends in maze and add them to list
        for (int y = 0; y < tilesHeight; y++)
            for (int x = 0; x < tilesWidth; x++)
                // if tile has 3 bounding walls & isn't blocked, its a dead end
                if (tiles[y, x].getNumWalls() == 3 & tiles[y,x].isOpen())
                    deadEnds.Add(tiles[y, x]);

        // remove dead-ends and check if adjacent tiles are now dead ends
        Tile tile, next;
        for (int j = 0; j < n; j++) {
            for (int i = 0; i < deadEnds.Count; i++) {
                tile = deadEnds[i];
                tile.setClosed();

                if ((tile.getWalls() & 0x1) == 0 & tile.getY() < tilesHeight - 1) {
                    next = tiles[tile.getY() + 1, tile.getX()];
                    next.closeWallOnSides(0x4);
                }

                else if ((tile.getWalls() & 0x2) == 0 & tile.getX() < tilesWidth - 1) {
                    next = tiles[tile.getY(), tile.getX() + 1];
                    next.closeWallOnSides(0x8);
                }
                else if ((tile.getWalls() & 0x4) == 0 & tile.getY() > 0) {
                    next = tiles[tile.getY()-1, tile.getX()];
                    next.closeWallOnSides(0x1);
                }

                else {
                    next = tiles[tile.getY(), tile.getX() - 1];
                    next.closeWallOnSides(0x2);
                }

                if (next.getNumWalls() == 3)
                    deadEnds[i] = next;
                else
                    deadEnds.RemoveAt(i--);

            }
        }
    }

}

