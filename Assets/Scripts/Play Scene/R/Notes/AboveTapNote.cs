#nullable enable
using System;
using UnityEngine;

namespace Reilas
{
    public sealed class AboveTapNote : MonoBehaviour
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
        
        public float aboveTapTime;

        public void Initialize(ReilasNoteEntity entity, bool kujo)
        {
            _noteSpeed = entity.Speed;
            aboveTapTime = entity.JudgeTime;
            _entity = entity;
            _kujo = kujo;
            _position = NotePositionCalculatorService.LeftOverPositionCalculator(aboveTapTime, _noteSpeed);
            _speedChangeIndex = 0;
            
            InitializeMesh();
            transform.localScale = new Vector3(1,1,1);
        }

        private void InitializeMesh()
        {
            if (meshFilter == null) throw new Exception();

            var size = _entity.Size + 1;
            var entityBase = size * 10;
            var uAndV = entityBase * 2;

            _vertices = new Vector3[uAndV];
            _uv = new Vector3[uAndV];
            _triangles = new int[entityBase * 6 + 12];

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
            _position -= NotePositionCalculatorService.SpanCalculator(_speedChangeIndex, aboveTapTime, _noteSpeed);
            _speedChangeIndex++;
        }

        private void RenderMesh(float currentTime)
        {
            if (meshFilter == null) return;

            const float div = 32f;
            const float outerLaneRadius = 5.6f;

            const float innerRadius = outerLaneRadius - 3f; // 内縁の半径

            for (var x = 0; x < _entity.Size + 1; x++)
            {
                var laneIndex = _entity.LanePosition + x;  //レーン番号
                var angleBase = div - laneIndex;   // レーンの角度
                var angle = Mathf.PI * angleBase / div;


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

                if (_vertices != null)
                {
                    _vertices[x * 2] = innerPoint;
                    _vertices[x * 2 + 1] = outerPoint;
                }

                var uvX = 1f / _entity.Size * 0.8f * x + 0.1f;

                // 手前
                if (_uv == null) continue;
                _uv[x * 2 + 0] = new Vector2(uvX, 1f);
                _uv[x * 2 + 1] = new Vector2(uvX, 0f);
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
            if (kujo) RhythmGamePresenter.AboveKujoTapNotes.Remove(this);
            else RhythmGamePresenter.AboveTapNotes.Remove(this);
            Destroy(gameObject);
        }
    }
}
