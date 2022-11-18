using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

// A possible item that can go in this slot. Configure the details in the Unity editor.
[System.Serializable]
public class Paintjob
{
    public string Name;
    public string Description;
    public Material Mat;
    public int Cost;
    public CarTraits Modifiers;
}

public class CustomisablePaint : MonoBehaviour, IPart
{
    public List<Paintjob> Variants;

    private static VisualTreeAsset template;

    [SerializeField]
    public string Name;

    internal int CurrentVariant = 0;

    [SerializeField]
    private Vector3 _cameraPosition;
    [SerializeField]
    private Vector3 _cameraTarget;

    public DerivedPart Derived;

    public void Awake() {
        template ??= Resources.Load<VisualTreeAsset>("ConfigurablePartTemplate");
    }

    [HideInInspector]
    public Paintjob Variant;

    private void UpdateVariant() {
        Variant = Variants[CurrentVariant];

        MeshRenderer renderer = GetComponent<MeshRenderer>();

        var oldMaterial = renderer.material;
        renderer.material = Variant.Mat;
        //get rid of unused material instances to avoid a memory leak.
        Destroy(oldMaterial);
    }

    public void NextVariant() {
        CurrentVariant = (CurrentVariant + 1) % Variants.Count;
        UpdateVariant();
    }

    public void PrevVariant() {
        CurrentVariant = CurrentVariant == 0 ? Variants.Count - 1 : CurrentVariant-1;
        UpdateVariant();
    }

    private void OnEnable() {
        //ensure the displayed model is consistent with this Behaviour, whatever is set in the editor
        UpdateVariant();
    }

    //implement Part interface
    public VisualElement Descriptor()
    {
        VisualElement ve = template.Instantiate();
        SerializedObject serializedPart = new SerializedObject(this);
        ve.Bind(serializedPart);

        //bind buttons
        ve.Q<Button>("Next").RegisterCallback<ClickEvent>((e)=>NextVariant());
        ve.Q<Button>("Previous").RegisterCallback<ClickEvent>((e)=>PrevVariant());

        return ve;
    }

    public int Cost => Variant.Cost;

    public Vector3 CameraPosition => _cameraPosition;
    public Vector3 CameraTarget => _cameraTarget;

    public CarTraits Modifiers => Variant.Modifiers;

    public string Summary() {
        return $"{Name}: {Variant.Name} - {Variant.Cost}t";
    }
}
