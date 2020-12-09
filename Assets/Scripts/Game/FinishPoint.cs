using ColorLine.GameEngine.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorLine.GameEngine {
    public class FinishPoint : MonoBehaviour {

        public void OnTriggerEnter(Collider other) {
            PlayerController.Instance.Win();
        }
    }
}