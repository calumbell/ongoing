using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    public int width;
    public int height;

    public GameObject floorPrefab;
    public GameObject floorParent;



    // 
    public GameObject[] wallPrefabs;
    public GameObject wallParent;

    // Start is called before the first frame update
    void Start() {
        byte[,] map = GenerateMapData(width, height);
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
    // GenerateMapData: returns a 2D array of ints that represents a map

    byte[,] GenerateMapData(int width, int height) {
        byte[,] map = new byte[height, width];

        // iterate over 2D array
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                map[y, x] = 0b1;

                if (y == 0 || y+1 == height || x == 0 || x+1 == width) {
                    map[y, x] = (byte) (map[y, x] | 0b10);
                }
            }
        }

        return map;
    }

    // ========================================
    // InstantiateMapData: takes a 2D array of map data and instantiates the
    // prefabs that render it in our scene.

    void InstantiateMapData(byte[,] map, int width, int height) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (map[y, x] > 0){
                    CreateChildPrefab(floorPrefab, floorParent, x, y, 0);
                }

                if (map[y, x] > 2) {
                    // check if we are at the left bound of our map
                    if (x == 0) {

                        // if at bottom of map, create a BottomLeft corner (index 6)
                        if (y == 0) {
                            CreateChildPrefab(wallPrefabs[6], wallParent, x, y, 0);
                        }

                        // if at top of map, create a TopLeft corner (index 0)
                        else if (y+1 == height) {
                            CreateChildPrefab(wallPrefabs[0], wallParent, x, y, 0);
                        }

                        // if not a corner, create a LeftCentre wall (index 3)
                        else {
                            CreateChildPrefab(wallPrefabs[3], wallParent, x, y, 0);
                        }
                    }

                    // check if we are at the right bound of our map
                    else if (x+1 == width) {
                        // if at bottom of map, create a BottomRight corner (index 6)
                        if (y == 0)
                        {
                            CreateChildPrefab(wallPrefabs[8], wallParent, x, y, 0);
                        }

                        // if at top of map, create a TopRight corner (index 0)
                        else if (y + 1 == height)
                        {
                            CreateChildPrefab(wallPrefabs[2], wallParent, x, y, 0);
                        }

                        // if not a corner, create a RightCentre wall (index 3)
                        else
                        {
                            CreateChildPrefab(wallPrefabs[5], wallParent, x, y, 0);
                        }
                    }

                    // if we are at bottom of map, create a BottomCentre wall (index 7)
                    else if (y == 0) {
                        CreateChildPrefab(wallPrefabs[7], wallParent, x, y, 0);
                    }

                    // else, it has to be a TopCentre wall (index 1)
                    else {
                        CreateChildPrefab(wallPrefabs[1], wallParent, x, y, 0);
                    }

                }
            }
        }
    }
}

