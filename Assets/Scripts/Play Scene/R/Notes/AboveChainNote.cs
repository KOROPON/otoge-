#nullable enable
using System;
using UnityEngine;

namespace Reilas
{
    public sealed class AboveChainNote : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector3[]? _uv;
        private int[]? _triangles;

        private Mesh? _mesh;

        private ReilasNoteEntity _entity = null!;

        public void Initialize(ReilasNoteEntity entity)
        {
            _entity = entity;
            InitializeMesh();

            transform.localScale = Vector3.one;
            ;
        }

        private void InitializeMesh()
        {
            if (_meshFilter == null)
            {
                throw new Exception();
                //return;
            }

            var size = _entity.Size + 1;

            _vertices = new Vector3[size * 2 * 2];
            _uv = new Vector3[size * 2 * 2];
            _triangles = new int[size * 6 * 2 + 12];

            // �O��
            for (var i = 0; i < size - 1; i++)
            {
                _triangles[i * 6 + 0] = 0 + i * 2;
                _triangles[i * 6 + 1] = 2 + i * 2;
                _triangles[i * 6 + 2] = 1 + i * 2;
                _triangles[i * 6 + 3] = 2 + i * 2;
                _triangles[i * 6 + 4] = 3 + i * 2;
                _triangles[i * 6 + 5] = 1 + i * 2;
            }

            // ���
            for (var i = 0; i < size - 1; i++)
            {
                var p = size + i;

                _triangles[p * 6 + 0] = 0 + p * 2;
                _triangles[p * 6 + 1] = 0 + i * 2;
                _triangles[p * 6 + 2] = 2 + p * 2;
                _triangles[p * 6 + 3] = 2 + p * 2;
                _triangles[p * 6 + 4] = 0 + i * 2;
                _triangles[p * 6 + 5] = 2 + i * 2;
            }

            // ��
            _triangles[size * 6 * 2 + 0] = 0;
            _triangles[size * 6 * 2 + 1] = 1;
            _triangles[size * 6 * 2 + 2] = size * 2;
            _triangles[size * 6 * 2 + 3] = 1;
            _triangles[size * 6 * 2 + 4] = size * 2;
            _triangles[size * 6 * 2 + 5] = size * 2 + 1;

            // �E
            _triangles[size * 6 * 2 + 6] = 0 + size * 2 - 2;
            _triangles[size * 6 * 2 + 7] = 1 + size * 2 - 2;
            _triangles[size * 6 * 2 + 8] = size * 2 + size * 2 - 2;
            _triangles[size * 6 * 2 + 9] = 1 + size * 2 - 2;
            _triangles[size * 6 * 2 + 10] = size * 2 + size * 2 - 2;
            _triangles[size * 6 * 2 + 11] = size * 2 + 1 + size * 2 - 2;

            // ���b�V���𐶐�����
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
        }

        private void RenderMesh(float currentTime)
        {
            if (_meshFilter == null) return;

            for (var z = 0; z < 2; z++)
            {
                for (var x = 0; x < _entity.Size + 1; x++)
                {
                    var laneIndex = _entity.LanePosition + x;

                    const float outerLaneRadius = 4.5f;

                    //const float sizeY = 0.075f;
                    float sizeZ = 1f; // SROptions.Current.NoteThickness * 0.1f;

                    float zz = z == 0 ? sizeZ : -sizeZ;

                    const float div = 36f;

                    var angle = Mathf.PI / div * laneIndex;

                    angle = Mathf.PI / 2f - angle;

                    const float innerRadius = outerLaneRadius - 0.3f;
                    const float outerRadius = outerLaneRadius;

                    var innerX = Mathf.Sin(angle) * innerRadius;
                    var innerY = Mathf.Cos(angle) * innerRadius;

                    var outerX = Mathf.Sin(angle) * outerRadius;
                    var outerY = Mathf.Cos(angle) * outerRadius;

                    var zPos = NotePositionCalculatorService.GetPosition(_entity, currentTime, true).z;

                    zPos += zz;

                    var innerPoint = new Vector3(innerX, innerY, zPos);
                    var outerPoint = new Vector3(outerX, outerY, zPos);


                    //(innerPoint, outerPoint) = (outerPoint, innerPoint);

                    var p = (_entity.Size + 1) * 2 * z;

                    if (_vertices != null)
                    {
                        _vertices[p + x * 2 + 0] = innerPoint;
                        _vertices[p + x * 2 + 1] = outerPoint;
                    }

                    float uvX = 1f / _entity.Size * x;

                    float alpha = 1f;
                    // ��O
                    if (z == 0)
                    {
                        if (_uv != null)
                        {
                            _uv[x * 2 + 0] = new Vector3(uvX, 0.5f, alpha);
                            _uv[x * 2 + 1] = new Vector3(uvX, 0f, alpha);
                        }
                    }
                    // ��
                    else
                    {
                        var w = z * (_entity.Size + 1) * 2 + (x * 2);

                        if (_uv != null)
                        {
                            _uv[w + 0] = new Vector3(uvX, 1f, alpha);
                            _uv[w + 1] = new Vector3(0, 0, alpha);
                        }
                    }
                }
            }

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

        public void NoteDeestroy()
        {
            Destroy(this.gameObject);
        }
    }
}