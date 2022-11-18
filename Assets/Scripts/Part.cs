using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IPart
{
    VisualElement Descriptor();
    int Cost
    {
        get;
    }
    Vector3 CameraPosition
    {
        get;
    }
    Vector3 CameraTarget
    {
        get;
    }
    CarTraits Modifiers
    {
        get;
    }
    string Summary();
}
