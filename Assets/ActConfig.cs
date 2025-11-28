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

   public ActConfig ParentAct
   {
      get => _parentAct;
      set => _parentAct = value;
   }

   //todo : logic should not be here , this is config?

   public event Action<ActConfig> OnActDone; 
   
   public async Task Log(string prefix)
   {
      await ProcessSubActs(prefix);
      
      //if log
      if (_parentAct == null)
        Debug.Log(prefix + " is doing " + name);
      else
      {
         Debug.Log($"{prefix} is doing {name} in order to prepare for {_parentAct.name}");
      }
      //
      
      OnActDone.Invoke(this);
   }

   private ActConfig _parentAct;
   
   public async Task ProcessSubActs(string prefix)
   {
      // Process sub acts
      if (requiredActs.Count > 0)
      {
         foreach (var subact in requiredActs)
         {
            subact.ParentAct = this;
            subact.OnActDone = OnActDone;
            await subact.Log(prefix);
         }
      }
   }


}
