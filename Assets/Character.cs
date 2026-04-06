using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using UnityEngine;

public interface IGoaperStrategy
{
    //decomposition : obstacles strategy?
    public Dictionary<NeedConfig, Act> GetActs(List<NeedConfig> needs, List<ActConfig> _actsRepo, List<Need> characterNeeds);
}

public interface IEntity
{
    
}

public struct Trait
{
    public string Name;
    public TraitType Type;
    public float Value;
}

public enum TraitType
{
    Normal,
    Insane
}

public enum MentalState
{
    Normal,
    Insane,
}

public class GameEntity : MonoBehaviour, IEntity
{
    [SerializeField] protected List<SkillComp> skills;

    public List<SkillComp> Skills => skills;

    public List<Trait> Traits => traits;

    public MentalState MentalState => _mentalState;

    [SerializeField] protected List<Trait> traits;
    
    private MentalState _mentalState;

    //todo : temp
    public virtual async Task<bool> Interact(GameEntity other, SkillComp? comp = null)
    {
        return false;
    }

}

[Serializable]
public struct NeedFullFill
{
    public NeedConfig needConfig;
    public int amount;
}
public class Character : GameEntity
{
    [SerializeField] private string name;
    
    [SerializeField]
    List<ActConfig> _actsRepo = new List<ActConfig>();
    
    [SerializeField]
    List<NeedConfig> needsConfigs = new List<NeedConfig>();
    
    Dictionary<NeedConfig, Act> tasks = new Dictionary<NeedConfig, Act>();
    
    Dictionary<NeedConfig, Act> tasksToDelete = new Dictionary<NeedConfig, Act>();
    
    //todo : interface
    RegularGoapStrategy regularGoapStrategy = new RegularGoapStrategy();
    
    [SerializeField]
    List<Need> needs = new List<Need>();
    
    [SerializeField]
    List<NeedFullFill> needsInitialValues = new List<NeedFullFill>();

    public List<Need> Needs => needs;

    public Dictionary<NeedConfig, Act> Tasks => tasks;

    public string Name => name;

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
            
            Needs.Add(newNeed);
        }
        
        //ProcessNeeds();
    }

    private void Update()
    {
        ProcessNeeds();
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

        var goalTasks = regularGoapStrategy.GetActs(needConfigsToUpdate.ToList(), _actsRepo, Needs);
        foreach (var task in goalTasks)
        {
            tasks.Add(task.Key, task.Value);
        }

        foreach (var task in tasks)
        {
            
            task.Value.OnActDone -= OnActDone;
            task.Value.OnActDone += OnActDone;
            
            task.Value.OnActFailed -= OnActFailed;
            task.Value.OnActFailed += OnActFailed;
            
            task.Value.OnRequestGainSkills -= RequestGainSkills;
            task.Value.OnRequestGainSkills += RequestGainSkills;

            await task.Value.Log(name);
            //todo : update on task done
            //UpdateNeedsFullFillScores(task.Value);
        }

        if ((tasks != null) && (tasks.Count > 0))
        {
            tasksToDelete.Clear();
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

    public async Task<bool> RequestGainSkills(Act task)
    {
        //await skills
        
        var gainSkillsResult = await GainSkills(task);

        if (gainSkillsResult == false)
        {
            Debug.Log("GainSkills failed for task : " + task.Name);
            return false;
        }
        
        return true;
    }

    private void OnActDone(Act task)
    {
        if (task.ParentAct == null)
        {
            UpdateNeedsFullFillScores(task);
        }
        else
        {
           
        }
    }

    private void OnActFailed(Act task)
    {
        Debug.Log($"OnActFailed: {task.Name}");
    }

    private void UpdateNeedsFullFillScores(Act task)
    {
        if (task._isFailed)
            return;
        
        foreach (var needEffect in task.config.effects)
        {
            var need = Needs.Find(n => n.config == needEffect.needConfig);
            need.FullFillAmount += needEffect.amount;
        }
    }

    void UpdateTasksState(KeyValuePair<NeedConfig, Act> task)
    {
        //
        var need = Needs.Find(n => n.config == task.Key);
        if (isNeedFullfilled(need))
            tasksToDelete.Add(task.Key, task.Value);
    }
    
    private bool isNeedFullfilled(Need need)
    {
        var config = needsConfigs.Find(nc => nc == need.config);
        return config.FullFillMax <= need.FullFillAmount;
    }

    private async Task<bool> GainSkills(Act task)
    {
        foreach (var skill in task.config.RequiredSkills)
        {
            bool hasSkill = Skills.FindAll(skl => skl.skillType == skill.skillType).Count > 0;
            SkillComp mySkill = Skills.Find(skl => skl.skillType == skill.skillType);

            if (!hasSkill || mySkill.skillLevel < skill.skillLevel)
            {
                var seekResult = await Seek(skill);

                bool interactResult = false;
                if (seekResult.success)
                {
                    interactResult = await Interact(seekResult.targetOwner, skill);
                    //todo : gain??
                }

                return interactResult;
            }
        }
        
        return true;
    }

    //await for other response or contine?
    //set result based on other response?
    //aligned with seeking comp?
    public override async Task<bool> Interact(GameEntity other, SkillComp? comp = null)
    {
        
        ActConfig actConfig = regularGoapStrategy.GetInterAct(new NeedConfig(), _actsRepo, comp, this, other);

        if (actConfig == null)
        {
            Debug.Log(name + " does not know what to do and just stares at some point for hours... ");
            return false;
        }

        string aboutPhrase;
        if (comp == null)
        {
            aboutPhrase = "";
        }
        else
        {
            aboutPhrase = $" about { comp.Value.skillName } ";
        }
        Debug.Log(name + " is " + actConfig.Name + " with " + other.name + aboutPhrase);
        
        bool result = false;
        
        //todo : temp
        Act tempact = new Act(actConfig, actConfig.Name, null);
        tempact.OnRequestGainSkills += RequestGainSkills;
        tempact.OnActDone += (act) => { result = true; };
        tempact.OnActFailed += (act) => { result = false; };
        await tempact.Log(this.Name);

        if (comp != null)
          await other.Interact(this,null);
        
        return result;
    }

    public struct ComponentSeekResult
    {
        public bool success;
        public GameEntity targetOwner;
    }

    private async Task<ComponentSeekResult> Seek(SkillComp skill)
    {
        Debug.Log(name + "is seeking " + skill.skillName);
        
        var targetOwner = SearchInteractionTargets(skill);

        if (targetOwner == null)
        {
            return new ComponentSeekResult()
            {
                success = false,
            };
        }
        else
        {
            return new ComponentSeekResult()
            {
                success = true,
                targetOwner = targetOwner,
            };
        }
    }

    private GameEntity SearchInteractionTargets(SkillComp skill)
    {
        // - a range factor
        //     -a obscure factor
        //     - a mistake factor
        //     - all affected by mental factors
            
        bool any = FindObjectsOfType<GameEntity>()
            .Where(ge => ge.Skills.Count(skl => skl.skillType == skill.skillType) > 0).Any();
        
        if (!any)
            return null;
        
        var interactionGoal = FindObjectsOfType<GameEntity>().Where(ge => ge.Skills.Count(skl => skl.skillType == skill.skillType) > 0).First();
        
        return interactionGoal;
    }
    
}
