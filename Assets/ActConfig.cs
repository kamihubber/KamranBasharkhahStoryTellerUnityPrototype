using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

[Serializable]
public struct NeedEffect
{
   public NeedConfig needConfig;
   public int amount;
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

   //todo : logic should not be here , this is config?
   
   public void Log(string prefix)
   {
      Debug.Log(prefix + " is doing " + name);
   }

}
