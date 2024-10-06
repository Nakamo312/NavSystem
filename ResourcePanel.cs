using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResourcePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject resourceFramePrefab;
    [SerializeField]
    private List<GameObject> resourses = new List<GameObject>();
    [SerializeField]
    private int size = 5;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject new_panel = Instantiate(resourceFramePrefab);
            resourses.Add(new_panel);
            new_panel.transform.SetParent(transform);
            RectTransform slotRect = new_panel.GetComponent<RectTransform>();
            slotRect.anchorMin = new Vector2(0, 1);
            slotRect.anchorMax = new Vector2(0, 1);
            slotRect.anchoredPosition = new Vector2((i + 0.5f) * slotRect.sizeDelta.x, 0);
            slotRect.pivot = new Vector2(0.5f, 1);

        }
    }

    public void UpdateValue(int id, int count)
    {
        resourses[id].GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
