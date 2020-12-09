using UnityEngine;

namespace ColorLine.Data {

    [System.Serializable]
    public class ObstacleData {
        public int ID;
        public Vector3 Position;

        public ObstacleData(int iD, Vector3 position) {
            ID = iD;
            Position = position;
        }
    }
}