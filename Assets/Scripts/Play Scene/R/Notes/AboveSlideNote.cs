#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reilas
{
    public sealed class AboveSlideNote : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector2[] _uv = null!;
        private int[]? _triangles;

        private Mesh? _mesh;

        private ReilasNoteLineEntity _entity = null!;

        private float thisNoteSize;
        private int leftRatio;
        private int rightRatio;

        private float _noteSpeed;

        public void Initialize(ReilasNoteLineEntity entity)
        {
            _noteSpeed = entity.Head.Speed;
            _entity = entity;
            InitializeMesh();
            //Debug.Log(_entity.Head.Size + "      " + _entity.Tail.Size + "           a");

            transform.localScale = Vector3.one;
        }

        private void InitializeMesh()
        {
            if (_meshFilter == null)
            {
                throw new Exception();
                //return;
            }

            var xDivision = _entity.Head.Size + 1;
            var zDivision = 2 + Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);

            //_vertices = new Vector3[(xDivision + xTailDivision) / 2 * zDivision];


            //_triangles = new int[(_vertices.Length - 2) * 6];

            leftRatio = Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);
            rightRatio = Mathf.Abs((_entity.Head.LanePosition + _entity.Head.Size - 1) - (_entity.Tail.LanePosition + _entity.Tail.Size - 1));


            //前面
            if (_entity.Head.Size <= _entity.Tail.Size)
            {
                thisNoteSize = _entity.Tail.Size;

                if (leftRatio > rightRatio) // 左辺のほうが変化量が多い
                {
                    _vertices = new Vector3[(_entity.Tail.Size + 1) * (leftRatio + 2)];
                    _triangles = new int[_entity.Tail.Size * (leftRatio + 1) * 6];
                    _uv = new Vector2[(_entity.Tail.Size + 1) * (leftRatio + 2)];

                    for (int z = 0; z < leftRatio + 1; z++)
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
                    _vertices = new Vector3[(_entity.Tail.Size + 1) * (rightRatio + 2)];
                    _triangles = new int[_entity.Tail.Size * (rightRatio + 1) * 6];
                    _uv = new Vector2[(_entity.Tail.Size + 1) * (rightRatio + 2)];

                    for (int z = 0; z < rightRatio + 1; z++)
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
                thisNoteSize = _entity.Head.Size;

                if (leftRatio > rightRatio)
                {
                    _vertices = new Vector3[(_entity.Head.Size + 1) * (leftRatio + 2)];
                    _triangles = new int[_entity.Head.Size * (leftRatio + 1) * 6];
                    _uv = new Vector2[(_entity.Head.Size + 1) * (leftRatio + 2)];

                    for (int z = 0; z < leftRatio + 1; z++)
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
                    _vertices = new Vector3[(_entity.Head.Size + 1) * (rightRatio + 2)];
                    _triangles = new int[_entity.Head.Size * (rightRatio + 1) * 6];
                    _uv = new Vector2[(_entity.Head.Size + 1) * (rightRatio + 2)];

                    for (int z = 0; z < rightRatio + 1; z++)
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
            for (int i = 0; i < newTriangles.Length; i++)
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

        public void Render(float currentTime, int noteNum, List<ReilasNoteLineEntity> noteList)
        {
            RenderMesh(currentTime, noteNum, noteList);
        }

        private void RenderMesh(float currentTime, int noteNum, List<ReilasNoteLineEntity> noteList)
        {
            if (_meshFilter == null || _mesh == null || _vertices == null)
            {
                return;
            }

            if (_entity.Tail.JudgeTime < currentTime) // NoteDestroy
            {
                foreach (Transform child in this.transform.GetChild(0))
                {
                    Destroy(child.gameObject);
                }
                Destroy(this.transform.GetChild(0).gameObject);
                Destroy(gameObject);
                noteList.RemoveAt(noteNum);
                RhythmGamePresenter._aboveSlideNotes.RemoveAt(noteNum);
                RhythmGamePresenter._aboveSlideEffectors.RemoveAt(noteNum);
            }

            if (!gameObject.activeSelf) // SetActive
            {
                gameObject.SetActive(true);
            }

            var zDiv = 2 + Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);

            var headZ = NotePositionCalculatorService.GetPosition(_entity.Head, currentTime, false, _noteSpeed).z;
            var tailZ = NotePositionCalculatorService.GetPosition(_entity.Tail, currentTime, false, _noteSpeed).z;


            int thisNoteZRatio;

            if (leftRatio > rightRatio)
            {
                thisNoteZRatio = leftRatio + 2;
            }
            else
            {
                thisNoteZRatio = rightRatio + 2;
            }

            const float div = 32f;
            const float outerLaneRadius = 4.4f;

            for (var z = 0; z < thisNoteZRatio; z++)
            {
                var p2 = 1f / (thisNoteZRatio - 1) * z;

                var currentZ = Mathf.Lerp(headZ, tailZ, p2);

                float nowLaneSize = Mathf.Lerp(_entity.Head.Size, _entity.Tail.Size, p2);

                for (int x = 0; x <= thisNoteSize; x++)
                {
                    var laneIndex = Mathf.Lerp(_entity.Head.LanePosition, _entity.Tail.LanePosition, p2) + nowLaneSize / thisNoteSize * x; //今作る頂点のレーン番号(小数点以下含む)

                    var angle = Mathf.PI / div * laneIndex;

                    angle = Mathf.PI / 2f - angle;

                    const float outerRadius = outerLaneRadius;

                    var outerX = Mathf.Sin(angle) * outerRadius;
                    var outerY = Mathf.Cos(angle) * outerRadius;

                    var outerPoint = new Vector3(-outerX, outerY, currentZ);

                    _vertices[((int)thisNoteSize + 1) * z + x] = outerPoint;
                    _uv[z * ((int)thisNoteSize + 1) + x] = new Vector2(1f / thisNoteSize * x, 1f / (thisNoteZRatio - 1) * z);
                }
            }

            _mesh.vertices = _vertices;

            //GetComponent<MeshRenderer>().material.cal

            _mesh.uv = _uv;
#if UNITY_EDITOR
            _mesh.RecalculateBounds();
#endif
            _meshFilter.mesh = _mesh;
        }
    }
}
