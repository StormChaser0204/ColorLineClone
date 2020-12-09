using UnityEngine;

namespace ColorLine.GameEngine.Components {
    public class CollisionComponent : MonoBehaviour {

        private ObstacleComponent _obstacleComponent;

        public void InitComponent(ObstacleComponent obstacle) {
            _obstacleComponent = obstacle;
        }

        private void OnTriggerEnter(Collider col) {
            _obstacleComponent.Collision();
        }
    }
}