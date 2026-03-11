using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponTriggerZone {

    public Vector3 position;
    public float radius;

}

public class MeleeWeaponController : MonoBehaviour, IWeaponObservable<GameObject> {

    [SerializeField] private WeaponTriggerZone[] triggerZones;
    [SerializeField] private LayerMask targetLayerMask;
    
    private HashSet<Collider> _hitColliders = new();
    private Vector3[] _previousTriggerPositions;
    
    List<IWeaponObserver<GameObject>> _observers = new();

    public void Subscribe(IWeaponObserver<GameObject> observer){
        if (!_observers.Contains(observer)){
            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IWeaponObserver<GameObject> observer){
        _observers.Remove(observer);
    }

    public void Notify(GameObject value){
        _observers.ForEach(observer => observer.OnNext(value));
    }

}