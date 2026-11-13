using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private bool isAwake;
    public bool awake => isAwake;

    private Rigidbody body;
    private Vector3 previousPosition;
    private int stillSteps;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 pullOffset, float speedMultiplier = 10f)
    {
        pullOffset.z = 0f;

        body.isKinematic = false;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.linearVelocity = -pullOffset * speedMultiplier;

        previousPosition = body.position;
        stillSteps = 0;
        isAwake = true;
    }

    private void FixedUpdate()
    {
        if (!isAwake || body.isKinematic)
            return;

        float distance = Vector3.Distance(body.position, previousPosition);
        previousPosition = body.position;

        if (distance <= 0.01f)
            stillSteps++;
        else
            stillSteps = 0;

        if (stillSteps >= 10)
        {
            body.Sleep();
            isAwake = false;
        }
    }
    
}