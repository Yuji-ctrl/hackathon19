using System.Collections;
using UnityEngine;

public class KnifeShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzle;//銃口

    [Header("Barrage")]
    [SerializeField] private int bulletsPerWave = 1;
    [SerializeField] private float wavesPerSecond = 1f;
    [SerializeField] private float bulletSpeed = 12f;

    [Header("Y constraint")]
    [SerializeField] private float fixedY = 0f;
    [SerializeField] private bool lockBulletY = true;

    // スタートボタンと連動させるので、OnEnableでは自動開始しない
    [SerializeField] private bool startOnEnable = false;

    private Coroutine loop;

    // ここは空、または startOnEnable が true のときだけ開始したいなら残してもOK
    private void OnEnable()
    {
        if (startOnEnable)
        {
            StartFire();
        }
    }

    private void OnDisable()
    {
        StopFire();
    }

    // GameManager などから呼ぶ用
    public void StartFire()
    {
        if (loop == null)
        {
            loop = StartCoroutine(FireLoop());
        }
    }

    public void StopFire()
    {
        if (loop != null)
        {
            StopCoroutine(loop);
            loop = null;
        }
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
        if (lockBulletY)
        {
            origin.y = fixedY;
        }

        for (int i = 0; i < bulletsPerWave; i++)
        {
            Vector3 dir = GetRandomDirectionXZ();

            GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.identity);

            var rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = dir * bulletSpeed;
                rb.useGravity = false;
            }

            bullet.transform.rotation = Quaternion.Euler(0,0,270);
        }
    }

    private Vector3 GetRandomDirectionXZ()
    {
        Vector2 circle;
        do
        {
            circle = Random.insideUnitCircle;
        } while (circle.x <= 0f);

        circle.Normalize();
        return new Vector3(circle.x, 0f, circle.y);
    }
}
