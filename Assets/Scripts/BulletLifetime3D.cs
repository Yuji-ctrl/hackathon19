using UnityEngine;

public class BulletLifetime3D : MonoBehaviour
{
    [SerializeField] private float lifeSeconds = 8f;

    private void OnEnable()
    {
        Destroy(gameObject, lifeSeconds);
    }
}
