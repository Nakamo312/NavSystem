using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    public Vector2Int Size = Vector2Int.one;
    public Renderer MainRenderer;
    private Color color;
    // Start is called before the first frame update
    void Start()
    {
        color = MainRenderer.material.color;
    }
    public void SetTransparent(bool available)
    {
        if (available)
        {
            MainRenderer.material.color = Color.green;
        }
        else
        {
            MainRenderer.material.color = Color.red;
        }
    }
    public void SetNormal()
    {
        Color color = MainRenderer.material.color;
        color.a = 0.0f;
        MainRenderer.material.color = color;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
