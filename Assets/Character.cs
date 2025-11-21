using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;

public interface IGoaperStrategy
{
    //decomposition : obstacles strategy?
    public Dictionary<NeedConfig, ActConfig> GetActs(List<NeedConfig> needs, List<ActConfig> _actsRepo, List<Need> characterNeeds);
}

[Serializable]
public struct NeedFullFill
{
    public NeedConfig needConfig;
    public int amount;
}
public class Character : MonoBehaviour
{
    [SerializeField] private string name;
    
    [SerializeField]
    List<ActConfig> _actsRepo = new List<ActConfig>();
    
    [SerializeField]
    List<NeedConfig> needsConfigs = new List<NeedConfig>();
    
    Dictionary<NeedConfig, ActConfig> tasks = new Dictionary<NeedConfig, ActConfig>();
    
    Dictionary<NeedConfig, ActConfig> tasksToDelete = new Dictionary<NeedConfig, ActConfig>();
    
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
        
        ProcessNeeds();
    }

    private void Update()
    {
        //ProcessNeeds();
    }
    
    bool ShouldProcessNeed(NeedConfig needsConfig, List<Need> characterNeeds)
    {
        Need targetNeed = characterNeeds.Find(cn => cn.config == needsConfig);
            
        return targetNeed != null && targetNeed.FullFillAmount < 5;
            
        return false;
    }

    private async void ProcessNeeds()
    {
        var needConfigsToUpdate = needsConfigs.Where(nd => tasks.Keys.Contains(nd) == false);

        var goalTasks = regularGoapStrategy.GetActs(needConfigsToUpdate.ToList(), _actsRepo, needs);
        foreach (var task in goalTasks)
        {
            tasks.Add(task.Key, task.Value);
        }

        foreach (var task in tasks)
        {
            await task.Value.Log(name);
            //todo : update on task done
            UpdateNeedsFullFillScores(task.Value);
        }

        if ((tasks != null) && (tasks.Count > 0))
        {
            foreach (var task in tasks)
            {
                UpdateTasksState(task);
            }
        }

        foreach (var expiredTask in tasksToDelete)
        {
            tasks.Remove(expiredTask.Key);
        }
    }

    private void UpdateNeedsFullFillScores(ActConfig task)
    {
        foreach (var needEffect in task.effects)
        {
            var need = needs.Find(n => n.config == needEffect.needConfig);
            need.FullFillAmount += needEffect.amount;
        }
    }

    void UpdateTasksState(KeyValuePair<NeedConfig, ActConfig> task)
    {
        //
        var need = needs.Find(n => n.config == task.Key);
        if (isNeedFullfilled(need))
            tasksToDelete.Add(task.Key, task.Value);
    }
    
    private bool isNeedFullfilled(Need need)
    {
        var config = needsConfigs.Find(nc => nc == need.config);
        return config.FullFillMax <= need.FullFillAmount;
        return false;
    }
}
