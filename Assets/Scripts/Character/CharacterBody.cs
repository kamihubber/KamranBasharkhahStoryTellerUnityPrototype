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

    [SerializeField] private GameObject mesh;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(GameEntity subject, Act act, SkillComp? skillComp)
    {
        Debug.Log($"Body Interact with {subject.name} using {act.Name}");
        MoveToSubject(subject.Body.gameObject);
        
        if (skillComp.HasValue)
          ShowDialougeBaloon($"i am doing {act.Name} with {subject.name} for {skillComp.Value.skillType}");
        else
        {
            ShowDialougeBaloon($"i am doing {act.Name} with {subject.name} in react");
        }
    }

    public void ShowDialougeBaloon(string text)
    {
        dialougeBaloon.SetActive(true);
        dialougeText.text = text;
        Debug.Log(text);
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
        FaceTarget(subject);
        
        Vector3 targetPos = subject.transform.position;
        int signRND = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        targetPos.x = targetPos.x + (Random.Range(3, 6) * signRND);
        transform.DOMove(targetPos, 7);
    }

    private void FaceTarget(GameObject target)
    {
        Vector2 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        mesh.transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
