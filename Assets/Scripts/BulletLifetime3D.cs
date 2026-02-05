using UnityEngine;

public class BulletLifetime3D : MonoBehaviour
{
    [SerializeField] private float lifeSeconds = 300f;

    private void OnEnable()
    {
        Destroy(gameObject, lifeSeconds);
    }
}
