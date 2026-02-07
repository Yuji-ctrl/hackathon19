using UnityEngine;

public class MockActionBall : MonoBehaviour
{
    [SerializeField] string action = "Cut";

  public void OnTriggerEnter(Collider other)
  {
        if (other.TryGetComponent<Food>(out var food))
        {
            food.ReceiveAction(action);
        }
  }
}