#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reilas
{
    public sealed class AboveSlideNote : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter = null!;
        
        [SerializeField] private float headJudgeTime;
        [SerializeField] private float tailJudgeTime;

        private Vector3[]? _vertices;
        private Vector2[] _uv = null!;
        private int[]? _triangles;

        private Mesh? _mesh;

        private ReilasNoteLineEntity _entity = null!;
        private RhythmGamePresenter _presenter = null!;

        private int _thisNoteSize;
        private int _leftRatio;
        private int _rightRatio;
        
        private float _noteSpeed;
        private float _headPosition;
        private float _tailPosition;

        private int _headIndex;
        private int _tailIndex;

        private AboveSlideEffector _effectorCs = null!;

        private bool _kujo;

        public void Initialize(ReilasNoteLineEntity entity, bool kujo)
        {
            _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
            _kujo = kujo;
            _noteSpeed = entity.Head.Speed;
            _entity = entity;
            _noteSpeed = _entity.Head.Speed;
            _headPosition = NotePositionCalculatorService.LeftOverPositionCalculator(headJudgeTime, _noteSpeed);
            _tailPosition = NotePositionCalculatorService.LeftOverPositionCalculator(tailJudgeTime, _noteSpeed);
            _headIndex = 0;
            _tailIndex = 0;
            
            InitializeMesh();

            transform.localScale = Vector3.one;

            _effectorCs = gameObject.transform.GetChild(0).gameObject.GetComponent<AboveSlideEffector>();
        }

        private void InitializeMesh()
        {
            if (meshFilter == null) throw new Exception();

            _leftRatio = Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);
            _rightRatio = Mathf.Abs(_entity.Head.LanePosition + _entity.Head.Size - 1 - (_entity.Tail.LanePosition + _entity.Tail.Size - 1));


            //前面
            if (_entity.Head.Size <= _entity.Tail.Size)
            {
                _thisNoteSize = _entity.Tail.Size;

                if (_leftRatio > _rightRatio) // 左辺のほうが変化量が多い
                {
                    _vertices = new Vector3[(_entity.Tail.Size + 1) * (_leftRatio + 2)];
                    _triangles = new int[_entity.Tail.Size * (_leftRatio + 1) * 6];
                    _uv = new Vector2[(_entity.Tail.Size + 1) * (_leftRatio + 2)];

                    for (var z = 0; z < _leftRatio + 1; z++)
                    {
                        for (var x = 0; x < _entity.Tail.Size; x++)
                        {
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 5] = z * (_entity.Tail.Size + 1) + x;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 4] = z * (_entity.Tail.Size + 1) + x + 1;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 3] = (z + 1) * (_entity.Tail.Size + 1) + x;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 2] = z * (_entity.Tail.Size + 1) + x + 1;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 1] = (z + 1) * (_entity.Tail.Size + 1) + x + 1;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 0] = (z + 1) * (_entity.Tail.Size + 1) + x;
                        }
                    }
                }
                else
                {
                    _vertices = new Vector3[(_entity.Tail.Size + 1) * (_rightRatio + 2)];
                    _triangles = new int[_entity.Tail.Size * (_rightRatio + 1) * 6];
                    _uv = new Vector2[(_entity.Tail.Size + 1) * (_rightRatio + 2)];

                    for (var z = 0; z < _rightRatio + 1; z++)
                    {
                        for (var x = 0; x < _entity.Tail.Size; x++)
                        {
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 5] = z * (_entity.Tail.Size + 1) + x;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 4] = z * (_entity.Tail.Size + 1) + x + 1;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 3] = (z + 1) * (_entity.Tail.Size + 1) + x;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 2] = z * (_entity.Tail.Size + 1) + x + 1;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 1] = (z + 1) * (_entity.Tail.Size + 1) + x + 1;
                            _triangles[z * _entity.Tail.Size * 6 + x * 6 + 0] = (z + 1) * (_entity.Tail.Size + 1) + x;
                        }
                    }
                }
            }
            else
            {
                _thisNoteSize = _entity.Head.Size;

                if (_leftRatio > _rightRatio)
                {
                    _vertices = new Vector3[(_entity.Head.Size + 1) * (_leftRatio + 2)];
                    _triangles = new int[_entity.Head.Size * (_leftRatio + 1) * 6];
                    _uv = new Vector2[(_entity.Head.Size + 1) * (_leftRatio + 2)];

                    for (var z = 0; z < _leftRatio + 1; z++)
                    {
                        for (var x = 0; x < _entity.Head.Size; x++)
                        {
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 5] = z * (_entity.Head.Size + 1) + x;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 4] = z * (_entity.Head.Size + 1) + x + 1;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 3] = (z + 1) * (_entity.Head.Size + 1) + x;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 2] = z * (_entity.Head.Size + 1) + x + 1;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 1] = (z + 1) * (_entity.Head.Size + 1) + x + 1;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 0] = (z + 1) * (_entity.Head.Size + 1) + x;
                        }
                    }
                }
                else
                {
                    _vertices = new Vector3[(_entity.Head.Size + 1) * (_rightRatio + 2)];
                    _triangles = new int[_entity.Head.Size * (_rightRatio + 1) * 6];
                    _uv = new Vector2[(_entity.Head.Size + 1) * (_rightRatio + 2)];

                    for (var z = 0; z < _rightRatio + 1; z++)
                    {
                        for (var x = 0; x < _entity.Head.Size; x++)
                        {
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 5] = z * (_entity.Head.Size + 1) + x;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 4] = z * (_entity.Head.Size + 1) + x + 1;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 3] = (z + 1) * (_entity.Head.Size + 1) + x;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 2] = z * (_entity.Head.Size + 1) + x + 1;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 1] = (z + 1) * (_entity.Head.Size + 1) + x + 1;
                            _triangles[z * _entity.Head.Size * 6 + x * 6 + 0] = (z + 1) * (_entity.Head.Size + 1) + x;
                        }
                    }
                }
            }

            var newTriangles = new int[_triangles.Length];
            for (var i = 0; i < newTriangles.Length; i++) newTriangles[i] = _triangles[newTriangles.Length - 1 - i];

            // メッシュを生成する.
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = newTriangles
            };
            _mesh.MarkDynamic();
        }

        public void Render(float currentTime, int noteNum, List<ReilasNoteLineEntity> noteList)
        {
            RenderMesh(currentTime, noteNum, noteList);
            
            if (!RhythmGamePresenter.checkSpeedChangeEntity ||
                currentTime < RhythmGamePresenter.CalculatePassedTime(_headIndex)) return;
            _headPosition -= NotePositionCalculatorService.SpanCalculator(_headIndex, headJudgeTime, _noteSpeed);
            _headIndex++;
            
            if (currentTime < RhythmGamePresenter.CalculatePassedTime(_tailIndex)) return;
            _tailPosition -= NotePositionCalculatorService.SpanCalculator(_tailIndex, tailJudgeTime, _noteSpeed);
            _tailIndex++;
        }

        private void RenderMesh(float currentTime, int noteNum, IList noteList)
        {
            if (meshFilter == null || _mesh == null || _vertices == null) return;


            if (tailJudgeTime < currentTime) // NoteDestroy
            {
                foreach (Transform child in transform.GetChild(0)) Destroy(child.gameObject);
                Destroy(transform.GetChild(0).gameObject);
                Destroy(gameObject);
                RhythmGamePresenter.AboveSlideEffectors.Remove(transform.GetChild(0).GetComponent<AboveSlideEffector>());
                if (_kujo)
                {
                    _presenter.reilasKujoAboveSlide.RemoveAt(noteNum);
                    RhythmGamePresenter.AboveKujoSlideNotes.RemoveAt(noteNum);
                }
                else
                {
                    noteList.RemoveAt(noteNum);
                    RhythmGamePresenter.AboveSlideNotes.RemoveAt(noteNum);
                }
            }

            // SetActive
            if (!gameObject.activeSelf) gameObject.SetActive(true);

            //var zDiv = 2 + Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);

            var headZ = NotePositionCalculatorService.GetPosition(headJudgeTime, currentTime, _noteSpeed, _headPosition, _headIndex);
            var tailZ = NotePositionCalculatorService.GetPosition(tailJudgeTime, currentTime, _noteSpeed, _tailPosition, _tailIndex);


            int thisNoteZRatio;

            if (_leftRatio > _rightRatio) thisNoteZRatio = _leftRatio + 2;
            else thisNoteZRatio = _rightRatio + 2;

            const float div = 32f;
            const float outerLaneRadius = 4.4f;

            for (var z = 0; z < thisNoteZRatio; z++)
            {
                var p2 = 1f / (thisNoteZRatio - 1) * z;

                var currentZ = Mathf.Lerp(headZ, tailZ, p2);

                var nowLaneSize = Mathf.Lerp(_entity.Head.Size, _entity.Tail.Size, p2);

                for (var x = 0; x <= _thisNoteSize; x++)
                {
                    var laneIndex = Mathf.Lerp(_entity.Head.LanePosition, _entity.Tail.LanePosition, p2) + nowLaneSize / _thisNoteSize * x; //今作る頂点のレーン番号(小数点以下含む)

                    var angle = Mathf.PI / div * laneIndex;

                    angle = Mathf.PI / 2f - angle;

                    var outerX = Mathf.Sin(angle) * outerLaneRadius;
                    var outerY = Mathf.Cos(angle) * outerLaneRadius;

                    var outerPoint = new Vector3(-outerX, outerY, currentZ);

                    _vertices[(_thisNoteSize + 1) * z + x] = outerPoint;
                    _uv[z * (_thisNoteSize + 1) + x] = new Vector2(1f / _thisNoteSize * x, 1f / (thisNoteZRatio - 1) * z);
                }
            }
            
            _mesh.vertices = _vertices;

            _mesh.uv = _uv;
#if UNITY_EDITOR
            _mesh.RecalculateBounds();
#endif
            meshFilter.mesh = _mesh;
        }


        public void NoteDestroy(bool kujo)
        {
            for (var a = 2; a >= 0; a--)
            {
                Destroy(transform.GetChild(0).GetChild(a).GetComponent<ParticleSystem>());
                Destroy(transform.GetChild(0).GetChild(a).gameObject);
            }
            Destroy(transform.GetChild(0).GetComponent<AboveSlideEffector>());
            RhythmGamePresenter.AboveSlideEffectors.Remove(transform.GetChild(0).GetComponent<AboveSlideEffector>());
            Destroy(transform.GetChild(0).gameObject);
            if (kujo)
            {
                _presenter.reilasKujoAboveSlide.RemoveAt(RhythmGamePresenter.AboveKujoSlideNotes.IndexOf(this));
                RhythmGamePresenter.AboveKujoSlideNotes.Remove(this);
            }
            else
            {
                _presenter.reilasAboveSlide.RemoveAt(RhythmGamePresenter.AboveSlideNotes.IndexOf(this));
                RhythmGamePresenter.AboveSlideNotes.Remove(this);
            }
            Destroy(gameObject);
        }
    }
}
