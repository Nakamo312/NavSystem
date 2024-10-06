using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BuildType
{
    Generator,
    Factory,
    Extra,
    Barracks,
    Resource
}
[CreateAssetMenu(fileName = "New Build", menuName = "Build", order = 1)]
public class Build : ScriptableObject
{
    public int id;
    public string name;
    public Sprite icon;
    public int cost;
    public string description;
    public BuildType type;
}

