using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    public int width;
    public int height;

    byte[,] map;

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

    // Update is called once per frame
    void Update() {
        
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

    void AddRoomToMap(byte[,] room, byte[,] map, int x, int y) {

        if (x < 1 || y < 1) {
            return;
        }

        // get dims of room & map
        int roomHeight = room.GetLength(0);
        int roomWidth = room.GetLength(1);
        int mapHeight = map.GetLength(0);
        int mapWidth = map.GetLength(1);

        if (x + roomWidth  > mapWidth || y + roomHeight > mapHeight) {
            return;
        }

        for (int i = 0; i < roomHeight; i++) {
            for (int j = 0; j < roomWidth; j++) {
                map[y+i, x+j] = room[i, j];
            }
        }
    }


    // ========================================
    // GenerateMapData: returns a 2D array of bytes that represents a map

    byte[,] GenerateMapData(int width, int height) {
        byte[,] map = new byte[height, width];
        byte[,] room = GenerateRoom(10, 6);
        AddRoomToMap(room, map, 6, 2);

        room = GenerateRoom(8, 8);
        AddRoomToMap(room, map, 2, 11);

        return map;
    }

    // ========================================
    // GenerateRoom: returns a 2D array of bytes representing a room

    byte[,] GenerateRoom(int width, int height)
    {
        byte[,] room = new byte[height, width];

        // iterate over 2D array
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // add a floor underneath each tile
                room[y, x] = 0b1;

                // add walls around the edges of room
                if (y == 0 || y + 1 == height || x == 0 || x + 1 == width) {
                    room[y, x] = (byte)(room[y, x] | 0x2);
                }
            }
        }

        return room;
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

