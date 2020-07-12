using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Triangle{
    public int first;
    public int second;
    public int third;

    public Triangle(int first, int second, int third)
    {
        this.first = first;
        this.second = second;
        this.third = third;
    }
}
public class DustMesh : MonoBehaviour
{
    private Mesh dustMesh;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    public Vector2Int resolution;
    public Vector2 xExtents;
    public Vector2 yExtents;

    [Header("Perlin Noise")]
    public float frequency;
    public float strength;

    private List<Vector3> vertices = new List<Vector3>();
    private List<float> dustAmount = new List<float>();
    private List<Triangle> triangles = new List<Triangle>();

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        dustMesh = new Mesh();
        meshFilter.mesh = dustMesh;
        meshCollider.sharedMesh = dustMesh;
        MakeMesh();
    }

    public void MakeMesh(){
        float seed = Random.Range(0f,1f);
        float seed2 = Random.Range(0f,1f);
        for (int i = 0, y = 0; y <= resolution.y; y++) {
			for (int x = 0; x <= resolution.x; x++, i++) {
                float addx = 0,addy = 0;
                if(x!=0 && x!=resolution.x && y!=0&&y!=resolution.y){
                    addx = Mathf.PerlinNoise(seed + frequency*(x+0.0f)/(resolution.x+0.0f),seed + frequency*(y+0.0f)/(resolution.y+0.0f));
                    addy = Mathf.PerlinNoise(seed2 + frequency*(x+0.0f)/(resolution.x+0.0f),seed2 + frequency*(y+0.0f)/(resolution.y+0.0f));

                    addx = (addx*2 - 1)*strength;
                    addy = (addy*2 - 1)*strength;
                }
                vertices.Add(new Vector3( 
                        Mathf.Lerp(xExtents.x,xExtents.y,(x+0.0f)/(resolution.x+0.0f)) + addx,
                        0, 
                        Mathf.Lerp(yExtents.x,yExtents.y,(y+0.0f)/(resolution.y+0.0f)) + addy
                    )
                );
                dustAmount.Add(0);
			}
		}
        dustMesh.vertices = vertices.ToArray();
        Vector2[] uvs = new Vector2[vertices.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        dustMesh.uv = uvs;
        for (int ti = 0, vi = 0, y = 0; y < resolution.y; y++, vi++) {
			for (int x = 0; x < resolution.x; x++, ti += 6, vi++) {
                Triangle trig1 = new Triangle(vi,vi+resolution.x+1,vi+1);
                Triangle trig2 = new Triangle(vi+1,vi+resolution.x+1,vi+resolution.x+2);
                /*
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
                */
                triangles.Add(trig1);
                triangles.Add(trig2);
			}
        }
        dustMesh.triangles = triangleToInt(triangles).ToArray();

        dustMesh.RecalculateNormals();
		
    }

    private List<int> triangleToInt(List<Triangle> list){
        List<int> intlist = new List<int>();
        foreach (var item in list)
        {
            intlist.Add(item.first);
            intlist.Add(item.second);
            intlist.Add(item.third);
        }
        return intlist;
    }
    
    private void UpdateMesh(){
        int k = 0;
        Vector3 temp;
        for(k = 0; k<vertices.Count;k++){
            temp = vertices[k];
            temp.y = dustAmount[k]*heightMultip;
            vertices[k] = temp;
        }
        dustMesh.vertices = vertices.ToArray();
        dustMesh.RecalculateNormals();
        meshCollider.enabled = false;
        meshCollider.enabled = true;

    }


    private struct AdjacencyData{
        public int verticeTarget;
        public float ratioDistance;
    }

    [Header("Raise settings")]
    public float maxRange;
    public float heightMultip;
    public AnimationCurve fallOff;
    public void AddDustAt(Vector3 position, float amount){
        int k = 0;
        float temp;
        position.y = 0;

        float totalRatioDistance = 0;
        List<AdjacencyData> dataList = new List<AdjacencyData>();
        foreach (var item in vertices)
        {
            Vector3 itemflat = item;
            itemflat.y = 0;
            temp = Vector3.Distance(itemflat,position);
            if(temp<=maxRange){
                AdjacencyData data = new AdjacencyData();
                data.verticeTarget = k;
                data.ratioDistance = fallOff.Evaluate(temp/maxRange);
                totalRatioDistance += data.ratioDistance;
                dataList.Add(data);
            }
            k++;
        }

        //need to distribute amount over the datas
        foreach (var item in dataList)
        {
            float newRatio = item.ratioDistance/totalRatioDistance;
            dustAmount[item.verticeTarget] += amount*newRatio;
        }

        UpdateMesh();
    }

    private Coroutine coroutine;
    public bool Smooth(){
        if(coroutine!=null){
            return false;
        }
        coroutine = StartCoroutine(smoothRoutine());
        return true;
    }

    public float lerpPerFrame;
    public float lerpFalloff;

    private IEnumerator smoothRoutine(){
        float lerper = lerpPerFrame;
        while(true){
            float sum = 0;
            foreach (var item in dustAmount)
            {
                sum+=item;
            }
            float average = sum/(dustAmount.Count+0.0f);
            for(int i = 0; i<dustAmount.Count;i++){
                float dust = dustAmount[i];
                float newdust = Mathf.Lerp(dust,average,lerper);
                dustAmount[i] = newdust;
            }
            lerper *= lerpFalloff;
            UpdateMesh();
            if(lerper < 0.0001f){
                break;
            }
            yield return null;
        }
        coroutine = null;
    }
}
