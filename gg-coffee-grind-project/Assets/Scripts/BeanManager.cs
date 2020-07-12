using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Bean{
    public GameObject gameObject;
    public Rigidbody rigidbody;
    public Collider collider;
    public ConstantForce force;
    public MeshRenderer meshRenderer;
}
public class BeanManager : MonoBehaviour
{
    public static int activeBeanCount =0;
    private List<Bean> allBeans = new List<Bean>();
    public Transform visibilityLine;
    public PhysicMaterial beanPhysicMaterial;
    public float mass;
    public float radius;
    public Vector2 forceAmount;
    public Vector2 scaleInterval;
    public Transform topCenter;
    public Vector2 topCenterForce;
    public float topCenterRadius;


    void Start()
    {
        Setup();
    }

    private void Setup(){
        for(int i = 0;i<transform.childCount;i++){
            Bean bean = new Bean();
            bean.gameObject = transform.GetChild(i).gameObject;
            bean.rigidbody = bean.gameObject.GetComponent<Rigidbody>();
            bean.rigidbody.mass = mass;
            bean.collider = bean.gameObject.GetComponent<BoxCollider>();
            bean.collider.material = beanPhysicMaterial;
            bean.force = bean.gameObject.AddComponent<ConstantForce>();
            bean.meshRenderer = bean.gameObject.GetComponent<MeshRenderer>();
            allBeans.Add(bean);
            activeBeanCount++;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        foreach (var item in allBeans)
        {
            Vector3 pos = item.gameObject.transform.position;
            pos.y = 0;
            Vector3 thisPos = transform.position;
            thisPos.y = 0;
            float dist = Vector3.Distance(pos,thisPos);
            float ratio = Mathf.Clamp(dist/radius,0,1);
            float targetScale = Mathf.Lerp(scaleInterval.x,scaleInterval.y,ratio);
            (item.collider as BoxCollider).size = Vector3.one*targetScale;
            //(item.collider as SphereCollider).radius = targetScale;
            //(item.collider as CapsuleCollider).radius = 0.37f*targetScale;
            //(item.collider as CapsuleCollider).height = 1.25f*targetScale;
            item.force.force = (thisPos-pos).normalized*Mathf.Lerp(forceAmount.x,forceAmount.y,(thisPos-pos).magnitude/radius);


            dist = Vector3.Distance(item.gameObject.transform.position,topCenter.position);
            ratio = Mathf.Clamp(dist/topCenterRadius,0,1);
            targetScale = Mathf.Lerp(topCenterForce.x,topCenterForce.y,ratio);
            item.force.force += Vector3.down*targetScale;
            if(item.meshRenderer.enabled && item.gameObject.transform.position.y<visibilityLine.position.y){
                item.meshRenderer.enabled = false;
                SpawnGrinded(item);
                activeBeanCount--;
            }

        }
    }

    public GameObject grindedParticlePrefab;

    private void SpawnGrinded(Bean bean){
        GameObject particle = GameObject.Instantiate(grindedParticlePrefab);
        particle.transform.position = visibilityLine.position;
    }

    
}
