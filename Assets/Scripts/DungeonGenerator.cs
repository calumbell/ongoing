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

        // select TopLeftCornerObtuse (indx 9) if there are open tiles Lft + Up
        if (map[y,x-1] == 0x1 & map[y+1,x] == 0x1) {
            CreateChildPrefab(wallPrefabs[9], wallParent, x, y, 0);
        }
        // select TopRightCornerObtuse (indx 10) if there are open tiles Rght + Up
        else if (map[y,x+1] == 0x1 & map[y+1,x] == 0x1)
            CreateChildPrefab(wallPrefabs[10], wallParent, x, y, 0);

        // select BtmLeftCornerObtuse (indx 11) if there are open tiles Lft + Dwm
        else if (map[y,x-1] == 0x1 & map[y-1,x] == 0x1)
            CreateChildPrefab(wallPrefabs[11], wallParent, x, y, 0);

        // select BtmRgtCornerObtuse (indx 11) if there are open tiles Rght + Dwm
        else if (map[y, x+1] == 0x1 & map[y-1, x] == 0x1)
            CreateChildPrefab(wallPrefabs[12], wallParent, x, y, 0);

        // select LeftCentre wall (indx 3) if there is an open tile to the right
        else if (map[y,x+1] == 0x1)
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
                                (byte)(map[(4*y)+i  +1, (4*x)+j+1] | tileMap[i, j]);
                        }
                    }

                    /*
                     * this next section handles literal corner cases. its func.
                     * is to generate walls for the 270º corners that occur in
                     * the maze (but not in the room). These come in 3 flavours
                     * 
                     * when a corridor turns (2 tile connections)
                     * T-junctions (3 tile connections)
                     * 4-way junctions (4 tile connections)
                    */

                    // check whether a tile has two or more bounding walls

                    if (((tileMap[1,0] & 0x2) + (tileMap[1,2] & 0x2) +
                        (tileMap[2,1] & 0x2) + (tileMap[0,1] & 0x2)) == 0x4){

                        
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
                        
                        if (y < tileY-1) {
                            // if tile above has a wall to the right, but current
                            // tile doesn't, add a wall to Top-R corner

                            if ((tiles[y, x].getWalls() & 0x2) == 0
                                & (tiles[y, x].getWalls() & 0x1) == 0
                                & (tiles[y + 1, x].getWalls() & 0x2) > 0) {
                                // (+3, +3) offset get the indx at Top-R of tile
                                map[4 * y + 3, 4 * x + 3] = 0x3;
                                // Debug.Log("x : " + (4 * x + 1) + " y: " + (4 * y + 1));
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


                        if (x < tileX - 1) {

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


                    // check whether a tile has one bounding walls
                    if (((tileMap[1, 0] & 0x2) + (tileMap[1, 2] & 0x2) +
                        (tileMap[2, 1] & 0x2) + (tileMap[0, 1] & 0x2)) == 0x2) {

                        // determine whether tile is a T-junction or room wall
                        // by summing the number of walls each connected adjacent
                        // tile has. room tiles will have 4 maximum.

                        int numWallsInAdjTiles = 0;
                        if ((tiles[y, x].getWalls() & 0x1) == 0)
                            numWallsInAdjTiles += tiles[y + 1, x].getNumWalls();

                        if ((tiles[y,x].getWalls() & 0x2) == 0)
                            numWallsInAdjTiles += tiles[y, x + 1].getNumWalls();

                        if ((tiles[y, x].getWalls() & 0x4) == 0)
                            numWallsInAdjTiles += tiles[y - 1, x].getNumWalls();

                        if ((tiles[y, x].getWalls() & 0x8) == 0)
                            numWallsInAdjTiles += tiles[y, x - 1].getNumWalls();

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
                            else if ((tiles[y, x].getWalls() & 0x4) > 0)
                            {
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