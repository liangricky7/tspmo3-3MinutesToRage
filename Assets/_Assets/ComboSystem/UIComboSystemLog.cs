using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIComboSystemLog : MonoBehaviour
{
    public static UIComboSystemLog Instance { get; private set; }

    [SerializeField] private GameObject logEntryPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private int maxEntries = 5;

    private Coroutine scrollCoroutine;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddEntry(string message)
    {
        if (scrollCoroutine != null)
            StopCoroutine(scrollCoroutine);

        GameObject entry = Instantiate(logEntryPrefab, content);
        entry.GetComponent<TextMeshProUGUI>().text = message;

        while (content.childCount > maxEntries)
            DestroyImmediate(content.GetChild(0).gameObject);

        scrollCoroutine = StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();

        float elapsed = 0f;
        float duration = 0.3f;
        float startPosition = scrollRect.verticalNormalizedPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPosition, 0f, elapsed / duration);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
    }
}