using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ChangeBallMaterial : MonoBehaviour
{
    public  Material[] materials;
    private GameObject spawnedBall;
    public int currentIndex = 0;
    private static Material lastMaterial; 

    private void Start()
    {   
        if (materials != null && materials.Length > 0)
        {
            lastMaterial = materials[4];
        }
    }

    public void SetSpawnedBall()
    {
        spawnedBall = BallShooter.currentBall;
    }

    public void ChangeMaterial()
    {
        if (spawnedBall != null && materials.Length > 0)
        {
            Renderer ballRenderer = spawnedBall.GetComponent<Renderer>();
            if (ballRenderer != null)
            {
                ballRenderer.material = materials[currentIndex];
                lastMaterial = materials[currentIndex];
                currentIndex = (currentIndex + 1) % materials.Length;
            }
        }
    }

    public void ButtonPressed()
    {
        SetSpawnedBall();
        ChangeMaterial();
    }

    public static Material GetLastMaterial()
    {
        return lastMaterial;
    }
}
