using UnityEngine;

public class WallCollider : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (other.isTrigger)
            return;

        ColliderDistance2D distance = other.Distance(GetComponent<Collider2D>());

        if (distance.isOverlapped)
        {
            other.transform.position += (Vector3)(distance.normal * distance.distance);
        }
    }
}