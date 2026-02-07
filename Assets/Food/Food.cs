using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Food : MonoBehaviour
{
    public FoodConfig Config { get; private set; }
    IFoodService foodService;

    [SerializeField] float detectRadius = 1f;
    [SerializeField] float resultSpreadRadius = 1f;


    public void Initialize(FoodConfig config, IFoodService service)
    {
        Config = config;
        foodService = service;

        // FBXモデルを子オブジェクトとして読み込み
        var model = service.LoadFBXModel(config.name);
        model.transform.SetParent(transform);
        model.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// アクションを受けた時に呼ぶ。該当レシピがあれば材料を消滅させ成果物を生成する。
    /// </summary>
    public List<Food> ReceiveAction(string action)
    {
        if (foodService == null) return null;

        // 辞書引きで O(1) ルックアップ
        var candidateRecipes = foodService.GetRecipesFor(action, Config.name);
        if (candidateRecipes.Count == 0) return null;

        // 近くのFoodはループの外で1回だけ取得
        var nearbyFoods = FindNearbyFoods();

        foreach (var recipe in candidateRecipes)
        {
            if (!TryMatchRecipe(recipe, nearbyFoods, out var matchedFoods)) continue;

            // 材料を全て消滅
            foreach (var mat in matchedFoods)
                Destroy(mat.gameObject);

            // 成果物を材料の位置に生成（xz平面のランダムオフセット付き）
            return recipe.resultFoods
                .Select((name, index) => 
                {
                    Vector2 randomDir = Random.insideUnitCircle.normalized;
                    Vector3 offset = new Vector3(randomDir.x, 0f, randomDir.y) * resultSpreadRadius;
                    Vector3 spawnPos = transform.position + offset;
                    return foodService.GenerateByName(name, spawnPos);
                })
                .Where(f => f != null)
                .ToList();
        }
        return null;
    }

    List<Food> FindNearbyFoods()
    {
        return Physics.OverlapSphere(transform.position, detectRadius)
            .Select(col => col.GetComponent<Food>())
            .Where(f => f != null)
            .ToList();
    }

    bool TryMatchRecipe(Recipe recipe, List<Food> nearbyFoods, out List<Food> matchedFoods)
    {
        var matched = new List<Food>();
        var remaining = new List<string>(recipe.materialFoods);

        // 自分自身を先にマッチ
        if (remaining.Remove(Config.name))
            matched.Add(this);

        // 残りの材料を近くのFoodからマッチ（消費済みを除外しながら）
        foreach (var need in remaining)
        {
            var found = nearbyFoods.FirstOrDefault(f => !matched.Contains(f) && f.Config.name == need);
            if (found == null) { matchedFoods = matched; return false; }
            matched.Add(found);
        }
        matchedFoods = matched;
        return true;
    }
}