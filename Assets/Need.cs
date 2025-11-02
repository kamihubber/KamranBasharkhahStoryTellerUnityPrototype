using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Need")]
public class NeedConfig : ScriptableObject
{
    [SerializeField] private string name;
    //others
}

public class Need
{
    public NeedConfig config;

    private int fullFillAmount;

    public int FullFillAmount
    {
        get { return fullFillAmount; }
        set { fullFillAmount = value; }
    }
}
