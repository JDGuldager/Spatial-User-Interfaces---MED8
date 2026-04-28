using UnityEngine;

// These are the three possible "modes" the object can be in
// Idle   = sitting in the player's hand (not moving)
// Throw  = flying away from the player
// Return = coming back to the player like Thor’s hammer
public enum State { Idle, Throw, Return }

public class ThrowLogic : MonoBehaviour
{
    // How fast the object moves when thrown or returning
    [SerializeField] float throwSpeed = 20f;

    // How close the object needs to be to the hand before it "snaps" back into it
    [SerializeField] float catchDistance = 0.25f;

    // Reference to the player's hand (this is where the object should return to)
    public Transform PlayersHand;

    // Physics component that controls movement
    Rigidbody rb;

    // Tracks the current state (Idle / Throw / Return)
    State state;

    private void Start()
    {
        // When the game starts, the object is not moving and is considered "held"
        state = State.Idle;

        // Get the Rigidbody component attached to this object
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Every frame, we check what state the object is in
        // and run the appropriate behavior
        switch (state)
        {
            case State.Idle:
                // Do nothing — object is just sitting in the hand
                break;

            case State.Throw:
                // Make sure the object is NOT attached to the hand anymore
                transform.SetParent(null);

                // Turn physics ON so it can move freely
                rb.isKinematic = false;

                // Move in its current direction at a constant speed
                // (normalized = direction only, without speed)
                rb.linearVelocity = rb.linearVelocity.normalized * throwSpeed;
                break;

            case State.Return:
                // Run the return logic (move toward the hand)
                ReturnToHand();
                break;
        }
    }

    // This function handles the object flying back to the player's hand
    private void ReturnToHand()
    {
        // Calculate direction and distance from object to the hand
        Vector3 toHand = PlayersHand.position - transform.position;
        float distance = toHand.magnitude;

        // If the object is still far away...
        if (distance > catchDistance)
        {
            // Get direction toward the hand
            Vector3 dir = toHand.normalized;

            // Make sure physics is active
            rb.isKinematic = false;

            // Move directly toward the hand
            rb.linearVelocity = dir * throwSpeed;
        }
        else
        {
            // If it's close enough → automatically "catch" it
            CatchObject();
        }
    }

    // This function "locks" the object back into the player's hand
    private void CatchObject()
    {
        // Set state back to Idle (no movement anymore)
        state = State.Idle;

        // Stop all movement completely
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Turn OFF physics so it stays perfectly still
        rb.isKinematic = true;

        // Attach the object to the player's hand
        // (so it moves with the hand automatically)
        transform.SetParent(PlayersHand);

        // Reset position so it sits exactly in the hand
        transform.localPosition = Vector3.zero;

        // Reset rotation so it aligns correctly
        transform.localRotation = Quaternion.identity;
    }

    // Called when you want the object to just sit in the hand
    public void IdleObject()
    {
        state = State.Idle;
    }

    // Called when you throw the object
    public void ThrowObject()
    {
        // Detach from the hand so it can fly freely
        transform.SetParent(null);

        // Turn physics ON
        rb.isKinematic = false;

        // Switch to "Throw" mode
        state = State.Throw;
    }

    // Called when you want the object to return like Thor’s hammer
    public void ReturnObject()
    {
        // Detach from the hand (just in case)
        transform.SetParent(null);

        // Turn physics ON so it can move
        rb.isKinematic = false;

        // Switch to "Return" mode
        state = State.Return;
    }
}