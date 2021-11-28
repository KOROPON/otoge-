#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class AboveSlideNote : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter = null!;

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

        private AboveSlideEffector _effectorCs = null!;

        private bool _kujo;

        public void Initialize(ReilasNoteLineEntity entity, bool kujo)
        {
            _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
            _kujo = kujo;
            _noteSpeed = entity.Head.Speed;
            _entity = entity;
            InitializeMesh();
            //Debug.Log(_entity.Head.Size + "      " + _entity.Tail.Size + "           a");

            transform.localScale = Vector3.one;

            _effectorCs = this.gameObject.transform.GetChild(0).gameObject.GetComponent<AboveSlideEffector>();
        }

        private void InitializeMesh()
        {
            if (meshFilter == null)
            {
                throw new Exception();
                //return;
            }

            var xDivision = _entity.Head.Size + 1;
            var zDivision = 2 + Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);

            //_vertices = new Vector3[(xDivision + xTailDivision) / 2 * zDivision];


            //_triangles = new int[(_vertices.Length - 2) * 6];

            _leftRatio = Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);
            _rightRatio = Mathf.Abs((_entity.Head.LanePosition + _entity.Head.Size - 1) - (_entity.Tail.LanePosition + _entity.Tail.Size - 1));


            //前面
            if (_entity.Head.Size <= _entity.Tail.Size)
            {
                _thisNoteSize = _entity.Tail.Size;

                if (_leftRatio > _rightRatio) // 左辺のほうが変化量が多い
                {
                    _vertices = new Vector3[(_entity.Tail.Size + 1) * (_leftRatio + 2)];
                    _triangles = new int[_entity.Tail.Size * (_leftRatio + 1) * 6];
                    _uv = new Vector2[(_entity.Tail.Size + 1) * (_leftRatio + 2)];

                    for (int z = 0; z < _leftRatio + 1; z++)
                    {
                        for (int x = 0; x < _entity.Tail.Size; x++)
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

                    for (int z = 0; z < _rightRatio + 1; z++)
                    {
                        for (int x = 0; x < _entity.Tail.Size; x++)
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

                    for (int z = 0; z < _leftRatio + 1; z++)
                    {
                        for (int x = 0; x < _entity.Head.Size; x++)
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

                    for (int z = 0; z < _rightRatio + 1; z++)
                    {
                        for (int x = 0; x < _entity.Head.Size; x++)
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
            /*
            else
            {
                _vertices = new Vector3[xDivision * zDivision];
                _triangles = new int[(_vertices.Length - 2) * 6];
                _uv = new Vector2[xDivision * zDivision];

                for (var z = zDivision - 2; z >= 0; z--)
                {
                    var n = z * (xDivision - 1) * 6;
                    for (var x = xDivision - 2; x >= 0; x--)
                    {
                        _triangles[n + x * 6 + 0] = z * (xDivision) + x;
                        _triangles[n + x * 6 + 1] = z * (xDivision) + x + 1;
                        _triangles[n + x * 6 + 2] = (z + 1) * (xDivision) + x;
                        _triangles[n + x * 6 + 3] = z * (xDivision) + x + 1;
                        _triangles[n + x * 6 + 4] = (z + 1) * (xDivision) + x + 1;
                        _triangles[n + x * 6 + 5] = (z + 1) * (xDivision) + x;
                    }
                }
            }
            */

            var newTriangles = new int[_triangles.Length];
            for (var i = 0; i < newTriangles.Length; i++)
            {
                newTriangles[i] = _triangles[newTriangles.Length - 1 - i];
            }

            // メッシュを生成する.
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = newTriangles
            };
            _mesh.MarkDynamic();
        }

        public void Render(float currentTime, int noteNum, List<ReilasNoteLineEntity> noteList, List<SpeedChangeEntity> speedChangeEntities)
        {
            RenderMesh(currentTime, noteNum, noteList, speedChangeEntities);
        }

        private void RenderMesh(float currentTime, int noteNum, IList noteList, List<SpeedChangeEntity> speedChangeEntities)
        {
            if (meshFilter == null || _mesh == null || _vertices == null) return;

            if (_entity.Tail.JudgeTime < currentTime) // NoteDestroy
            {
                foreach (Transform child in this.transform.GetChild(0)) Destroy(child.gameObject);
                Destroy(this.transform.GetChild(0).gameObject);
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

            var headZ = NotePositionCalculatorService.GetPosition(_entity.Head, currentTime, _noteSpeed, speedChangeEntities);
            var tailZ = NotePositionCalculatorService.GetPosition(_entity.Tail, currentTime, _noteSpeed, speedChangeEntities);


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

            /*
            if (_effectorCs.blJudge) // 押されていたら
            {
                var timeRatio = (currentTime - _entity.Head.JudgeTime) / (_entity.Tail.JudgeTime - _entity.Head.JudgeTime);
                var judgeLaneSize = Mathf.Lerp(_entity.Head.Size, _entity.Tail.Size, timeRatio);
                var judgeLaneMin = Mathf.Lerp(_entity.Head.LanePosition, _entity.Tail.LanePosition, timeRatio);

                for (int znum = 0; znum < Mathf.Floor(thisNoteZRatio * timeRatio) - 1; znum++)
                {
                    for(var xNum = 0; xNum <= _thisNoteSize; xNum++)
                    {
                        var angle = Mathf.PI / div * (judgeLaneMin + judgeLaneSize / _thisNoteSize * xNum);
                        angle = Mathf.PI / 2f - angle;
                        var x = Mathf.Sin(angle) * outerLaneRadius;
                        var y = Mathf.Cos(angle) * outerLaneRadius;
                        Debug.Log(thisNoteZRatio + " " + _thisNoteSize);
                        Debug.Log(_vertices.Length + "  >  " + znum + "X" + (_thisNoteSize + 1) + " = " + znum * (_thisNoteSize + 1));
                        _vertices[znum * (_thisNoteSize + 1) + xNum] = new Vector3(-x, y, -0.4f);
                    }
                }
                _mesh.vertices = _vertices;

                //GetComponent<MeshRenderer>().material.cal

                _mesh.uv = _uv;
#if UNITY_EDITOR
                _mesh.RecalculateBounds();
#endif
                meshFilter.mesh = _mesh;
            }
            */

            _mesh.vertices = _vertices;

            //GetComponent<MeshRenderer>().material.cal

            _mesh.uv = _uv;
#if UNITY_EDITOR
            _mesh.RecalculateBounds();
#endif
            meshFilter.mesh = _mesh;
        }


        public void NoteDestroy(bool kujo)
        {
            for (int a = 2; a >= 0; a--)
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
                _presenter._reilasAboveSlide.RemoveAt(RhythmGamePresenter.AboveSlideNotes.IndexOf(this));
                RhythmGamePresenter.AboveSlideNotes.Remove(this);
            }
            Destroy(gameObject);
        }
    }
}
