using System.Collections.Generic;
using UnityEngine;

namespace ColorLine.Data {

    [System.Serializable]
    public class LevelData {
        public Vector3 StartPoint;
        public Vector3 FinishPoint;
        public List<Vector3> Waypoints;
        public List<Vector3> ModifiedVertices;
        public List<ObstacleData> ObstacleData;

        public LevelData(Vector3 startPoint, Vector3 finishPoint, List<Vector3> waypoints, List<Vector3> modifiedVertices, List<ObstacleData> obstacleData) {
            StartPoint = startPoint;
            FinishPoint = finishPoint;
            Waypoints = waypoints;
            ModifiedVertices = modifiedVertices;
            ObstacleData = obstacleData;
        }
    }
}