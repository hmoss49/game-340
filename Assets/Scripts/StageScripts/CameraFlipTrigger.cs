using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraFlipTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var camSetter = FindFirstObjectByType<CameraTargetSetter>();
            if (camSetter != null)
                camSetter.FlipScreenX();
        }
    }
}