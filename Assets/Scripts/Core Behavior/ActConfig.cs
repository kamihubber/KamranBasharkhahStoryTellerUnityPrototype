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
   physicalwork,
   talkable,
   tradable,
   placeable,
   useable,
   farming,
   politics,
   fighting,
   romance,
   medical,
   caring,
   science,
   food,
   huntable,
   wealth
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

   public List<SkillComp> ProvidingSkills => providingSkills;

   public bool ShouldEngageSubjectOwner => shouldEngageSubjectOwner;

   public bool NeedsSubject
   {
      get => needsSubject;
      set => needsSubject = value;
   }

   public List<SkillComp> SubjectRequiredSkills
   {
      get => subjectRequiredSkills;
      set => subjectRequiredSkills = value;
   }

   [SerializeField] private float requiredTimeSeconds;
   [SerializeField] private float fullFillAmount;
   
   [SerializeField] List<SkillComp> requiredSkills = new List<SkillComp>();
   [SerializeField] List<SkillComp> subjectRequiredSkills = new List<SkillComp>();
   [SerializeField] List<SkillComp> providingSkills = new List<SkillComp>();
   
   [SerializeField] List<SkillComp> components = new List<SkillComp>();

   [SerializeField] private bool shouldEngageSubjectOwner;
   
   [SerializeField] private bool needsSubject;
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

   public List<SkillComp> ProvidingSkills => providingSkills;

   public event Action<Act> OnActDone;
   public event Action<Act> OnActFailed;
   public bool _isFailed;
   
   public event Action<Act> OnActProcessUpdate;
   
   //todo : this should be probably out of this class
   public event Action<Act> OnActTryProcessUpdate;

   private float _requiredJobTimeSeconds;
   private float _fullFillAmount;
   
   List<SkillComp> components = new List<SkillComp>();
   List<SkillComp> requiedSkills = new List<SkillComp>();
   List<SkillComp> subjectRequiedSkills = new List<SkillComp>();
   List<SkillComp> providingSkills = new List<SkillComp>();

   public event Func<Act, Task<bool>> OnRequestGainSkills;
   
   private bool needsSubject;
   
   private GameEntity subjectEntity;
   
   public bool HasSubject => SubjectEntity != null;

   public GameEntity SubjectEntity
   {
      get => subjectEntity;
      set => subjectEntity = value;
   }

   public bool NeedsSubject
   {
      get => needsSubject;
      set => needsSubject = value;
   }

   public List<SkillComp> SubjectRequiedSkills
   {
      get => subjectRequiedSkills;
      set => subjectRequiedSkills = value;
   }

   public Act(ActConfig config, string name, Act parentAct, GameEntity subject = null)
   {
      this.config = config;
      this.name = name;
      _parentAct = parentAct;
      this.NeedsSubject = config.NeedsSubject;
      this.SubjectEntity = subject;

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
      providingSkills = config.ProvidingSkills;
      subjectRequiedSkills = config.SubjectRequiredSkills;

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
   
   
   public async Task Log(string prefix, CancellationToken token, GameEntity subject = null)
   {
      if (_isFailed)
         return;
      
      //todo : try : rewise this
      OnActTryProcessUpdate?.Invoke(this);
      
      
      float minVisibleTime = 2.2f; // seconds, tweak to taste
      float elapsed = 0f;
      while (elapsed < minVisibleTime)
      {
         elapsed += Time.deltaTime;
         await Task.Yield();
      }
      //Debug.Log("trying");
      //
      
      //are we ignoring subacts results?!
      //todo : shouldnt subacts and gainskills be done outside act performnce(log) ?
      //like before that ?like a qualification?
      bool subActsResult = await ProcessSubActs(prefix, token);
      if (!subActsResult)
      {
         OnActFailed?.Invoke(this);
         return;
      }
      
      var gainSkillResult = await OnRequestGainSkills(this);
      if (!gainSkillResult)
      {
         OnActFailed?.Invoke(this);
         return;
      }

      if (NeedsSubject)
      {
         if (subject == null)
         {
            OnActFailed?.Invoke(this);
            return;
         }
         else
         {
            
         }
            
      }

      _requiredJobTimeSeconds = 0;
      while (_requiredJobTimeSeconds < config.RequiredTimeSeconds)
      {
         //if log
         if (_parentAct == null)
            Debug.Log(prefix + " is doing " + name);
         else
         {
            Debug.Log($"{prefix} is doing {name} in order to prepare for {_parentAct.name}");
         }
         
         OnActProcessUpdate?.Invoke(this);
         //
         
         _requiredJobTimeSeconds += Time.deltaTime;
         // 0.001 can be an effort parameter for fail
         _fullFillAmount += Time.deltaTime * 10f;
         
         await Task.Yield();
      }

      if (IsFullfilled())
        OnActDone?.Invoke(this);
      else
      {
         _isFailed = true;
         OnActFailed?.Invoke(this);
      }
   }
   
   public async Task<bool> ProcessSubActs(string prefix, CancellationToken token)
   {
      bool result = true;
      
      // Process sub acts
      if (requiredActs.Count > 0)
      {
         foreach (var subact in requiredActs)
         {
            if (subact.IsFullfilled())
               return true;
            
            subact.ParentAct = this;
            //check???
            //logs??
            subact.OnActDone += (subact) => { result = true; };
            subact.OnActFailed += (subact) => { result = false; };
            //
            subact.OnRequestGainSkills = OnRequestGainSkills;
            await subact.Log(prefix, token);
            return result;
         }
      }
      
      return result;
   }

}
