using UnityEngine;
using Recall.Gameplay.Interfaces;

public class CheckpointTrigger : Trigger2D
{
    protected override void Action(Collider2D col)
    {
        if (col.TryGetComponent<IRespawnable>(out var respawnable))
            respawnable.SetRespawnPosition(transform.position);
        base.Action(col);
    }
}