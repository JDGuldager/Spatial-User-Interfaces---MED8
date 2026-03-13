using UnityEngine;

public class Rock : Ball
{
    [SerializeField] float rotationSpeed = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        effect.transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
    }
}
