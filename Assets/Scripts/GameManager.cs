using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyBarrageShooter3D enemyShooter;
    [SerializeField] private KnifeShooter knifeShooter;
    // 食べ物生成スクリプトもあれば同じように参照を持つ
    // [SerializeField] private FoodSpawner foodSpawner;

    private bool isGameStarted = false;

    public void OnStartButtonPressed()
    {
        if (isGameStarted) return;
        isGameStarted = true;

        // ここで弾幕と食べ物生成を開始
        if (enemyShooter != null)
        {
            enemyShooter.StartFire();
        }
        if(knifeShooter != null) { knifeShooter.StartFire(); }

        // if (foodSpawner != null) foodSpawner.StartSpawn();
    }
}
