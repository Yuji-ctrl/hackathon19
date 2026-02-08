using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    private int currentHp;

    public int CurrentHP => currentHp; // HPBoard.cs‚©‚çŽQÆ—p
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
}
