using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DustManager : MonoBehaviour
{
    public static DustManager Instance;
    public DustMesh dustMesh;
    public float dustPerParticle;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void AddDust(Vector3 position, float amount){
        dustMesh.AddDustAt(position,amount*dustPerParticle);
    }

    public void Smooth(){
        if(dustMesh.Smooth()){
            dustMesh.transform.parent.DOShakePosition(0.2f,0.2f,25,0);
        }
    }
}
