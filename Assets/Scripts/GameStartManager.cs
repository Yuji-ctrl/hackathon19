using UnityEngine;
using System.Collections;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] private GameObject gameStartButton;
    [SerializeField] private float fadeDuration = 0.5f; // �t�F�[�h�A�E�g����

    public void OnStartButtonClick()
    {
        StartCoroutine(FadeOutCanvas());
    }

    private IEnumerator FadeOutCanvas()
    {
        CanvasGroup canvasGroup = gameStartButton.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            // CanvasGroup ���Ȃ���΍쐬
            canvasGroup = gameStartButton.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // ���S�ɓ����ɂȂ������A�N�e�B�u�ɂ���
        gameStartButton.SetActive(false);
    }
}