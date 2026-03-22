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
    [SerializeField] private float minFontSize = 10f;
    [SerializeField] private float maxFontSize = 24f;
    [SerializeField] private float topWidthPercent = 1f;
    [SerializeField] private float bottomWidthPercent = 0.62f;
    [SerializeField] private float textPadding = 4f;

    private Coroutine scrollCoroutine;
    private Coroutine layoutCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Pre-size content so RectMask2D has a valid area from the start
        RectTransform contentRect = (RectTransform)content;
        float contentWidth = ((RectTransform)scrollRect.transform).rect.width;
        float contentHeight = ((RectTransform)scrollRect.transform).rect.height;
        contentRect.sizeDelta = new Vector2(contentWidth, contentHeight);
    }

    public void AddEntry(string message)
    {
        if (scrollCoroutine != null)
            StopCoroutine(scrollCoroutine);

        GameObject entry = Instantiate(logEntryPrefab, content);
        entry.GetComponent<TextMeshProUGUI>().text = message;

        // Force position inside panel immediately before anything else runs
        RectTransform entryRect = entry.GetComponent<RectTransform>();
        entryRect.anchorMin = new Vector2(1f, 1f);
        entryRect.anchorMax = new Vector2(1f, 1f);
        entryRect.pivot = new Vector2(1f, 1f);
        entryRect.sizeDelta = new Vector2(((RectTransform)scrollRect.transform).rect.width * topWidthPercent, 40f);
        entryRect.anchoredPosition = new Vector2(0f, 0f);

        entry.transform.SetAsFirstSibling();

        while (content.childCount > maxEntries)
            DestroyImmediate(content.GetChild(content.childCount - 1).gameObject);

        if (layoutCoroutine != null)
            StopCoroutine(layoutCoroutine);
        layoutCoroutine = StartCoroutine(RefreshLayoutNextFrame());
    }

    private IEnumerator RefreshLayoutNextFrame()
    {
        // First yield — Unity finishes its layout pass
        yield return new WaitForEndOfFrame();
        // Second yield — TMP finishes rebuilding its text geometry
        yield return new WaitForEndOfFrame();
        RefreshLayout();
        scrollCoroutine = StartCoroutine(ScrollToBottom());
    }

    private void RefreshLayout()
    {
        int total = content.childCount;
        RectTransform contentRect = (RectTransform)content;

        // Use the ScrollRect for a stable width reference
        float contentWidth = ((RectTransform)scrollRect.transform).rect.width;

        float topWidth = contentWidth * topWidthPercent;
        float bottomWidth = contentWidth * bottomWidthPercent;

        // First pass — calculate heights using estimated widths
        float[] heights = new float[total];
        float totalHeight = 0f;

        for (int i = 0; i < total; i++)
        {
            TMP_Text tmp = content.GetChild(i).GetComponent<TMP_Text>();
            float t = Mathf.Clamp01(1f - ((float)i / (maxEntries - 1)));
            tmp.fontSize = Mathf.Lerp(minFontSize, maxFontSize, t);

            float estimatedWidth = Mathf.Lerp(topWidth, bottomWidth, Mathf.Clamp01((float)i / (maxEntries - 1))) - textPadding;
            heights[i] = tmp.GetPreferredValues(tmp.text, estimatedWidth, 0f).y;
            totalHeight += heights[i];
        }

        // Set content size before positioning so entries aren't clipped
        contentRect.sizeDelta = new Vector2(contentWidth, totalHeight);

        // Second pass — position entries using accurate totalHeight
        float topY = 0f;

        for (int i = 0; i < total; i++)
        {
            content.GetChild(i).gameObject.SetActive(true); // re-enable before positioning
            RectTransform rt = (RectTransform)content.GetChild(i);
            TMP_Text tmp = rt.GetComponent<TMP_Text>();

            float yPercent = totalHeight > 0 ? (topY / totalHeight) : 0f;
            float trapWidth = Mathf.Lerp(topWidth, bottomWidth, yPercent) - textPadding;
            float t = Mathf.Clamp01(1f - ((float)i / (maxEntries - 1)));

            tmp.fontSize = Mathf.Lerp(minFontSize, maxFontSize, t);
            float entryHeight = tmp.GetPreferredValues(tmp.text, trapWidth, 0f).y;

            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            
            rt.sizeDelta = new Vector2(trapWidth, entryHeight);
            rt.anchoredPosition = new Vector2(0f, -topY);

            topY += entryHeight;
        }

        contentRect.sizeDelta = new Vector2(contentWidth, topY);
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

    public void ClearEntries()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}