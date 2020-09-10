using System.Collections.Generic;
using UnityEngine;

public class Maze
{

    // ==========================
    // Member Fields

    // width and height of maze
    private int width, height;

    // the tiles that make up the maze
    private Tile[,] tiles;

    // an array of which tiles have been marks as off-limits. its indices line up
    // with the tiles array. tiles are blocked if they are part of a room, or
    // have already been visited by our maze gen. algorithm

    private bool[,] blockedTiles;



    // ===========================
    // Class Constructors
  
    public Maze(Tile[,] tilesInput, Room[] rooms)
    {

        // instantiate w/ rooms array to generate the maze
        tiles = tilesInput;
        width = tiles.GetLength(1);
        height = tiles.GetLength(0);
        
        blockedTiles = new bool[height, width];

        // block out all of the rooms in the tilemap
        foreach (Room room in rooms) 
            if (room != null) 
                BlockOutRoom(room);

        GenerateMaze();
    }

    // ===========================
    // Getters

    public Tile[,] getTiles() { return tiles; }
    public Tile getTile(int x, int y) { return tiles[y, x]; }


    // ===========================
    // Instance Methods

    public void BlockOutRoom(Room room)
    {
        // takes a room as an argument and marks all tiles contained
        // within as out of bounds for the maze gen. algorithm


        // get room position to use as offset for blockedTiles array
        int offsetX = room.getX();
        int offsetY = room.getY();

        // iterate across all tiles in rooms and block them for the maze gen
        // go 1 index over in all directions to preserve space around room

        for (int y = 0; y < room.getHeight(); y++)
            for (int x = 0; x < room.getWidth(); x++)
                blockedTiles[y+offsetY, x+offsetX] = true;
    }
   
    public void GenerateMaze()
    {
        // recursive backtracker algorithm for creating a perfect maze
        // http://www.astrolog.org/labyrnth/algrithm.htm#perfect

        // GenerateMazeCourse fills empty spaces in the tiles array with a maze
        Stack<Tile> tileStack = new Stack<Tile>();
        Tile currentTile, nextTile;

        // iterate over all tiles in our dungeon
        for (int y = 0; y <  height; y++)
            for (int x = 0; x < width; x++) {

                // at a perimeter wall to our maze
                if (y == 0)
                    tiles[y, x].closeWallOnSides(0x4);
                else if (y == height - 1)
                    tiles[y, x].closeWallOnSides(0x1);
                else if (x == 0)
                    tiles[y, x].closeWallOnSides(0x8);
                else if (x == width - 1)
                    tiles[y, x].closeWallOnSides(0x2);
      

                // once we find an unblocked tile, begin algorithm
                if (!blockedTiles[y,x])
                {

                    // push initial tile too stack and mark it as visited
                    tileStack.Push(tiles[y, x]);
                    blockedTiles[y, x] = true;

                    // while the stack is not empty
                    while (tileStack.Count > 0)
                    {

                        // pop a tile from our stack and check whether it has
                        // any unvisited neighbours
                        currentTile = tileStack.Pop();
                        currentTile.setOpen();
                        string[] unvisitedNeighbours = getUnvisitedNeighbours(currentTile.getX(), currentTile.getY(), blockedTiles);

                        // if tile has unvisited neighbours, pick one and create
                        // a path between the two tiles
                        if (unvisitedNeighbours.Length > 0)
                        {

                            // pick rndm dir with an unvisited neighbour
                            string direction = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Length)];

                            if (direction == "up")
                            {
                                nextTile = tiles[currentTile.getY() + 1, currentTile.getX()];

                                // rmv upper wall of current a lower wall of next
                                currentTile.openWallOnSides(0x1);
                                nextTile.openWallOnSides(0x4);

                                // mark next tile as visited
                                blockedTiles[currentTile.getY() + 1, currentTile.getX()] = true;
                            }

                            else if (direction == "right")
                            {
                                nextTile = tiles[currentTile.getY(), currentTile.getX() + 1];

                                //rmv right wall of current and left wall of next
                                currentTile.openWallOnSides(0x2);
                                nextTile.openWallOnSides(0x8);

                                // mark next tile as visited
                                blockedTiles[currentTile.getY(), currentTile.getX() + 1] = true;
                            }

                            else if (direction == "down")
                            {
                                nextTile = tiles[currentTile.getY() - 1, currentTile.getX()];

                                //rmv lower wall of current and upper wall of next
                                currentTile.openWallOnSides(0x4);
                                nextTile.openWallOnSides(0x1);

                                // mark next tile as visited
                                blockedTiles[currentTile.getY() - 1, currentTile.getX()] = true;
                            }

                            else
                            {
                                nextTile = tiles[currentTile.getY(), currentTile.getX() - 1];

                                //rmv left wall of current and right wall of next
                                currentTile.openWallOnSides(0x8);
                                nextTile.openWallOnSides(0x2);

                                // mark next tile as visited
                                blockedTiles[currentTile.getY(), currentTile.getX() - 1] = true;
                            }

                            // push current & next tile to stack, repeat
                            tileStack.Push(currentTile);
                            tileStack.Push(nextTile);
                        }
                    }
                }
            }
    }

    private string[] getUnvisitedNeighbours(int x, int y, bool[,] visitedTiles)
    {
    // returns an array of directions to unvisited neighbours from the tile at
    // coordinates (x, y)

        List<string> neighbours = new List<string>();

        // check that tile is not on the edge of map to avoid seg faults
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
}
