using UnityEngine;
using System.Collections;

public class GearHold : MonoBehaviour
{
    Collider2D hit;
    int mask;

    void Awake()
    {
        mask = 1 << LayerMask.NameToLayer("Interactable");
    }

    void FixedUpdate()
    {
        hit = Physics2D.OverlapPoint(transform.position, mask);
        if (hit != null)
        {
            Gear g = hit.GetComponent<Gear>();
            if (g != null)
            {
                g.Attach(transform.position);
                gameObject.SetActive(false);
            }
        }
    }
}