using UnityEngine;

public class Entity : MonoBehaviour
{
    public GameObject prefab;
    public Collider2D hurtbox;

    public Vector3 location;
    public int id;
    public string entityName;

    public Entity(GameObject prefabInput, Vector3 locationInput, int n)
    {
        prefab = prefabInput;
        id = n;
        location = locationInput;
    }

    public bool CollidesWith(Entity other)
    {
        if (hurtbox == null || other.hurtbox == null)
        {
            return false;
        }

        return AABBCollision(transform.position.x, transform.position.y,
            other.transform.position.x, other.transform.position.y);
    }

    public bool IsTouchingAnotherEntity(Entity[] entities)
    {
        foreach (Entity entity in entities)
        {
            if (this == entity)
            {
                continue;
            }

            if (CollidesWith(entity))
            {
                return true;
            }
        }

        return false;
    }

    public bool AABBCollision(float x1, float y1, float x2, float y2)
    {
        if (x1 < x2 + 1.0f && x1 + 1.0f > x2 &&
            y1 < y2 + 1.0f && y1 + 1.0f > y2)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
