using UnityEngine;

public class Room
{
    private int x;
    private int y;
    private int width;
    private int height;
    private Tile[,] tiles;

    // ===========================
    // Class Constructor

    // pass room dims to generate a random room with odd params
    public Room(int roomX, int roomY) {
        width = Random.Range(4, 8); // includes a 1-tile buffer around room
        height = Random.Range(4, 8);
        x = Random.Range(0, roomX - width - 1); // -1 for 0-indexing
        y = Random.Range(0, roomY - height - 1);
        tiles = new Tile[height, width];
        InitRoom(width, height);
    }

    // ===========================
    // Getters

    public int getX() { return x; }
    public int getY() { return y; }
    public int getWidth() { return width; }
    public int getHeight() { return height; }
    public Tile getTile(int x, int y) { return tiles[y, x]; }
    public Tile[,] getTiles() { return tiles; }

    // ===========================
    // AABBCollisionDetection returns turn if the two rects (+1 boarder tile) described by the
    // arguments intersect, else returns false

    private bool AABBCollisionDetection(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2) {
        // Add one to each width to make sure that each box has a boarder
        if (x1 < x2 + w2 &&
            x1 + w1 > x2 &&
            y1 < y2 + h2 &&
            y1 + h1 > y2) {
            return true;
        }

        else {
            return false;
        }
    }

    // ===========================
    // CollidesWithRoom takes a second room as an argument and returns true if
    // the two rooms overlap. otherwise it returns false

    public bool CollidesWithRoom(Room otherRoom) {
        return AABBCollisionDetection(x, y, width, height,
            otherRoom.getX(), otherRoom.getY(), otherRoom.getWidth(), otherRoom.getHeight());
    }

    void InitRoom(int width, int height) {       
        // iterate over 2D array
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                byte walls = 0x0;
                
                if (y == 0)
                    walls = (byte)(walls | 0x4);
                else if (y == height-1)
                    walls = (byte)(walls | 0x1);
                if (x == 0)
                    walls = (byte)(walls | 0x8);
                else if (x == width-1)
                    walls = (byte)(walls | 0x2);

                // create new tile and add it to tiles array
                tiles[y, x] = new Tile(x, y, true, walls);
            }
        }
    }
}
