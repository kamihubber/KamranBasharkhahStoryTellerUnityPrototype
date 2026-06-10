using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] private GameObject dialougeBaloon;
    [SerializeField] private TextMeshPro dialougeText;

    private bool isMoving;
    [SerializeField] private float moveSpeed = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(GameEntity subject, Act act, SkillComp skillComp)
    {
        Debug.Log($"Body Interact with {subject.name}");
        MoveToSubject(subject.Body.gameObject);
        ShowDialougeBaloon($"i am doing {act.Name} with {subject.name} for {skillComp.skillType}");
    }

    private void ShowDialougeBaloon(string text)
    {
        dialougeBaloon.SetActive(true);
        dialougeText.text = text;
    }
    
    private void HideDialougeBaloon()
    {
        dialougeBaloon.SetActive(false);
        dialougeText.text = "";
    }

    public void Act(GameObject subject)
    {
        Debug.Log($"Body Act at {subject.name}");
    }

    private void MoveToSubject(GameObject subject)
    {
        transform.DOMove(subject.transform.position, 5);
    }
}
