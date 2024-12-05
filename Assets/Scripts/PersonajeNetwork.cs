using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class PersonajeNetwork : NetworkBehaviour
{
    public static PersonajeNetwork Instance;

    public int scores;

    public readonly SyncVar<int> score = new SyncVar<int>();

    //readonly SyncVar<Color> myColor = new SyncVar<Color>();
    //readonly SyncVar<GameObject> target = new SyncVar<GameObject>();

    //List, hashset, dictionary
    readonly SyncList<int> myList = new SyncList<int>(); // List<int>

    #region C#: List, hashet, dictionary
    // C#
    // int[] su limitante es fijo
    // List<int> // arreglo dinamico, myList.Contains(num) regresa bool, desventaja: cuando es grande la lista toma mucho timepo en recorrer
    // Hashset<int> // Lista con hash. Hash: proceso donde un numero genera otro numero que es unico para determinar el espacio en RAM. Es inmediato para buscar elementos.
    // Dictionary<string, int> // String es llave (key), int es valor. Key vinculada a valor. Ej. agenda
    #endregion

    // Se puede sincronizar GameObject solo SI tiene el Network Object

    #region Funciones Start

    // Funciones equivalentes a Start para multiplayer

    // public override void OnStartNetwork(){} // Se llama tanto server y cliente

    public override void OnStartClient() { } // Start con syncvar sincronizados, pero solo del cliente (referencias al UI)

    public override void OnStartServer() { } // Start del lado del Server, se puede modificar syncvar que al enviarse al jugador llegue con esos datos

    #endregion

    #region Funciones de Owners

    //Se llaman cuando un GameObject cambia de duen~o
    public override void OnOwnershipServer(NetworkConnection prevOwner) { } // Solo del lado del server
 
    public override void OnOwnershipClient(NetworkConnection prevOwner) { } // cuando el objeto le pertenece a un jugador y solo lo ve ese jugador

    #endregion

    #region Funciones de Destroy
    
    // Cuando un objeto es destruido en red

    public override void OnDespawnServer(NetworkConnection connection) { } // Despawnea algo pero solo del lago del server

    // ---- Funciones para el Destroy del multiplayer ----

    public override void OnStopServer() { } // Solo del lado del servidor y que se destruyo el objeto

    public override void OnStopClient() { } // lado cliente

    public override void OnStopNetwork() { } // lado de network

    #endregion

    void Awake()
    {
        Instance = this;
        //myColor.OnChange += miColorChange;
        myList.OnChange += MyListChange;
        //score.OnChange += scoreChange;
    }

    private void OnScoreChange(int oldValue, int newValue, bool asServer)
    {
        score.Value = newValue;
    }

    void MyListChange(SyncListOperation op, int index, int oldValue, int newValue, bool asServer)
    {
        print($"{op} - index: {index} - old: {oldValue} - new: {newValue}");
        switch (op)
        {
            case SyncListOperation.Add: // myList.Add(20); // Se llama cuando se agrega un valor nuevo
                // index (posicion insertada), newValue (que valor fue agregado)
                break;
            case SyncListOperation.Insert: // myList.Insert(20, 5) // Agregar un numero (20) en cierta posicion (5).
                // index, newValue
                break;
            case SyncListOperation.Set: // myList[2] = 13, solo se puede modificar un valor existente
                // index, oldValue, newValue
                break;
            case SyncListOperation.RemoveAt: // myList.RemoveAt(1); // Eliminaria el index[1]
                // index, oldValue
                break;
            case SyncListOperation.Clear: // myList.Clear(); // Limpia todo
                break;
            case SyncListOperation.Complete: // Cuando se llamaron todas las operaciones en este fishent frame.
                // Las actualizaciones en multiplayer son a traves del tiempo, no en cada frame.
                break;
        }
    }

    /*
    [Server] // Solo ejecuta el codigo si es servidor, ahorra el " if (!IsServerStarted) return; " 
    private void LateUpdate()
    {
        //if (!IsServerStarted) return; // solo el server puede modificar la lista

        if (Input.GetKeyDown(KeyCode.Alpha1))
            myList.Add(Random.Range(1, 100));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            myList.RemoveAt(Random.Range(0, myList.Count));
        if (Input.GetKeyDown(KeyCode.Alpha3))
            myList.Clear();
    }
    */

    // [SYNC] myList.Add(10); myList.Add(15); myList.Remove(0); [SYNC]

    /*
    [ServerRpc] //una funcion que se ejecuta en el servidor
    void CambiarColorServerRPC()
    {
        myColor.Value = Random.ColorHSV();
        print("Se cambio a color: " + myColor.Value);

    }*/

    // requiere Ownership = puede llamar esta funcion en gameObjects que no me pertenecen

    [ServerRpc(RequireOwnership = false)]
    void CambiarColorServerRPC2()
    {
        Color nuevoColor = Random.ColorHSV();
        CambiarColorRPC2(nuevoColor);
    }

    // RunLocally = permite que el servidor ejecute el mismo codigo que el cliente SIN ser el cliente
    [ObserversRpc(RunLocally = true)] // observer es un cliente, solo se eejecuta en clientes
    void CambiarColorRPC2(Color nuevoColor)
    {
        GetComponent<MeshRenderer>().material.color = nuevoColor;

    }

    /*
    void scoreChange(int before, int next, bool asServer) //next == miColor.Value
    {
        score.Value = next;
    }*/
    /*
    void miColorChange(Color before, Color next, bool asServer) //next == miColor.Value
    {
        GetComponent<MeshRenderer>().material.color = next;
    }*/

    public Sprite partnerShip;

    public override void OnStartNetwork() //Start para que sincronice
    {
        if (Owner.IsLocalClient == false)
        {
            name += "(local)";
            //GetComponentInChildren<SpriteRenderer>().color = Color.green;
            GetComponentInChildren<SpriteRenderer>().sprite = partnerShip;
        }
        scores = 0;
        score.Value = 0;
    }

    [ObserversRpc]
    void Teleport(GameObject target)
    {
        transform.position = target.transform.position;
    }

    void SendTeleport()
    {
        //GameObject go;
        //NetworkObject networkObject = go.GetComponent<NetworkObject>();
        //Teleport(networkObject.ObjectId);        
    }

    void Update()
    {
        if (IsOwner == false) return;

        if (scores <= 0)
        {
            scores = 0;
        }

        if (score.Value <= 0)
        {
            score.Value = 0;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            CambiarColorServerRPC2();
        }

        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        transform.Translate(moveInput * Time.deltaTime * 8f);
    }

    //readonly SyncVar<int> score = new SyncVar<int>();
    

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServerStarted) return; //para que solo el servidor pueda confirmar la accion, aqui se puede poner un feedback al usuario

        
        if(other.CompareTag("Enemy"))
        {
            Despawn(other.gameObject);
            ReduceScore(Owner);
            // Owner // -Para saber de que jugador se esta hablando/realizando la accion

            //EquipWeaponRPC(Owner);
        }

        if(other.CompareTag("Item"))
        {
            Despawn(other.gameObject);
            GetScore(Owner);
        }
    }

    [TargetRpc]
    void ReduceScore(NetworkConnection conn)
    {
        score.Value--;
        //GainScore();
        scores--;
        print(scores);
    }

    [TargetRpc]
    void GetScore(NetworkConnection conn)
    {
        score.Value++;
        //GainScore();
        scores++;
        print(scores);
    }
}
