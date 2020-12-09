using ColorLine.GameEngine.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SceneSingleton<CameraController> {
    [SerializeField]
    private Transform _camera;
    [SerializeField]
    private Transform _player;

    private Vector3 _offset;

    protected override void Init() {
        _offset = new Vector3(0, 15, -10);
    }


    public void Update() {
        _camera.position = _player.position + _offset;
    }
}
