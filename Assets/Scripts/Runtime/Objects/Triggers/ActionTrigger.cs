using UnityEngine;
using UnityEngine.Events;

public class ActionTrigger : Trigger2D
{
    public UnityEvent onTrigger;

    protected override void Action(Collider2D col)
    {
        if (onTrigger != null)
            onTrigger.Invoke();
        base.Action(col);
    }
}