using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class Bullet : NetworkBehaviour 
{
    //float timeToDestroy = 5f;
    public readonly SyncVar<int> ownerId= new SyncVar<int>(); // Id de quien dispaa
}
