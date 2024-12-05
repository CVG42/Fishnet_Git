using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class Weapon : NetworkBehaviour
{
    public static Weapon Instance;
    public GameObject bulletPrefab;
    public float bulletSpeed;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!IsOwner) return; // para que solo dispare el duen~o

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PersonajeNetwork.Instance.scores--;
            print(PersonajeNetwork.Instance.scores);
            ShootServer();
        }

    }

    [ServerRpc]
    public void ShootServer()
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform.position + transform.right, Quaternion.identity);
        // Modificar todas las variables SyncVar
        newBullet.GetComponent<Bullet>().ownerId.Value = OwnerId;
        newBullet.GetComponent<Rigidbody>().velocity = Vector3.right * bulletSpeed;
        Spawn(newBullet); // Al final, se le avisa a todos los clientes de la nueva bala
    }
}
