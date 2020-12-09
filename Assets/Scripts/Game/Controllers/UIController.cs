using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ColorLine.GameEngine.Controllers;
using ColorLine.GameEngine.Singleton;

namespace ColorLine.GameEngine {
    public class UIController : SceneSingleton<UIController> {

        [SerializeField]
        private LevelController _levelController;
        [SerializeField]
        private PlayerController _playerController;
        [SerializeField]
        private Image _progressBar;
        [SerializeField]
        private TextMeshProUGUI _levelNumber;

        [SerializeField]
        private GameObject _nextLevelPopup;
        [SerializeField]
        private GameObject _restartLevelPopup;

        protected override void Init() {
        }

        public void SetRestartPopup(bool value) {
            _restartLevelPopup.SetActive(value);
        }

        public void SetNextLevelPopup(bool value) {
            _nextLevelPopup.SetActive(value);
        }

        public void Restart() {
            _levelController.Restart();
            SetRestartPopup(false);
        }

        public void NextLevel() {
            var levelID = _playerController.CurrentLevel;
            _levelController.ClearLevel();
            _levelController.LoadLevel(levelID.ToString());
            SetNextLevelPopup(false);
        }
    }
}