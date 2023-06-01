using UnityEngine;
using System;

public class TriggerPublish : MonoBehaviour
{
    public event Action<bool, Collider> OnStateChanged;

    private void OnTriggerEnter(Collider other)
    {
        OnStateChanged?.Invoke(true, other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnStateChanged?.Invoke(false, other);
    }

}
