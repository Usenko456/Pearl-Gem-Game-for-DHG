using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class BallShooter : MonoBehaviour
{
    public GameObject Panel;
    public LineRenderer lineRenderer;
    public Transform spawnPoint;
    public GameObject ballPrefab;
    [SerializeField] float baseForce = 9f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float mass = 0.9f;
    public static GameObject currentBall;
    private bool ballLaunched = false;
    private Vector3 launchDirection;
    private Vector3 touchStartPos;
    private int bulletsleft = 19;
    public TextMeshProUGUI textMeshPro;

    private void Start()
    {
        Panel.SetActive(false);
        SpawnBall();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void Update()
    {
        if (bulletsleft == 0 || GameObject.FindGameObjectsWithTag("Finish").Length == 0)
        {
            Panel.SetActive(true);
        }

        if (currentBall != null && !IsBallOnScreen(currentBall))
        {
            Destroy(currentBall);
            if (bulletsleft >1)
            { 
                SpawnBall();
                
            }
            bulletsleft--;
        }
        if (textMeshPro != null)
        {
            textMeshPro.text = "Shots left: " + bulletsleft;
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y * 2, 1f));

            if (touch.phase == TouchPhase.Began && !ballLaunched)
            {
                touchStartPos = touch.position;
                launchDirection = (spawnPoint.position - touchPos).normalized;
            }
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            else
            {
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
    }
   void SpawnBall()
    {
        currentBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        currentBall.GetComponent<Rigidbody>().isKinematic = true;
        ballLaunched = false;
        Renderer ballRenderer = currentBall.GetComponent<Renderer>();
        if (ballRenderer != null)
        {
            Material lastMaterial = ChangeBallMaterial.GetLastMaterial();
            if (lastMaterial != null)
            {
                ballRenderer.material = lastMaterial;
            }
        }
    }

     void ShootBall(Vector3 touchPos)
    {
        Rigidbody rb = currentBall.GetComponent<Rigidbody>(); 
        rb.isKinematic = false;              
        Vector3 direction = (spawnPoint.position - touchPos).normalized;
        float verticalDisplacement = Mathf.Abs(touchStartPos.y - Input.GetTouch(0).position.y);
        float adjustedForce = baseForce + (verticalDisplacement * 2);  
        rb.AddForce(direction * adjustedForce);

        ballLaunched = true;
        lineRenderer.positionCount = 0; 
    }
    void UpdateTrajectory(Vector3 touchPos)
    {
        int linePoints = 2220;  
        Vector3[] trajectoryPoints = new Vector3[linePoints];
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        Vector3 initialVelocity = (spawnPoint.position - touchPos).normalized * baseForce / mass;
        Vector3 startPos = currentBall.transform.position;  
        for (int i = 0; i < linePoints; i++)
        {
            float t = i * 0.01f; 
            Vector3 point = startPos +
                            initialVelocity * t +
                            0.6f * new Vector3(0, gravity, 0) * t * t;
            if (i > 0)
            {
                RaycastHit hit;
                Vector3 direction = (point - trajectoryPoints[i - 1]).normalized;
                if (Physics.Raycast(trajectoryPoints[i - 1], direction, out hit, Vector3.Distance(trajectoryPoints[i - 1], point)))
                {
                    trajectoryPoints[i] = hit.point;  
                    lineRenderer.positionCount = i + 1;
                    lineRenderer.SetPositions(trajectoryPoints);
                    return;  
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
