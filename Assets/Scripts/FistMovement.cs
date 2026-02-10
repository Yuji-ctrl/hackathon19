using UnityEngine;

public class FistMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private GameObject target;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Enemy");//インスペクターで指定できないのでタグで検索

        if (target != null)
        {
            // オブジェクトをターゲット方向に回転（正面を向ける）
            transform.LookAt(target.transform.position);

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
        //if (transform.position.magnitude > 50f)
        //{
        //    Destroy(gameObject);
        //}
        //FistAttackの方で消してくれるので多分ここで消さなくてもいい
    }
}
