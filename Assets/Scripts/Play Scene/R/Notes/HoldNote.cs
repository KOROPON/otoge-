#nullable enable

using System;
using System.Collections.Generic;
using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class HoldNote : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector2[] _uv = null!;
        private int[]? _triangles;

        private Mesh? _mesh;

        private Material mate;

        private ReilasNoteLineEntity _entity = null!;
        private RhythmGamePresenter _presenter = null!;
        private float _noteSpeed;

        private bool _kujo;

        public float time;
        private float _noteLane;
        private float _noteLeftPos;

        public void Initialize(ReilasNoteLineEntity entity, bool kujo)
        {
            _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
            _entity = entity;
            _kujo = kujo;
            time = _entity.Head.JudgeTime;
            _noteSpeed = _entity.Head.Speed;
            _noteLane = _entity.Head.LanePosition;
            InitializeMesh();
        }

        private void InitializeMesh()
        {
            if (meshFilter == null)
            {
                throw new Exception();
                //return;
            }

            _noteLeftPos = -4f + 2f * _noteLane;

            _vertices = new Vector3[4];
            _triangles = new int[6] { 1, 0, 3, 0, 2, 3 };
            _uv = new Vector2[4] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

            // ���b�V���𐶐�����.
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = _triangles
            };
            _mesh.MarkDynamic();
        }

        public void Render(float currentTime, int noteNum, List<ReilasNoteLineEntity> noteList, List<SpeedChangeEntity> speedChangeEntities)
        {
            if (_entity.Tail.JudgeTime < currentTime)
            {
                foreach (Transform child in transform)
                {
                    foreach (Transform inChild in child) Destroy(inChild.gameObject);
                }
                foreach (Transform child in transform) Destroy(child.gameObject);
                Destroy(gameObject);
                RhythmGamePresenter.HoldEffectors.Remove(transform.GetChild(0).GetComponent<HoldEffector>());
                if (_kujo)
                {
                    RhythmGamePresenter.HoldKujoNotes.RemoveAt(noteNum);
                    _presenter.reilasKujoHold.RemoveAt(noteNum);
                }
                else
                {
                    RhythmGamePresenter.HoldNotes.RemoveAt(noteNum);
                    noteList.RemoveAt(noteNum);
                }
            }
            if (!gameObject.activeSelf) gameObject.SetActive(true);

            var scale = NotePositionCalculatorService.GetScale(_entity.Head);
            
            float headPos = NotePositionCalculatorService.GetPosition(_entity.Head, currentTime, _noteSpeed, speedChangeEntities);
            float tailPos = NotePositionCalculatorService.GetPosition(_entity.Tail, currentTime, _noteSpeed, speedChangeEntities);

            _vertices[0] = new Vector3(_noteLeftPos, 0, headPos);
            _vertices[1] = new Vector3(_noteLeftPos + 2f, 0, headPos);
            _vertices[2] = new Vector3(_noteLeftPos, 0, tailPos);
            _vertices[3] = new Vector3(_noteLeftPos + 2f, 0, tailPos);


            _mesh.vertices = _vertices;

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
            Destroy(transform.GetChild(0).GetComponent<HoldEffector>());
            RhythmGamePresenter.HoldEffectors.Remove(transform.GetChild(0).GetComponent<HoldEffector>());
            Destroy(transform.GetChild(0).gameObject);
            if (kujo)
            {
                _presenter.reilasKujoHold.RemoveAt(RhythmGamePresenter.HoldKujoNotes.IndexOf(this));
                RhythmGamePresenter.HoldKujoNotes.Remove(this);
            }
            else
            {
                _presenter._reilasHold.RemoveAt(RhythmGamePresenter.HoldNotes.IndexOf(this));
                RhythmGamePresenter.HoldNotes.Remove(this);
            }
            Destroy(gameObject);
        }
    }
}
