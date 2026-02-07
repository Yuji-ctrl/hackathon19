using UnityEngine;
using TMPro;

public class HPBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private TextMeshProUGUI enemyHPText;

    [SerializeField] private MockPlayer mockPlayer;
    [SerializeField] private GameObject enemyObject; // panCreature
    private Health enemyHealth; // 敵の Health 参照

    private float playerHP;
    private float maxPlayerHP;

    private void Start()
    {
        // プレイヤーHPの初期化
        maxPlayerHP = 100f; // 必要に応じて変更
        playerHP = maxPlayerHP;

        // 敵のHealthコンポーネントを取得
        if (enemyObject != null)
        {
            enemyHealth = enemyObject.GetComponent<Health>();
        }

        UpdateHPDisplay();
    }

    private void Update()
    {
        UpdateHPDisplay();
    }

    private void UpdateHPDisplay()
    {
        // プレイヤーHP表示
        playerHPText.text = $"Player HP: {playerHP:F0} / {maxPlayerHP:F0}";

        // 敵HP表示
        if (enemyHealth != null)
        {
            enemyHPText.text = $"Enemy HP: {enemyHealth.CurrentHP:F0} / {enemyHealth.MaxHP:F0}";
        }
        else
        {
            enemyHPText.text = "Enemy HP: --";
        }
    }

    // プレイヤーがダメージを受けたときに呼び出す
    public void TakeDamage(float damage)
    {
        playerHP -= damage;
        playerHP = Mathf.Clamp(playerHP, 0, maxPlayerHP);

        if (playerHP <= 0)
        {
            Debug.Log("プレイヤーが倒れました!");
        }
    }

    // プレイヤーがHP回復したときに呼び出す
    public void Heal(float healAmount)
    {
        playerHP += healAmount;
        playerHP = Mathf.Clamp(playerHP, 0, maxPlayerHP);
    }
}