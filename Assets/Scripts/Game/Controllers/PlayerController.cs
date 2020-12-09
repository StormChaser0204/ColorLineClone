using ColorLine.GameEngine.Components;
using ColorLine.GameEngine.Singleton;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ColorLine.GameEngine.Controllers {
    public class PlayerController : SceneSingleton<PlayerController> {
        [SerializeField]
        private MoveComponent _moveComponent;

        private int _currentLevel;
        private PlayerData _playerData;

        public int CurrentLevel {
            get {
                return _currentLevel;
            }
            set {
                _currentLevel = value;
            }
        }

        protected override void Init() {
            LoadPlayerData();
        }

        private void LoadPlayerData() {
            var savePath = Application.persistentDataPath + "/" + "player.sav";
            if (!File.Exists(savePath)) {
                _playerData = new PlayerData();
                File.Create(savePath);
            }
            else {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(savePath, FileMode.Open);
                _playerData = (PlayerData)bf.Deserialize(file);
            }
            _currentLevel = _playerData.CurrentLevel;
            LevelController.Instance.LoadLevel(_currentLevel.ToString());
        }

        private void SavePlayerData() {
            _playerData.CurrentLevel = _currentLevel;
            var savePath = Application.persistentDataPath + "/" + "player.sav";
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(savePath);
            bf.Serialize(file, _playerData);
        }

        [ContextMenu("ClearSave")]
        private void ClearSave() {
            var savePath = Application.persistentDataPath + "/" + "player.sav";
            File.Delete(savePath);
        }

        private void OnApplicationQuit() {
            SavePlayerData();
        }

        public void Win() {
            _currentLevel++;
            SavePlayerData();
            UIController.Instance.SetNextLevelPopup(true);
        }

        public void Lose() {
            UIController.Instance.SetRestartPopup(true);
            _moveComponent.CanMove = false;
        }
    }

    [System.Serializable]
    public class PlayerData {
        public int CurrentLevel;

        public PlayerData() {
            CurrentLevel = 1;
        }
    }
}
