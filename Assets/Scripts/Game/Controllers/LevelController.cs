using ColorLine.Data;
using ColorLine.GameEngine.Components;
using ColorLine.GameEngine.Singleton;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ColorLine.GameEngine.Controllers {
    public class LevelController : SceneSingleton<LevelController> {

        [SerializeField]
        private MeshFilter _levelMesh;
        [SerializeField]
        private Transform _startLevelObject;
        [SerializeField]
        private Transform _finishLevelObject;
        [SerializeField]
        private MoveComponent _playerMovement;
        [SerializeField]
        private List<ObstacleComponent> _obstacles;

        private BinaryFormatter _binaryFormatter;
        private List<GameObject> _placedObstacles;
        private LevelData _currentLevelData;

        protected override void Init() {
            _binaryFormatter = new BinaryFormatter();
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            _placedObstacles = new List<GameObject>();

            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            _binaryFormatter.SurrogateSelector = surrogateSelector;
        }

        public void Restart() {
            _playerMovement.transform.position = _currentLevelData.StartPoint;
            _playerMovement.InitComponent(_currentLevelData.Waypoints);
        }

        public void ClearLevel() {
            _placedObstacles.ForEach(o => DestroyObstacle(o));
        }

        public void LoadLevel(string levelId) {
            FileStream file = File.Open("Assets/Resources/Levels/" + string.Format("Level_{0}.lvl", levelId), FileMode.Open);
            var levelData = (LevelData)_binaryFormatter.Deserialize(file);
            file.Close();
            _currentLevelData = levelData;
            _startLevelObject.position = levelData.StartPoint;
            _finishLevelObject.position = levelData.FinishPoint;
            _playerMovement.transform.position = levelData.StartPoint;
            _playerMovement.InitComponent(levelData.Waypoints);
            _levelMesh.mesh.SetVertices(levelData.ModifiedVertices);
            _levelMesh.mesh.RecalculateNormals();
            levelData.ObstacleData.ForEach(o => SetObstacle(o));
        }

        private void SetObstacle(ObstacleData obstacleData) {
            var obstaclePrefab = _obstacles[obstacleData.ID];
            var obstacleObject = Instantiate(obstaclePrefab, obstacleData.Position, Quaternion.identity);
            obstacleObject.InitComponent();
            _placedObstacles.Add(obstacleObject.gameObject);
        }

        private void DestroyObstacle(GameObject obstacle) {
            Destroy(obstacle);
        }
    }
}