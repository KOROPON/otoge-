using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class AboveNotespawner : MonoBehaviour {
	public int areaAngle  = 360;  //作成する角度
	public int startAngle =   0;  //スタート地点の角度
	public int height     =  10;  //高さ
	public int quality = 100;     //360degのときのtriangle数/2
	public bool isOutward = false; //内向きか外向きか
	public Color color    = new Color(0, 1, 0, 0); //RGBA



	private Vector3[] vertices; //頂点
	private int[] triangles;    //index

	private void makeParams(){
		List<Vector3> vertList = new List<Vector3>();
		List<int> triList = new List<int>();

    float th,v1,v2;
		int max=(int)quality*areaAngle/360;
    for (int i=0;i<=max;i++){
			th=i*areaAngle/max + startAngle;
			v1=Mathf.Sin(th * Mathf.Deg2Rad);
			v2=Mathf.Cos(th * Mathf.Deg2Rad);
			vertList.Add(new Vector3(v1,0,v2));
			vertList.Add(new Vector3(v1,height,v2));
			if(i<=max-1){
				if(isOutward){
					triList.Add(2*i);triList.Add(2*i+3);triList.Add(2*i+1);
	  				triList.Add(2*i);triList.Add(2*i+2);triList.Add(2*i+3);
				}else{
  					triList.Add(2*i);triList.Add(2*i+1);triList.Add(2*i+3);
	  				triList.Add(2*i);triList.Add(2*i+3);triList.Add(2*i+2);
				}
			}
    }
    vertices  = vertList.ToArray();
		triangles = triList.ToArray();
	}

	private void setParams(){
		Mesh mesh = new Mesh();

		mesh.vertices = vertices;
		mesh.triangles = triangles;

		// 法線とバウンディングの計算
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		mesh.name = "tubeMesh";


		GetComponent<MeshFilter>().sharedMesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;

		// 色指定
		GetComponent<MeshRenderer>().material.color = color;
	}

	void Start(){
		makeParams();
		setParams();
	}
}
