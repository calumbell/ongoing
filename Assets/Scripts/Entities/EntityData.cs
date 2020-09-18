using UnityEngine;

public class EntityData
{
    public GameObject prefab;
    public Vector3 location;
    public int id;

    public EntityData(GameObject prefabInput, int n)
    {
        prefab = prefabInput;
        id = n;
        location = new Vector3();
    }
}
