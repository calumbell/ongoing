using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    private int width;
    private int height;
    private Tile[,] tiles;
    private bool[,] blockedTiles;

    private byte[,] map;
    private int numTilesRemaining;

    // ===========================
    // Class Constructors

    
    public Maze(Tile[,] tilesInput, Room[] rooms) {
        // instantiate w/ rooms array to generate the maze
        tiles = tilesInput;
        width = tiles.GetLength(1);
        height = tiles.GetLength(0);
        
        blockedTiles = new bool[height, width];

        numTilesRemaining = width * height;

        foreach (Room room in rooms) {
            if (room != null) {
                BlockOutRoom(room);
                numTilesRemaining -= room.getWidth() * room.getHeight();
            }
        }

        GenerateMazeCourse();
    }

    // ===========================
    // Getters

    public Tile[,] getTiles() { return tiles; }
    public Tile getTile(int x, int y) { return tiles[y, x]; }


    // ===========================
    // Instance Methods

    public void BlockOutRoom(Room room) {
        // BlockOutRooms: takes a room as an argument and marks all tiles contained
        // within as out of bounds for the maze gen. algorithm


        // get room position to use as offset for blockedTiles array
        int offsetX = room.getX();
        int offsetY = room.getY();

        // iterate across all tiles in rooms and block them for the maze gen
        // go 1 index over in all directions to preserve space around room

        for (int y = 0; y < room.getHeight(); y++) {
            for (int x = 0; x < room.getWidth(); x++)
                blockedTiles[y+offsetY, x+offsetX] = true;
        }
        return;
    }
   
    public void GenerateMazeCourse() {
        // GenerateMazeCourse fills empty spaces in the tiles array with a maze
        Stack<Tile> tileStack = new Stack<Tile>();
        Tile currentTile, nextTile;

        for (int y = 0; y <  height; y++) {
            for (int x = 0; x < width; x++) {
                
                if (y == 0)
                    tiles[y, x].closeWallOnSides(0x4);
                else if (y == height - 1)
                    tiles[y, x].closeWallOnSides(0x1);
                else if (x == 0)
                    tiles[y, x].closeWallOnSides(0x8);
                else if (x == width - 1)
                    tiles[y, x].closeWallOnSides(0x2);
      

                // once we find an unblocked tile, begin algorithm
                if (!blockedTiles[y,x]) {

                    // push initial tile too stack and mark it as visited
                    //tiles[y, x].setOpen();
                    tileStack.Push(tiles[y, x]);
                    blockedTiles[y, x] = true;

                    // while the stack is not empty
                    while (tileStack.Count > 0) {
                        currentTile = tileStack.Pop();
                        currentTile.setOpen();
                        string[] unvisitedNeighbours = getUnvisitedNeighbours(currentTile.getX(), currentTile.getY(), blockedTiles);

                        if (unvisitedNeighbours.Length > 0) {
                            
                            string direction = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Length)];

                            if (direction == "up") {
                                nextTile = tiles[currentTile.getY() + 1, currentTile.getX()];

                                // rmv upper wall of current a lower wall of next
                                currentTile.openWallOnSides(0x1);
                                nextTile.openWallOnSides(0x4);

                                // mark next tile as visited
                                blockedTiles[currentTile.getY() + 1, currentTile.getX()] = true;
                            }

                            else if (direction == "right") {
                                nextTile = tiles[currentTile.getY(), currentTile.getX() + 1];

                                //rmv right wall of current and left wall of next
                                currentTile.openWallOnSides(0x2);
                                nextTile.openWallOnSides(0x8);

                                // mark next tile as visited
                                blockedTiles[currentTile.getY(), currentTile.getX() + 1] = true;
                            }

                            else if (direction == "down") {
                                nextTile = tiles[currentTile.getY() - 1, currentTile.getX()];

                                //rmv lower wall of current and upper wall of next
                                currentTile.openWallOnSides(0x4);
                                nextTile.openWallOnSides(0x1);

                                // mark next tile as visited
                                blockedTiles[currentTile.getY() - 1, currentTile.getX()] = true;
                            }
                            else {
                                nextTile = tiles[currentTile.getY(), currentTile.getX() - 1];

                                //rmv left wall of current and right wall of next
                                currentTile.openWallOnSides(0x8);
                                nextTile.openWallOnSides(0x2);

                                // mark next tile as visited
                                blockedTiles[currentTile.getY(), currentTile.getX() - 1] = true;
                            }

                            tileStack.Push(currentTile);
                            tileStack.Push(nextTile);
                        }
                    }
                }
            }
        }

        return;
    }

    private string[] getUnvisitedNeighbours(int x, int y, bool[,] visitedTiles) {
        List<string> neighbours = new List<string>();

        // check that tile is not on the edge to avoid seg faults
        if (y > 0) 
            if (!visitedTiles[y-1, x])
                neighbours.Add("down");

        if (y < visitedTiles.GetLength(0) - 1)
            if (!visitedTiles[y+1, x])
                neighbours.Add("up");

        if (x > 0)
            if (!visitedTiles[y, x-1])
                neighbours.Add("left");

        if (x < visitedTiles.GetLength(1) - 1)
            if (!visitedTiles[y, x + 1])
                neighbours.Add("right");

        return neighbours.ToArray();
    }

    // ===========================
    // Static Methods

    public static byte[,] GenerateMazeFine(byte[,] map) {
        // GenerateMazeFine fills empty spaces in the map with corridors, esp.
        // corridors between rooms which GenerateMazeCourse ignores
        return map;
    }
}
