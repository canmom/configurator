using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public struct CarTraits {
    public int Horsepower;
    public int Torque;
    public int Grip;
    public int Armour;
    public float DragCoefficient;
    public int FuelEconomy;
    public int Mass;

    public static CarTraits operator +(CarTraits x, CarTraits y) {
        return new CarTraits
        {
            Horsepower = x.Horsepower + y.Horsepower,
            Torque = x.Torque + y.Torque,
            Grip = x.Grip + y.Grip,
            Armour = x.Armour + y.Armour,
            DragCoefficient = x.DragCoefficient + y.DragCoefficient,
            FuelEconomy = x.FuelEconomy + y.FuelEconomy,
            Mass = x.Mass + y.Mass
        };
    }
}

public class Car : MonoBehaviour
{
    public string Name;
    public string Description;

    [SerializeField]
    public int BaseCost;

    [SerializeField]
    public Vector3 DefaultCameraPosition;
    [SerializeField]
    public Vector3 DefaultCameraTarget;

    public CarTraits baseTraits;

    public static string ListTraits(CarTraits traits)
    {
        return $@"Horsepower: {traits.Horsepower}
Torque: {traits.Torque}N
Grip: {traits.Grip}
Armour: {traits.Armour}
Drag Coefficient: {traits.DragCoefficient}
Fuel Economy: {traits.FuelEconomy} MPG
Mass: {traits.Mass}kg";
    }
}
