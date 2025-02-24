using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    [SerializeField]
    private GameObject _blurLens;
    [SerializeField]
    private float _lensScaleFactor, _zoomSpeed;
    private CameraFollow _cameraFollow;

    private void Start() {
        _blurLens.transform.localScale = Vector3.zero;
        _cameraFollow = this.GetComponent<CameraFollow>();
    }

    private void Update() {
        if (_blurLens.transform.localScale == Vector3.zero) {
            _blurLens.SetActive(false);
        }
        Vector2 _curScreenPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 _screenToWorldPoint = new Vector2(Camera.main.ScreenToWorldPoint(_curScreenPoint).x, Camera.main.ScreenToWorldPoint(_curScreenPoint).y);
        if (Input.GetMouseButton(0)) {
            _blurLens.SetActive(true);
            _blurLens.transform.position = Vector2.Lerp(_blurLens.transform.position, _screenToWorldPoint, Time.deltaTime * 10);
            _blurLens.transform.localScale = Vector3.Slerp(_blurLens.transform.localScale, _lensScaleFactor * Vector3.one, Time.deltaTime * _zoomSpeed);
        }
        else {
            _blurLens.transform.localScale = Vector3.Slerp(_blurLens.transform.localScale, Vector3.zero, Time.deltaTime * _zoomSpeed);
        }
    }
}