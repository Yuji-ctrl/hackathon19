using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] Transform spawnArea;
    [SerializeField] int gridPerEdge = 10;
    [SerializeField] Transform playerPosition;
    [SerializeField] FoodGenerator foodGenerator;
    
    [Header("Spawn Settings")]
    [SerializeField] float spawnInterval = 2f;
    [SerializeField] int maxFoodCount = 20;
    [SerializeField] int playerExclusionRadius = 1; // 3x3エリア（中心から半径1）
    
    private Vector2[,] gridPositions;
    private GameObject[,] gridOccupancy;
    private Coroutine spawnCoroutine;
    private int activeFoodCount = 0;
    
    private void Start()
    {
        InitializeGrid();
        StartSpawning();
    }
    
    private void InitializeGrid()
    {
        gridPositions = new Vector2[gridPerEdge, gridPerEdge];
        gridOccupancy = new GameObject[gridPerEdge, gridPerEdge];
        
        Vector3 areaScale = spawnArea.localScale;
        Vector3 areaCenter = spawnArea.position;
        
        float cellSizeX = areaScale.x / gridPerEdge;
        float cellSizeZ = areaScale.z / gridPerEdge;
        
        for (int x = 0; x < gridPerEdge; x++)
        {
            for (int z = 0; z < gridPerEdge; z++)
            {
                float worldX = areaCenter.x - (areaScale.x / 2f) + (x + 0.5f) * cellSizeX;
                float worldZ = areaCenter.z - (areaScale.z / 2f) + (z + 0.5f) * cellSizeZ;
                
                gridPositions[x, z] = new Vector2(worldX, worldZ);
                gridOccupancy[x, z] = null;
            }
        }
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
            
            // 破壊されたフードをグリッドから除去
            ScanForDestroyedFoods();
            
            if (activeFoodCount < maxFoodCount)
            {
                SpawnFood();
            }
        }
    }
    
    private void ScanForDestroyedFoods()
    {
        for (int x = 0; x < gridPerEdge; x++)
        {
            for (int z = 0; z < gridPerEdge; z++)
            {
                GameObject food = gridOccupancy[x, z];
                if (food != null && !food) // Unity destroyed object check
                {
                    gridOccupancy[x, z] = null;
                    activeFoodCount--;
                }
            }
        }
    }
    
    private void SpawnFood()
    {
        List<Vector2Int> availableCells = GetAvailableCells();
        
        if (availableCells.Count == 0)
        {
            return;
        }
        
        // ランダムなセルを選択
        Vector2Int randomCell = availableCells[Random.Range(0, availableCells.Count)];
        Vector2 worldPos = gridPositions[randomCell.x, randomCell.y];
        
        // フードを生成
        GameObject food = foodGenerator.Generate()?.gameObject;
        if (food != null)
        {
            food.transform.position = new Vector3(worldPos.x, spawnArea.position.y, worldPos.y);
            
            // グリッドに登録
            gridOccupancy[randomCell.x, randomCell.y] = food;
            activeFoodCount++;
        }
    }
    
    private List<Vector2Int> GetAvailableCells()
    {
        List<Vector2Int> availableCells = new List<Vector2Int>();
        
        // プレイヤーのグリッド位置を計算
        Vector2Int playerGridPos = WorldToGridPosition(playerPosition.position);
        
        for (int x = 0; x < gridPerEdge; x++)
        {
            for (int z = 0; z < gridPerEdge; z++)
            {
                // プレイヤー周辺の3x3エリアを除外
                if (IsInPlayerExclusionZone(x, z, playerGridPos))
                {
                    continue;
                }
                
                // セルが空いているかO(1)でチェック
                if (gridOccupancy[x, z] == null)
                {
                    availableCells.Add(new Vector2Int(x, z));
                }
            }
        }
        
        return availableCells;
    }
    
    private Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3 areaScale = spawnArea.localScale;
        Vector3 areaCenter = spawnArea.position;
        
        float cellSizeX = areaScale.x / gridPerEdge;
        float cellSizeZ = areaScale.z / gridPerEdge;
        
        float localX = worldPosition.x - (areaCenter.x - areaScale.x / 2f);
        float localZ = worldPosition.z - (areaCenter.z - areaScale.z / 2f);
        
        int gridX = Mathf.Clamp(Mathf.FloorToInt(localX / cellSizeX), 0, gridPerEdge - 1);
        int gridZ = Mathf.Clamp(Mathf.FloorToInt(localZ / cellSizeZ), 0, gridPerEdge - 1);
        
        return new Vector2Int(gridX, gridZ);
    }
    
    private bool IsInPlayerExclusionZone(int gridX, int gridZ, Vector2Int playerGridPos)
    {
        int deltaX = Mathf.Abs(gridX - playerGridPos.x);
        int deltaZ = Mathf.Abs(gridZ - playerGridPos.y);
        
        return deltaX <= playerExclusionRadius && deltaZ <= playerExclusionRadius;
    }
    
    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    
    public void ClearAllFoods()
    {
        for (int x = 0; x < gridPerEdge; x++)
        {
            for (int z = 0; z < gridPerEdge; z++)
            {
                if (gridOccupancy[x, z] != null)
                {
                    Destroy(gridOccupancy[x, z]);
                    gridOccupancy[x, z] = null;
                }
            }
        }
        activeFoodCount = 0;
    }
    
    public int GetActiveFoodCount()
    {
        ScanForDestroyedFoods();
        return activeFoodCount;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (spawnArea == null) return;
        
        // スポーンエリアの描画
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnArea.position, spawnArea.localScale);
        
        // グリッドの描画
        if (gridPositions != null)
        {
            Gizmos.color = Color.white;
            float cellSizeX = spawnArea.localScale.x / gridPerEdge;
            float cellSizeZ = spawnArea.localScale.z / gridPerEdge;
            
            for (int x = 0; x < gridPerEdge; x++)
            {
                for (int z = 0; z < gridPerEdge; z++)
                {
                    Vector2 cellPos = gridPositions[x, z];
                    Vector3 worldPos = new Vector3(cellPos.x, spawnArea.position.y, cellPos.y);
                    
                    // 占有されているセルは青で表示
                    if (gridOccupancy != null && gridOccupancy[x, z] != null)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawCube(worldPos, new Vector3(cellSizeX * 0.8f, 0.1f, cellSizeZ * 0.8f));
                        Gizmos.color = Color.white;
                    }
                    
                    Gizmos.DrawWireCube(worldPos, new Vector3(cellSizeX, 0.1f, cellSizeZ));
                }
            }
        }
        
        // プレイヤー除外エリアの描画
        if (playerPosition != null && gridPositions != null)
        {
            Gizmos.color = Color.red;
            Vector2Int playerGridPos = WorldToGridPosition(playerPosition.position);
            float cellSizeX = spawnArea.localScale.x / gridPerEdge;
            float cellSizeZ = spawnArea.localScale.z / gridPerEdge;
            
            for (int x = -playerExclusionRadius; x <= playerExclusionRadius; x++)
            {
                for (int z = -playerExclusionRadius; z <= playerExclusionRadius; z++)
                {
                    int gridX = playerGridPos.x + x;
                    int gridZ = playerGridPos.y + z;
                    
                    if (gridX >= 0 && gridX < gridPerEdge && gridZ >= 0 && gridZ < gridPerEdge)
                    {
                        Vector2 cellPos = gridPositions[gridX, gridZ];
                        Vector3 worldPos = new Vector3(cellPos.x, spawnArea.position.y + 0.1f, cellPos.y);
                        Gizmos.DrawCube(worldPos, new Vector3(cellSizeX, 0.1f, cellSizeZ));
                    }
                }
            }
        }
    }
    #endif
}