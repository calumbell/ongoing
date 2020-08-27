using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    public int width;
    public int height;

    byte[,] map;
    Room[] rooms;
    Maze maze;

    public GameObject floorPrefab;
    public GameObject floorParent;

    // 
    public GameObject[] wallPrefabs;
    public GameObject wallParent;

    // Start is called before the first frame update
    void Start() {
        map = GenerateMapData(width, height);
        InstantiateMapData(map, width, height);
    }

    // ========================================
    // CreateChildPrefab: instantiates a prefab and parents it to another game obj
    void CreateChildPrefab(GameObject prefab, GameObject parent, int x, int y, int z) {
        var myPrefab = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
        myPrefab.transform.parent = parent.transform;
    }

    // ========================================
    // CreateWallPrefab: instantiates the correct wall prefab based on room geometry
    void CreateWallPrefab(GameObject parent, int x, int y) {
        // if there is floor and no wall below this tile
        if (!((map[y-1, x] & 0x2) > 0) & ((map[y-1, x] & 0x1) > 0)) {
            // select a TopCentre wall piece (index 1)
            CreateChildPrefab(wallPrefabs[1], wallParent, x, y, 0);
        }

        // if there is floor and no wall above this tile
        else if (!((map[y+1, x] & 0x2) > 0) & ((map[y+1, x] & 0x1) > 0)) {
            // select a BottomCentre wall piece (index 7)
            CreateChildPrefab(wallPrefabs[7], wallParent, x, y, 0);
        }

        // if there is floor and no wall to the right of this tile
        else if (!((map[y, x+1] & 0x2) > 0) & ((map[y, x+1] & 0x1) > 0)) {
            // select a LeftCentre wall piece (index 3)
            CreateChildPrefab(wallPrefabs[3], wallParent, x, y, 0);
        }


        // if there is floor and no wall to the left of this tile
        else if (!((map[y, x-1] & 0x2) > 0) & ((map[y, x-1] & 0x1) > 0)) {
            // select a RightCentre wall piece (index 5)
            CreateChildPrefab(wallPrefabs[5], wallParent, x, y, 0);
        }

        // if there is floor and no wall up-right of tile
        else if (!((map[y+1, x+1] & 0x2) > 0) & ((map[y+1, x+1] & 0x1) > 0)) {
            // select a BottomLeft wall piece (index 6)
            CreateChildPrefab(wallPrefabs[6], wallParent, x, y, 0);
        }

        // if there is floor and no wall up-left of tile
        else if (!((map[y+1, x-1] & 0x2) > 0) & ((map[y+1, x-1] & 0x1) > 0)) {
            // select a BottomRight wall piece (index 8)
            CreateChildPrefab(wallPrefabs[8], wallParent, x, y, 0);
        }

        // if there is floor and no wall down-right of tile
        else if (!((map[y-1, x+1] & 0x2) > 0) & ((map[y-1, x+1] & 0x1) > 0)) {
            // select a TopLeft wall piece (index 0)
            CreateChildPrefab(wallPrefabs[0], wallParent, x, y, 0);
        }

        // if there is floor and no wall down-left of tile
        else if (!((map[y-1, x-1] & 0x2) > 0) & ((map[y-1, x-1 ] & 0x1) > 0))
        {
            // select a TopRight wall piece (index 2)
            CreateChildPrefab(wallPrefabs[2], wallParent, x, y, 0);
        }
    }


    // ========================================
    // AddRoomToMap: adds a room to a map and the coords x & y
    void AddRoomToMap(Room room, byte[,] map) {
        if (room == null) {
            return;
        }

        // get map dims
        int mapHeight = map.GetLength(0);
        int mapWidth = map.GetLength(0);

        // get room dims & coords
        int offsetX = room.getX();
        int offsetY = room.getY();
        int roomWidth = room.getWidth();
        int roomHeight = room.getHeight();

        // iterate across room & map
        for (int y = 0; y < roomHeight; y++) {
            for (int x = 0; x < roomWidth; x++) {
                // copy room data into map at an offset
                map[y + offsetY, x + offsetX] = room.getTile(x, y);
            }
        }
    }

    // ========================================
    // GenerateMapData: returns a 2D array of bytes that represents a map

    byte[,] GenerateMapData(int width, int height) {
        byte[,] map = new byte[height, width];

        // create rooms and add them to map
        rooms = GenerateRooms(6, width, height);
        maze = new Maze(width, height, rooms);

        map = maze.getMap();

        foreach (Room room in rooms) {
            AddRoomToMap(room, map);
        }
        return map;
    }

    Room[] GenerateRooms(int n, int boundX, int boundY) {
        Room[] rooms = new Room[n];

        // only allow n+10 attempts to create a room (base-case)
        int attempts = n + 10;

        for (int i = 0; i < n; i++) {

            // instantiate new room
            Room newRoom = new Room(width, height);
            
            // check for collisions between new and existing rooms
            bool roomCollisionFlag = false;
            foreach(Room room in rooms) {

                // make sure array index is not null to avoid seg. faults
                if (room != null) {                    
                    if (room.CollidesWithRoom(newRoom)) {
                        roomCollisionFlag = true;
                    }
                }    
            }

            // if no collisions, add newRoom to rooms array
            if (!roomCollisionFlag) {
                rooms[i] = newRoom;
            }

            // else, decrement iterator so that the loop repeats
            // possible bug: if geometries of rooms do not allow another room to be
            // added, we have an infinity loop - add some kind of base-case?

            else {
                i--;
            }

            if (attempts <= 0) {
                break;
            }

            else {
                attempts--;
            }
           
        }       
        
        return rooms;
    }

    // ========================================
    // InstantiateMapData: takes a 2D array of map data and instantiates the
    // prefabs that render it in our scene.

    void InstantiateMapData(byte[,] map, int width, int height) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                // add a floor if 1st bit of mask is 1
                if ((map[y, x] & 0x1) > 0){
                    CreateChildPrefab(floorPrefab, floorParent, x, y, 0);
                }
                
                if ((map[y, x] & 0x2) > 0) {
                    CreateWallPrefab(wallParent, x, y);
                }
            }
        }
    }
}