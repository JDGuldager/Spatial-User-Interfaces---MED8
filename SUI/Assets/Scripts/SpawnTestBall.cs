using UnityEngine;
using System.Collections;

public class SpawnTestBall : MonoBehaviour
{
    [SerializeField] private GameObject testBall;
    [SerializeField] private Transform spawnPoint;

    private bool canSpawn = true;

    private void Start()
    {
        Instantiate(testBall, spawnPoint.position, spawnPoint.rotation);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("StartBall") && canSpawn)
        {
            canSpawn = false;
            StartCoroutine(WaitToSpawnBall(3f));
        }
    }

    private IEnumerator WaitToSpawnBall(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(testBall, spawnPoint.position, spawnPoint.rotation);
        canSpawn = true;
    }
}