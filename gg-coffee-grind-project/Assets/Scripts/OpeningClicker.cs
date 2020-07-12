using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningClicker : MonoBehaviour
{
    private Coroutine coroutine;
    public AnimationCurve beanCountToOpeningAmount;
    public Opening opening;

    private bool mouseOver = false;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        StartCoroutine(closedState());
    }

    void OnMouseEnter()
    {
        mouseOver = true;
    }

    void OnMouseExit() {
        mouseOver = false;
    }

    IEnumerator closedState(){
        opening.expandAmount = 0;
        while(true){
            if(Input.GetMouseButton(0) && mouseOver){
                StartCoroutine(openState());
                yield break;
            }

            opening.target = Mathf.Lerp(opening.target,2,0.3f);


            yield return null;
        }
    }

    IEnumerator openState(){
        
        opening.expandAmount = 0.05f;
        while(true){
            if(!Input.GetMouseButton(0) || !mouseOver){
                StartCoroutine(closedState());
                yield break;
            }
            float target = beanCountToOpeningAmount.Evaluate(BeanManager.activeBeanCount);
            opening.target = Mathf.Lerp(opening.target,target,0.3f);
            yield return null;
        }
    }
}
