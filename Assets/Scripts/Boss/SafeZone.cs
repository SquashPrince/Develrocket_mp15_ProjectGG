using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private bool _isPlayerInSight = false;

    public bool IsPlayerInSight
    {
        get
        {
            return _isPlayerInSight;
        }
        set
        {
            _isPlayerInSight = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            IsPlayerInSight = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            IsPlayerInSight = false;
        }
    }
}
