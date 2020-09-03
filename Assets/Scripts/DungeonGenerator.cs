using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    public int width;
    public int height;
    public int numberOfRooms;

    Tile[,] tiles;
    Room[] rooms;
    Maze maze;

    Dungeon dungeon;

    public GameObject floorPrefab;
    public GameObject floorParent;

    
    public GameObject[] wallPrefabs;
    public GameObject wallParent;

    // Start is called before the first frame update
    void Start() {
        tiles = GenerateDungeonTiles(width/3, height/3);
        dungeon = new Dungeon(tiles, rooms);
        InstantiateDungeon(dungeon);
    }
    
    void CreateChildPrefab(GameObject prefab, GameObject parent, int x, int y, int z) {
        // CreateChildPrefab: instantiates a prefab and parents it to another game obj
        var myPrefab = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
        myPrefab.transform.parent = parent.transform;
    }

    
    void CreateWallPrefab(GameObject parent, int x, int y, Dungeon d) {

        // select TopLeftCornerObtuse (indx 9) if there are open tiles Lft + Up
        if (d.getByte(x-1, y) == 0x1 & d.getByte(x, y+1) == 0x1) {
            CreateChildPrefab(wallPrefabs[9], wallParent, x, y, 0);
        }
        // select TopRightCornerObtuse (indx 10) if there are open tiles Rght + Up
        else if (d.getByte(x+1, y) == 0x1 & d.getByte(x, y+1) == 0x1)
            CreateChildPrefab(wallPrefabs[10], wallParent, x, y, 0);

        // select BtmLeftCornerObtuse (indx 11) if there are open tiles Lft + Dwm
        else if (d.getByte(x-1, y) == 0x1 & d.getByte(x, y-1) == 0x1)
            CreateChildPrefab(wallPrefabs[11], wallParent, x, y, 0);

        // select BtmRgtCornerObtuse (indx 11) if there are open tiles Rght + Dwm
        else if (d.getByte(x+1, y) == 0x1 & d.getByte(x, y-1) == 0x1)
            CreateChildPrefab(wallPrefabs[12], wallParent, x, y, 0);

        // select LeftCentre wall (indx 3) if there is an open tile to the right
        else if (d.getByte(x+1, y) == 0x1)
            CreateChildPrefab(wallPrefabs[3], wallParent, x, y, 0);

        // select RightCentre wall (indx 5) if there is an open tiles to the left
        else if (d.getByte(x-1, y) == 0x1)
            CreateChildPrefab(wallPrefabs[5], wallParent, x, y, 0);

        // select TopCentre wall (indx 1) if there is an open tile below
        else if (d.getByte(x, y-1) == 0x1)
            CreateChildPrefab(wallPrefabs[1], wallParent, x, y, 0);

        // select BottomCentre wall (index 7) if there is an open tile above
        else if (d.getByte(x, y+1) == 0x1)
                CreateChildPrefab(wallPrefabs[7], wallParent, x, y, 0);

        // select TopLeft wall (index 0) if there is an open tile to SE
        else if (d.getByte(x+1, y-1) == 0x1)            
            CreateChildPrefab(wallPrefabs[0], wallParent, x, y, 0);

        // select TopRight wall (index 2) if there is an open tile to SW
        else if (d.getByte(x-1, y-1) == 0x1)
            CreateChildPrefab(wallPrefabs[2], wallParent, x, y, 0);

        // select BottomLeft wall (index 6) if there is an open tile to NE
        else if (d.getByte(x+1, y+1) == 0x1)
            CreateChildPrefab(wallPrefabs[6], wallParent, x, y, 0);

        // select BottomRight wall (index 8) if there is an open tile NW
        else if (d.getByte(x-1, y+1) == 0x1)
            CreateChildPrefab(wallPrefabs[8], wallParent, x, y, 0);

    }

    void AddMazeToTiles(Maze maze, Tile[,] tiles) {
        if (maze == null)
            return;

        // get map dims
        int mapHeight = tiles.GetLength(0);
        int mapWidth = tiles.GetLength(1);

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                    tiles[y, x] = maze.getTile(x, y);
            }
        }
    }

    void AddRoomToTiles(Room room, Tile[,] tiles) {

    // AddRoomToTiles: adds a room to a map and the coords x & y
        if (room == null) 
            return;

        // get map dims
        int mapHeight = tiles.GetLength(0);
        int mapWidth = tiles.GetLength(1);

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


    Tile[,] GenerateDungeonTiles(int x, int y) {

        // GenerateDungeonTiles: returns a 2D array of tiles
        Tile[,] tiles = new Tile[y, x];

        for (int i = 0; i < y; i++) {
            for (int j = 0; j < x; j++)
                tiles[j, i] = new Tile(i, j, false, 0xF);
        }

        // create rooms
        rooms = GenerateRooms(numberOfRooms, x, y);

        // add rooms to tiles
        foreach (Room room in rooms) {
            AddRoomToTiles(room, tiles);
        }

        maze = new Maze(tiles, rooms);
        AddMazeToTiles(maze, tiles);
        return tiles;
    }

    Room[] GenerateRooms(int n, int boundX, int boundY) {
        Room[] rooms = new Room[n];

        // only allow n+10 attempts to create a room (base-case)
        int attempts = n + 30;

        for (int i = 0; i < n; i++) {

            // instantiate new room
            Room newRoom = new Room(boundX, boundY);
            
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
        
        return rooms;
    }

    void InstantiateDungeon(Dungeon dungeon) {

        int width = dungeon.getWidth();
        int height = dungeon.getHeight();

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // add a floor if 1st bit of mask is 1
                if ((byte)(dungeon.getByte(x,y) & 0x1) > 0)
                    CreateChildPrefab(floorPrefab, floorParent, x, y, 0);

                if ((byte)(dungeon.getByte(x,y) & 0x2) > 0)
                    CreateWallPrefab(wallParent, x, y, dungeon);
            }
        }
    }   
}