using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Sector1 : MonoBehaviour
{
    public UnityEvent onComplete;

    void FixedUpdate()
    {
        if (GetComponentInChildren<Enemy>() == null && onComplete != null)
        {
            onComplete.Invoke();
            gameObject.SetActive(false);
        }
    }
}