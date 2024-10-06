using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private List<int> resourses = new List<int>();
    [SerializeField]
    private UIManager uiPanel;
    // Start is called before the first frame update
    void Start()
    {
        resourses.Add(0);
        resourses.Add(0);
        resourses.Add(0);
    }
    public bool UpdateResourse(int id, int count)
    {
        if (count < 0)
        {
            if (resourses[id] >= -count)
            {
                resourses[id] += count;
                return true;
            }
            else
            {
                return false;
            }
        }
        resourses[id] += count;

        uiPanel.resourcePanel.UpdateValue(id, resourses[id]);
        return true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
