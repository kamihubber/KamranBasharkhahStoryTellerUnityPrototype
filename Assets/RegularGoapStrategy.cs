using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegularGoapStrategy : IGoaperStrategy
{
    //think , experience, ...
    
    public Dictionary<NeedConfig, Act> GetActs(List<NeedConfig> needsConfigs, List<ActConfig> _actsRepo, List<Need> characterNeeds)
    {
        Dictionary<NeedConfig, Act> acts = new Dictionary<NeedConfig, Act>();
        
        //we are trying to reach interactions frist , we use static style for rapid test
        //( ** maybe , add a feature , some subacts cant be sarted untill their requirements are met)
        //interactions , targeting
        //objects
        
        //traits
        
        //dashborad improve
        
        //distraction
        
        //objects for acts,needs?
        ////create act need dynamically ?
        
        //(subtasks does not effect needs now,we may need to pull subact
        //code from act and instead add subacts to output here)
        
        //(needs for needs)
        
        //subacts process order
        //needs priority, energy , time , ...
        
        //subact improve, fullfill of parent (2 factor system or need effect style for parent) (done for now)
        
        //(everything needs energy style?which maks people seek acts and objects which have energy...)
        
        //the need effects of subacts can be parent acts ?

        //how to know if we can do an act or we should first fullfill its requirements??
        
        //-traits
        //-if an act has an active task then only change it if u have some special adjectives , like "u cant focus"
        
        // - act target(people,objects)
        // - Radius(act or anything)
        
        //Done- character initial need points -> select need (done) //prospective base class feature
        //Done- character should call this strategy each frame but this should only update tasks if needed(done)
        
        //- act time
        //task time issue?
        //- task result/effect
        
        //- (learn after task result , experience)
        
        //- (idealogy over adjective)
        
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

            ActConfig actConfig = possibleActsForThisNeed.ElementAt(randomindex);
            Act act = new Act(actConfig, actConfig.name, null);
            
            acts.Add(needConfig, act);
        }
        
        return acts;
    }
}
