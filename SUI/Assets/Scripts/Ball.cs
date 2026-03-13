
using UnityEngine;

public abstract class Ball : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject effect;
    [SerializeField] GameObject onImpact;
    [SerializeField] private float destroyDelay = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Debug.Log("Ball Start");
        if (onImpact != null)
        {
            onImpact.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            meshRenderer.enabled = false;
            effect.SetActive(false);
            onImpact.SetActive(true);
            
            Destroy(gameObject, destroyDelay);
        }
    }
}
