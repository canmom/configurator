using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

// A possible item that can go in this slot. Configure the details in the Unity editor.
[System.Serializable]
public class CarElement
{   
    public string Name;
    public string Description;
    public Mesh Variant;
    public int Cost;
    public Vector3 PositionOffset;
    public AudioClip Sound;
    public CarTraits modifiers;
}

// A slot on the car that can hold multiple different types of object.
public class CustomisablePart : MonoBehaviour, IPart
{
    /// <summary>A list of possible variants that can go in this slot.</summary>
    [SerializeField]
    public List<CarElement> Variants;

    [SerializeField]
    /// <summary>The name of the slot itself, such as 'Tires' or 'Gun', displayed adjacent to the variant picker.</summary>
    public string Name;

    private static VisualTreeAsset template;

    private int CurrentVariant = 0;

    [HideInInspector]
    public CarElement Variant;

    [SerializeField]
    private Vector3 _cameraPosition;
    [SerializeField]
    private Vector3 _cameraTarget;

    public DerivedPart Derived;

    public void Awake() {
        template ??= Resources.Load<VisualTreeAsset>("ConfigurablePartTemplate");
    }

    private void UpdateVariant() {
        //nb. the first time this is run, Variant will be null so oldOffset will initialize to a zero vector
        Vector3 oldOffset = Variant.PositionOffset;
        Variant = Variants[CurrentVariant];
        MeshFilter[] meshfilters = gameObject.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in meshfilters)
        {
            mf.mesh = Variant.Variant;
            mf.transform.Translate(Variant.PositionOffset - oldOffset);
        }
        if (Derived != null)
        {
            Derived.SwitchMesh(CurrentVariant);
        }
        AudioSource audiosource = GetComponent<AudioSource>();
        if (audiosource != null)
        {
            audiosource.clip = Variant.Sound;
            audiosource.Play();
        }
    }

    /// <summary>Switch this part to the next variant in the sequence, and load the matching models.</summary>
    public void NextVariant() {
        CurrentVariant = (CurrentVariant + 1) % Variants.Count;
        UpdateVariant();
    }

    /// <summary>Switch this part to the next variant in the sequence, and load the matching models.</summary>
    public void PrevVariant() {
        CurrentVariant = CurrentVariant == 0 ? Variants.Count - 1 : CurrentVariant-1;
        UpdateVariant();
    }

    private void OnEnable() {
        //ensure the displayed model is consistent with this Behaviour, whatever is set in the editor
        UpdateVariant();
    }

    //implement Part interface
    /// <summary>Create a UIToolkit Visual Element to describe this part with a toggle to add or remove it.</summary>
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
    public CarTraits Modifiers => Variant.modifiers;

    public string Summary() {
        return $"{Name}: {Variant.Name} - {Variant.Cost}t";
    }
}
