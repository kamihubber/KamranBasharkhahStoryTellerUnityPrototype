using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Need")]
public class NeedConfig : ScriptableObject
{
    [SerializeField] private string name;
    
    [SerializeField] private int fullFillMax;
    
    public int FullFillMax { get { return fullFillMax; } set { fullFillMax = value; } }

    public string Name => name;
    //others
}

public class Need
{
    public NeedConfig config;

    private float fullFillAmount;

    public float FullFillAmount
    {
        get { return fullFillAmount; }
        set { fullFillAmount = value; }
    }
}
