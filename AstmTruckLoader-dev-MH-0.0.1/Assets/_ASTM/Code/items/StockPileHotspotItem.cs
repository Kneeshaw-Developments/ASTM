using UnityEngine;

public class StockPileHotspotItem : MonoBehaviour
{
    #region Setters/Private Variables

    [SerializeField] private Material _iMat;
    [SerializeField] private float _blinkSpeed = 2f;

    private bool _doBlinking = true;
    private Color _baseColor;

    #endregion

    #region Unity Methods

    private void Start()
    {
        _baseColor = _iMat.color;
    }
    private void Update()
    {
        if (!_doBlinking) return;

        float alpha = Mathf.PingPong(Time.time * _blinkSpeed, 0.8f);
        Color newColor = _baseColor;
        newColor.a = alpha;
        _iMat.color = newColor;
    }

    private void OnMouseDown()
    {
        _doBlinking = false;

        GameController.Instance.GetUiControllerRef.ActivateStockPileState(this);
    }
    private void OnDisable()
    {
        _doBlinking = true;

        Color newColor = _baseColor;
        newColor.a = 1;
        _iMat.color = newColor;
    }

    #endregion

    public void forceHide()
    {
        Color newColor = _baseColor;
        newColor.a = 0;
        _iMat.color = newColor;
    }

}
