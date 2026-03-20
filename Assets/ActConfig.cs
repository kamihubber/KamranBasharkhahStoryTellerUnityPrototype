using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct NeedEffect
{
   public NeedConfig needConfig;
   public float amount;
}

[Serializable]
public struct SkillComp
{
   public string skillName;
   public int skillLevel;
   public SkillType skillType;
   //giving, maybe each skillcomp gives an amount of seekingcomp, if succeeded
}

public enum SkillType
{
   physical,
   talkable,
   tradable,
   placeable,
   useable,
   farming,
   politics,
   fighting,
}

[CreateAssetMenu(menuName = "Act")]
public class ActConfig : ScriptableObject
{
   [SerializeField] private string name;
   
   [SerializeField]
   public List<NeedEffect> effects = new List<NeedEffect>();

   // [SerializeField] 
   // private List<Act> requirements;
   
   [SerializeField]
   private int wisdomFactor = 1;
   
   [SerializeField]
   private int actFactor = 1;
   
   //create act need dynamically ?
   [SerializeField]
   List<ActConfig> requiredActs = new List<ActConfig>();
   

   //todo : logic should not be here , this is config?
   public List<ActConfig> RequiredActs => requiredActs;

   public float RequiredTimeSeconds => requiredTimeSeconds;

   public float FullFillAmount => fullFillAmount;

   public List<SkillComp> RequiredSkills => requiredSkills;

   public List<SkillComp> Components => components;

   public string Name => name;

   [SerializeField] private float requiredTimeSeconds;
   [SerializeField] private float fullFillAmount;
   
   [SerializeField] List<SkillComp> requiredSkills = new List<SkillComp>();
   
   [SerializeField] List<SkillComp> components = new List<SkillComp>();
}


public class Act
{
   public ActConfig config;

   [SerializeField] private string name;
   
   //create act need dynamically ?
   [SerializeField]
   List<Act> requiredActs = new List<Act>();
   
   private Act _parentAct;

   public Act ParentAct
   {
      get => _parentAct;
      set => _parentAct = value;
   }

   public string Name => name;

   public List<Act> RequiredActs => requiredActs;

   public bool IsFailed => _isFailed;

   public List<SkillComp> Components => components;
   public List<SkillComp> RequiredSkills => requiedSkills;

   public event Action<Act> OnActDone;
   public event Action<Act> OnActFailed;
   public bool _isFailed;

   private float _requiredJobTimeSeconds;
   private float _fullFillAmount;
   
   List<SkillComp> components = new List<SkillComp>();
   List<SkillComp> requiedSkills = new List<SkillComp>();

   public Act(ActConfig config, string name, Act parentAct)
   {
      this.config = config;
      this.name = name;
      _parentAct = parentAct;

      if (config.RequiredActs != null && config.RequiredActs.Count > 0)
      {
         foreach (var subActConfig in config.RequiredActs)
         {
            var subact = new Act(subActConfig, subActConfig.name, this);
            subact.OnActFailed += OnActFailed;
            requiredActs.Add(subact);
         }
      }

      components = config.Components;
      requiedSkills = config.RequiredSkills;

   }

   private void HandleActFailed(Act subact)
   {
      _isFailed = true;
      OnActFailed.Invoke(this);
   }

   public bool IsFullfilled()
   {
      return _fullFillAmount >= config.FullFillAmount;
   }
   
   
   public async Task Log(string prefix)
   {
      if (_isFailed)
         return;
      
      await ProcessSubActs(prefix);

      while (_requiredJobTimeSeconds < config.RequiredTimeSeconds)
      {
         //if log
         if (_parentAct == null)
            Debug.Log(prefix + " is doing " + name);
         else
         {
            Debug.Log($"{prefix} is doing {name} in order to prepare for {_parentAct.name}");
         }
         //
         
         _requiredJobTimeSeconds += Time.deltaTime;
         // 0.001 can be an effort parameter for fail
         _fullFillAmount += Time.deltaTime * 10f;
      }

      if (IsFullfilled())
        OnActDone.Invoke(this);
      else
      {
         _isFailed = true;
         OnActFailed.Invoke(this);
      }
   }
   
   public async Task ProcessSubActs(string prefix)
   {
      // Process sub acts
      if (requiredActs.Count > 0)
      {
         foreach (var subact in requiredActs)
         {
            if (subact.IsFullfilled())
               return;
            
            subact.ParentAct = this;
            subact.OnActDone = OnActDone;
            await subact.Log(prefix);
         }
      }
   }

}
