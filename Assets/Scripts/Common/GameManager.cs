using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Constants;

public class GameManager : Singleton<GameManager> {

    private bool _isCursorLock;
    public Canvas canvas => GetCanvas();

    public void SetCursorLock(){
        Cursor.visible = _isCursorLock;
        Cursor.lockState = _isCursorLock ? CursorLockMode.None : CursorLockMode.Locked;
        _isCursorLock = !_isCursorLock;
    }

    public void LoadScene(ESceneType type){
        StartCoroutine(LoadSceneAsync(type));
    }

    private IEnumerator LoadSceneAsync(ESceneType sceneType){
        var loadingPanelPrefab = Resources.Load<GameObject>("Loading Panel");
        var loadingPanelObject = Instantiate(loadingPanelPrefab, canvas.transform);
        var loadingPanelController = loadingPanelObject.GetComponent<LoadingPanelController>();

        //로딩 창 표시
        bool showDone = false;
        loadingPanelController.Show(() => showDone = true);
        yield return new WaitUntil(() => showDone);
        
        // 씬 로드 진행
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneType.ToString());
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress <0.9f){
            loadingPanelController.SetProgress(asyncOperation.progress);
            yield return null;
        }
        loadingPanelController.SetProgress(1f);
        asyncOperation.allowSceneActivation = true;
        
        bool hideDone = false;
        loadingPanelController.Hide(() => hideDone = true);
        yield return new WaitUntil(() => hideDone);
        
        Destroy(loadingPanelObject);
    }

    public Canvas GetCanvas(){
        var canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        Canvas result = null;
        if (!canvasObject){
            canvasObject = new GameObject("Canvas");
            canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            result = canvasObject.GetComponent<Canvas>();
            result.renderMode = RenderMode.ScreenSpaceOverlay;
            result.tag = "Canvas";
        }
        else{
            result = canvasObject.GetComponent<Canvas>();
        }

        return result;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode){
    }

}