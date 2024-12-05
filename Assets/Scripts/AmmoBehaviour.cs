using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class AmmoBehaviour : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            Despawn(gameObject);
        }
    }
}
