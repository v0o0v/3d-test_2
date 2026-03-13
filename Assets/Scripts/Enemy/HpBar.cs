using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour {

    [SerializeField] private Image gauge;
    
    public void setHPGauge(float hp){
        gauge.fillAmount = hp;
    }

}