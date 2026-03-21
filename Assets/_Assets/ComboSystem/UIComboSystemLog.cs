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
    [SerializeField] private int maxEntries = 20;

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
        GameObject entry = Instantiate(logEntryPrefab, content);
        entry.GetComponent<TextMeshProUGUI>().text = message;

        if (content.childCount > maxEntries)
            Destroy(content.GetChild(0).gameObject);

        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}