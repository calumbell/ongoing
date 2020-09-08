using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap {

    /* The TileMap class is responsible for handling the the representation of
     * a dungeon as a series of Tiles (see the Tile class). This representation
     * is a little more abstract than the ByteMap (each tile represents a 4x4 
     * area of the ByteMap) and it is easier to generate the level geometries
     * (rooms, mazes, etc) at this level of detail.
     * 
     * TileMaps will contain the Room, Maze and Tile classes, and will be contained
     * by the Dungeon class.
    */

    private Tile[,] tiles;
    private int width, height;

    private Room[] rooms;
    private Maze maze;


    // =========
    // Getters

    public int getHeight() { return height; }
    public Tile getTile(int x, int y) { return tiles[y, x]; }
    public Tile[,] getTiles() { return tiles; }
    public Room getRoom(int i) { return rooms[i]; }
    public Room[] getRooms() { return rooms; }
    public int getWidth() { return width; }



    // =========
    // Class Constructor

    public TileMap(int inputWidth, int inputHeight) {
        // set tileMap dimensions
        width = inputWidth;
        height = inputHeight;

        // instantiate tileMap as a 2D array of tiles
        tiles = new Tile[height, width];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++)
                tiles[y, x] = new Tile(x, y, false, 0xF);
        }

        // Generate rooms and add them to tileMap
        GenerateRooms(20);
        foreach(Room room in rooms)
            AddRoomToTiles(room);

        // Generate a maze and add it to tileMap
        maze = new Maze(tiles, rooms);
        AddMazeToTiles();

        // Add connections between the rooms and the maze
        ConnectRoomsToMaze();

        // Remove some of the dead-ends from the map
        removeDeadEnds(10);
    }

    // =========
    // Private Methods

    private void AddMazeToTiles() {

    /* 
    * AddMazeToTiles 
    * 
    * Iterates over the maze field and add its tile to the tiles field.
    * 
    */

        if (maze == null)
            return;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                    tiles[y, x] = maze.getTile(x, y);
            }
        }
    }



    private void AddRoomToTiles(Room room) {

        /* AddRoomToTiles
         * 
         * Adds the tiles from a room passed in as an arugment to the tiles
         * field. 
         * 
         */

        if (room == null) 
            return;

        // get room dims & coords
        int offsetX = room.getX();
        int offsetY = room.getY();
        int roomWidth = room.getWidth();
        int roomHeight = room.getHeight();

        // iterate across room & map
        for (int y = 0; y < roomHeight; y++) {
            for (int x = 0; x < roomWidth; x++) {
                // copy room data into map at an offset
                tiles[y + offsetY, x + offsetX] = room.getTile(x, y);
            }
        }
    }

    
    void GenerateRooms(int n) {

        /* GenerateRooms(int n)
         * 
         * GenerateRooms attempts the generate n Rooms and adds them to the rooms
         * arrays. Depending on the precise level geometries, you might get fewer
         * than n rooms. The rooms will not intersect, room collisions are tested 
         * before adding a room.
         * 
         */

        rooms = new Room[n];

        // only allow n+30 attempts to create a room (base-case)
        int attempts = n + 30;

        for (int i = 0; i < n; i++) {

            // instantiate new room
            Room newRoom = new Room(width, height);
            
            // check for collisions between new and existing rooms
            bool roomCollisionFlag = false;
            foreach(Room room in rooms) {
                // make sure array index is not null to avoid seg. faults
                if (room != null) {                    
                    if (room.CollidesWithRoom(newRoom))
                        roomCollisionFlag = true;
                }
            }

            // if no collisions, add newRoom to rooms array
            if (!roomCollisionFlag)
                rooms[i] = newRoom;

            // else, decrement iterator so that the loop repeats
            // possible bug: if geometries of rooms do not allow another room to be
            // added, we have an infinity loop - add some kind of base-case?
            else 
                i--;

            if (attempts <= 0)
                break;

            else
                attempts--;          
        }       
    }

    private void ConnectRoomsToMaze() {

        /* ConnectRoomsToMaze
         * 
         * Iterates over all of the rooms in the TileMap and adds some connections
         * between them and the maze that surrounds them. Without this method,
         * there would be no way to get from the rooms to the maze.
         * 
         */

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

                if (edge == 0x1 & (roomY + roomHeight) < height
                    & (0x1 & edgeBitmask) == 0) {

                    edgeBitmask = (byte)(edgeBitmask | 0x1);

                    x = Random.Range(roomX, roomX + roomWidth);
                    y = roomY + roomHeight - 1;

                    if (tiles[y + 1, x].isOpen()) {
                        tiles[y + 1, x].openWallOnSides(0x4);
                        tiles[y, x].openWallOnSides(0x1);
                    }
                }

                else if (edge == 0x4 & roomY > 0 & (0x4 & edgeBitmask) == 0) {
                    x = Random.Range(roomX, roomX + roomWidth);
                    y = roomY;

                    edgeBitmask = (byte)(edgeBitmask | 0x4);

                    if (tiles[y - 1, x].isOpen()) {
                        tiles[y - 1, x].openWallOnSides(0x1);
                        tiles[y, x].openWallOnSides(0x4);
                    }
                }

                else if (edge == 0x2 & (roomX + roomWidth) < height & (0x2 & edgeBitmask) == 0) {

                    edgeBitmask = (byte)(edgeBitmask | 0x2);

                    x = roomX + roomWidth - 1;
                    y = Random.Range(roomY, roomY + roomHeight);

                    if (tiles[y, x + 1].isOpen()) {
                        tiles[y, x + 1].openWallOnSides(0x8);
                        tiles[y, x].openWallOnSides(0x2);
                    }
                }

                else if (edge == 0x8 & roomX > 0 & (0x8 & edgeBitmask) == 0) {
                    edgeBitmask = (byte)(edgeBitmask | 0x8);

                    x = roomX;
                    y = Random.Range(roomY, roomY + roomHeight);

                    if (tiles[y, x - 1].isOpen()) {
                        tiles[y, x - 1].openWallOnSides(0x2);
                        tiles[y, x].openWallOnSides(0x8);
                    }
                }

                else
                    i--;
            }
        }
    }

    
    private void removeDeadEnds(int n) {

        /*
         * removeDeadEnds(int n)
         * 
         * Locates dead-ends in the maze and recursively removes them by 
         * checking for new adjacent dead-ends once one is removed. Without this
         * method, the dungeon would have loads of passages leading nowhere. 
         *
         */

        // create a list of tiles to store dead ends
        var deadEnds = new List<Tile>();

        // find all dead ends in maze and add them to list
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                // if tile has 3 bounding walls & isn't blocked, its a dead end
                if (tiles[y, x].getNumWalls() == 3 & tiles[y,x].isOpen())
                    deadEnds.Add(tiles[y, x]);

        // remove dead-ends and check if adjacent tiles are now dead ends

        Tile tile, next;

        // n controls depth of recursion, iterate that many times
        for (int j = 0; j < n; j++) {

            // iterate across all dead-ends in our list
            for (int i = 0; i < deadEnds.Count; i++) {

                // mark the tile as closed
                tile = deadEnds[i];
                tile.setClosed();

                // if the closed tile had a wall going up, close the wall going
                // down from the tile above - store the next tile
                if ((tile.getWalls() & 0x1) == 0 & tile.getY() < height - 1) {
                    next = tiles[tile.getY() + 1, tile.getX()];
                    next.closeWallOnSides(0x4);
                }

                // if the closed tile had a wall going to the right, close the 
                // wall going left from the tile to the right - store the next tile
                else if ((tile.getWalls() & 0x2) == 0 & tile.getX() < width - 1) {
                    next = tiles[tile.getY(), tile.getX() + 1];
                    next.closeWallOnSides(0x8);
                }

                // if the closed tile had a wall going down, close the wall 
                // going up from the tile below - store the next tile
                else if ((tile.getWalls() & 0x4) == 0 & tile.getY() > 0) {
                    next = tiles[tile.getY()-1, tile.getX()];
                    next.closeWallOnSides(0x1);
                }

                // if the closed tile had a wall going to the right, close the 
                // wall going left from the tile to the right - store the next tile
                else {
                    next = tiles[tile.getY(), tile.getX() - 1];
                    next.closeWallOnSides(0x2);
                }

                // if the next tile is now a dead-end, replace the closed tile
                // in our dead-ends list with it
                if (next.getNumWalls() == 3)
                    deadEnds[i] = next;

                // if the next tile isn't a dead-end, remove the current index
                else
                    deadEnds.RemoveAt(i--);
            }
        }
    }

}
