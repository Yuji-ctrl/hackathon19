using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour
{
    // インスペクターで設定できる最大HP（初期値）
    [SerializeField] private int maxHp = 100;

    // 現在のHP（実行中に変動する値）
    private int currentHp;

    // HPが変化したときにUIなどを更新するためのイベント
    public UnityEvent<int> OnHpChanged;
    public UnityEvent OnDie;

    void Start()
    {
        // ゲーム開始時にHPをリセット
        currentHp = maxHp;
        // 初期状態を通知
        OnHpChanged?.Invoke(currentHp);
    }

    // ダメージを受ける処理
    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        // HPが0未満にならないようにする
        currentHp = Mathf.Max(currentHp, 0);

        // 変更を通知（UIバーの更新など）
        OnHpChanged?.Invoke(currentHp);

        // 0になったら死亡イベント発火
        if (currentHp <= 0)
        {
            OnDie?.Invoke();
        }
    }
}
