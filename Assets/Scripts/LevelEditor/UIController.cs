using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ColorLine.Editor {
    public class UIController : MonoBehaviour {
        [SerializeField]
        private Brush _brushSettings;
        [SerializeField]
        private MeshGenerator _meshGenerator;
        [SerializeField]
        private LevelController _levelController;
        [SerializeField]
        private TMP_InputField _levelID;
        [SerializeField]
        private SelectLevelPopup _selectLevelPopup;

        [SerializeField]
        private Slider _brushSize;

        private void Start() {
            _brushSize.onValueChanged.AddListener(delegate { ChangeBrushSize(); });
            ChangeBrushSize();
        }

        public void NewLevel() {
            _meshGenerator.Clear();
        }

        public void SaveLevel() {
            _levelController.Save(_levelID.text);
        }

        public void OpenSelectLevelPopup() {
            _selectLevelPopup.gameObject.SetActive(true);
            _selectLevelPopup.FillLevels();
            _brushSettings.CanDraw = false;
        }

        public void LoadLevel(string levelId) {
            var levelData = _levelController.Load(levelId);
            _levelID.text = levelId;
            _meshGenerator.SetLevelData(levelData);
            _selectLevelPopup.gameObject.SetActive(false);
            _brushSettings.CanDraw = true;
        }

        public void ChangeBrushSize() {
            var targetValue = _brushSize.value;
            _brushSettings.ChangeBrushSize(targetValue);
        }

        public void SetLocationBrush() {
            _meshGenerator.SetBrush(BrushType.Location);
            _brushSettings.SetBrush(BrushType.Location);
            _brushSize.gameObject.SetActive(true);
        }

        public void SetRevertBrush() {
            _meshGenerator.SetBrush(BrushType.Revert);
            _brushSettings.SetBrush(BrushType.Revert);
            _brushSize.gameObject.SetActive(true);
        }

        public void SetRoadBrush() {
            _brushSize.value = 0.5f;
            ChangeBrushSize();
            _meshGenerator.SetBrush(BrushType.Road);
            _brushSettings.SetBrush(BrushType.Road);
            _brushSize.gameObject.SetActive(false);
        }

        public void SetObstacleBrush(int obstacleId) {
            _meshGenerator.SetBrush(BrushType.Obstacle);
            _brushSettings.SetBrush(BrushType.Obstacle);
            _meshGenerator.SetObstacleID(obstacleId);
            _brushSize.gameObject.SetActive(false);
        }
    }
}