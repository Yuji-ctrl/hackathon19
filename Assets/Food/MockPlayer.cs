
using UnityEngine;

public class MockPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float pickupRadius = 3f;
    [SerializeField] private IFoodService foodService;
    [SerializeField] private Vector3 heldFoodOffset = new Vector3(0, 1f, 0f);

    private Food heldFood = null;

    private void Update()
    {
        HandleMovement();
        HandleFoodInteraction();
    }

    private void HandleMovement()
    {
        // コントローラまたはキーボード入力を取得
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 入力がない場合は処理をスキップ
        if (horizontal == 0 && vertical == 0) return;

        // カメラの前方向と右方向を取得
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // カメラの前方向と右方向をxz平面に投影（y成分を0にする）
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // 移動方向を計算（カメラ基準）
        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        // プレイヤーを移動
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 持っているFoodもプレイヤーについていく（オフセット位置に）
        if (heldFood != null)
        {
            heldFood.transform.position = transform.position + heldFoodOffset;
        }
    }

    private void HandleFoodInteraction()
    {
        // スペースキー入力を検出
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (heldFood == null)
        {
            // Foodを拾い上げる
            PickupFood();
        }
        else
        {
            // Foodを設置して、その場所のFoodに"Mix"アクションを発火
            PlaceFood();
        }
    }

    private void PickupFood()
    {
        // プレイヤーの周辺にあるFoodを検索
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius);
        
        Food closestFood = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            Food food = collider.GetComponent<Food>();
            if (food != null)
            {
                float distance = Vector3.Distance(transform.position, food.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFood = food;
                }
            }
        }

        if (closestFood != null)
        {
            heldFood = closestFood;
            // 拾い上げたFoodを非表示にするか、プレイヤーの手となるよう配置することも可能
            Debug.Log($"Foodを拾い上げました: {heldFood.Config.name}");
        }
    }

    private void PlaceFood()
    {
        if (heldFood == null)
            return;

        // Foodをプレイヤーの位置に設置（オフセットを考慮）
        heldFood.transform.position = transform.position;
        
        // 設置したFoodに"Mix"アクションを発火
        // （周辺のFood検索やマッチングはReceiveAction内で実行）
        var results = heldFood.ReceiveAction("Mix");
        if (results != null && results.Count > 0)
        {
            Debug.Log($"Mix成功: {heldFood.Config.name}が反応しました");
        }

        Debug.Log($"Foodを設置しました: {heldFood.Config.name}");
        heldFood = null;
    }
}