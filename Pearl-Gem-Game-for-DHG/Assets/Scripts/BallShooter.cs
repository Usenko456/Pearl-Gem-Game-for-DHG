using UnityEngine;

public class BallShooter : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform spawnPoint;
    public GameObject ballPrefab;
    public float baseForce = 9f;  // Base force for initial direction
    public float gravity = -9.81f;  // Acceleration due to gravity
    public float mass = 1f;
    private GameObject currentBall;
    private bool ballLaunched = false;
    private Vector3 launchDirection;
    private Vector3 touchStartPos;  // To store the initial touch position

    private void Start()
    {
        SpawnBall(); // Spawn ball at the start
    }

    private void Update()
    {
        if (currentBall != null && !IsBallOnScreen(currentBall))
        {
            Destroy(currentBall);
            SpawnBall();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y * 2, 1f));

            if (touch.phase == TouchPhase.Began && !ballLaunched)
            {
                touchStartPos = touch.position;  // Store the initial touch position
                launchDirection = (spawnPoint.position - touchPos).normalized;
            }

            if (touch.phase == TouchPhase.Moved && !ballLaunched)
            {
                UpdateTrajectory(touchPos);
            }

            if (touch.phase == TouchPhase.Ended && !ballLaunched)
            {
                ShootBall(touchPos);
            }
        }
    }

    void SpawnBall()
    {
        currentBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        currentBall.GetComponent<Rigidbody>().isKinematic = true;
        ballLaunched = false;
    }

    void ShootBall(Vector3 touchPos)
    {
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        // Calculate the direction from the spawn point to the touch position
        Vector3 direction = (spawnPoint.position - touchPos).normalized;

        // Calculate the force based on the vertical displacement of the touch
        float verticalDisplacement = Mathf.Abs(touchStartPos.y - Input.GetTouch(0).position.y);
        float adjustedForce = baseForce + (verticalDisplacement * 2);  // Adjust multiplier to fine-tune the effect

        // Apply the force to the ball
        rb.AddForce(direction * adjustedForce);

        ballLaunched = true;
        lineRenderer.positionCount = 0; // Clear the line after the shot
    }

    void UpdateTrajectory(Vector3 touchPos)
    {
        int linePoints = 2220;  // Optimal number of points for smooth trajectory
        Vector3[] trajectoryPoints = new Vector3[linePoints];

        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        Vector3 initialVelocity = (spawnPoint.position - touchPos).normalized * baseForce / mass;

        Vector3 startPos = currentBall.transform.position;  // Start the trajectory from the ball's current position

        for (int i = 0; i < linePoints; i++)
        {
            float t = i * 0.01f;  // Time step
            Vector3 point = startPos +
                            initialVelocity * t +
                            0.5f * new Vector3(0, gravity, 0) * t * t;

            // Check for collision with surfaces (ground, walls, etc.)
            if (i > 0)
            {
                RaycastHit hit;
                Vector3 direction = (point - trajectoryPoints[i - 1]).normalized;

                // Cast a ray from the previous point towards the next predicted point to check for a hit
                if (Physics.Raycast(trajectoryPoints[i - 1], direction, out hit, Vector3.Distance(trajectoryPoints[i - 1], point)))
                {
                    trajectoryPoints[i] = hit.point;  // Update the trajectory to the collision point
                    lineRenderer.positionCount = i + 1;
                    lineRenderer.SetPositions(trajectoryPoints);
                    return;  // Stop drawing the trajectory once it hits a surface
                }
            }

            trajectoryPoints[i] = point;
        }

        lineRenderer.positionCount = linePoints;
        lineRenderer.SetPositions(trajectoryPoints);
    }

    bool IsBallOnScreen(GameObject ball)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(ball.transform.position);
        return screenPoint.x >= 0 && screenPoint.x <= Screen.width && screenPoint.y >= 0 && screenPoint.y <= Screen.height;
    }
}
