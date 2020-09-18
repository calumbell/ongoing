using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private static int i;
    public GameObject[] entityPrefabs;

    public EntityData GenerateEntity(int n)
    {
        n = n > entityPrefabs.Length - 1 ? entityPrefabs.Length - 1 : n;

        return new EntityData(entityPrefabs[n], i++);    
    }
}
