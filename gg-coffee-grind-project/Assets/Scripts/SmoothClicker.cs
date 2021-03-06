﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothClicker : MonoBehaviour
{
    /// <summary>
    /// OnMouseDown is called when the user has pressed the mouse button while
    /// over the GUIElement or Collider.
    /// </summary>
    void OnMouseDown()
    {
        DustManager.Instance.Smooth();
    }
}
