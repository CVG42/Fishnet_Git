using FishNet.Object;
using UnityEngine;

public class EnemySpawn : NetworkBehaviour
{
    [SerializeField] private GameObject objectPrefab;

    [SerializeField] private float[] spawnPointsY = { 3.8f, 1f, -1.5f };
    [SerializeField] private float spawnX = 5.31f;
    [SerializeField] private float spawnInterval = 2f;

    private void Start()
    {
        if (IsServerInitialized)
        {
            InvokeRepeating(nameof(SpawnObject), spawnInterval, spawnInterval);
        }
    }

    [Server]
    private void SpawnObject()
    {
        int randomIndex = Random.Range(0, spawnPointsY.Length);
        Vector3 spawnPosition = new Vector3(spawnX, spawnPointsY[randomIndex], 0f);
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        newObject.GetComponent<Rigidbody>().velocity = Vector3.right * 5;

        Spawn(newObject);
    }
}
