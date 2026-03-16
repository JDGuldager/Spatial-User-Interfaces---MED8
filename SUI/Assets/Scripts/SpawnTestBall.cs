using UnityEngine;

public class SpawnTestBall : MonoBehaviour
{
    private Rigidbody Rigidbody;
    [SerializeField]
    private GameObject testBall;
    [SerializeField]
    private Transform spawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Instantiate(testBall, spawnPoint.position, spawnPoint.rotation);

    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "StartBall")
        {
            Instantiate(testBall, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
