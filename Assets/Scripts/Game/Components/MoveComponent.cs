using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorLine.GameEngine.Components {
    public class MoveComponent : MonoBehaviour {

        [SerializeField]
        private float _maxSpeed;
        [SerializeField]
        private float _speedDelta;
        [SerializeField]
        private TrailRenderer _trail;

        private List<Vector3> _waypoints;
        private Vector3 _targetWaypoint;

        private int _currentWaypointID;
        private float _currentSpeed;

        public bool CanMove { get; set; }

        public void InitComponent(List<Vector3> waypoints) {
            _waypoints = waypoints;
            _targetWaypoint = _waypoints[1];
            _currentWaypointID = 0;
            _currentSpeed = 0;
            _trail.Clear();
            CanMove = true;
        }

        public void Update() {
#if UNITY_EDITOR
            if (!CanMove) return;
            if (Input.GetMouseButton(0)) {
                _currentSpeed += _speedDelta * Time.deltaTime;
                MoveObject();
            }
            else {
                if (_currentSpeed <= 0) return;
                _currentSpeed -= _speedDelta * Time.deltaTime * 2;
                MoveObject();
            }
#else
            if (Input.touchCount > 0) {
                currentSpeed += _speedDelta * Time.deltaTime;
                MoveObject();
            }
            else {
                if (_currentSpeed <= 0) return;
                _currentSpeed -= _speedDelta * Time.deltaTime * 2;
                MoveObject();
            }
#endif
        }

        private void MoveObject() {
            if (_targetWaypoint == Vector3.zero) return;
            var targetDirection = _targetWaypoint - transform.position;

            if (targetDirection.magnitude < 1f) {
                _targetWaypoint = GetNextWaypoint();
                return;
            }
            else {
                var rotation = Quaternion.LookRotation(_targetWaypoint - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);

                transform.position += targetDirection.normalized * _currentSpeed * Time.deltaTime;
            }
        }

        public Vector3 GetNextWaypoint() {
            _currentWaypointID++;
            if (_currentWaypointID >= _waypoints.Count) {
                return Vector3.zero;
            }
            return _waypoints[_currentWaypointID];
        }
    }
}
