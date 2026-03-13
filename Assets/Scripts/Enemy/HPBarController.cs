using System.Collections;
using UnityEngine;

public class HPBarController : MonoBehaviour {

    [SerializeField] private GameObject hpBarPrefab;

    private Canvas _canvas;
    private Camera _camera;
    private HpBar _hpBar;
    private RectTransform _hpBarRectTransform;
    private Vector3 _offset;
    private Coroutine _hideHPBarCoroutine;
    private WaitForSeconds _waitSeconds = new(1f);

    private void Start(){
        _camera = Camera.main;
        _canvas = GameManager.Instance.GetCanvas();
        _hpBar = Instantiate(hpBarPrefab, _canvas.transform).GetComponent<HpBar>();
        _hpBarRectTransform = _hpBar.GetComponent<RectTransform>();
        _offset = new Vector3(0, 1.5f, 0);
        SetActiveHPBar(false);
    }

    public void SetActiveHPBar(bool active){
        _hpBar.gameObject.SetActive(active);
    }

    public void SetHp(float hp){
        _hpBar.setHPGauge(hp);
        SetActiveHPBar(true);
        if (_hideHPBarCoroutine != null){
            StopCoroutine(_hideHPBarCoroutine);
        }

        _hideHPBarCoroutine = StartCoroutine(HideHPBarAfterDelay());
    }

    IEnumerator HideHPBarAfterDelay(){
        yield return _waitSeconds;
        SetActiveHPBar(false);
        _hideHPBarCoroutine = null;
    }

    private void LateUpdate(){
        var screenPosition = _camera.WorldToScreenPoint(transform.position + _offset);
        bool isVisible =
            screenPosition.z > 0
            && screenPosition.x > 0 && screenPosition.x < Screen.width
            && screenPosition.y > 0 && screenPosition.y < Screen.height;
        if (isVisible){
            _hpBarRectTransform.position = screenPosition;
        }
        else{
            SetActiveHPBar(false);
        }
    }

}