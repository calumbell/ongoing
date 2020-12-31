using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Script References")]
    public GameObject prefab;
    public Collider2D hurtbox;

    [Header("State Variables")]
    public Vector3 location;
    public int id;
    public string entityName;

    public Entity(GameObject _prefab, Vector3 _location, int _id)
    {
        prefab = _prefab;
        id = _id;
        location = _location;
    }

    public bool CollidesWith(Entity other)
    {
        if (hurtbox == null || other.hurtbox == null) return false;

        return AABBCollision(
            transform.position.x, transform.position.y, 1.0f, 1.0f,
            other.transform.position.x, other.transform.position.y, 1.0f, 1.0f);
    }

    public bool IsTouchingAnotherEntity(Entity[] entities)
    {
        foreach (Entity entity in entities)
        {
            if (this == entity) continue;


            if (CollidesWith(entity))
                return true;
        }

        return false;
    }

    public bool IsTouchingWalls()
    {
        GameObject wallsList = GameObject.FindGameObjectWithTag("WallsList");

        if (wallsList == null) return false;

        Collider2D[] wallColliders = wallsList.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D wall in wallColliders)
        {
            if (AABBCollision(transform.position.x, transform.position.y, 0.65f, 0.65f,
                wall.transform.position.x, wall.transform.position.y, 0.65f, 0.65f))
            {
                return true;
            }
        }

        return false;
    }

    public bool AABBCollision(float x1, float y1, float w1, float h1,
                              float x2, float y2, float h2, float w2)
    {
        if (x1 < x2 + w2 && x1 + w1 > x2 && y1 < y2 + h2 && y1 + h1 > y2)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
