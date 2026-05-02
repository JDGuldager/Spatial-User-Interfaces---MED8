using UnityEngine;
using UnityEngine.PlayerLoop;

public class Rock : Ball
{
    [SerializeField] float rotationSpeed = 50f;
    //[SerializeField] float exploForce = 5f;
    //[SerializeField] float exploRadius = 2f;
    [SerializeField] GameObject ball;
    private Rigidbody RB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        effect.transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
        base.Update();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            RB.isKinematic = true;
            ball.SetActive(false);
            onImpact.SetActive(true);
            interactable.enabled = false;
            Destroy(gameObject, destroyDelay);
        }
        //foreach (Rigidbody rb in onImpact.GetComponentsInChildren<Rigidbody>())
        //{
        //    rb.AddExplosionForce(exploForce, transform.position, exploRadius);
        //}


        

    }
}
