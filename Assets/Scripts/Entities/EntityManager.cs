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
        for (int i = 0; i < n; i++)
        {
            // Pick a random room, and pick a spot inside it
            Room room = d.GetRandomRoom();
 
            Vector3 location = new Vector3(
                4*(room.getX() + Random.Range(0, room.getWidth())) + 1,
                4*(room.getY() + Random.Range(0, room.getHeight())) + 1,
                -1);

            // Pick a random entity type
            int type = Random.Range(0, entityPrefabs.Length);

            // Add entity to the list inside dungeon
            d.GetEntities().Add(new EntityData(entityPrefabs[type], location, i++));
        }
    }
}
