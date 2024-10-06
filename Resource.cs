using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resourse", menuName = "Resourse", order = 2)]
public class Resource : ScriptableObject
{
        public int id;
        public string name;
        public Sprite icon;
        public string description;
}
