using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -6f;
    public float maxY = 6f;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        targetPos = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        transform.position = targetPos;
    }
}