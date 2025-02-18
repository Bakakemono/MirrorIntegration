using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class scale : MonoBehaviour
{
   [SerializeField] Vector3 scalechange;
    [SerializeField] float duration;    

    // Start is called before the first frame update
    void Start()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 newScale = originalScale + scalechange;

        ScaleBackAndForth(newScale, originalScale);
        
    }

    // Update is called once per frame
    void ScaleBackAndForth(Vector3 scaleTo, Vector3 scaleFrom)  
    {
        transform.DOScale(scaleTo, duration).OnComplete(
            () => ScaleBackAndForth(scaleFrom, scaleTo)); 
    }  
    
}
