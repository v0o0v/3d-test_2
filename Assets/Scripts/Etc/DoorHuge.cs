using UnityEngine;

public class DoorHuge : MonoBehaviour {

    private Animator _animator;

    private void Awake(){
        _animator = GetComponent<Animator>();
    }

    public void Open(){
        _animator.SetTrigger("Open");
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