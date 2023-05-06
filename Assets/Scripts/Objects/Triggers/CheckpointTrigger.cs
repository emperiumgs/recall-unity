using UnityEngine;
using System.Collections;

public class CheckpointTrigger : Trigger2D
{
    protected override void Action(Collider2D col)
    {
        col.GetComponent<Liss>().checkpoint = transform;
        base.Action(col);
    }
}