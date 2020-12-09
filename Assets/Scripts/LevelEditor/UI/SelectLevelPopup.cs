using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace ColorLine.Editor {

    public class SelectLevelPopup : MonoBehaviour {
        [SerializeField]
        private UIController _uiController;
        [SerializeField]
        private Level _levelPrefab;
        [SerializeField]
        private Transform _levelParent;
        [SerializeField]
        private LevelController _levelController;

        private List<Level> _loadedLevels;

        public void FillLevels() {
            _loadedLevels = new List<Level>();
            var path = "Assets/Resources/Levels/";
            var info = new DirectoryInfo(path);
            var fileInfo = info.GetFiles();

            foreach (var level in fileInfo) {
                if (level.Name.Contains(".meta")) continue;
                var levelId = Regex.Replace(level.Name, "[^0-9]", "");
                var levelButton = Instantiate(_levelPrefab, _levelParent);
                levelButton.FillLevelData(levelId, this);
                _loadedLevels.Add(levelButton);
            }
        }

        public void LoadLevel(string levelId) {
            _uiController.LoadLevel(levelId);
            foreach (var level in _loadedLevels) {
                Destroy(level.gameObject);
            }
            _loadedLevels.Clear();
        }
    }
}