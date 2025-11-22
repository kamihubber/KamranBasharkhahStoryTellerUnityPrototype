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
   
   public async Task Log(string prefix)
   {
      await ProcessSubActs(prefix);
      Debug.Log(prefix + " is doing " + name);
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
            await subact.ProcessSubActs(prefix);
         }
      }

      // Perform this act over a random time
      if (_parentAct != null)
      {
         Debug.Log($"{prefix} is doing {name} in order to prepare for {_parentAct.name}");
         
         // float duration = Random.Range(1f, 2f); // seconds
         // float progress = 0f;
         //
         // while (progress < 1f)
         // {
         //    token.ThrowIfCancellationRequested();
         //
         //    Debug.Log($"{prefix} is doing {name} in order to prepare for {_parentAct.name}");
         //
         //    progress += Time.deltaTime / duration;
         //    await Task.Yield(); // returns control but continues loop
         // }
      }
   }


}
