using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public float moveTime = .25f;

    const float THRESHOLD = .05f;

    Coroutine move;
    Vector3 openPos,
        closedPos;
    bool open;

    void Awake()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        openPos = transform.position + Vector3.up * col.size.y;
        closedPos = transform.position;
        open = false;
    }

    public void Open()
    {
        if (open)
            return;
        open = true;
        if (move != null)
            StopCoroutine(move);
        move = StartCoroutine(MoveToPosition(openPos));
    }

    public void Close()
    {
        if (!open)
            return;
        open = false;
        if (move != null)
            StopCoroutine(move);
        move = StartCoroutine(MoveToPosition(closedPos));
    }

    IEnumerator MoveToPosition(Vector3 pos)
    {
        while (Vector3.Distance(transform.position, pos) > THRESHOLD)
        {
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime / moveTime);
            yield return null;
        }
        transform.position = pos;
        move = null;
    }
}