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

    // (action, materialFood) → 該当レシピの辞書。Awakeで1回だけ構築
    readonly Dictionary<(string action, string food), List<Recipe>> recipeIndex = new();

    void Awake()
    {
        config = JsonConvert.DeserializeObject<CookConfig>(jsonFile.text);
        BuildRecipeIndex();
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

    public Food Generate()
    {
        var food = Instantiate(foodPrefab);
        var randomConfig = config.foods[Random.Range(0, config.foods.Length)];
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