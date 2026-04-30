using UnityEngine;

public class PedestalSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] Transform[] spawnPoints;

    [Header("Possible Prefabs")]
    [SerializeField] GameObject[] possiblePrefabs;

    [Header("Respawn")]
    [SerializeField] float respawnDelay = 0.5f;

    GameObject[] spawnedObjects;

    private void Start()
    {
        spawnedObjects = new GameObject[spawnPoints.Length];
        SpawnOnAllPedestals();
    }

    public void SpawnOnAllPedestals()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
            SpawnRandomPrefabOnPoint(i);
    }

    public void SpawnRandomPrefabOnPoint(int index)
    {
        if (index < 0 || index >= spawnPoints.Length)
            return;

        if (possiblePrefabs.Length == 0)
            return;

        if (spawnedObjects[index] != null)
            Destroy(spawnedObjects[index]);

        int randomIndex = Random.Range(0, possiblePrefabs.Length);
        GameObject prefabToSpawn = possiblePrefabs[randomIndex];

        Transform spawnPoint = spawnPoints[index];

        GameObject obj = Instantiate(
            prefabToSpawn,
            spawnPoint.position,
            spawnPoint.rotation
        );

        spawnedObjects[index] = obj;

        SpawnedPedestalItem item = obj.GetComponent<SpawnedPedestalItem>();

        if (item == null)
            item = obj.AddComponent<SpawnedPedestalItem>();

        item.Setup(this, index, spawnPoint);
    }

    public void NotifyItemUsed(int index, GameObject usedObject)
    {
        if (index < 0 || index >= spawnedObjects.Length)
            return;

        if (spawnedObjects[index] != usedObject)
            return;

        spawnedObjects[index] = null;

        Invoke(nameof(RespawnAllMissing), respawnDelay);
    }

    private void RespawnAllMissing()
    {
        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            if (spawnedObjects[i] == null)
                SpawnRandomPrefabOnPoint(i);
        }
    }
}