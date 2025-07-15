using UnityEngine;
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
    }

    private void Update()
    {
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
    public void DisableObjs()
    {
        _incorrectObj.SetActive(false);

        _incorrectObj.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
    }
    private void FlattenSampleBtn_fn()
    {
        if (_state == TruckPosState.Perfect)
        {
            _perfectObj.SetActive(true);
            _perfectObj.GetComponent<Button>().onClick.AddListener(MoveToFlattenSequence);
            LeanTween.delayedCall(3, () =>
            {
                _perfectObj.SetActive(false);

                MoveToFlattenSequence();
            });
        }
        else if (_state == TruckPosState.Correct)
        {
            _correctObj.SetActive(true);
            _correctObj.GetComponent<Button>().onClick.AddListener(MoveToFlattenSequence);
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
}

