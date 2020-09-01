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


    private int CalculateAdjacentWalls(int x, int y) {

        int numWallsInAdjTiles = 0;

        if ((tiles[y, x].getWalls() & 0x1) == 0)
            numWallsInAdjTiles += tiles[y + 1, x].getNumWalls();

        if ((tiles[y, x].getWalls() & 0x2) == 0)
            numWallsInAdjTiles += tiles[y, x + 1].getNumWalls();

        if ((tiles[y, x].getWalls() & 0x4) == 0)
            numWallsInAdjTiles += tiles[y - 1, x].getNumWalls();

        if ((tiles[y, x].getWalls() & 0x8) == 0)
            numWallsInAdjTiles += tiles[y, x - 1].getNumWalls();

        return numWallsInAdjTiles;
    }

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
        for (int x = 4; x < map.GetLength(0) - 1; x += 4)
        {
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
    }


    private void InitialiseDungeon() {

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

                    // add our tile to the map
                    GenerateMapFromTile(tileMap, x, y);

                    // add walls to obtuse (270º) corner
                    GenerateObtuseCorners(tileMap, x, y);

                    // add walls to T-junction corners
                    GenerateTJunction(tileMap, x, y);

                    // add walls to 4-way intersection corners
                    GenerateFourWayJunction(tileMap, x, y);
                }
            }
        }

        // add walls & floors to buffer btwn tiles to connect the map
        ConnectMapTiles();
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

        // check whether a tile has two or more bounding walls
        if (((tileMap[1, 0] & 0x2) + (tileMap[1, 2] & 0x2) +
        (tileMap[2, 1] & 0x2) + (tileMap[0, 1] & 0x2)) == 0x4) {
            if (y > 0) {

                // if tile below has a wall to the right, but current
                // tile doesn't, add a wall to Btm-R corner
                if ((tiles[y, x].getWalls() & 0x2) == 0
                    & (tiles[y, x].getWalls() & 0x4) == 0
                    & (tiles[y-1, x].getWalls() & 0x2) > 0) {
                    // (+3, +3) offset get the indx at Btm-R of tile
                    map[4 * y + 1, 4 * x + 3] = 0x3;
                }

                // if tile below has a wall to the left, but current
                // tile doesn't, add a wall to Btm-L cornter

                if ((tiles[y, x].getWalls() & 0x8) == 0
                    & (tiles[y, x].getWalls() & 0x4) == 0
                    & (tiles[y - 1, x].getWalls() & 0x8) > 0) {
                    // (+1, +1) offset get the indx at Btm-R of tile
                    map[4 * y + 1, 4 * x + 1] = 0x3;
                }
            }
                        
            if (y < tilesHeight-1) {
                // if tile above has a wall to the right, but current
                // tile doesn't, add a wall to Top-R corner

                if ((tiles[y, x].getWalls() & 0x2) == 0
                    & (tiles[y, x].getWalls() & 0x1) == 0
                    & (tiles[y + 1, x].getWalls() & 0x2) > 0) {
                    // (+3, +3) offset get the indx at Top-R of tile
                    map[4 * y + 3, 4 * x + 3] = 0x3;
                }

                // if tile above has a wall to the left, but current
                // tile doesn't, add a wall to Top-L corner

                if ((tiles[y, x].getWalls() & 0x8) == 0
                    & (tiles[y, x].getWalls() & 0x1) == 0
                    & (tiles[y + 1, x].getWalls() & 0x8) > 0) {
                    // (+1, +3) offset gets the indx at Top-L of tile
                    map[4 * y + 3, 4 * x + 1] = 0x3;
                }
            }

            if (x > 0) {

                // if tile to left has a top wall, but current
                // tile doesn't, add a wall to Top-L corner

                if ((tiles[y, x].getWalls() & 0x1) == 0
                    & (tiles[y, x].getWalls() & 0x8) == 0
                    & (tiles[y, x - 1].getWalls() & 0x1) > 0) {
                    // (+1, +3) offset get the indx at Top-L of tile
                    map[4 * y + 3, 4 * x + 1] = 0x3;
                }

                // if tile to left has a bottom wall, but current
                // tile doesn't, add a wall to Btm-L corner

                if ((tiles[y, x].getWalls() & 0x4) == 0
                    & (tiles[y, x].getWalls() & 0x8) == 0
                    & (tiles[y, x - 1].getWalls() & 0x4) > 0) {
                    // (+1, +3) offset gets the indx at Btm-L of tile
                    map[4 * y + 1, 4 * x + 1] = 0x3;
                }
            }


            if (x < tilesWidth - 1) {

                // if tile to right has a top wall, but current
                // tile doesn't, add a wall to Top-R corner

                if ((tiles[y, x].getWalls() & 0x1) == 0
                    & (tiles[y, x].getWalls() & 0x2) == 0
                    & (tiles[y, x + 1].getWalls() & 0x1) > 0) {
                    // (+3, +3) offset get the indx at Top-R of tile
                    map[4 * y + 3, 4 * x + 3] = 0x3;
                }

                // if tile to right has a bottom wall, but current
                // tile doesn't, add a wall to Btm-R corner

                if ((tiles[y, x].getWalls() & 0x4) == 0
                    & (tiles[y, x].getWalls() & 0x2) == 0
                    & (tiles[y, x + 1].getWalls() & 0x4) > 0) {
                    // (+1, +3) offset gets the indx at Btm-R of tile
                    map[4 * y + 1, 4 * x + 3] = 0x3;

                }
            }
        }
    }

    private void GenerateTJunction(byte[,] tileMap, int x, int y) {

        // check whether a tile has one bounding walls
        if (((tileMap[1, 0] & 0x2) + (tileMap[1, 2] & 0x2) +
            (tileMap[2, 1] & 0x2) + (tileMap[0, 1] & 0x2)) == 0x2) {

            // determine whether tile is a T-junction or room wall
            // by summing the number of walls each connected adjacent
            // tile has. room tiles will have 4 maximum.

            int numWallsInAdjTiles = CalculateAdjacentWalls(x, y);

            // 3 is the maximum a room tile can have, if we have more
            // tile must be a T junction
            if(numWallsInAdjTiles >= 4) {

                // if top wall blocked, add L-btm + R-btm corners
                if ((tiles[y, x].getWalls() & 0x1) > 0) {
                    map[4 * y + 1, 4 * x + 1] = 0x3;
                    map[4 * y + 1, 4 * x + 3] = 0x3;
                }

                // if rgt wall blocked, add L-btm + L-top corners
                else if ((tiles[y, x].getWalls() & 0x2) > 0) {
                    map[4 * y + 1, 4 * x + 1] = 0x3;
                    map[4 * y + 3, 4 * x + 1] = 0x3;
                }

                // if top wall blocked, add L-top + R-top corners
                else if ((tiles[y, x].getWalls() & 0x4) > 0) {
                    map[4 * y + 3, 4 * x + 1] = 0x3;
                    map[4 * y + 3, 4 * x + 3] = 0x3;
                }

                // if left wall blocked, add R-top + R-btm corners
                else {
                    map[4 * y + 3, 4 * x + 3] = 0x3;
                    map[4 * y + 1, 4 * x + 3] = 0x3;
                }
            }
        }
    }

    private void GenerateFourWayJunction(byte [,] tileMap, int x, int y) {

        // first check whether tile has no bounding walls
        if (((tileMap[1, 0] & 0x2) + (tileMap[1, 2] & 0x2) +
            (tileMap[2, 1] & 0x2) + (tileMap[0, 1] & 0x2)) == 0x0) {

            // check how many sides are walled off in adjacent tiles
            int numWallsInAdjTiles = CalculateAdjacentWalls(x, y);

            // min. of 8 walled sides in T-junction (4 passages w/ 2 sides each)
            if (numWallsInAdjTiles >= 8) {
                // add wall corners to map
                map[4 * y + 1, 4 * x + 1] = 0x3;
                map[4 * y + 3, 4 * x + 1] = 0x3;
                map[4 * y + 1, 4 * x + 3] = 0x3;
                map[4 * y + 3, 4 * x + 3] = 0x3;
            }
        }
    }

}

