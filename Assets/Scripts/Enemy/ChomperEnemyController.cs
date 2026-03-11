using System;
using UnityEngine;

public class ChomperEnemyController : EnemyController, IWeaponObserver<GameObject> {

    private MeleeWeaponController _meleeWeaponController;

    private void Start(){
        _meleeWeaponController = GetComponent<MeleeWeaponController>();
        _meleeWeaponController.Subscribe(this);
    }

    public void OnNext(GameObject value){ }

    public void OnComplete(){
        _meleeWeaponController.Unsubscribe(this);
    }
    public void OnError(Exception error){ }

    public void PlayStep(){ }

    public void Grunt(){ }

    public void AttackBegin(){ }

    public void AttackEnd(){ }

}