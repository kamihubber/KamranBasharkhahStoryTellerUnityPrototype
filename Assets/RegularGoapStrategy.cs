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
        
        //- character initial need points -> select need
        //-character should call this strategy each frame but this should only update tasks if needed
        //-if an act has an active task then only change it if u have some special adjectives , like "u cant focus"
        //- act time
        //- task result/effect
        //- (learn after task result , experience)
        //- (idealogy over adjective)
        //- maybe we should check not to duplicate handled needs tasks
        //- should we know / track which goal/need each task relates to ?
        //when character chnages the goal tasks again?
        
        foreach (var need in needs)
        {
            var possibleActsForThisNeed =
                _actsRepo.Where(actr => actr.effects.Find(ne => ne.need == need).amount > 0);
            
            //strategy
            //decide on adjectives, and scores
            //and then acts requirements (a kind of decomposition here??)
            int randomindex = UnityEngine.Random.Range(0, possibleActsForThisNeed.Count());
            
            acts.Add(possibleActsForThisNeed.ElementAt(randomindex));
        }
        
        return acts;
    }
}
