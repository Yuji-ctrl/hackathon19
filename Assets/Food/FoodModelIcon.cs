using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 食材の3Dモデルを正方形枠内で回転させ、RawImage に RenderTexture 経由で描画する。
/// </summary>
[RequireComponent(typeof(RawImage))]
public class FoodModelIcon : MonoBehaviour
{
    const int PREVIEW_LAYER = 31;
    const float SPACING = 4f;
    const float BASE_Y = -1000f;

    static int counter;

    RenderTexture rt;
    Camera cam;
    GameObject model;
    Vector3 rotCenter;

    float rotSpeed = 50f;

    /// <summary>
    /// 指定された食材名で3Dモデルプレビューをセットアップする。
    /// </summary>
    /// <param name="viewAngle">カメラの見下ろし角度（度数）。0=水平、90=真上から</param>
    public void Setup(string foodName, string modelDir = "Foods", float viewAngle = 35f)
    {
        int idx = counter++;
        Vector3 pos = new Vector3(idx * SPACING, BASE_Y, 0);

        // ─── RenderTexture ───
        rt = new RenderTexture(128, 128, 16, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 2;
        rt.Create();
        GetComponent<RawImage>().texture = rt;

        // ─── モデル読み込み ───
        var asset = Resources.Load<GameObject>($"{modelDir}/{foodName}");
        if (asset == null)
        {
            Debug.LogWarning($"[FoodModelIcon] モデルが見つかりません: {modelDir}/{foodName}");
            return;
        }

        model = Instantiate(asset);
        model.transform.position = pos;
        SetLayerRecursive(model, PREVIEW_LAYER);

        // 物理干渉を防止
        foreach (var c in model.GetComponentsInChildren<Collider>())
            Destroy(c);
        foreach (var rb in model.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        // ─── バウンズ計算 & カメラ配置 ───
        var bounds = GetBounds(model);
        rotCenter = bounds.center;
        float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z, 0.01f);

        var camGo = new GameObject($"_FoodPreviewCam_{idx}");
        
        // 角度に基づいてカメラ位置を計算
        float distance = size * 2.5f;
        float angleRad = viewAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            0, 
            distance * Mathf.Sin(angleRad),
            -distance * Mathf.Cos(angleRad)
        );
        camGo.transform.position = rotCenter + offset;
        camGo.transform.LookAt(rotCenter);

        cam = camGo.AddComponent<Camera>();
        cam.targetTexture = rt;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.cullingMask = 1 << PREVIEW_LAYER;
        cam.fieldOfView = 25f;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = size * 20f;
        cam.allowHDR = false;
        cam.depth = -100;

        // メインカメラからプレビューレイヤーを除外
        if (Camera.main != null)
            Camera.main.cullingMask &= ~(1 << PREVIEW_LAYER);
    }

    void Update()
    {
        if (model != null)
            model.transform.RotateAround(rotCenter, Vector3.up, rotSpeed * Time.deltaTime);
    }

    void OnDestroy()
    {
        if (model != null) Destroy(model);
        if (cam != null) Destroy(cam.gameObject);
        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
        }
    }

    static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    static Bounds GetBounds(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one * 0.1f);

        var b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);
        return b;
    }
}
