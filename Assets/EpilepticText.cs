using System.Collections;
using TMPro;
using UnityEngine;

public class EpilepticText : MonoBehaviour
{
    public Color[] colors = new Color[]
    {
        Color.white,
        Color.red,
        Color.yellow,
    };

    public float interval = 0.1f;
    private TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(Flicker());
    }

    void OnEnable()
    {
        StartCoroutine(Flicker());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Flicker()
    {
        int index = 0;
        while (true)
        {
            textMesh.color = colors[index];
            index = (index + 1) % colors.Length;
            yield return new WaitForSeconds(interval);
        }
    }
}
