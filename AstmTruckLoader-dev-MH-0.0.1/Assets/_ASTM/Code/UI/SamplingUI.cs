using UnityEngine;
using UnityEngine.UI;
using WSMGameStudio.HeavyMachinery;
using System.Collections;

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
        // Play and pause immediately at the start
        _truckAnimator.Play(_animationStateName, 0, 0f);
        _truckAnimator.speed = 0f;
    }

    private void Update()
    {
        Debug.Log(_playbackTime);
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

    private void SampleBtn_fn()
    {
        float currentHeight = _loaderBucket.MovementInput;

        if (_playbackTime >= 0.13 && _playbackTime <= 0.23)
        {
          
           
            _correctHeightObj.SetActive(true);

            LeanTween.delayedCall(3, () =>
            {
                _correctHeightObj.SetActive(false);
              //  PlayAnimatorNormally();
                MoveToCollectSampleSequence();
            });
        }
        else if (_playbackTime <= 0.08)
        {
            MoveToTooLowSequence();
        }
        else
        {
            _tooHighObj.SetActive(true);

            LeanTween.delayedCall(3, () =>
            {
                _tooHighObj.SetActive(false);

            });
        }



        /*float currentHeight = _loaderBucket.MovementInput;

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

            LeanTween.delayedCall(3, () =>
            {
                _correctHeightObj.SetActive(false);

                MoveToCollectSampleSequence();
            });
        }*/
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



    [Header("Animator")]
    [SerializeField] private Animator _truckAnimator;

    [Header("Animation State Name")]
    [SerializeField] private string _animationStateName = "Adjustloader"; // your animation clip name

    private float _savedTime = 0f;
    private bool _isReversing = false;
    public float _playbackTime = 0.1f;
    private float _animSpeed = 0.5f;


    public void OnUpButtonDown()
    {
        /* if (_playbackTime >= 0.95f)
         {
             UpButton.interactable = false;
         }*/
        //   DownButton.interactable = true;
        StartCoroutine(PlayForward());
    }

    public void OnDownButtonDown()
    {
        /*if (_playbackTime > 0.02)
        {
            DownButton.interactable = false;
        }*/
        //  UpButton.interactable = true;
        StartCoroutine(PlayBackward());
    }

    public void OnUpButtonUp()
    {
        StopAllCoroutines();
    }

    public void OnDownButtonUp()
    {
        StopAllCoroutines();
    }

    private IEnumerator PlayForward()
    {
        while (_playbackTime < 1f)
        {
            _playbackTime += (_animSpeed * Time.deltaTime) / _truckAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            _truckAnimator.Play(_animationStateName, 0, _playbackTime);
            yield return null;
        }
    }

    private IEnumerator PlayBackward()
    {

        while (_playbackTime > -0.1f)
        {
            if (_playbackTime <= 0)
            {
                _playbackTime = 0;
            }

            _playbackTime -= (_animSpeed *Time.deltaTime )/ _truckAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            _truckAnimator.Play(_animationStateName, 0, _playbackTime);
            yield return null;
        }

    }
    public void TakeSample()
    {
        if (_playbackTime >= 0.13 && _playbackTime <= 0.23)
        {
            Debug.Log("Corect");
        }
        else if (_playbackTime <= 0.08)
        {
            Debug.LogWarning("Too low");
        }
        else
        {
            Debug.LogError("wrong");
        }
    }


    public void PlayAnimatorNormally()
    {
      /*  StopAllCoroutines(); // Stop manual scrubbing if any
        _truckAnimator.speed = 1f; // Resume normal speed
        _truckAnimator.Play(_animationStateName, 0); // Play from start (optional)*/
    }

}
