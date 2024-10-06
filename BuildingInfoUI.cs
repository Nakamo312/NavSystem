using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class BuildingInfoUI : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image icon;
    private Build info;
    private bool isOpen;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.pointerEnter == null)
        {
            gameObject.SetActive(false);
        }
    }
    public bool IsOpen()
    {
        return isOpen;
    }
    public void SetOpen(bool open)
    {
        isOpen = open;
    }
    void BuildSelect(Build info)
    {
        nameText.text = info.name;
        descriptionText.text = info.description;
    }
    public bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        for (int i = 0; i < raycastResults.Count; i++) 
        {
            if (raycastResults[i].gameObject.layer == 5)
            {
                return false;
            }
        }

        return true;

    }
    void Awake()
    {
        EventBus.onSelected += BuildSelect;
    }

    void OnDisable()
    {
        /*EventBus.onSelected -= BuildSelect;*/
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }
}
