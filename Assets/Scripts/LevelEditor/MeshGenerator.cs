using ColorLine.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorLine.Editor {
    [RequireComponent(typeof(MeshFilter))]
    public class MeshGenerator : MonoBehaviour {
        [SerializeField]
        private List<GameObject> _obstacles;

        private Mesh _mesh;
        private MeshCollider _meshCollider;

        private Vector3[] _vertices;
        private int[] _triangles;

        private static int _height = 100;
        private static int _width = 100;

        private List<Vector3> _originalVertices;
        private List<Vector3> _modifiedVertices;

        private List<Vector3> _waypoints;
        private Vector3 _startWaypoint;
        private Vector3 _finishWaypoint;
        private Vector3 _lastWaypoint;

        private List<ObstacleData> _obstaclesData;
        private List<GameObject> _placedObstacles;

        private BrushType _currentBrushType;
        private float _maximumDepression;
        private int _currentObstacleID;

        private void Start() {
            _originalVertices = new List<Vector3>();
            _modifiedVertices = new List<Vector3>();
            _obstaclesData = new List<ObstacleData>();
            _placedObstacles = new List<GameObject>();
            _waypoints = new List<Vector3>();
            _meshCollider = GetComponent<MeshCollider>();
            _maximumDepression = -1;
            GenerateBaseMesh();
        }

        public void SetBrush(BrushType brushType) {
            _currentBrushType = brushType;
            switch (brushType) {
                case BrushType.Location:
                    _maximumDepression = -1;
                    break;
                case BrushType.Revert:
                    _maximumDepression = 1;
                    break;
                case BrushType.Road:
                    _maximumDepression = 0.01f;
                    break;
            }
        }

        public void GenerateBaseMesh() {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            CreateShape();
            UpdateShape();
            MeshRegenerated();
        }

        private void CreateShape() {

            _vertices = new Vector3[(_width + 1) * (_height + 1)];

            for (int i = 0, z = 0; z <= _height; z++) {
                for (int x = 0; x <= _width; x++) {
                    _vertices[i] = new Vector3(x, 0, z);
                    i++;
                }
            }

            int vert = 0;
            int tris = 0;
            _triangles = new int[_width * _height * 6];


            for (int z = 0; z < _height; z++) {
                for (int x = 0; x < _width; x++) {
                    _triangles[tris + 0] = vert + 0;
                    _triangles[tris + 1] = vert + _width + 1;
                    _triangles[tris + 2] = vert + 1;
                    _triangles[tris + 3] = vert + 1;
                    _triangles[tris + 4] = vert + _width + 1;
                    _triangles[tris + 5] = vert + _width + 2;

                    vert++;
                    tris += 6;
                }
                vert++;
            }
        }

        private void UpdateShape() {
            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _meshCollider.sharedMesh = _mesh;
            _mesh.RecalculateNormals();
        }

        public void MeshRegenerated() {
            _mesh.MarkDynamic();
            _originalVertices = _mesh.vertices.ToList();
            _modifiedVertices = _mesh.vertices.ToList();
        }

        public void AddDepression(Vector3 depressionPoint, float radius) {
            var worldPos4 = transform.worldToLocalMatrix * depressionPoint;
            var worldPos = new Vector3(worldPos4.x, worldPos4.y, worldPos4.z);
            for (int i = 0; i < _modifiedVertices.Count; ++i) {
                var distance = (worldPos - (_modifiedVertices[i] + Vector3.down * _maximumDepression)).magnitude;
                if (distance < radius) {
                    Vector3 newVert = new Vector3();
                    switch (_currentBrushType) {
                        case BrushType.Location:
                            newVert = new Vector3(_originalVertices[i].x, 1, _originalVertices[i].z);
                            break;
                        case BrushType.Revert:
                            newVert = new Vector3(_originalVertices[i].x, 0, _originalVertices[i].z);
                            break;
                        case BrushType.Road:
                            newVert = new Vector3(_originalVertices[i].x, 0.5f, _originalVertices[i].z);
                            break;
                    }
                    _modifiedVertices.RemoveAt(i);
                    _modifiedVertices.Insert(i, newVert);
                }
            }

            _mesh.SetVertices(_modifiedVertices);
            _mesh.RecalculateNormals();
        }

        public void SetObstacleID(int obstacleID) {
            _currentObstacleID = obstacleID;
        }

        public void Tap(Vector3 position, float brushSize) {
            switch (_currentBrushType) {
                case BrushType.Location:
                    AddDepression(position, brushSize);
                    break;
                case BrushType.Revert:
                    AddDepression(position, brushSize);
                    break;
                case BrushType.Road:
                    AddDepression(position, brushSize);
                    AddWaypoint(position);
                    break;
                case BrushType.Obstacle:
                    AddObstacle(position);
                    break;
            }
        }

        private void AddWaypoint(Vector3 position) {
            if (_lastWaypoint == Vector3.zero)
                _lastWaypoint = position;

            if (_startWaypoint == Vector3.zero) {
                _startWaypoint = position;
                _waypoints.Add(_startWaypoint);
                return;
            }

            var distance = (_lastWaypoint - position).magnitude;
            if (distance < 5f) return;
            _waypoints.Add(position);
            _lastWaypoint = position;
            _finishWaypoint = _waypoints.Last();
            Debug.Log(_waypoints.Count);
        }

        private void AddObstacle(Vector3 position) {
            var obstacleData = new ObstacleData(_currentObstacleID, position);
            var obstacle = Instantiate(GetObstacle(_currentObstacleID), position, Quaternion.identity);
            _placedObstacles.Add(obstacle);
            _obstaclesData.Add(obstacleData);
        }

        public GameObject GetObstacle(int obstacleID) {
            return _obstacles[obstacleID];
        }

        public void Clear() {
            GenerateBaseMesh();
            _waypoints.Clear();
            _startWaypoint = Vector3.zero;
            _finishWaypoint = Vector3.zero;
            _lastWaypoint = Vector3.zero;
            foreach (var obstacle in _placedObstacles) {
                Destroy(obstacle);
            }
            _obstaclesData.Clear();
            _placedObstacles.Clear();
        }

        private void OnDrawGizmos() {
            if (_waypoints == null) return;
            if (_waypoints.Count == 0) return;
            foreach (var waypoint in _waypoints) {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(waypoint, 0.5f);
            }
        }

        public List<Vector3> GetWaypoints() {
            return _waypoints;
        }

        public LevelData GetLevelData() {
            var levelData = new LevelData(_startWaypoint, _finishWaypoint, _waypoints, _modifiedVertices, _obstaclesData);
            return levelData;
        }

        public void SetLevelData(LevelData levelData) {
            _startWaypoint = levelData.StartPoint;
            _finishWaypoint = levelData.FinishPoint;
            _waypoints = levelData.Waypoints;
            _modifiedVertices = levelData.ModifiedVertices;

            _mesh.SetVertices(_modifiedVertices);
            _mesh.RecalculateNormals();
        }
    }
}