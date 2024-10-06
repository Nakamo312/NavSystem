using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviour
{
    public BuildingInfoUI panel_info;
    public ResourcePanel resourcePanel;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
        EventBus.onSelected += ShowInfoPanel;
    }
    void ShowInfoPanel(Build info)
    {
        panel_info.gameObject.SetActive(true);
        panel_info.SetOpen(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (panel_info.IsPointerOverUIObject() && !panel_info.IsOpen())
            {
                EventBus.OnClickOutside?.Invoke(true);
                panel_info.gameObject.SetActive(false);
            }
            else
            {
                panel_info.SetOpen(false);
            }
        }
    }
}
