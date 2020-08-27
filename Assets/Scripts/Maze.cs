﻿public class Maze
{
    private int width;
    private int height;
    private byte[,] map;
    private bool[,] blockedTiles;

    // ===========================
    // Class Constructors

    public Maze(int widthIn, int heightIn) {
        width = widthIn;
        height = heightIn;
        map = new byte[height, width];
        blockedTiles = new bool[height, width];
    }

    // instantiate w/ a rooms array to generate the maze
    public Maze(int widthIn, int heightIn, Room[] rooms) {
        width = widthIn;
        height = heightIn;
        map = new byte[height, width];
        blockedTiles = new bool[height, width];

        foreach (Room room in rooms) {
            if (room != null) 
                BlockOutRoom(room);            
        }

        GenerateMaze();
    }

    // ===========================
    // Getters

    public byte[,] getMap() { return map; }
    public byte getTile(int x, int y) { return map[y, x]; }


    // ===========================
    // BlockOutRooms: takes a room as an argument and marks all tiles contained in it
    // (and a 1 block buffer around it) as out of bounds for the maze gen algorithm.

    public void BlockOutRoom(Room room) {
        // get room position to use as offset for blockedTiles array
        int offsetX = room.getX();
        int offsetY = room.getY();

        // iterate across all tiles in rooms and block them for the maze gen
        // go 1 index over in all directions to preserve space around room

        for (int y = -1; y < room.getHeight() + 1; y++) {
            for (int x = -1; x < room.getWidth() + 1; x++)
                blockedTiles[y+offsetY, x+offsetX] = true;
        }
        return;
    }


    // ===========================
    // GenerateMaze fills the map with a maze. Tiles that are flagged true at
    // the corrosponding indices in the blockedTiles array ignored by the algorithm

    public void GenerateMaze() {

        // iterate over tiles in maze (offset by 1 to leave a boarder)
        for (int y = 1; y < height - 1; y++) {
            for (int x = 1; x < width - 1; x++) {

                // only proceed if this tile is not blocked
                if (!blockedTiles[y,x])
                    map[y, x] = 0x1;
            }
        }
        return;
    }
}