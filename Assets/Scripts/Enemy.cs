using FishNet.Object;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            Despawn(gameObject);
        }
    }
}
