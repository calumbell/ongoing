using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    public int width;
    public int height;

    Tile[,] tiles;
    byte[,] map;
    Room[] rooms;
    Maze maze;

    public GameObject floorPrefab;
    public GameObject floorParent;

    
    public GameObject[] wallPrefabs;
    public GameObject wallParent;

    // Start is called before the first frame update
    void Start() {
        tiles = GenerateDungeonTiles(width/3, height/3);
        map = GenerateMapFromTiles();
        InstantiateMapData(map);
    }
    
    void CreateChildPrefab(GameObject prefab, GameObject parent, int x, int y, int z) {
        // CreateChildPrefab: instantiates a prefab and parents it to another game obj
        var myPrefab = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
        myPrefab.transform.parent = parent.transform;
    }

    
    void CreateWallPrefab(GameObject parent, int x, int y) {
        // How does this work? The map is divided into 4x4 grid (3x3 tiles with
        // a 1 tile border). By using factorials we can figure out the position
        // of a wall piece relative to the open space that flanks

        // select LeftCentre wall (indx 3) if there is an open tile to the right
        if (map[y,x+1] == 0x1)
            CreateChildPrefab(wallPrefabs[3], wallParent, x, y, 0);

        // select RightCentre wall (indx 5) if there is an open tiles to the left
        else if (map[y, x-1] == 0x1)
            CreateChildPrefab(wallPrefabs[5], wallParent, x, y, 0);

        // select TopCentre wall (indx 1) if there is an open tile below
        else if (map[y-1,x] == 0x1)
            CreateChildPrefab(wallPrefabs[1], wallParent, x, y, 0);

        // select BottomCentre wall (index 7) if there is an open tile above
        else if (map[y+1, x] == 0x1)
                CreateChildPrefab(wallPrefabs[7], wallParent, x, y, 0);

        // select TopLeft wall (index 0) if there is an open tile to SE
        else if (map[y-1, x+1] == 0x1)            
            CreateChildPrefab(wallPrefabs[0], wallParent, x, y, 0);

        // select TopRight wall (index 2) if there is an open tile to SW
        else if (map[y-1, x-1] == 0x1)
            CreateChildPrefab(wallPrefabs[2], wallParent, x, y, 0);

        // select BottomLeft wall (index 6) if there is an open tile to NE
        else if (map[y+1,x+1] == 0x1)
            CreateChildPrefab(wallPrefabs[6], wallParent, x, y, 0);

        // select BottomRight wall (index 8) if there is an open tile NW
        else if (map[y+1, x-1] == 0x1)
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
        rooms = GenerateRooms(5, x, y);

        // add rooms to tiles
        foreach (Room room in rooms) {
            AddRoomToTiles(room, tiles);
        }

        maze = new Maze(tiles, rooms);
        AddMazeToTiles(maze, tiles);
        return tiles;
    }

    byte[,] GenerateMapFromTiles() {
        // get array dims
        int tileX = tiles.GetLength(1);
        int tileY = tiles.GetLength(0);
        int mapX = 4 * tileX + 1;
        int mapY = 4 * tileY + 1;

        // create new map of level
        byte[,] map = new byte[mapY, mapX];

        byte[,] tileMap;

        // iterate for all of our tiles
        for (int y = 0; y < tileY; y++) {
            for (int x = 0; x < tileX; x++) {

                // if the current tile isn't null, convert to a 3x3 byte array
                if (tiles[y, x] != null) {
                    tileMap = tiles[y, x].getMap();

                    // iterate over the byte array and add each on to the map
                    for (int i = 0; i < 3; i++) {
                        for (int j = 0; j < 3; j++) {
                          map[4 * y + i + 1, 4 * x + j + 1] =
                                (byte)(map[(4*y)+i+1, (4*x)+j+1] | tileMap[i, j]);
                        }
                    }
                }
            }
        }


        // fill in rows between tiles, connect everything up nicely
        for (int y = 4; y < map.GetLength(1) - 1; y += 4) {
            for (int x = 1; x < map.GetLength(0) -1; x++) {

                // if there are two floors either side of a space, connect them
                if (map[y - 1, x] == 0x1 & map[y + 1, x] == 0x1)
                    map[y, x] = (byte)(map[y,x] | 0x1);

                // if there is a wall N+S AND either floor NE+SE OR floor NW+SW
                // then this is the edge of a vertical passage, make a wall.

                else if (map[y - 1, x] == 0x3 & map[y + 1, x] == 0x3
                    & ((map[y - 1, x + 1] == 0x1 & map[y + 1, x + 1] == 0x1)
                    | ((map[y - 1, x - 1] == 0x1 & map[y + 1, x - 1] == 0x1))))
                    
                    map[y, x] = (byte)(map[y, x] | (byte)0x3);

                else if (map[y, x-1] == 0x1)
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
        
        // once we have converted our tiles to a map, fill in maze fine detail
        map = Maze.GenerateMazeFine(map);

        return map;
    }

    Room[] GenerateRooms(int n, int boundX, int boundY) {
        Room[] rooms = new Room[n];

        // only allow n+10 attempts to create a room (base-case)
        int attempts = n + 10;

        for (int i = 0; i < n; i++) {

            // instantiate new room
            Room newRoom = new Room(boundX, boundY);
            
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
            else 
                i--;

            if (attempts <= 0)
                break;

            else
                attempts--;          
        }       
        
        return rooms;
    }

    void InstantiateMapData(byte[,] map) {
    // InstantiateMapData: takes a 2D array of map data and instantiates the
    // prefabs that render it in our scene.
        int width = map.GetLength(1);
        int height = map.GetLength(0);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                // add a floor if 1st bit of mask is 1
                if ((byte)(map[y, x] & 0x1) > 0)
                    CreateChildPrefab(floorPrefab, floorParent, x, y, 0);
                
                if ((byte)(map[y, x] & 0x2) > 0) 
                    CreateWallPrefab(wallParent, x, y);
                
            }
        }
    }
}