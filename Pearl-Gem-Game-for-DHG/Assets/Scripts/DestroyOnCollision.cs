using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Renderer otherRenderer = collision.gameObject.GetComponent<Renderer>();
        Renderer thisRenderer = GetComponent<Renderer>();
        if (otherRenderer != null && thisRenderer != null)
        {
            if (otherRenderer.sharedMaterial.name ==  thisRenderer.sharedMaterial.name)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
