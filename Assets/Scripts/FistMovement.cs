// FistMovement.cs �����v���n�u�ɃA�^�b�`
using UnityEngine;

public class FistMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;  // �����ɑO���ړ�
    }

    // �I�v�V����: ��莞�Ԍ�Destroy
    void Update()
    {
        if (transform.position.magnitude > 50f) Destroy(gameObject);
    }
}
