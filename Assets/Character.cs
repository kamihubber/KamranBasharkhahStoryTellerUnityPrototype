using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public interface IGoaperStrategy
{
    public List<Act> GetActs(List<Need> needs, List<Act> _actsRepo);
}

[Serializable]
public struct NeedFullFill
{
    public Need need;
    public int amount;
}
public class Character : MonoBehaviour
{
    [SerializeField]
    List<Act> _actsRepo = new List<Act>();
    
    [SerializeField]
    List<Need> _needs = new List<Need>();
    
    List<Act> tasks = new List<Act>();
    
    //todo : interface
    RegularGoapStrategy regularGoapStrategy = new RegularGoapStrategy();
    
    [SerializeField]
    List<NeedFullFill> needsFullfillDict = new List<NeedFullFill>();

    private void Start()
    {
        foreach (var task in regularGoapStrategy.GetActs(_needs, _actsRepo))
        {
            tasks.Add(task);
        }

        foreach (var task in tasks)
        {
            task.Log();
            UpdateNeedsFullFillScores(task);

        }
    }

    private void UpdateNeedsFullFillScores(Act task)
    {
        foreach (var needEffect in task.effects)
        {
            var nfl = needsFullfillDict.Find(nf => nf.need.name == needEffect.need.name);
            nfl.amount += needEffect.amount;
        }
    }
}
