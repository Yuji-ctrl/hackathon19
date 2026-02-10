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
            // オブジェクトをターゲット方向に回転（正面を向ける）
            transform.LookAt(target.position);

            // forward方向（発射時点の敵向き）に速度を設定、直進
            rb.linearVelocity = transform.forward * speed;
        }
        else
        {
            // ターゲット未設定時はデフォルト方向
            rb.linearVelocity = -Vector3.right * speed;
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
