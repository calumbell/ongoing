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
        GenerateRooms(10);
        foreach(Room room in rooms)
            AddRoomToTiles(room);

        // Generate a maze and add it to tileMap
        maze = new Maze(tiles, rooms);
        AddMazeToTiles();
    }


    // =========
    // Getters

    public Tile[,] getTiles() { return tiles; }
    public Room[] getRooms() { return rooms; }


    void AddMazeToTiles() {
    /* AddMazeToTiles iterates over the maze field and add its tile to the tiles
    * field */

        if (maze == null)
            return;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                    tiles[y, x] = maze.getTile(x, y);
            }
        }
    }

    void AddRoomToTiles(Room room) {
    /* AddRoomToTiles adds the tiles from a room passed in as an arugment to
     * the tiles field */

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
}
