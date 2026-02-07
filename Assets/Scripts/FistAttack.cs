using UnityEngine;

public class FistAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;  // ダメージ量

    private void OnTriggerEnter(Collider other)
    {
        // 敵にCharacterHealthがあるかチェック
        CharacterHealth health = other.GetComponent<CharacterHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);  // ダメージ適用
            Destroy(gameObject);  // 拳を消去（オプション）
        }
    }
}
