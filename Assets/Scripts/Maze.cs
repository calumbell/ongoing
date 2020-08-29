public class Maze
{
    private int width;
    private int height;
    private Tile[,] tiles;
    private byte[,] map;
    private bool[,] blockedTiles;

    // ===========================
    // Class Constructors

    // instantiate w/ rooms array to generate the maze
    public Maze(int widthIn, int heightIn, Room[] rooms) {
        width = widthIn;
        height = heightIn;
        tiles = new Tile[height, width];
        blockedTiles = new bool[height, width];

        foreach (Room room in rooms) {
            if (room != null) 
                BlockOutRoom(room);            
        }

        GenerateMazeCourse();
    }

    // ===========================
    // Getters

    public Tile[,] getTiles() { return tiles; }
    public Tile getTile(int x, int y) { return tiles[y, x]; }


    // ===========================
    // BlockOutRooms: takes a room as an argument and marks all tiles contained
    // within as out of bounds for the maze gen. algorithm

    public void BlockOutRoom(Room room) {
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


    // ===========================
    // GenerateMazeCourse fills empty spaces in the tiles array with a maze
    public void GenerateMazeCourse() {    
        return;
    }


    // ===========================
    // GenerateMazeFine fills empty spaces in the map with corridors, esp.
    // corridors between rooms which GenerateMazeCourse ignores

    public static byte[,] GenerateMazeFine(byte[,] map) {
        return map;
    }
}
