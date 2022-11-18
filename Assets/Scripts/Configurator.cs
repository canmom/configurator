using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class Configurator : MonoBehaviour
{
    public string Name;
    public string Description;

    private List<VisualElement> _partDescriptors = new List<VisualElement>();
    private VisualElement _menu;

    private void OnEnable()
    {
        var parts = GetComponentsInChildren<IPart>(true);

        _menu = GetComponentInParent<UIDocument>().rootVisualElement[0];

        foreach (IPart part in parts)
        {
            VisualElement descriptor = part.Descriptor();
            descriptor.RegisterCallback<ClickEvent>((e) => GetComponentInParent<Root>().MoveCamera(part.CameraPosition, part.CameraTarget));
            _partDescriptors.Add(descriptor);
            _menu.Add(descriptor);
        }
    }

    private void OnDisable()
    {
        foreach (VisualElement descriptor in _partDescriptors)
        {
            _menu.Remove(descriptor);
        }
        _partDescriptors.Clear();
    }

    public int Cost
    {
        get {
            //get only active parts - ignore toggleable parts which are disabled!
            var _parts = GetComponentsInChildren<IPart>(false);
            return _parts.Select(m => m.Cost).Sum();
        }
    }
}
