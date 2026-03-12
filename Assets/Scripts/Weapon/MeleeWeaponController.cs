using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WeaponTriggerZone {

    public Vector3 position;
    public float radius;

}

public class MeleeWeaponController : MonoBehaviour, IWeaponObservable<GameObject> {

    [SerializeField] private WeaponTriggerZone[] triggerZones;
    [SerializeField] private LayerMask targetLayerMask;

    private HashSet<Collider> _hitColliders;
    private Vector3[] _previousTriggerPositions;

    List<IWeaponObserver<GameObject>> _observers = new();

    private bool _isTriggering;

    private void Awake(){
        _previousTriggerPositions = new Vector3[triggerZones.Length];
        _hitColliders = new();
        _isTriggering = false;
    }

    private void FixedUpdate(){
        if (!_isTriggering)
            return;
        for (int i = 0; i < triggerZones.Length; i++){
            var worldPosition = GetTriggerWorldPosition(_previousTriggerPositions[i]);
            var direction = worldPosition - _previousTriggerPositions[i];
            Ray ray = new Ray(worldPosition, direction);
            RaycastHit[] hits = new RaycastHit[10];
            var hitCount = Physics.SphereCastNonAlloc(ray, triggerZones[i].radius, hits, direction.magnitude,
                targetLayerMask);
            for (int j = 0; j < hitCount; j++){
                var hit = hits[j];
                _hitColliders.Add(hit.collider);
            }

            _previousTriggerPositions[i] = worldPosition;
        }
    }

    private void OnDrawGizmos(){
        if (!Application.isPlaying) return;

        for (int i = 0; i < triggerZones.Length; i++){
            var worldPosition = GetTriggerWorldPosition(triggerZones[i].position);
            var direction = worldPosition - _previousTriggerPositions[i];
            Gizmos.color = Color.blueViolet;
            Gizmos.DrawWireSphere(worldPosition, triggerZones[i].radius);
            Gizmos.color = Color.darkOrange;
            Gizmos.DrawWireSphere(worldPosition + direction, triggerZones[i].radius);
        }
    }

    public void StartTrigger(){
        _hitColliders.Clear();
        for (int i = 0; i < triggerZones.Length; i++){
            _previousTriggerPositions[i] = GetTriggerWorldPosition(triggerZones[i].position);
        }

        _isTriggering = true;
    }

    public void EndTrigger(){
        foreach (Collider hitCollider in _hitColliders){
            Notify(hitCollider.gameObject);
        }

        _isTriggering = false;
    }

    private Vector3 GetTriggerWorldPosition(Vector3 position){
        return transform.position + transform.TransformDirection(position);
    }

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