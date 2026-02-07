using System.Collections;
using UnityEngine;

public class RandomPrefabSpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    [SerializeField] private GameObject[] prefabs;     // 複数PrefabをInspectorで登録
    [SerializeField] private Transform spawnPoint;     // スポーンさせたい場所
    [SerializeField] private float startDelay = 0f;    // 開始までの待ち時間
    [SerializeField] private float interval = 1f;      // スポーン間隔(秒)
    [SerializeField] private bool usePrefabRotation = false;

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
    }

    private IEnumerator SpawnLoop()
    {
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        while (true)
        {
            SpawnOnce();
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnOnce()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("prefabs が未設定、または要素数0です。");
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogWarning("spawnPoint が未設定です。");
            return;
        }

        int index = Random.Range(0, prefabs.Length);
        GameObject prefab = prefabs[index];

        Quaternion rot = usePrefabRotation ? prefab.transform.rotation : Quaternion.identity;
        Instantiate(prefab, spawnPoint.position, rot);
    }
}
