using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opening : MonoBehaviour
{
    public float target;
    public float rotationSpeed;
    public float expandAmount;

    float rotation = 0;
    // Update is called once per frame
    void Update()
    {
        rotation += Time.deltaTime*rotationSpeed;
        transform.rotation = Quaternion.Euler(0,rotation,0);
        for(int i = 0; i<transform.childCount;i++){
            Transform child = transform.GetChild(i);
            Vector3 scale = child.localScale;
            scale.z = target;
            scale.y = 0.77f + Mathf.Sin(rotation + 1.0715f*i)*expandAmount;
            child.localScale = scale;
        }
    }
}
