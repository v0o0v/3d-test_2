using System.Collections;
using UnityEngine;

public class DoorHuge : MonoBehaviour {

    [SerializeField] Constants.ESceneType sceneType;
    private Animator _animator;

    private void Awake(){
        _animator = GetComponent<Animator>();
    }

    public void Open(){
        _animator.SetTrigger("Open");
        StartCoroutine(LoadSceneRoutine(sceneType));
    }

    private IEnumerator LoadSceneRoutine(Constants.ESceneType type){
        yield return new WaitForSeconds(2f);
        GameManager.Instance.LoadScene(sceneType);
    }
    
    public void Close(){
        _animator.SetTrigger("Close");
    }

    private void OnTriggerEnter(Collider other){
        if (other.tag == "Player"){
            Open();
        }
    }

    private void OnTriggerExit(Collider other){
        if (other.tag == "Player"){
            Close();
        }
    }

}