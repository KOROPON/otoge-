#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace Reilas
{
    public sealed class AboveHoldNote : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector2[] _uv = null!;
        
        private int[]? _triangles;

        private Mesh? _mesh;
        private ReilasNoteLineEntity _entity = null!;
        private RhythmGamePresenter _presenter = null!;

        private float _noteSpeed;
        private bool _kujo;

        public void Initialize(ReilasNoteLineEntity entity, bool kujo)
        {
            _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
            _entity = entity;
            _kujo = kujo;
            _noteSpeed = _entity.Head.Speed;
            
            InitializeMesh();
            
            transform.localScale = Vector3.one;
        }

        private void InitializeMesh()
        {
            if (meshFilter == null) return;

            var xDivision = _entity.Head.Size + 1;
            var zDivision = 2 + Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);

            _vertices = new Vector3[xDivision * zDivision];
            _uv = new Vector2[xDivision * zDivision];
            _triangles = new int[(xDivision - 1) * 6 * (zDivision - 1)];

            //前面
            for (var z = 0; z < zDivision - 1; z++)
            {
                var n = z * (xDivision - 1) * 6;
                
                for (var x = 0; x < xDivision - 1; x++)
                {
                    _triangles[n + x * 6 + 0] = z * (xDivision) + x;
                    _triangles[n + x * 6 + 1] = z * (xDivision) + x + 1;
                    _triangles[n + x * 6 + 2] = (z + 1) * (xDivision) + x;
                    _triangles[n + x * 6 + 3] = z * (xDivision) + x + 1;
                    _triangles[n + x * 6 + 4] = (z + 1) * (xDivision) + x + 1;
                    _triangles[n + x * 6 + 5] = (z + 1) * (xDivision) + x;
                }
            }

            // メッシュを生成する
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = _triangles
            };
            _mesh.MarkDynamic();
        }

        public void Render(float currentTime, int noteNum, List<ReilasNoteLineEntity> noteList)
        {
            RenderMesh(currentTime, noteNum, noteList);
        }

        private void RenderMesh(float currentTime, int noteNum, List<ReilasNoteLineEntity> noteList)
        {
            if ( meshFilter == null || _mesh == null || _vertices == null) return;

            if(_entity.Tail.JudgeTime < currentTime)
            {
                foreach (Transform child in transform.GetChild(0)) Destroy(child.gameObject);
                
                Destroy(transform.GetChild(0).gameObject);
                Destroy(gameObject);
                RhythmGamePresenter.AboveHoldEffectors.Remove(transform.GetChild(0).GetComponent<AboveHoldEffector>());
                
                if (_kujo)
                {
                    _presenter.reilasKujoAboveHold.RemoveAt(noteNum);
                    RhythmGamePresenter.AboveKujoHoldNotes.RemoveAt(noteNum);
                }
                else
                {
                    noteList.RemoveAt(noteNum);
                    RhythmGamePresenter.AboveHoldNotes.RemoveAt(noteNum);
                }
            }
            if (!gameObject.activeSelf) gameObject.SetActive(true);

            var zDiv = 2 + Mathf.Abs(_entity.Head.LanePosition - _entity.Tail.LanePosition);
            var headZ = NotePositionCalculatorService.GetPosition(_entity.Head.JudgeTime, currentTime, _noteSpeed);
            var tailZ = NotePositionCalculatorService.GetPosition(_entity.Tail.JudgeTime, currentTime, _noteSpeed);

            for (var z = 0; z < zDiv; z++)
            {
                var p2 = 1f / (zDiv - 1) * z;
                var currentZ = Mathf.Lerp(headZ, tailZ, p2);
                for (var x = 0; x < _entity.Head.Size + 1; x++)
                {
                    var laneIndex = Mathf.Lerp(_entity.Head.LanePosition, _entity.Tail.LanePosition, p2) + x;

                    const float outerLaneRadius = 4.4f;
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
            Destroy(transform.GetChild(0).GetComponent<AboveHoldEffector>());
            RhythmGamePresenter.AboveHoldEffectors.Remove(transform.GetChild(0).GetComponent<AboveHoldEffector>());
            Destroy(transform.GetChild(0).gameObject);
            if (kujo)
            {
                _presenter.reilasKujoAboveHold.RemoveAt(RhythmGamePresenter.AboveKujoHoldNotes.IndexOf(this));
                RhythmGamePresenter.AboveKujoHoldNotes.Remove(this);
            }
            else
            {
                _presenter.reilasAboveHold.RemoveAt(RhythmGamePresenter.AboveHoldNotes.IndexOf(this));
                RhythmGamePresenter.AboveHoldNotes.Remove(this);
            }
            Destroy(gameObject);
        }
    }
}