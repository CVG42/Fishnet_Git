using FishNet.Object;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject objectPrefab;

    [SerializeField] private float[] spawnPointsX = { -5f, 0f, 5f };
    [SerializeField] private float spawnY = 10f;
    [SerializeField] private float spawnInterval = 2f;

    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private float[] spawnPointsY = { 3.8f, 1f, -1.5f };
    [SerializeField] private float spawnX = 5.31f;

    private void Start()
    {
        /*
        if (IsServerInitialized)
        {
            InvokeRepeating(nameof(SpawnObject), spawnInterval, spawnInterval);
            InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
        }*/
    }

    [Server]
    private void Update()
    {
        if(IsServerInitialized && Input.GetKeyDown(KeyCode.T))
        {
            InvokeRepeating(nameof(SpawnObject), spawnInterval, spawnInterval);
            InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
        }

        if(EnemyManager.Instance.isAlive == false)
        {
            CancelSpawnInvokes();
        }
    }

    [Server]
    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPointsY.Length);
        Vector3 spawnPosition = new Vector3(spawnX, spawnPointsY[randomIndex], 0f);
        GameObject newObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        newObject.GetComponent<Rigidbody>().velocity = Vector3.left * 5;

        Spawn(newObject);
    }

    [Server]
    private void SpawnObject()
    {
        int randomIndex = Random.Range(0, spawnPointsX.Length);
        Vector3 spawnPosition = new Vector3(spawnPointsX[randomIndex], spawnY, 0f);
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        Spawn(newObject);
    }

    [Server]
    public void CancelSpawnInvokes()
    {
        CancelInvoke(nameof(SpawnObject));
        CancelInvoke(nameof(SpawnEnemy));
    }
}
