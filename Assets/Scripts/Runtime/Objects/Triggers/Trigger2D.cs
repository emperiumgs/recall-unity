using UnityEngine;

public abstract class Trigger2D : MonoBehaviour
{
    public LayerMask targetedMask;
    public Vector2 offsetPosition,
        size;
    public bool once;

    Collider2D col;

    Vector2 centerPos
    {
        get { return (Vector2)transform.position + offsetPosition; }
    }

    void FixedUpdate()
    {
        col = Physics2D.OverlapBox(centerPos, size, transform.localEulerAngles.z, targetedMask);
        if (col != null)
            Action(col);
    }

    protected virtual void Action(Collider2D col)
    {
        if (once)
            gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        var color = Gizmos.color;
        Gizmos.color = new Color(1, 0, 1, .3f);
        Gizmos.DrawCube(centerPos, size);
        Gizmos.color = color;
    }
}