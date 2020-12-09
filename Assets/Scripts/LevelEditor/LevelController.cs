using ColorLine.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ColorLine.Editor {
    public class LevelController : MonoBehaviour {

        [SerializeField]
        private MeshGenerator _meshGenerator;

        public void Save(string levelId) {
            var levelData = _meshGenerator.GetLevelData();

            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();

            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            bf.SurrogateSelector = surrogateSelector;

            FileStream file = File.Create("Assets/Resources/Levels/" + string.Format("Level_{0}.lvl", levelId));
            bf.Serialize(file, levelData);
            file.Close();
            Debug.Log("Level saved");

        }

        public LevelData Load(string levelId) {

            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();

            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            bf.SurrogateSelector = surrogateSelector;

            FileStream file = File.Open("Assets/Resources/Levels/" + string.Format("Level_{0}.lvl", levelId), FileMode.Open);
            var levelData = (LevelData)bf.Deserialize(file);
            file.Close();
            Debug.Log(string.Format("Level {0} loaded", levelId));

            return levelData;
        }
    }
}
