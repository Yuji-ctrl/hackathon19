//test

using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab; // Inspectorから設定

    void Start()
    {
        // 基本的な生成（位置と回転を指定）
        Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
