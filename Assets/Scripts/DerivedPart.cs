using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DerivedPart : MonoBehaviour
{
    public List<Mesh> Variants;

    public void SwitchMesh(int newIndex)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = Variants[newIndex];
    }
}
