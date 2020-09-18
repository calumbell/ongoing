using UnityEngine;

public class EntityData
{
    public GameObject prefab;
    public Vector3 location;
    public int id;

    public EntityData(GameObject prefabInput, Vector3 locationInput, int n)
    {
        prefab = prefabInput;
        id = n;
        location = locationInput;
    }
}
