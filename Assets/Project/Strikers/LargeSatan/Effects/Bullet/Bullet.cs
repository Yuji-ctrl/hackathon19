using UnityEngine;

namespace Core.LargeSatan {

    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour {
        [SerializeField] float damage = 10f;
        [SerializeField] float nockbackSpeed = 10f;
        [SerializeField] float speed = 20;
        Rigidbody rb;

        [SerializeField] GameObject rotationTarget;
        [SerializeField] float rotationSpeed = 700;

        [SerializeField] GameObject impactPrefab;
        [SerializeField] GameObject trail;


        void Awake() {
            rb = GetComponent<Rigidbody>();
        }

        void Start() {
            rb.linearVelocity = speed * rb.transform.forward;
        }

        void Update() {
            rotationTarget.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
