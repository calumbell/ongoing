public class Room
{
    private int x;
    private int y;
    private int width;
    private int height;
    private byte[,] map;

    // ===========================
    // Class Constructor
    public Room(int xIn, int yIn, int widthIn, int heightIn)
    {
        x = xIn;
        y = yIn;
        width = widthIn;
        height = heightIn;
        map = GenerateRoomMap(width, height);
    }

    // ===========================
    // Getters

    public int getX() { return x; }
    public int getY() { return y; }
    public int getWidth() { return width; }
    public int getHeight() { return height; }
    public byte getTile(int x, int y) { return map[y, x];  }

    // ===========================
    // AABBCollisionDetection returns turn if the two rects (+1 boarder tile) described by the
    // arguments intersect, else returns false

    private bool AABBCollisionDetection(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2) {
        // Add one to each width to make sure that each box has a boarder
        if (x1 < x2 + w2 + 1 &&
            x1 + w1 + 1 > x2 &&
            y1 < y2 + h2 + 1 &&
            y1 + h1 + 1 > y2) {
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

    // ===========================
    // ContainsTile returns true if the coordinates passed to the function
    // are contained in the parent room, else returns false
    // ==

    public bool ContainsTile(int otherX, int otherY) {
        // run AABB on a rect parent & rect with dims 1x1 to represent a single tile
        return AABBCollisionDetection(x, y, width, height,
            otherX, otherY, 1, 1);
    }

    // ===========================
    // GenerateRoomMap()
    // ==

    byte[,] GenerateRoomMap(int width, int height) {
        byte[,] room = new byte[height, width];

        // iterate over 2D array
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // add a floor underneath each tile
                room[y, x] = 0x1;

                // add walls around the edges of room
                if (y == 0 || y + 1 == height || x == 0 || x + 1 == width) {
                    room[y, x] = (byte)(room[y, x] | 0x2);
                }
            }
        }

        return room;
    }


}
