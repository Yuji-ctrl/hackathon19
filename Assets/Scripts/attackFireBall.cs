using System.Collections;
using UnityEngine;

public class EnemyBarrageShooter3D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzle;//銃口

    [Header("Barrage")]
    [SerializeField] private int bulletsPerWave = 1;
    [SerializeField] private float wavesPerSecond = 1f;
    [SerializeField] private float bulletSpeed = 12f;

    [Header("Y constraint")]
    [SerializeField] private float fixedY = 0f; // 弾の高さ（Y座標）を固定
    [SerializeField] private bool lockBulletY = true; // trueで弾が常にY固定

    [SerializeField] private bool startOnEnable = true;

    private Coroutine loop;

    private void OnEnable()
    {
        if (startOnEnable) loop = StartCoroutine(FireLoop());
    }

    private void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
    }

    private IEnumerator FireLoop()
    {
        var wait = new WaitForSeconds(1f / Mathf.Max(0.01f, wavesPerSecond));
        while (true)
        {
            FireWave();
            yield return wait;
        }
    }

    private void FireWave()
    {
        Vector3 origin = (muzzle != null) ? muzzle.position : transform.position;
        // Y座標を固定したい場合、発射位置のYを上書き
        if (lockBulletY)
        {
            origin.y = fixedY;
        }

        for (int i = 0; i < bulletsPerWave; i++)
        {
            // XZ平面上のランダム方向（Y=0の水平面）
            Vector3 dir = GetRandomDirectionXZ();

            GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.identity);

            // Rigidbodyで速度を与える
            var rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = dir * bulletSpeed;
                // 重力を使いたくない場合
                rb.useGravity = false;
            }

            // 弾の向きを進行方向に合わせる（弾モデルが前方=Z軸の前提）
            bullet.transform.rotation = Quaternion.LookRotation(dir);

            // Y固定を維持したいなら、弾に FixYPosition スクリプトを付ける（後述）
        }
    }

    /// <summary>
    /// XZ平面上（Y=0）のランダム方向ベクトルを返す
    /// </summary>
    private Vector3 GetRandomDirectionXZ()
    {
        Vector2 circle;
        do
        {
            circle = Random.insideUnitCircle;
            // x > 0（右半分）に限定。x >= 0 で前方半円状
        } while (circle.x <= 0f);  // x > 0 を厳密にしたいなら < 0f

        circle.Normalize();  // 円周上の方向ベクトルに
        return new Vector3(circle.x, 0f, circle.y);
    }

}
