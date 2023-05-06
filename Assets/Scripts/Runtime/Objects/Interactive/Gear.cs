using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Gear : InteractiveObject
{
    public UnityEvent onRun;

    const int ROTATION_SPEED = -150;

    bool attached,
        running;

    protected override void Update()
    {
        if (running)
        {
            transform.Rotate(0, 0, Time.deltaTime * ROTATION_SPEED);
            return;
        }
        if (!interactable)
            return;

        if (!attached)
        {
            if (controlled)
            {
                ControlPosition();
                if (Input.GetButtonUp("Fire1"))
                    DeactivateControl();
            }
            else if (inputHit)
                ActivateControl();
        }
        else if (inputHit)
            Run();
    }

    void Run()
    {
        running = true;
        col.enabled = false;
        if (onRun != null)
            onRun.Invoke();
    }

    public void Attach(Vector3 pos)
    {
        DeactivateControl();
        attached = true;
        transform.position = pos;
        rb.isKinematic = true;
        Liss.ToggleFocusHandler -= ToggleFocusEvent;
    }
}