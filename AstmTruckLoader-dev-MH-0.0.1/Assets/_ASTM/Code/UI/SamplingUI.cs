using UnityEngine;
using UnityEngine.UI;
using WSMGameStudio.HeavyMachinery;

public class SamplingUI : MonoBehaviour
{
    #region Setters/Private Variables

    [Header("Controls")]
    [SerializeField] private float _loaderMoveSpeed = 0.01f;
    [Space][SerializeField] private float _minCorrectHeight = 0.03f;
    [SerializeField] private float _maxCorrectHeight = 0.09f;
    
    [Header("Resources")]
    [SerializeField] private RotatingMechanicalPart _loaderBucket;

    [Header("Buttons")]
    [SerializeField] private Button _sampleBtn;
    [SerializeField] private Button _tryAgainBtn;

    [Header("Objects")]
    [SerializeField] private GameObject _scaleObj;
    [SerializeField] private GameObject _controlsObj;
    [SerializeField] private GameObject _tooHighObj;
    [SerializeField] private GameObject _correctHeightObj;
    [SerializeField] private GameObject _tooLowObj;

    private bool _raiseBtnDown = false;
    private bool _lowerBtnDown = false;

    #endregion

    #region Unity Methods

    private void Start()
    {
        RegisterButtonEvents();
    }

    private void Update()
    {
        if (_raiseBtnDown && _loaderBucket.MovementInput <= 0.99f)
        {
            _loaderBucket.MovementInput += _loaderMoveSpeed * Time.deltaTime;
        }

        if (_lowerBtnDown && _loaderBucket.MovementInput > 0f)
        {
            _loaderBucket.MovementInput -= _loaderMoveSpeed * Time.deltaTime;
        }
    }

    #endregion

    #region Register Button Events

    private void RegisterButtonEvents()
    {
        _sampleBtn.onClick.RemoveAllListeners();
        _tryAgainBtn.onClick.RemoveAllListeners();

        _sampleBtn.onClick.AddListener(SampleBtn_fn);
        _tryAgainBtn.onClick.AddListener(TryAgainBtn_fn);
    }


    public void RaiseBtnDown_fn()
    {
        _raiseBtnDown = true;
        _lowerBtnDown = false;
    }
    public void RaiseBtnUp_fn()
    {
        _raiseBtnDown = false;
        _lowerBtnDown = false;

        _loaderBucket.MovementInput = Mathf.Clamp01(_loaderBucket.MovementInput);
    }

    public void LowerBtnDown_fn()
    {
        _raiseBtnDown = false;
        _lowerBtnDown = true;
    }
    public void LowerBtnUp_fn()
    {
        _raiseBtnDown = false;
        _lowerBtnDown = false;

        _loaderBucket.MovementInput = Mathf.Clamp01(_loaderBucket.MovementInput);
    }
    public void DisableObjs()
    {
        _tooHighObj.SetActive(false);
        _tooLowObj.SetActive(false);

        _tooHighObj.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
        _tooLowObj.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
    }
    private void SampleBtn_fn()
    {
        float currentHeight = _loaderBucket.MovementInput;

        if (currentHeight > _maxCorrectHeight)
        {
            _tooHighObj.SetActive(true);

            LeanTween.delayedCall(3, () =>
            {
                _tooHighObj.SetActive(false);
            });
        }
        else if (currentHeight < _minCorrectHeight)
        {
            MoveToTooLowSequence();
        }
        else
        {
            _correctHeightObj.SetActive(true);
            _correctHeightObj.GetComponent<Button>().onClick.AddListener(MoveToCollectSampleSequence);

            LeanTween.delayedCall(3, () =>
            {
                _correctHeightObj.SetActive(false);

                MoveToCollectSampleSequence();
            });
        }
    }

    private void TryAgainBtn_fn()
    {
        gameObject.SetActive(false);

        _tooLowObj.SetActive(false);

        _scaleObj.SetActive(true);
        _controlsObj.SetActive(true);

        GameController.Instance.GetUiControllerRef.ActivateSamplingState();
    }

    #endregion

    private void MoveToCollectSampleSequence()
    {
        gameObject.SetActive(false);

        GameController.Instance.GetUiControllerRef.ActivateCollectSampleState();
    }
    private void MoveToTooLowSequence()
    {
        _scaleObj.SetActive(false);
        _controlsObj.SetActive(false);

        GameController.Instance.GetUiControllerRef.ActivateTooLowState(() => 
        {
            LeanTween.delayedCall(3f, () =>
            {
                _tooLowObj.SetActive(true);
            });
        });
    }
}
