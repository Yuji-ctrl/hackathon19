using System.Collections;
using UnityEngine;

public class SimpleAutoShooter : MonoBehaviour
{
    [Header("�ݒ�")]
    [SerializeField] private GameObject bulletPrefab; // �e��Prefab
    [SerializeField] private Transform muzzle;        // ���ˈʒu�i��Ȃ玩���̈ʒu�j
    [SerializeField] private float speed = 500f;       // �e�̑���
    [SerializeField] private float interval = 3.0f;   // ���ˊԊu�i�b�j

    private void OnEnable()
    {
        // �I�u�W�F�N�g���L���ɂȂ����甭�˃��[�v���J�n
        StartCoroutine(ShootLoop());
    }

    private IEnumerator ShootLoop()
    {
        while (true)
        {
            // 1. ���ˈʒu�����߂�imuzzle�����ݒ�Ȃ玩���̈ʒu�j
            Transform originTransform = (muzzle != null) ? muzzle : transform;

            // 2. �e�𐶐��i�ʒu�Ɖ�]�͔��ˌ��ɍ��킹��j
            GameObject bullet = Instantiate(bulletPrefab, originTransform.position, originTransform.rotation);

            // 3. �O���i�����̕����j�֔�΂�
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false; // �d�͖����i�܂�������΂����߁j
                // Unity 6�ȍ~�� linearVelocity�A�Â��o�[�W������ velocity
                //rb.linearVelocity = originTransform.right * speed;
                rb.linearVelocity = originTransform.right * speed;

            }

            // 4. �w�莞�ԑ҂i1�b�j
            yield return new WaitForSeconds(interval);
        }
    }
}
