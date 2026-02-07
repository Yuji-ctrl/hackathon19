using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ActionSpriteEntry
{
    public string actionName;
    public Sprite sprite;
}

public class RecipeModalItem : MonoBehaviour
{
    [SerializeField] float iconSize = 80f;
    [SerializeField] float plusIconSize = 40f;
    [SerializeField] float arrowIconSize = 40f;
    [SerializeField] float actionIconSize = 30f;
    [SerializeField] string modelDir = "Foods";
    [SerializeField] Sprite plusSprite;
    [SerializeField] Sprite arrowSprite;
    [SerializeField] List<ActionSpriteEntry> actionSprites = new();

    Dictionary<string, Sprite> actionSpriteMap = new();

    void OnEnable()
    {
        actionSpriteMap.Clear();
        foreach (var entry in actionSprites)
        {
            if (!string.IsNullOrEmpty(entry.actionName) && entry.sprite != null)
                actionSpriteMap[entry.actionName] = entry.sprite;
        }
    }

    public void Setup(Recipe recipe)
    {
        for (int i = 0; i < recipe.materialFoods.Length; i++)
        {
            if (i > 0) CreateSymbolIcon();
            CreateFoodIcon(recipe.materialFoods[i]);
        }

        CreateArrowWithAction(recipe.action);

        for (int i = 0; i < recipe.resultFoods.Length; i++)
        {
            if (i > 0) CreateSymbolIcon();
            CreateFoodIcon(recipe.resultFoods[i]);
        }
    }

    void CreateFoodIcon(string foodName)
    {
        var go = new GameObject(foodName, typeof(RectTransform), typeof(RawImage), typeof(FoodModelIcon));
        go.transform.SetParent(transform, false);
        
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(iconSize, iconSize);
        rt.anchoredPosition = Vector2.zero;
        
        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = iconSize;
        le.preferredHeight = iconSize;
        
        go.GetComponent<FoodModelIcon>().Setup(foodName, modelDir, 15);
    }

    void CreateSymbolIcon()
    {
        if (plusSprite == null) return;
        
        var go = new GameObject("+", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);
        
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(plusIconSize, plusIconSize);
        rt.anchoredPosition = Vector2.zero;
        
        go.GetComponent<Image>().sprite = plusSprite;
        
        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = plusIconSize;
        le.preferredHeight = plusIconSize;
    }

    void CreateArrowWithAction(string action)
    {
        if (arrowSprite == null) return;
        
        var container = new GameObject("ArrowAction", typeof(RectTransform));
        container.transform.SetParent(transform, false);
        
        var containerRt = container.GetComponent<RectTransform>();
        containerRt.anchorMin = new Vector2(0.5f, 0.5f);
        containerRt.anchorMax = new Vector2(0.5f, 0.5f);
        containerRt.pivot = new Vector2(0.5f, 0.5f);
        containerRt.sizeDelta = new Vector2(arrowIconSize, arrowIconSize);
        containerRt.anchoredPosition = Vector2.zero;
        
        var le = container.AddComponent<LayoutElement>();
        le.preferredWidth = arrowIconSize;
        le.preferredHeight = arrowIconSize;
        
        var arrow = new GameObject("Arrow", typeof(RectTransform), typeof(Image));
        arrow.transform.SetParent(container.transform, false);
        
        var arrowRt = arrow.GetComponent<RectTransform>();
        arrowRt.anchorMin = Vector2.zero;
        arrowRt.anchorMax = Vector2.one;
        arrowRt.offsetMin = Vector2.zero;
        arrowRt.offsetMax = Vector2.zero;
        
        arrow.GetComponent<Image>().sprite = arrowSprite;
        
        if (actionSpriteMap.TryGetValue(action, out var sprite))
        {
            var actionIcon = new GameObject($"Action_{action}", typeof(RectTransform), typeof(Image));
            actionIcon.transform.SetParent(container.transform, false);
            
            var actionRt = actionIcon.GetComponent<RectTransform>();
            actionRt.anchorMin = new Vector2(0.5f, 1f);
            actionRt.anchorMax = new Vector2(0.5f, 1f);
            actionRt.pivot = new Vector2(0.5f, 0f);
            actionRt.sizeDelta = new Vector2(actionIconSize, actionIconSize);
            actionRt.anchoredPosition = Vector2.zero;
            
            actionIcon.GetComponent<Image>().sprite = sprite;
        }
    }
}