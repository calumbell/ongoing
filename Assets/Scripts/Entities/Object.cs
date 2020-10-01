using UnityEngine;

public class Object : Entity
{
    public Object(GameObject prefabInput, Vector3 locationInput, int n) : base(prefabInput, locationInput, n)
    {
        prefab = prefabInput;
        id = n;
        location = locationInput;
    }
}
