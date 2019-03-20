using UnityEngine;
using System.Collections;

public class InteractiveObject : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected AudioSource source;
    protected Collider2D col;
    protected Camera cam;
    protected bool interactable;
    protected bool controlled;
    protected int mask;

    protected bool inputHit
    {
        get
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Vector2 point = cam.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(point, mask);
                if (hit != null && hit == col)
                    return true;
            }
            return false;
        }
    }

    GameObject p;
    Ray ray;

    void Awake()
    {
        p = GetComponentInChildren<ParticleSystem>(true).gameObject;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        source = GetComponent<AudioSource>();
        Liss.ToggleFocusHandler += ToggleFocusEvent;
        cam = Camera.main;
        mask = 1 << gameObject.layer;
    }

    protected virtual void Update()
    {
        if (!interactable)
            return;

        if (controlled)
        {
            ControlPosition();
            if (Input.GetButtonUp("Fire1"))
                DeactivateControl();
        }
        else if (inputHit)
            ActivateControl();
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        source.Play();
        if (controlled)
            DeactivateControl();
    }

    protected void ActivateControl()
    {
        controlled = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        p.SetActive(true);
    }

    protected void ControlPosition()
    {
        Vector2 targetPos = cam.ScreenToWorldPoint(Input.mousePosition);
        rb.MovePosition(targetPos);
    }

    protected void DeactivateControl()
    {
        controlled = false;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        p.SetActive(false);
    }

    protected void ToggleFocusEvent(bool value)
    {
        interactable = value;
    }
}