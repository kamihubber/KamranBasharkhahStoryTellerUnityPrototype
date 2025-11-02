using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public interface IGoaperStrategy
{
    public List<Act> GetActs(List<NeedConfig> needs, List<Act> _actsRepo, List<Need> characterNeeds);
}

[Serializable]
public struct NeedFullFill
{
    public NeedConfig needConfig;
    public int amount;
}
public class Character : MonoBehaviour
{
    [SerializeField]
    List<Act> _actsRepo = new List<Act>();
    
    [SerializeField]
    List<NeedConfig> needsConfigs = new List<NeedConfig>();
    
    List<Act> tasks = new List<Act>();
    
    //todo : interface
    RegularGoapStrategy regularGoapStrategy = new RegularGoapStrategy();
    
    [SerializeField]
    List<Need> needs = new List<Need>();
    
    [SerializeField]
    List<NeedFullFill> needsInitialValues = new List<NeedFullFill>();

    private void Start()
    {
        //lets say , for now we run for 10 times or maybe untill everybody is gone
        //
        // - should we know/track which goal/need each task relates to ?
        
        //create needs based on configs
        foreach (var _needConfig in needsConfigs)
        {
            Need newNeed = new Need()
            {
                FullFillAmount = needsInitialValues.Find(niv => niv.needConfig == _needConfig).amount,
                config = _needConfig
            };
            
            needs.Add(newNeed);
        }
        
        foreach (var task in regularGoapStrategy.GetActs(needsConfigs, _actsRepo, needs))
        {
            tasks.Add(task);
        }

        foreach (var task in tasks)
        {
            task.Log();
            //todo : update on task done
            UpdateNeedsFullFillScores(task);

        }
    }

    private void UpdateNeedsFullFillScores(Act task)
    {
        foreach (var needEffect in task.effects)
        {
            var need = needs.Find(n => n.config == needEffect.needConfig);
            need.FullFillAmount += needEffect.amount;
        }
    }
    
    private bool isNeedFullfilled()
    {
        return false;
    }
}
