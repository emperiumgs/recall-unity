using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    public Vector2 minMap,
        maxMap;
    public Vector3 offset;

    Camera cam;
    Vector2 minPoint,
        maxPoint;
    Vector3 pos;

	void Awake()
    {
        cam = GetComponent<Camera>();

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        minPoint.x = minMap.x + horzExtent;
        minPoint.y = minMap.y + vertExtent;
        maxPoint.x = maxMap.x - horzExtent;
        maxPoint.y = maxMap.y - vertExtent;
    }

    void FixedUpdate()
    {
        pos = Vector3.Lerp(transform.position, target.position + offset, .1f);
        pos.x = Mathf.Clamp(pos.x, minPoint.x, maxPoint.x);
        pos.y = Mathf.Clamp(pos.y, minPoint.y, maxPoint.y);
        pos.z = -10;
        transform.position = pos;
    }

    public void SwitchDirection()
    {
        offset.x *= -1;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(minMap, maxMap);
    //    Gizmos.DrawWireCube(transform.position - offset + Vector3.up * 0.7f, new Vector3(0.5f, 1.4f));
    //}
}