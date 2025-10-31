using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegularGoapStrategy : IGoaperStrategy
{
    //think , experience, ...
    
    
    public List<Act> GetActs(List<Need> needs, List<Act> _actsRepo)
    {
        List<Act> acts = new List<Act>();

        //how to know if we can do an act or we should first fullfill its requirements??
        
        foreach (var need in needs)
        {
            var possibleActsForThisNeed = _actsRepo.Where(actr => actr.effects.Find(ne => ne.need.name == need.name).amount > 0);
            int randomindex = UnityEngine.Random.Range(0, possibleActsForThisNeed.Count());
            acts.Add(possibleActsForThisNeed.ElementAt(randomindex));
        }
        
        return acts;
    }
}
