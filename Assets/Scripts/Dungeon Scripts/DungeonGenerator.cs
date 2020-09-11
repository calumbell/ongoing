using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public Dungeon dungeon;

    public GameObject dungeonPrefab;
    private GameObject currentDungeon;
    public GameObject floorPrefab;
    private GameObject floorParent;

    private GameObject objectsParent;

    public GameObject[] wallPrefabs;
    public GameObject wallParent;
    public GameObject stairsDownPrefab;


    public GameObject playerPrefab;

    void Start()
    {
        CreateNewDungeon();

        // pick a random room and teleport the player there
        Instantiate(playerPrefab, new Vector3(width/2, height/2, 0), Quaternion.identity);
        
    }
    
    void CreateChildPrefab(GameObject prefab, GameObject parent, int x, int y, int z)
    {
        // CreateChildPrefab: instantiates a prefab and parents it to another game obj
        var myPrefab = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
        myPrefab.transform.parent = parent.transform;
    }

    void CreateNewDungeon()
    {
        currentDungeon = Instantiate(dungeonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        floorParent = currentDungeon.transform.GetChild(0).gameObject;
        wallParent = currentDungeon.transform.GetChild(1).gameObject;
        objectsParent = currentDungeon.transform.GetChild(2).gameObject;

        dungeon = new Dungeon(width / 3, height / 3);
        InstantiateDungeonFeatures(dungeon);
    }

    
    void CreateWallPrefab(GameObject parent, int x, int y, Dungeon d)
    {
        // select TopLeftCornerObtuse (indx 9) if there are open tiles Lft + Up
        if (d.getByte(x-1, y) == 0x1 & d.getByte(x, y+1) == 0x1)
            CreateChildPrefab(wallPrefabs[9], wallParent, x, y, 0);

        // select TopRightCornerObtuse (indx 10) if there are open tiles Rght + Up
        else if (d.getByte(x+1, y) == 0x1 & d.getByte(x, y+1) == 0x1)
            CreateChildPrefab(wallPrefabs[10], wallParent, x, y, 0);

        // select BtmLeftCornerObtuse (indx 11) if there are open tiles Lft + Dwm
        else if (d.getByte(x-1, y) == 0x1 & d.getByte(x, y-1) == 0x1)
            CreateChildPrefab(wallPrefabs[11], wallParent, x, y, 0);

        // select BtmRgtCornerObtuse (indx 11) if there are open tiles Rght + Dwm
        else if (d.getByte(x+1, y) == 0x1 & d.getByte(x, y-1) == 0x1)
            CreateChildPrefab(wallPrefabs[12], wallParent, x, y, 0);

        // select LeftCentre wall (indx 3) if there is an open tile to the right
        else if (d.getByte(x+1, y) == 0x1)
            CreateChildPrefab(wallPrefabs[3], wallParent, x, y, 0);

        // select RightCentre wall (indx 5) if there is an open tiles to the left
        else if (d.getByte(x-1, y) == 0x1)
            CreateChildPrefab(wallPrefabs[5], wallParent, x, y, 0);

        // select TopCentre wall (indx 1) if there is an open tile below
        else if (d.getByte(x, y-1) == 0x1)
            CreateChildPrefab(wallPrefabs[1], wallParent, x, y, 0);

        // select BottomCentre wall (index 7) if there is an open tile above
        else if (d.getByte(x, y+1) == 0x1)
                CreateChildPrefab(wallPrefabs[7], wallParent, x, y, 0);

        // select TopLeft wall (index 0) if there is an open tile to SE
        else if (d.getByte(x+1, y-1) == 0x1)            
            CreateChildPrefab(wallPrefabs[0], wallParent, x, y, 0);

        // select TopRight wall (index 2) if there is an open tile to SW
        else if (d.getByte(x-1, y-1) == 0x1)
            CreateChildPrefab(wallPrefabs[2], wallParent, x, y, 0);

        // select BottomLeft wall (index 6) if there is an open tile to NE
        else if (d.getByte(x+1, y+1) == 0x1)
            CreateChildPrefab(wallPrefabs[6], wallParent, x, y, 0);

        // select BottomRight wall (index 8) if there is an open tile NW
        else if (d.getByte(x-1, y+1) == 0x1)
            CreateChildPrefab(wallPrefabs[8], wallParent, x, y, 0);

    }

    void InstantiateDungeonFeatures(Dungeon dungeon)
    {
        int width = dungeon.getWidth();
        int height = dungeon.getHeight();

        for (int y = 0; y < height; y++) 
            for (int x = 0; x < width; x++) {
                // add a floor if 1st bit of mask is 1
                if ((byte)(dungeon.getByte(x,y) & 0x1) > 0)
                    CreateChildPrefab(floorPrefab, floorParent, x, y, 0);

                // add a wall if 2nd bit is a 1
                if ((byte)(dungeon.getByte(x,y) & 0x2) > 0)
                    CreateWallPrefab(wallParent, x, y, dungeon);

                // add a staircase if 3rd bit is a 1
                if ((byte)(dungeon.getByte(x, y) & 0x4) > 0)
                    CreateChildPrefab(stairsDownPrefab, objectsParent, x, y, 0);
            }
    }

    
    // -= Events =- 

    public void OnStairsInteractEventReceived()
    {
        Debug.Log("What a wonderful set of stairs!");
    }
}