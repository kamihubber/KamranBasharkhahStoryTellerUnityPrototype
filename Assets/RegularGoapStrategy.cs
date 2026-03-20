using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RegularGoapStrategy : IGoaperStrategy
{
    //think , experience, ...
    
    public Dictionary<NeedConfig, Act> GetActs(List<NeedConfig> needsConfigs, List<ActConfig> _actsRepo, List<Need> characterNeeds)
    {
        Dictionary<NeedConfig, Act> acts = new Dictionary<NeedConfig, Act>();
        
        //where is emergant factor?
        //now we have seek and interact and gain skills for components/skills
        //this might change tho
        //but for now we need apply emergant based on traits ON..
        // ON seek & interact & gain skills
        
        //we are trying to reach interactions frist , we use static style for rapid test
        //( ** maybe , add a feature , some subacts cant be sarted untill their requirements are met)
        
        //acts fullfil their parents insted of main needs
        //+ time or required work
        // + a chance of fail
        
        //interactions , targeting
          //we look for (components of ) what we seek , in every thing , people , objects , things , events , moments ..... 
          //objects
        
        //subact -> provide skill/factor -> fullfill parent act
        //after interactions,targeting and objects , we might add
        //add something like skill or grading among acts
        //acts might provide some other factors than needs
        //inorder to fullfill their parent act
        
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
    
    //emergant candidate factors : change of skills and traits in time
    //emergant candidate factors : envoirment effects on current state
    
    //emergant sseking process should be placed here 
    //some skills like thinking might effect this process
    //intelligence might be a better name for thinking skill

    public ActConfig GetInterAct(NeedConfig needsConfig, List<ActConfig> characterActs, SkillComp seekingComp, GameEntity self,
        GameEntity otherCharacterEntity)
    {
        //myabe check our staibility first, for now using traits but it should be mental state
        //if not stable more insane randomize
        //if stable ,score on interactable components of two entities (might interefere seekingcomp or traits later) ->
        //probably with a simple distraction/mistake randomness
        
        //tagging/groupping components, (types?)
        
        
        
        if (self.MentalState == MentalState.Normal)
        {
            //always check placeable comp for materials/physical domain
            //(acts which have seekingComp may or may not be an option)
            //find acts suitable for interacting? have special interact components?
            //may first check thinking skill
            
            //final comments to perform , for now
            //0 - find common interact skills among self and other
            //0 - can go out of common range based on mistake/random factor
            //1 - sort descending other entity skill with skill amount
            //2 - my traits + simple random (mistake , ...)
              //2-0 sort traits?
              //2 -1 get a range of them based on my traits (sort tarits?)
              //2 -2 choose in range with a randomness , can exeed range based on mistake or ...

              //how traits effect?
              //list of interactable acts for each entity (type/group or single) with different values
              //
              
              //choose based on skill levels?
              //multiple interacts (walk , ...), basic interacts...

              self.Skills.Sort((comp, skillComp) =>
              {
                  return comp.skillLevel - skillComp.skillLevel;
              });
              
              otherCharacterEntity.Skills.Sort((comp, skillComp) =>
              {
                  return comp.skillLevel - skillComp.skillLevel;
              });
              
              List<SkillComp> commonSkills = new List<SkillComp>();
              
              foreach (var skill in self.Skills)
              {
                  otherCharacterEntity.Skills.FindAll(skl => skl.skillType == skill.skillType).ForEach(skl => commonSkills.Add(skill));
              }

              SkillComp skillResult = new SkillComp();

              if (commonSkills.Count == 0)
                  skillResult = self.Skills.Last();
              else
              {
                  SkillComp max;
                  max = commonSkills[0];
                  foreach (var cs in commonSkills)
                  {
                      if (cs.skillLevel > max.skillLevel)
                          max = cs;
                  }
                  skillResult = max;
              }

              foreach (var act in characterActs)
              {
                  if (act.RequiredSkills.Count > 0)
                  {
                      var allActsAcceptable = characterActs.FindAll(actr => actr.RequiredSkills.Any(rs => rs.skillType == skillResult.skillType));
                      return allActsAcceptable.FirstOrDefault();
                  }
              }

        }
        else
        {
            //go insane
            //go disappointed
            //..
            //having some traits or components might normalize this a bit!
        }
        
        return null;
    }
}
