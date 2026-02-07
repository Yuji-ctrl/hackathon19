using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// レシピ表示UI。手に持っている食材の関連レシピを自動表示する。
/// フレームなし、スクロールなし、ボタンなし。
/// </summary>
public class RecipeModalUI : MonoBehaviour
{
    [SerializeField] Transform contentParent;
    [SerializeField] GameObject recipeItemPrefab;

    RecipeNavigator navigator;
    readonly List<GameObject> spawnedItems = new();
    string currentFoodName;

    void Start()
    {
        contentParent.gameObject.SetActive(false);
    }

    public void InitializeNavigator(CookConfig config)
    {
        navigator = new RecipeNavigator(config);
    }

    /// <summary>
    /// 指定した食材のレシピを表示する。持った瞬間に呼ぶ。
    /// </summary>
    public void Show(Food food)
    {
        if (navigator == null || food == null) return;
        if (currentFoodName == food.Config.name) return;
        currentFoodName = food.Config.name;

        ClearItems();
        contentParent.gameObject.SetActive(true);

        var recipes = navigator.GetDirectRecipes(food.Config.name);
        // 材料が多い順にソート
        var sortedRecipes = recipes.OrderByDescending(r => r.materialFoods.Length).ToList();
        foreach (var recipe in sortedRecipes)
        {
            var go = Instantiate(recipeItemPrefab, contentParent);
            spawnedItems.Add(go);
            var item = go.GetComponent<RecipeModalItem>();
            if (item != null)
                item.Setup(recipe);
        }

        // レシピが0件なら非表示のまま
        if (recipes.Count == 0)
            contentParent.gameObject.SetActive(false);
    }

    /// <summary>
    /// レシピ表示を非表示にする。手放した瞬間に呼ぶ。
    /// </summary>
    public void Hide()
    {
        currentFoodName = null;
        contentParent.gameObject.SetActive(false);
        ClearItems();
    }

    void ClearItems()
    {
        foreach (var item in spawnedItems)
            Destroy(item);
        spawnedItems.Clear();
    }
}
