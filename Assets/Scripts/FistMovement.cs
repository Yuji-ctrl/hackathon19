using UnityEngine;

public class FistMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform target; // panCreature をインスペクタで指定
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (target != null)
        {
            // ターゲット方向の正規化ベクトル
            Vector3 dir = (target.position - transform.position).normalized;
            rb.linearVelocity = dir * speed;
        }
        else
        {
            // ターゲット未設定時は自分のforward
            rb.linearVelocity = transform.forward * speed;
        }
    }

    void Update()
    {
        if (transform.position.magnitude > 50f)
        {
            Destroy(gameObject);
        }
    }
}
