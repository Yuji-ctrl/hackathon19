using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] Transform spawnArea;
    [SerializeField] Transform playerPosition;
    [SerializeField] FoodGenerator foodGenerator;
    
    [Header("Spawn Settings")]
    [SerializeField] float spawnInterval = 2f;
    [SerializeField] int maxFoodCount = 50;
    
    private Coroutine spawnCoroutine;
    private List<GameObject> spawnedFoods = new List<GameObject>();
    
    private void Start()
    {
        StartSpawning();
    }
    
    private void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }
    
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnFood();
        }
    }
    
    private void SpawnFood()
    {
        // リストから破棄されたオブジェクトを削除
        spawnedFoods.RemoveAll(food => food == null);
        
        // 上限チェック
        if (spawnedFoods.Count >= maxFoodCount)
        {
            return;
        }
        
        // エリア内のランダムな位置を取得
        Vector3 spawnPosition = GetRandomPositionInArea();
        
        // フードを生成
        GameObject food = foodGenerator.GenerateNatural()?.gameObject;
        if (food != null)
        {
            food.transform.position = spawnPosition;
            spawnedFoods.Add(food);
        }
    }
    
    private Vector3 GetRandomPositionInArea()
    {
        Vector3 areaScale = spawnArea.localScale;
        Vector3 areaCenter = spawnArea.position;
        
        float randomX = Random.Range(-areaScale.x / 2f, areaScale.x / 2f);
        float randomZ = Random.Range(-areaScale.z / 2f, areaScale.z / 2f);
        
        Vector3 spawnPos = areaCenter + new Vector3(randomX, 0, randomZ);
        spawnPos.y = areaCenter.y;
        
        return spawnPos;
    }
    
    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        spawnedFoods.Clear();
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (spawnArea == null) return;
        
        // スポーンエリアの描画
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnArea.position, spawnArea.localScale);
    }
    #endif
}