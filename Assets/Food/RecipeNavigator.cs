using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 指定した食材から作れるレシピツリーを探索するユーティリティ。
/// </summary>
public class RecipeNavigator
{
    readonly CookConfig config;

    // foodName → そのfoodを材料に含むレシピ一覧
    readonly Dictionary<string, List<Recipe>> materialToRecipes = new();

    // foodName → FoodConfig
    readonly Dictionary<string, FoodConfig> foodLookup = new();

    public RecipeNavigator(CookConfig config)
    {
        this.config = config;

        foreach (var food in config.foods)
            foodLookup[food.name] = food;

        foreach (var recipe in config.recipes)
        {
            foreach (var mat in recipe.materialFoods)
            {
                if (!materialToRecipes.TryGetValue(mat, out var list))
                {
                    list = new List<Recipe>();
                    materialToRecipes[mat] = list;
                }
                list.Add(recipe);
            }
        }
    }

    /// <summary>
    /// 指定した食材から直接作れるレシピ一覧を返す。
    /// </summary>
    public List<Recipe> GetDirectRecipes(string foodName)
    {
        return materialToRecipes.TryGetValue(foodName, out var list) ? list : new List<Recipe>();
    }

    /// <summary>
    /// 指定した食材から到達可能な全レシピツリー（深さ優先）を返す。
    /// 各ノードは (レシピ, 深さ) のペア。循環を防止する。
    /// </summary>
    public List<RecipeNode> GetReachableRecipeTree(string foodName, int maxDepth = 5)
    {
        var result = new List<RecipeNode>();
        var visited = new HashSet<string> { foodName };
        CollectRecipes(foodName, 0, maxDepth, visited, result);
        return result;
    }

    void CollectRecipes(string foodName, int currentDepth, int maxDepth, HashSet<string> visited, List<RecipeNode> result)
    {
        if (currentDepth >= maxDepth) return;

        var recipes = GetDirectRecipes(foodName);
        foreach (var recipe in recipes)
        {
            var node = new RecipeNode
            {
                Recipe = recipe,
                Depth = currentDepth,
                CanCraft = false // UIで近くの素材チェック時に更新
            };
            result.Add(node);

            // 成果物からさらに作れるレシピを探索
            foreach (var resultFood in recipe.resultFoods)
            {
                if (visited.Add(resultFood))
                {
                    CollectRecipes(resultFood, currentDepth + 1, maxDepth, visited, result);
                }
            }
        }
    }

    /// <summary>
    /// FoodConfigを名前で取得。
    /// </summary>
    public FoodConfig GetFoodConfig(string name)
    {
        return foodLookup.TryGetValue(name, out var cfg) ? cfg : null;
    }
}

public class RecipeNode
{
    public Recipe Recipe;
    public int Depth;
    public bool CanCraft;
}
