﻿using UnityEngine;

public class Room {

    // ======================
    // Member Fields

    // position of room within the dungeon
    private int x, y;

    // dimensions of the room
    private int width, height;

    // the tiles that make up the room
    private Tile[,] tiles;

    // ===========================
    // Class Constructor

    public Room(int roomX, int roomY) {
        width = Random.Range(3, 7); // nb, room dims include a 1-tile buffer
        height = Random.Range(3, 7);
        x = Random.Range(0, roomX - width - 1);
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
    // Instance Methods

    private bool AABBCollisionDetection(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2) {
    // AABBCollisionDetection returns turn if the two rects described by the
    // arguments intersect, else returns false
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


    public bool CollidesWithRoom(Room otherRoom) {
    // takes a second room as an argument and runners AABB collision detection on
    // it and the current box. returns true if they collide, else returns false

        return AABBCollisionDetection(x, y, width+1, height+1,
            otherRoom.getX(), otherRoom.getY(), otherRoom.getWidth()+1, otherRoom.getHeight()+1);
    }

    void InitRoom(int width, int height) {
    // sets up the room, creates a set of empty tiles with a perimeter wall

        // iterate over 2D array
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                byte walls = 0x0;

                // if tile is at an edge, create a wall at that edge
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
