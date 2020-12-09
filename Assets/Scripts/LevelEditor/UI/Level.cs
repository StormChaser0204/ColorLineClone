using UnityEngine;
using TMPro;

namespace ColorLine.Editor {
    public class Level : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI _levelIdText;

        private string _levelId;
        private SelectLevelPopup _selectLevelPopup;

        public void FillLevelData(string levelId, SelectLevelPopup selectLevelPopup) {
            _levelIdText.text = levelId;
            _levelId = levelId;
            _selectLevelPopup = selectLevelPopup;
        }

        public void LoadLevel() {
            _selectLevelPopup.LoadLevel(_levelId);
        }
    }
}