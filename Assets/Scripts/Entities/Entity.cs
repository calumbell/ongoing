using UnityEngine;

public class Entity : MonoBehaviour
{
    public GameObject prefab;

    public Vector3 location;

    public int id;

    public string entityName;

    public Entity(GameObject prefabInput, Vector3 locationInput, int n)
    {
        prefab = prefabInput;
        id = n;
        location = locationInput;
    }
}
