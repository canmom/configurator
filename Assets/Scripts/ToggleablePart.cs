using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class ToggleablePart : MonoBehaviour, IPart
{
    //A Toggleable Part is very simple: it has no alternate variants, but simply can be present (active) or not (inactive).
    public string Name;
    public string Description;
    [SerializeField]
    private int _cost;
    [SerializeField]
    private Vector3 _cameraPosition;
    [SerializeField]
    private Vector3 _cameraTarget;
    [SerializeField]
    private CarTraits _modifiers;

    private static VisualTreeAsset template;

    public void Awake()
    {
        template ??= Resources.Load<VisualTreeAsset>("ToggleablePartTemplate");
    }

    //implementation of Part interface
    /// <summary>Create a UIToolkit Visual Element to describe this part with a toggle to add or remove it.</summary>
    public VisualElement Descriptor()
    {
        VisualElement ve = template.Instantiate();
        SerializedObject serializedPart = new SerializedObject(this);
        ve.Bind(serializedPart);
        Toggle toggle = ve.Q<Toggle>("Equipped");
        toggle.value = gameObject.activeSelf;

        //when the toggle switch is toggled, show or hide the part to match
        toggle.RegisterCallback<ChangeEvent<bool>>((e) =>
        {
            gameObject.SetActive(e.newValue);
        });

        return ve;
    }

    public int Cost => _cost;

    public Vector3 CameraPosition => _cameraPosition;
    public Vector3 CameraTarget => _cameraTarget;
    public CarTraits Modifiers => _modifiers;

    public string Summary() {
        return $"{Name} - {Cost}t";
    }
}
