using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    public int width;
    public int height;

    public GameObject floorPrefab;
    public GameObject floorParent;

    // Start is called before the first frame update
    void Start() {
        int[,] map = GenerateMapData(width, height);
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

    int[,] GenerateMapData(int width, int height) {
        int[,] map = new int[height, width];

        // iterate over 2D array
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                map[y, x] = 1;
            }
        }

        return map;
    }

    // ========================================
    // InstantiateMapData: takes a 2D array of map data and instantiates the
    // prefabs that render it in our scene.

    void InstantiateMapData(int[,] map, int width, int height) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                CreateChildPrefab(floorPrefab, floorParent, x, y, 0);
            }
        }
    }
}

