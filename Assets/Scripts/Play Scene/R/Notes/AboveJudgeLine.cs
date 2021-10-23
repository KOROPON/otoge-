#nullable enable

using System;
using UnityEngine;

namespace Reilas
{

    class AboveJudgeLine : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector3[]? _uv;
        private int[]? _triangles;
        private Mesh? _mesh;
        const float div = 32f;
        const float outerLaneRadius = 4.4f;

        const float innerRadius = outerLaneRadius - 0.03f; // 内縁の半径
        const float outerRadius = outerLaneRadius;        // 外縁の半径

        void Start()
        {
            if (_meshFilter == null)
            {
                return;
            }
            
            _vertices = new Vector3[66];
            _uv = new Vector3[66];
            _triangles = new int[192];

            // 前面
            for (var i = 0; i < 33 - 1; i++)
            {
                _triangles[i * 6 + 0] = 0 + i * 2;
                _triangles[i * 6 + 1] = 1 + i * 2;
                _triangles[i * 6 + 2] = 3 + i * 2;
                _triangles[i * 6 + 3] = 2 + i * 2;
                _triangles[i * 6 + 4] = 0 + i * 2;
                _triangles[i * 6 + 5] = 3 + i * 2;
            }


            for (var z = 0; z < 1; z++)
            {
                for (var x = 0; x < 33; x++)
                {
                    var laneIndex = x;  //レーン番号

                    var angle = Mathf.PI / div * laneIndex;   // レーンの角度

                    angle = Mathf.PI - angle;


                    var innerY = Mathf.Sin(angle) * innerRadius;
                    var innerX = Mathf.Cos(angle) * innerRadius;

                    var outerY = Mathf.Sin(angle) * outerRadius;
                    var outerX = Mathf.Cos(angle) * outerRadius;



                    //zPos += zz;

                    var innerPoint = new Vector3(innerX, innerY, 0);
                    var outerPoint = new Vector3(outerX, outerY, 0);


                    //(innerPoint, outerPoint) = (outerPoint, innerPoint);

                    var p = 66 * z;

                    if (_vertices != null)
                    {
                        _vertices[p + x * 2 + 0] = innerPoint;
                        _vertices[p + x * 2 + 1] = outerPoint;
                    }

                    float uvX = 1f / 32 * 0.8f * x + 0.1f;

                    // 手前
                    if (z == 0)
                    {
                        if (_uv != null)
                        {
                            _uv[x * 2 + 0] = new Vector2(uvX, 1f);
                            _uv[x * 2 + 1] = new Vector2(uvX, 0f);
                        }
                    }
                }
            }

            // メッシュを生成する
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = _triangles
            };
            _mesh.MarkDynamic();


            if (_mesh != null)
            {
                _mesh.vertices = _vertices;

                //GetComponent<MeshRenderer>().material.cal

                _mesh.SetUVs(0, _uv);
#if UNITY_EDITOR
                _mesh.RecalculateBounds();
#endif
                _meshFilter.mesh = _mesh;
            }
        }
    }
}