using UnityEngine;

public class Entity : MonoBehaviour
{
    public GameObject prefab;

    public Vector3 location { get; set; }

    public int id { get; set; }

    public string entityName;

    public Entity(GameObject prefabInput, Vector3 locationInput, int n)
    {
        prefab = prefabInput;
        id = n;
        location = locationInput;
    }
}
