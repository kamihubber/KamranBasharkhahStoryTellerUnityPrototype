using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashboardHandler : MonoBehaviour
{
    // [SerializeField] private Text need1name;
    // [SerializeField] private Text need2name;
    // [SerializeField] private Text need3name;
    // [SerializeField] private Text need4name;
    // [SerializeField] private Text need5name;
    // [SerializeField] private Text need6name;
    // [SerializeField] private Text need7name;
    //
    // [SerializeField] private Text need1value;
    // [SerializeField] private Text need2value;
    // [SerializeField] private Text need3value;
    // [SerializeField] private Text need4value;
    // [SerializeField] private Text need5value;
    // [SerializeField] private Text need6value;
    // [SerializeField] private Text need7value;
    
    [SerializeField]
    List<Text> needsNames = new List<Text>();
    
    [SerializeField]
    List<Text> needsValues = new List<Text>();
    
    [SerializeField]
    Text charNameText;
    
    [SerializeField]
    Text actsText;
    
    [SerializeField]
    ActsLogger actsLogger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Character currentChar = FindObjectOfType<Character>();
        
        charNameText.text = currentChar.name;

        int h = 0;
        foreach (var need in currentChar.Needs)
        {
            needsNames[h].text = need.config.name;
            needsValues[h].text = need.FullFillAmount.ToString();
            h++;
        }

        foreach (var task in currentChar.Tasks)
        {
            string output = currentChar.GetComponent<Character>().Name + " is doing " + task.Value.Name + " for " + task.Key.Name;

            if (task.Value.RequiredActs != null)
            {
                if (task.Value.RequiredActs.Count > 0)
                {
                    foreach (var subAct in task.Value.RequiredActs)
                    {
                        output += "\n" + "  - " + subAct.Name + " for " + task.Value.Name; ;
                    }
                }
            }
            
            actsLogger.AddLine(output);
        }
    }
}
