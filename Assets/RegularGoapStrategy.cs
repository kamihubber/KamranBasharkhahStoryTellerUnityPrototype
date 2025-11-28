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
        
        //refactor?
        //task time issue?
        
        //subtasks does not effect needs now,we may need to pull subact
        //code from act and instead add subacts to output here
        
        //needs for acts,can this help with scoring problem ?
        //everything needs energy style?which maks people seek acts and objects which ahve nergy...
        //(needs for needs)
        
        //distraction
        
        //objects for acts,needs?
        ////create act need dynamically ?
        
        //subact improve, fullfill of parent (2 factor system or need effect style for parent)
        //acts time (test)
        //subacts process order
        //needs priority, energy , time , ...
        
        //negative,minus effect amounts on needs -> releasing current act for critical need
        //(create act need dynamically ?)
        //the need effects of subacts can be parent acts ?

        //how to know if we can do an act or we should first fullfill its requirements??
        
        //-traits
        //-if an act has an active task then only change it if u have some special adjectives , like "u cant focus"
        
        // - act target(people,objects)
        // - Radius(act or anything)
        
        //Done- character initial need points -> select need (done) //prospective base class feature
        //Done- character should call this strategy each frame but this should only update tasks if needed(done)
        
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
