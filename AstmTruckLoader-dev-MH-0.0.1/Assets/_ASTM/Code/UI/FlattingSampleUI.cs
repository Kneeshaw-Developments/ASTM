using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlattingSampleUI : MonoBehaviour
{
    #region Setters/Private Variables

    [Header("Controls")]
    [SerializeField] private float _moveSpeed = 0.1f;
    [Space][SerializeField] private float _mostAccuratePos_1;
    [SerializeField] private float _mostAccuratePos_2;
    [Space][SerializeField] private float _withinTolerance_1;
    [SerializeField] private float _withinTolerance_2;
    [Space][SerializeField] private float _tooOntopos;
    [SerializeField] private float _tooFarPos;

    [Header("Resources")]
    [SerializeField] private Transform _truckTransform;
    [SerializeField] private Transform _frontWheelMeshTransform;
    [SerializeField] private Transform _backWheelMeshTransform;

    [Header("Buttons")]
    [SerializeField] private Button _flattenSampleBtn;

    [Header("Objects")]
    [SerializeField] private GameObject _correctObj;
    [SerializeField] private GameObject _perfectObj;
    [SerializeField] private GameObject _incorrectObj;

    private bool _forwardBtnDown = false;
    private bool _backBtnDown = false;

    private float _movementInput = 0;
    private TruckPosState _state;

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
       // Debug.Log(_playbackTime);
        if (_forwardBtnDown)
        {
            _movementInput += _moveSpeed * Time.deltaTime;

            _truckTransform.localPosition += new Vector3(0, 0, Mathf.Clamp01(_movementInput));
            _frontWheelMeshTransform.localEulerAngles += new Vector3(Mathf.Clamp01(_movementInput), 0, 0);
            _backWheelMeshTransform.localEulerAngles += new Vector3(Mathf.Clamp01(_movementInput), 0, 0);
        }

        if (_backBtnDown)
        {
            _movementInput -= _moveSpeed * Time.deltaTime;

            _truckTransform.localPosition += new Vector3(0, 0, Mathf.Clamp(_movementInput, -1, 0));
        }

        ValidateTrcukLoaderPos();
    }

    #endregion

    #region Register Button Events

    private void RegisterButtonEvents()
    {
        _flattenSampleBtn.onClick.RemoveAllListeners();

        _flattenSampleBtn.onClick.AddListener(FlattenSampleBtn_fn);
    }

    public void ForwardBtnDown_fn()
    {
        _forwardBtnDown = true;
        _backBtnDown = false;
    }
    public void ForwardBtnUp_fn()
    {
        _forwardBtnDown = false;
        _backBtnDown = false;

        _movementInput = 0;
    }

    public void BackBtnDown_fn()
    {
        _forwardBtnDown = false;
        _backBtnDown = true;
    }
    public void BackBtnUp_fn()
    {
        _forwardBtnDown = false;
        _backBtnDown = false;

        _movementInput = 0;
    }

    private void FlattenSampleBtn_fn()
    {
        if (_playbackTime >= 0.51 && _playbackTime <= 0.55)
        {
            _perfectObj.SetActive(true);

            LeanTween.delayedCall(3, () =>
            {
                _perfectObj.SetActive(false);

                MoveToFlattenSequence();
            });
        }
        else if (_playbackTime >= 0.45 && _playbackTime <= 0.6)
        {
            _correctObj.SetActive(true);

            LeanTween.delayedCall(3, () =>
            {
                _correctObj.SetActive(false);

                MoveToFlattenSequence();
            });
        }
        else
        {
            _incorrectObj.SetActive(true);

            LeanTween.delayedCall(3, () =>
            {
                _incorrectObj.SetActive(false);

            });
        }
        /*  if (_state == TruckPosState.Perfect)
          {
              _perfectObj.SetActive(true);

              LeanTween.delayedCall(3, () =>
              {
                  _perfectObj.SetActive(false);

                  MoveToFlattenSequence();
              });
          }
          else if (_state == TruckPosState.Correct)
          {
              _correctObj.SetActive(true);

              LeanTween.delayedCall(3, () =>
              {
                  _correctObj.SetActive(false);

                  MoveToFlattenSequence();
              });
          }
          else
          {
              _incorrectObj.SetActive(true);

              LeanTween.delayedCall(3, () =>
              {
                  _incorrectObj.SetActive(false);

              });
          }*/
    }

    #endregion

    private void ValidateTrcukLoaderPos()
    {
        float posZ = _truckTransform.localPosition.z;

        if(posZ < _tooFarPos)
        {
            _truckTransform.localPosition = new Vector3(_truckTransform.localPosition.x, _truckTransform.localPosition.y, _tooFarPos);
            posZ = _tooFarPos;
        }
        else if (posZ > _tooOntopos)
        {
            _truckTransform.localPosition = new Vector3(_truckTransform.localPosition.x, _truckTransform.localPosition.y, _tooOntopos);
            posZ = _tooOntopos;
        }

        if (posZ >= _mostAccuratePos_1 && posZ <= _mostAccuratePos_2)
        {
            _state = TruckPosState.Perfect;
        }
        else if ((posZ > _mostAccuratePos_2 && posZ <= _withinTolerance_2) || ( posZ < _mostAccuratePos_1 && posZ >= _withinTolerance_1))
        {
            _state = TruckPosState.Correct;
        }
        else if(posZ > _withinTolerance_1)
        {
            _state = TruckPosState.TooOnToIncorrect;
        }
        else
        {
            _state = TruckPosState.TooFarIncorrect;
        }
    }

    private void MoveToFlattenSequence()
    {
        gameObject.SetActive(false);

        GameController.Instance.GetUiControllerRef.ActivateFlattenState();
    }





    [Header("Animator")]
    [SerializeField] private Animator _truckAnimator;

    [Header("Animation State Name")]
    [SerializeField] private string _animationStateName = "Adjustloader"; // your animation clip name

    private float _savedTime = 0f;
    private bool _isReversing = false;
    private float _playbackTime = 0f;
    private float _animSpeed = 0.4f;


    public void OnUpButtonDown()
    {
        StartCoroutine(PlayForward());
    }

    public void OnDownButtonDown()
    {
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

            _playbackTime -= (_animSpeed * Time.deltaTime) / _truckAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
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
}

