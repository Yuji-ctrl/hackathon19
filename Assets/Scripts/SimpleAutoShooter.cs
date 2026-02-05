using System.Collections;
using UnityEngine;

public class SimpleAutoShooter : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private GameObject bulletPrefab; // 弾のPrefab
    [SerializeField] private Transform muzzle;        // 発射位置（空なら自分の位置）
    [SerializeField] private float speed = 15f;       // 弾の速さ
    [SerializeField] private float interval = 1.0f;   // 発射間隔（秒）

    private void OnEnable()
    {
        // オブジェクトが有効になったら発射ループを開始
        StartCoroutine(ShootLoop());
    }

    private IEnumerator ShootLoop()
    {
        while (true)
        {
            // 1. 発射位置を決める（muzzleが未設定なら自分の位置）
            Transform originTransform = (muzzle != null) ? muzzle : transform;

            // 2. 弾を生成（位置と回転は発射口に合わせる）
            GameObject bullet = Instantiate(bulletPrefab, originTransform.position, originTransform.rotation);

            // 3. 前方（青い矢印の方向）へ飛ばす
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false; // 重力無効（まっすぐ飛ばすため）
                // Unity 6以降は linearVelocity、古いバージョンは velocity
                rb.linearVelocity = originTransform.right * speed;
            }

            // 4. 指定時間待つ（1秒）
            yield return new WaitForSeconds(interval);
        }
    }
}
