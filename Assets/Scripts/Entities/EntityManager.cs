using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private static int i;
    public GameObject[] NPCPrefabs;
    public GameObject[] objectPrefabs;

    public Entity GenerateNPC(int n, Vector3 location)
    {
        n = n > NPCPrefabs.Length - 1 ? NPCPrefabs.Length - 1 : n;

        return new Entity(NPCPrefabs[n], location, i++);    
    }

    public Entity GenerateObject(int n, Vector3 location)
    {
        n = n > objectPrefabs.Length - 1 ? objectPrefabs.Length - 1 : n;
        return new Entity(objectPrefabs[n], location, i++);
    }

    public void PopulateDungeon(Dungeon d, int n)
    {
        Room[] rooms = d.GetTileMap().getRooms();

        foreach (Room room in rooms)
        {

            Vector3 location = new Vector3(
                4 * (room.getX() + Random.Range(1, room.getWidth() - 1)),
                4 * (room.getY() + Random.Range(1, room.getHeight() - 1)),
                -1);

            int type = 0;

            d.GetEntities().Add(new Entity(objectPrefabs[type], location, i++));

            if (room != d.getStartRoom())
            {
                location = new Vector3(
                4 * (room.getX() + Random.Range(1, room.getWidth()-1)),
                4 * (room.getY() + Random.Range(1, room.getHeight()-1)),
                -1);

                // Pick a random entity type
                type = Random.Range(0, NPCPrefabs.Length);

                // Add entity to the list inside dungeon
                d.GetEntities().Add(new Entity(NPCPrefabs[type], location, i++));
            }
        }

    }

    public void UpdateEntitiesInDungeon(Dungeon d, GameObject parent)
    {
        List<Entity> entitiesInMemory = d.GetEntities();
        Entity[] entitiesInScene = parent.GetComponentsInChildren<Entity>();
        Entity entityInMemory;

        bool entityStillExistsInScene;

        for (int i = entitiesInMemory.Count; i > 0; i--)
        //foreach (Entity entityInMemory in entitiesInMemory)
        {
            entityInMemory = entitiesInMemory[i - 1];
            entityStillExistsInScene = false;

            foreach (Entity entityInScene in entitiesInScene)
            {
                if (entityInMemory.id == entityInScene.id)
                {
                    entityInMemory.location = entityInScene.transform.position;
                    entityStillExistsInScene = true;
                }
            }

            if (!entityStillExistsInScene)
            {
                entitiesInMemory.Remove(entityInMemory);
            }
        }
    }
}
