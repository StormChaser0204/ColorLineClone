using ColorLine.GameEngine.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorLine.GameEngine.Components {
    public class ObstacleComponent : MonoBehaviour {

        [SerializeField]
        private List<GameObject> _obstacleParts;

        public void InitComponent() {
            foreach (var part in _obstacleParts) {
                part.AddComponent<CollisionComponent>().InitComponent(this);
            }
        }

        public void Collision() {
            PlayerController.Instance.Lose();
        }
    }
}