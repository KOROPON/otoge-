#nullable enable
using System;
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

        public void Initialize(ReilasNoteLineEntity entity)
        {
            _entity = entity;
            InitializeMesh();

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
            var zDivision = 2 + (Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition));

            _vertices = new Vector3[xDivision * zDivision];
            _uv = new Vector2[xDivision * zDivision];

            _triangles = new int[(xDivision - 1) * 6 * (zDivision - 1)];

            //前面
            for (var z = zDivision - 2; z >= 0 ; z--)
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


            var newTriangles = new int[(xDivision - 1) * 6 * (zDivision - 1)];
            for (int i = 0; i < newTriangles.Length; i++)
            {
                newTriangles[i] = _triangles[newTriangles.Length - 1 -i];
            }
            // メッシュを生成する.
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = newTriangles
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
            if (_mesh == null)
            {
                return;
            }

            if (_vertices == null)
            {
                return;
            }

            var zDiv = 2 + Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);

            var headZ = NotePositionCalculatorService.GetPosition(_entity.Head, currentTime, false).z;
            var tailZ = NotePositionCalculatorService.GetPosition(_entity.Tail, currentTime, false).z;

            for (var z = 0; z < zDiv; z++)
            {
                var p2 = 1f / (zDiv - 1) * z;

                var currentZ = Mathf.Lerp(headZ, tailZ, p2);

                for (var x = 0; x < _entity.Head.Size + 1; x++)
                {
                    var laneIndex = Mathf.Lerp(_entity.Head.LanePosition, _entity.Tail.LanePosition, p2) + x;

                    const float outerLaneRadius = 4.4f;

                    //float sizeZ = 1f; // SROptions.Current.NoteThickness * 0.1f;


                    const float div = 36f;

                    var angle = Mathf.PI / div * laneIndex;

                    angle = Mathf.PI / 2f - angle;

                    const float outerRadius = outerLaneRadius;

                    var outerX = Mathf.Sin(angle) * outerRadius;
                    var outerY = Mathf.Cos(angle) * outerRadius;

                    var outerPoint = new Vector3(outerX, outerY, currentZ);

                    _vertices[(_entity.Head.Size + 1) * z + x] = outerPoint;
                    _uv[z * (_entity.Head.Size + 1) + x] = new Vector2(1f / _entity.Head.Size * x, 1f / (zDiv - 1) * z);
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
