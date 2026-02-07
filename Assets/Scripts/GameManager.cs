// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsGameStarted { get; private set; } = false;

    public void StartGame()
    {
        IsGameStarted = true;
        // 必要ならここで敵スポーン用コルーチン開始など
    }
}
