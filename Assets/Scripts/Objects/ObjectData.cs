using UnityEngine;

public struct ObjectData
{
    public GameObject prefab;
    public Vector3 position;
    public int id;


    public ObjectData(GameObject obj, Vector3 pos, int n)
    {
        prefab = obj;
        position = pos;
        id = n;
    }
}
