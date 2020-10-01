using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour
{
    private static int i;
    public GameObject[] objectPrefabs;

    public ObjectData GenerateObject(int n, Vector3 location)
    {
        n = n > objectPrefabs.Length - 1 ? objectPrefabs.Length - 1 : n;

        return new ObjectData(objectPrefabs[n], location, i++);
    }

    public void PopulateDungeon(Dungeon d, int n)
    {
        Room[] rooms = d.GetTileMap().getRooms();

        foreach (Room room in rooms)
        {

        }
    }
}
