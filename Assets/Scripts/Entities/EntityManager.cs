using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private static int i;
    public GameObject[] entityPrefabs;

    public EntityData GenerateEntity(int n, Vector3 location)
    {
        n = n > entityPrefabs.Length - 1 ? entityPrefabs.Length - 1 : n;

        return new EntityData(entityPrefabs[n], location, i++);    
    }

    public void PopulateDungeon(Dungeon d, int n)
    {
        Room[] rooms = d.GetTileMap().getRooms();

        foreach (Room room in rooms)
        {
            if (room != d.getStartRoom())
            {
                Vector3 location = new Vector3(
                4 * (room.getX() + Random.Range(1, room.getWidth()-1)),
                4 * (room.getY() + Random.Range(1, room.getHeight()-1)),
                -1);

                // Pick a random entity type
                int type = Random.Range(0, entityPrefabs.Length);

                // Add entity to the list inside dungeon
                d.GetEntities().Add(new EntityData(entityPrefabs[type], location, i++));
            }
        }
    }

    public void UpdateEntitiesInDungeon(Dungeon d, GameObject parent)
    {
        List<EntityData> entityData = d.GetEntities();
        EntityBaseBehaviour[] entityScripts = parent.GetComponentsInChildren<EntityBaseBehaviour>();

        foreach (EntityData data in entityData)
        {
            foreach(EntityBaseBehaviour script in entityScripts)
            {
                if (data.id == script.GetId())
                    data.location = script.GetTransform().position;
            }
        }
    }
}
