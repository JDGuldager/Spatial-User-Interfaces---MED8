
using UnityEngine;

public abstract class Ball : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] protected GameObject effect;
    [SerializeField] GameObject onImpact;
    [SerializeField] private float destroyDelay = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
        if (onImpact != null)
        {
            onImpact.SetActive(false);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
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
