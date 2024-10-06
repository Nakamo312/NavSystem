using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EventBus : MonoBehaviour
{
    public static Action<Build> onSelected;
    public static Action<bool> OnClickOutside;
}
