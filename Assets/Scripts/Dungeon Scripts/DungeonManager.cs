using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public int width;
    public int height;

    public Dungeon dungeon;
    public IntValue currentFloor;
    public IntEvent onFloorChange;

    private Stack<Dungeon> floorsAbove;
    private Stack<Dungeon> floorsBelow;

    public GameObject dungeonPrefab;
    private GameObject dungeonParent;

    public GameObject floorPrefab;
    private GameObject floorParent;
    
    public GameObject[] wallPrefabs;
    private GameObject wallParent;

    private GameObject objectsParent;
    public GameObject stairsDownPrefab;
    public GameObject stairsUpPrefab;

    private GameObject entitiesParent;
    private EntityManager entityManager;

    public GameObject playerPrefab;
    private GameObject playerInstance;

    // Keep track of this SO to disable/reenable player input during dun. gen.
    public BoolValue inputEnabled;


    void Start()
    {
        inputEnabled.value = false;

        floorsAbove = new Stack<Dungeon>();
        floorsBelow = new Stack<Dungeon>();

        entityManager = gameObject.GetComponent<EntityManager>();

        dungeon = new Dungeon(width / 3, height / 3);
        entityManager.PopulateDungeon(dungeon, 7);

        InstantiateDungeon(dungeon);

        // pick a random room and teleport the player there
        playerInstance = Instantiate(playerPrefab, new Vector3(dungeon.getStartCoordX(), dungeon.getStartCoordY(), -4), Quaternion.identity);

        InstantiateEntitiesInDungeon(dungeon);

        inputEnabled.value = true;
    }
    
    void CreateChildPrefab(GameObject prefab, GameObject parent, int x, int y, int z)
    {
        // CreateChildPrefab: instantiates a prefab and parents it to another game obj
        var myPrefab = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
        myPrefab.transform.parent = parent.transform;
    }

    void InstantiateDungeon(Dungeon d)
    {
        InstantiateDungeonContainers();    
        InstantiateDungeonFeatures(d);
    }

    void InstantiateEntitiesInDungeon(Dungeon d)
    {
        foreach (Entity entity in d.GetEntities())
        { 
            var myPrefab = Instantiate(entity.prefab,
                new Vector3(entity.location.x, entity.location.y, -1),
                Quaternion.identity);
            myPrefab.transform.parent = entitiesParent.transform;

            myPrefab.GetComponent<Entity>().id = entity.id;
        }
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

    void InstantiateDungeonContainers()
    {
        // Instantiate dungeon prefabs, and get references to child components
        dungeonParent = Instantiate(dungeonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        floorParent = dungeonParent.transform.GetChild(0).gameObject;
        wallParent = dungeonParent.transform.GetChild(1).gameObject;
        objectsParent = dungeonParent.transform.GetChild(2).gameObject;
        entitiesParent = dungeonParent.transform.GetChild(3).gameObject;
    }

    void InstantiateDungeonFeatures(Dungeon dungeon)
    {
        int width = dungeon.getWidth();
        int height = dungeon.getHeight();

        for (int y = 0; y < height; y++) 
            for (int x = 0; x < width; x++) {
                // Add a floor if 1st bit of mask is 1
                // z = 1 so that it renders underneath everything else
                if ((byte)(dungeon.getByte(x,y) & 0x1) > 0)
                    CreateChildPrefab(floorPrefab, floorParent, x, y, 1);

                // add a wall if 2nd bit is a 1
                if ((byte)(dungeon.getByte(x,y) & 0x2) > 0)
                    CreateWallPrefab(wallParent, x, y, dungeon);

                // add a down staircase if 3rd bit is a 1
                if (dungeon.getByte(x, y) == 0x4)
                    CreateChildPrefab(stairsDownPrefab, entitiesParent, x, y, 0);

                if (dungeon.getByte(x, y) == 0x5 & floorsAbove.Count > 0)
                    CreateChildPrefab(stairsUpPrefab, entitiesParent, x, y, 0);
            }
    }

    
    // -= Events =- 

    public void OnStairsInteractEventReceived(int input)
    {
        inputEnabled.value = false;

        // an input of 0 means that this is a staircase going down
        if (input == 0)
        {
            entityManager.UpdateEntitiesInDungeon(dungeon, entitiesParent);
            floorsAbove.Push(dungeon);
            Destroy(dungeonParent);

            // load dungeon if there are floors below, else generate new floor
            if (floorsBelow.Count > 0)
                dungeon = floorsBelow.Pop();
            else
            {
                dungeon = new Dungeon(width / 3, height / 3);
                entityManager.PopulateDungeon(dungeon, 5);
            }

            InstantiateDungeon(dungeon);
            InstantiateEntitiesInDungeon(dungeon);
            playerInstance.GetComponent<PlayerPhysics>().Teleport(
                new Vector3(dungeon.getStartCoordX(), dungeon.getStartCoordY(), playerInstance.transform.position.z));

        }

        // an input of 1 means that this is a staircase going up
        else if (input == 1)
        {
            entityManager.UpdateEntitiesInDungeon(dungeon, entitiesParent);
            floorsBelow.Push(dungeon);
            Destroy(dungeonParent);

            if (floorsAbove.Count > 0)
                dungeon = floorsAbove.Pop();
            else
            {
                dungeon = new Dungeon(width / 3, height / 3);
                entityManager.PopulateDungeon(dungeon, 5);
            }

            InstantiateDungeon(dungeon);
            InstantiateEntitiesInDungeon(dungeon);
            playerInstance.GetComponent<PlayerPhysics>().Teleport(
                new Vector3(dungeon.getEndCoordX(), dungeon.getEndCoordY(), playerInstance.transform.position.z));
        }

        currentFloor.value = floorsAbove.Count + 1;
        onFloorChange.Raise(currentFloor.value);

        inputEnabled.value = true;
    }
}