using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBallMaterial : MonoBehaviour
{
    public Material[] materials; 
    private GameObject spawnedBall;
    public static int currentIndex = 0;
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
                currentIndex = (currentIndex + 1) % materials.Length;               
            }
        }
    }
    public  void Buttonpressed()
    {    
        SetSpawnedBall(); 
        ChangeMaterial();
    }
}
