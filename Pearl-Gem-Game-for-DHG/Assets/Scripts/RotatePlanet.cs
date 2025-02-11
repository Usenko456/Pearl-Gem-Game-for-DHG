using UnityEngine;

public class RotatePlanet : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 60, 0);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
