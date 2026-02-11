using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using JetBrains.Annotations;

[System.Serializable]
public class CookConfig{
    public FoodConfig[] foods;
    public Recipe[] recipes;
}

[System.Serializable]
public class FoodConfig
{
    public string name;
    public bool isNatural;
    public float attack;
}

[System.Serializable]
public class Recipe{
    public string[] materialFoods;
    public string action;
    public string[] resultFoods;
}

public interface IFoodService
{
    GameObject LoadFBXModel(string itemName);
    List<Recipe> GetRecipesFor(string action, string foodName);
    Food GenerateByName(string foodName);
    Food GenerateByName(string foodName, Vector3 position);
}

public class FoodGenerator : MonoBehaviour, IFoodService
{
    [SerializeField] TextAsset jsonFile;
    [SerializeField] string foodModelDirPath = "Foods";
    CookConfig config = new();
    [SerializeField] Food foodPrefab;

    /// <summary>パース済みのCookConfigを外部に公開</summary>
    public CookConfig Config => config;

    // (action, materialFood) → 該当レシピの辞書。Awakeで1回だけ構築
    readonly Dictionary<(string action, string food), List<Recipe>> recipeIndex = new();

    // 自然食材の重み付きキャッシュ
    private FoodConfig[] naturalFoods;
    private int[] naturalFoodWeights;
    private int totalWeight;
    
    // 収束速度の係数（小さいほど早く収束、大きいほど遅く収束。上限は常に3）
    [SerializeField] float convergenceRate = 3f;
    [SerializeField] float maxLimit = 3f;

    [SerializeField] RecipeModalUI recipeModal;

    void Awake()
    {
        config = JsonConvert.DeserializeObject<CookConfig>(jsonFile.text);
        BuildRecipeIndex();
        BuildNaturalFoodWeights();

        // モーダルUIにCookConfigを渡して初期化
        if (recipeModal != null)
            recipeModal.InitializeNavigator(config);
    }

    void BuildRecipeIndex()
    {
        foreach (var recipe in config.recipes)
        {
            foreach (var mat in recipe.materialFoods)
            {
                var key = (recipe.action, mat);
                if (!recipeIndex.TryGetValue(key, out var list))
                {
                    list = new List<Recipe>();
                    recipeIndex[key] = list;
                }
                list.Add(recipe);
            }
        }
    }

    void BuildNaturalFoodWeights()
    {
        naturalFoods = System.Array.FindAll(config.foods, f => f.isNatural);
        if (naturalFoods.Length == 0) return;
        
        naturalFoodWeights = new int[naturalFoods.Length];
        totalWeight = 0;
        
        for (int i = 0; i < naturalFoods.Length; i++)
        {
            var foodName = naturalFoods[i].name;
            var visited = new HashSet<string>();
            
            // この食材から到達できる末端レシピ（最終成果物）の数をカウント
            int terminalRecipeCount = CountTerminalRecipes(foodName, visited);
            
            // レシピ数+1に収束関数を適用（上限3に固定、収束速度はconvergenceRateで調整）
            float x = terminalRecipeCount + 1;
            float normalizedWeight = maxLimit * x / (x + convergenceRate);
            naturalFoodWeights[i] = Mathf.Max(1, Mathf.RoundToInt(normalizedWeight * 100));
            totalWeight += naturalFoodWeights[i];
        }
        
        // ウエイトランキングをログ出力
        LogWeightRanking();
    }

    int CountTerminalRecipes(string foodName, HashSet<string> visited)
    {
        if (visited.Contains(foodName)) return 0; // 循環参照を防ぐ
        visited.Add(foodName);
        
        int count = 0;
        
        // この食材を材料として使うレシピを探す
        foreach (var recipe in config.recipes)
        {
            if (System.Array.Exists(recipe.materialFoods, m => m == foodName))
            {
                // このレシピの結果食材を確認
                foreach (var resultFood in recipe.resultFoods)
                {
                    // 結果食材が他のレシピの材料になっているか確認
                    bool isUsedInOtherRecipe = false;
                    foreach (var checkRecipe in config.recipes)
                    {
                        if (System.Array.Exists(checkRecipe.materialFoods, m => m == resultFood))
                        {
                            isUsedInOtherRecipe = true;
                            break;
                        }
                    }
                    
                    if (isUsedInOtherRecipe)
                    {
                        // まだ他のレシピで使われるので、再帰的に探索
                        count += CountTerminalRecipes(resultFood, new HashSet<string>(visited));
                    }
                    else
                    {
                        // これが末端レシピ（最終成果物）
                        count++;
                    }
                }
            }
        }
        
        return count;
    }

    void LogWeightRanking()
    {
        var foodWeightPairs = new List<(string name, int weight)>();
        for (int i = 0; i < naturalFoods.Length; i++)
        {
            foodWeightPairs.Add((naturalFoods[i].name, naturalFoodWeights[i]));
        }
        
        foodWeightPairs.Sort((a, b) => b.weight.CompareTo(a.weight));
        
        string log = "=== Natural Food Weight Ranking ===\n";
        for (int i = 0; i < foodWeightPairs.Count; i++)
        {
            log += $"{i + 1}. {foodWeightPairs[i].name}: {foodWeightPairs[i].weight}\n";
        }
        log += $"Total Weight: {totalWeight}";
        
        Debug.Log(log);
    }



    public Food GenerateNatural()
    {
        if (naturalFoods == null || naturalFoods.Length == 0) return null;
        
        var food = Instantiate(foodPrefab);
        
        // 事前計算済みの重みを使って選択
        int randomValue = Random.Range(0, totalWeight);
        int cumulative = 0;
        int selectedIndex = 0;
        
        for (int i = 0; i < naturalFoodWeights.Length; i++)
        {
            cumulative += naturalFoodWeights[i];
            if (randomValue < cumulative)
            {
                selectedIndex = i;
                break;
            }
        }
        
        var randomConfig = naturalFoods[selectedIndex];
        food.Initialize(randomConfig, this);
        return food;
    }

    public Food GenerateByName(string foodName)
    {
        return GenerateByName(foodName, Vector3.zero);
    }

    public Food GenerateByName(string foodName, Vector3 position)
    {
        var foodConfig = System.Array.Find(config.foods, f => f.name == foodName);
        if (foodConfig == null) return null;
        var food = Instantiate(foodPrefab);
        food.transform.position = position;
        food.Initialize(foodConfig, this);
        return food;
    }

    static readonly List<Recipe> emptyRecipes = new();

    public List<Recipe> GetRecipesFor(string action, string foodName)
    {
        return recipeIndex.TryGetValue((action, foodName), out var list) ? list : emptyRecipes;
    }

    public GameObject LoadFBXModel(string itemName)
    {
        GameObject fbxAsset = Resources.Load<GameObject>($"{foodModelDirPath}/{itemName}");
        if (fbxAsset != null)
        {
            return Instantiate(fbxAsset);
        }
        else
        {
            throw new System.Exception($"見つかりません: {itemName}");
        }
    }
}