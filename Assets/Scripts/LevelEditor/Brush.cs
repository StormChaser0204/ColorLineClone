using System.Collections.Generic;
using UnityEngine;

namespace ColorLine.Editor {
    public class Brush : MonoBehaviour {
        [SerializeField]
        private MeshGenerator _meshGenerator;
        [SerializeField]
        private SpriteRenderer _brushSprite;
        [SerializeField]
        private Transform _brushTransform;

        private Camera _camera;
        private float _brushSize;

        private BrushType _currentBrushType;
        private bool _canDraw;
        private Transform _currentObstacle;

        private List<Vector3> _waypoints;

        public bool CanDraw { set => _canDraw = value; }

        private void Start() {
            _camera = Camera.main;
            CanDraw = true;
            _waypoints = new List<Vector3>();
        }

        public void ChangeBrushSize(float targetBrushSize) {
            _brushTransform.localScale = new Vector3(targetBrushSize / 2, targetBrushSize / 2, 1);
            _brushSize = targetBrushSize;
        }

        public void SetBrush(BrushType brushType) {
            _currentBrushType = brushType;
            switch (_currentBrushType) {
                case BrushType.Location:
                case BrushType.Revert:
                case BrushType.Road:
                    _brushSprite.enabled = true;
                    break;
                case BrushType.Obstacle:
                    _brushSprite.enabled = true;
                    _waypoints = _meshGenerator.GetWaypoints();
                    break;
            }
        }

        public void Update() {
            if (!_canDraw) return;
            var worldMousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(worldMousePos.x, 1f, worldMousePos.z);

            switch (_currentBrushType) {
                case BrushType.Location:
                case BrushType.Revert:
                case BrushType.Road:
                    if (Input.GetMouseButton(0)) {
                        if (Physics.Raycast(worldMousePos, Vector3.down, out RaycastHit hit)) {
                            Transform objectHit = hit.transform;
                            if (objectHit == null) return;
                            _meshGenerator.Tap(transform.position, _brushSize);
                        }
                    }
                    break;
                case BrushType.Obstacle:
                    var closestWaypoint = GetClosestToRoadWaypoint(worldMousePos);
                    var distance = (closestWaypoint - worldMousePos).magnitude;
                    if (distance > 50) return;
                    transform.position = closestWaypoint;
                    if (Input.GetMouseButtonDown(0)) {
                        _meshGenerator.Tap(transform.position, _brushSize);
                        return;
                    }
                    break;
            }
        }

        private Vector3 GetClosestToRoadWaypoint(Vector3 currentPosition) {
            Vector3 closestWaypoint = currentPosition;
            float minDistance = int.MaxValue;
            foreach (var waypoint in _waypoints) {
                var distance = (waypoint - currentPosition).magnitude;
                if (distance < minDistance) {
                    minDistance = distance;
                    closestWaypoint = waypoint;
                }
            }
            return closestWaypoint;
        }
    }

    public enum BrushType {
        Location = 0,
        Revert = 1,
        Road = 2,
        Obstacle = 3
    }
}