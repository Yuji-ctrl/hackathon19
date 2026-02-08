using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    private int currentHp;

    public int CurrentHP => currentHp; // HPBoard.csから参照用
    public int MaxHP => maxHp;

    public UnityEvent<int> OnHpChanged;
    public UnityEvent OnDie;

    void Start()
    {
        currentHp = maxHp;
        OnHpChanged?.Invoke(currentHp);
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        OnHpChanged?.Invoke(currentHp);

        if (currentHp <= 0)
        {
            OnDie?.Invoke();
        }
    }

    // ★ここから追加（どちらか片方だけ残す）

    // 1) コライダーが Trigger でない場合
    private void OnCollisionEnter(Collision collision)
    {
        // 弾に "Bullet" タグを付けておく
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10); // とりあえず 10 ダメージ
        }
    }

    // 2) コライダーが Trigger の場合
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(10);
        }
    }
}
