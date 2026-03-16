using UnityEngine;

public enum State { Idle, Throw, Return }
public class ThrowLogic : MonoBehaviour
{
    [SerializeField]
    float throwSpeed = 20;

    public Transform PlayersHand;

    Rigidbody rb;
    State state;
    private void Start()
    {
        state = State.Idle;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                Debug.Log("Object is Idle");
                break;

            case State.Throw:
                Debug.Log("Object is being thrown");
                rb.linearVelocity = rb.linearVelocity.normalized * throwSpeed;
                break;

            case State.Return:
                Debug.Log("Object is Returning");
                if (Vector3.SqrMagnitude(PlayersHand.position - transform.position) > 16)
                {
                    Vector3 dir = (PlayersHand.position - transform.position).normalized;
                    rb.linearVelocity = dir * throwSpeed;
                }
                else
                {
                    state = State.Idle;
                }
                    break;

            default:
                break;
        }
    }


    public void IdleObject()
    {
        state = State.Idle;
    }
    public void ThrowObject()
    {
        state = State.Throw;
    }
    public void ReturnObject() 
    {
         state = State.Return;
    }
}
