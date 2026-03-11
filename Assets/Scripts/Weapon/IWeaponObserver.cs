using System;

public interface IWeaponObserver<T> {

    void OnNext(T value);
    void OnComplete();
    void OnError(Exception error);

}