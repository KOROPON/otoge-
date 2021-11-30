#nullable enable
using System;
using UnityEngine;

namespace Reilas
{
    public sealed class AboveChainNote : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector3[]? _uv;
        private int[]? _triangles;

        private Mesh? _mesh;

        private ReilasNoteEntity _entity = null!;

        private bool _kujo;

        private float _noteSpeed;
        private float _position;
        
        private int _speedChangeIndex;
        
        public float aboveChainTime;

        public void Initialize(ReilasNoteEntity entity, bool kujo)
        {
            aboveChainTime = entity.JudgeTime;
            _noteSpeed = entity.Speed;
            _entity = entity;
            _kujo = kujo;
            _position = NotePositionCalculatorService.LeftOverPositionCalculator(aboveChainTime, _noteSpeed);
            _speedChangeIndex = 0;
            
            InitializeMesh();

            transform.localScale = Vector3.one;
        }

        private void InitializeMesh()
        {
            if (meshFilter == null)
            {
                throw new Exception();
                //return;
            }

            var size = _entity.Size + 1;

            _vertices = new Vector3[size * 2 * 10];
            _uv = new Vector3[size * 2 * 10];
            _triangles = new int[size * 6 * 10 + 12];

            // 前面
            for (var i = 0; i < size - 1; i++)
            {
                _triangles[i * 6 + 0] = 0 + i * 2;
                _triangles[i * 6 + 1] = 1 + i * 2;
                _triangles[i * 6 + 2] = 3 + i * 2;
                _triangles[i * 6 + 3] = 2 + i * 2;
                _triangles[i * 6 + 4] = 0 + i * 2;
                _triangles[i * 6 + 5] = 3 + i * 2;
            }
            
            // メッシュを生成する
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = _triangles
            };
            _mesh.MarkDynamic();
        }

        public void Render(float currentTime)
        {
            RenderMesh(currentTime);
            
            if (!RhythmGamePresenter.checkSpeedChangeEntity ||
                currentTime < RhythmGamePresenter.CalculatePassedTime(_speedChangeIndex)) return;
            _position -= NotePositionCalculatorService.SpanCalculator(_speedChangeIndex, aboveChainTime, _noteSpeed);
            _speedChangeIndex++;
        }

        private void RenderMesh(float currentTime)
        {
            if (meshFilter == null)
            {
                Debug.Log("return");
                return;
            }

            const float outerLaneRadius = 5.6f;
            const float innerRadius = outerLaneRadius - 3f; // 内縁の半径
            const float div = 32f;

            for (var z = 0; z < 1; z++)
            {
                for (var x = 0; x < _entity.Size + 1; x++)
                {
                    var laneIndex = _entity.LanePosition + x;  //レーン番号

                    var angle = Mathf.PI / div * laneIndex;   // レーンの角度

                    angle = Mathf.PI - angle;


                    var innerY = Mathf.Sin(angle) * innerRadius;
                    var innerX = Mathf.Cos(angle) * innerRadius;

                    var outerY = Mathf.Sin(angle) * outerLaneRadius;
                    var outerX = Mathf.Cos(angle) * outerLaneRadius;

                    var judgeTime = _entity.JudgeTime;

                    var difference = judgeTime - currentTime;

                    if (!gameObject.activeSelf && difference < 5f) gameObject.SetActive(true);
                    
                    var zPos = NotePositionCalculatorService.GetPosition(judgeTime, currentTime, _noteSpeed, _position, _speedChangeIndex);


                    //zPos += zz;

                    var innerPoint = new Vector3(innerX, innerY, zPos);
                    var outerPoint = new Vector3(outerX, outerY, zPos);


                    //(innerPoint, outerPoint) = (outerPoint, innerPoint);

                    var p = (_entity.Size + 1) * 2 * z;

                    if (_vertices != null)
                    {
                        _vertices[p + x * 2 + 0] = innerPoint;
                        _vertices[p + x * 2 + 1] = outerPoint;
                    }

                    var uvX = 1f / _entity.Size * x;

                    const float alpha = 1f;

                    // 手前
                    if (z != 0) continue;
                    if (_uv == null) continue;
                    _uv[x * 2 + 0] = new Vector3(uvX, 1f, alpha);
                    _uv[x * 2 + 1] = new Vector3(uvX, 0f, alpha);
                }
            }

            if (_mesh == null) return;
            _mesh.vertices = _vertices;

            //GetComponent<MeshRenderer>().material.cal

            _mesh.SetUVs(0, _uv);
#if UNITY_EDITOR
            _mesh.RecalculateBounds();
#endif
            meshFilter.mesh = _mesh;
        }

        public void NoteDestroy(bool kujo)
        {
            if (kujo) RhythmGamePresenter.AboveKujoChainNotes.Remove(this);
            else RhythmGamePresenter.AboveChainNotes.Remove(this);
            Destroy(gameObject);
        }
    }
}