using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Transform _hpBar;

    public void SetHpBar(float ratio)
    {
        ratio = Mathf.Clamp(ratio, 0, 1);
        _hpBar.localScale = new Vector3(ratio, 1, 1);
    }
}