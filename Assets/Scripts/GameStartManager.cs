using UnityEngine;
using System.Collections;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] private GameObject gameStartCanvas;
    [SerializeField] private float fadeDuration = 0.5f; // フェードアウト時間

    public void OnStartButtonClick()
    {
        StartCoroutine(FadeOutCanvas());
    }

    private IEnumerator FadeOutCanvas()
    {
        CanvasGroup canvasGroup = gameStartCanvas.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            // CanvasGroup がなければ作成
            canvasGroup = gameStartCanvas.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // 完全に透明になったら非アクティブにする
        gameStartCanvas.SetActive(false);
    }
}