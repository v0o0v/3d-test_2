public interface IWeaponObservable<T> {

    void Subscribe(IWeaponObserver<T> observer);
    void Unsubscribe(IWeaponObserver<T> observer);
    void Notify(T value);

}