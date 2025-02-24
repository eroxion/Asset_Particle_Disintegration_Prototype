using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject _sprite;
    [SerializeField]
    private float _moveDamping, _moveSpeed;


    private void Update() {
        Vector2 _curScreenPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 _screenToWorldPoint = new Vector2(Camera.main.ScreenToWorldPoint(_curScreenPoint).x, Camera.main.ScreenToWorldPoint(_curScreenPoint).y);
        Vector2 _offset = _screenToWorldPoint / _moveDamping;
        _sprite.transform.position = Vector2.Lerp(_sprite.transform.position, -_screenToWorldPoint + _offset, Time.deltaTime * _moveSpeed);
    }

    internal Vector3 GetSpritePosition() {
        return _sprite.transform.position;
    }
}
