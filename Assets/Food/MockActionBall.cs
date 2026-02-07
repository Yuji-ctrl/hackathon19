using UnityEngine;

public class MockActionBall : MonoBehaviour
{
    [SerializeField] string action = "Cut";
    [SerializeField] float cooldownTime = 0.5f; // クールタイム（秒）
    
    private float lastActionTime = -999f; // 最後にアクションを実行した時刻

  public void OnTriggerEnter(Collider other)
  {
        // クールタイムチェック
        if (Time.time - lastActionTime < cooldownTime)
        {
            return;
        }

        if (other.TryGetComponent<Food>(out var food))
        {
            food.ReceiveAction(action);
            lastActionTime = Time.time; // アクション実行時刻を記録
        }
  }
}