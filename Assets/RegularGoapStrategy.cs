using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegularGoapStrategy : IGoaperStrategy
{
    //think , experience, ...
    
    
    
    public Dictionary<NeedConfig, ActConfig> GetActs(List<NeedConfig> needsConfigs, List<ActConfig> _actsRepo, List<Need> characterNeeds)
    {
        Dictionary<NeedConfig, ActConfig> acts = new Dictionary<NeedConfig, ActConfig>();

        //how to know if we can do an act or we should first fullfill its requirements??
        
        //-traits
        
        //- character initial need points -> select need (done) //prospective base class feature
        //- character should call this strategy each frame but this should only update tasks if needed
        //-if an act has an active task then only change it if u have some special adjectives , like "u cant focus"
        
        //- act time
        //- task result/effect
        //- (learn after task result , experience)
        //- (idealogy over adjective)
        //- maybe we should check not to duplicate handled needs tasks
        //- should we know / track which goal/need each task relates to ?
        //when character chnages the goal tasks again?
        
        //probably should be filtered in character class
        var filteredNeedsConfigsForProcess = needsConfigs.Where(nc => ShouldProcessNeed(nc, characterNeeds)).ToList();

        bool ShouldProcessNeed(NeedConfig needsConfig, List<Need> characterNeeds)
        {
            Need targetNeed = characterNeeds.Find(cn => cn.config == needsConfig);
            
            return targetNeed != null && targetNeed.FullFillAmount < 5;
            
            return false;
        }
        
        foreach (var needConfig in filteredNeedsConfigsForProcess)
        {
            var possibleActsForThisNeed =
                _actsRepo.Where(actr => actr.effects.Find(ne => ne.needConfig == needConfig).amount > 0);
            
            //strategy
            //decide on adjectives, and scores
            //and then acts requirements (a kind of decomposition here??)
            int randomindex = UnityEngine.Random.Range(0, possibleActsForThisNeed.Count());
            
            acts.Add(needConfig, possibleActsForThisNeed.ElementAt(randomindex));
        }
        
        return acts;
    }
}
