using System.Collections;
using UnityEngine;

public class SimpleAutoShooter : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private GameObject bulletPrefab; // 弾Prefab
    [SerializeField] private Transform muzzle;        // 発射位置（空なら自分の位置）
    [SerializeField] private float speed = 15f;       // 弾の速さ（500は大きすぎ）
    [SerializeField] private float interval = 3.0f;   // 発射間隔（秒）

    private void OnEnable()
    {
        StartCoroutine(ShootLoop());
    }

    private IEnumerator ShootLoop()
    {
        while (true)
        {
            Transform originTransform = (muzzle != null) ? muzzle : transform;

            GameObject bullet = Instantiate(bulletPrefab, originTransform.position, originTransform.rotation);

            // LayerをBulletに変更（衝突回避）
            bullet.layer = LayerMask.NameToLayer("Bullet");

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;

                // 🚨 修正1: Unity 6以前は velocity を使用
#if UNITY_6_0_OR_NEWER
                rb.linearVelocity = originTransform.right * speed;
#else
                rb.linearVelocity = originTransform.right * speed;  // ← これが正解
#endif
            }
            else
            {
                Debug.LogError("弾PrefabにRigidbodyがありません！", bullet);
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
