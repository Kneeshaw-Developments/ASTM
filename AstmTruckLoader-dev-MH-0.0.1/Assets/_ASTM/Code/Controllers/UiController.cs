using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WSMGameStudio.HeavyMachinery;
using WSMGameStudio.Vehicles;
using System.Collections;
using System.Xml.Linq;


public class UiController : MonoBehaviour
{
    #region Setters/Private Variables

    [Header("Vehicle Control")]
    [SerializeField] private WSMVehicleController _vehicleController;
    [SerializeField] private RotatingMechanicalPart _loaderBucket;

    [Header("Panels")]
    [SerializeField] private GameObject _startPromptUI;
    [SerializeField] private GameObject _mcqPromptUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _instructionsUI;
    [SerializeField] private GameObject _samplingUI;
    [SerializeField] private GameObject _flattenSamplingUI;
    [SerializeField] private GameObject _errorSamplingUI;
    [SerializeField] private GameObject _winUI;

    [Header("Buttons")]
    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _settingsBtn;
    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _openInstructionsBtn;
    [SerializeField] private Button _closeInstructionsBtn;
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _retryBtn;

    [Header("Cameras")]
    [SerializeField] private GameObject _defaultCamera;
    [SerializeField] private GameObject _samplingCamera;
    [SerializeField] private GameObject _tooLowCamera;
    [SerializeField] private GameObject _collectedSampleCamera;
    [SerializeField] private GameObject _stockPileCamera;
    [SerializeField] private GameObject _flattenSampleCamera;
    [SerializeField] private GameObject _flatStockPileCamera;
    [SerializeField] private GameObject _bucketShovelCamera;

    [Header("Objects")]
    [SerializeField] private GameObject _scrapedMudPatchObj;
    [SerializeField] private GameObject _stockPilePatch1Obj;
    [SerializeField] private GameObject _bucketObj;
    [SerializeField] private GameObject _shovelObj;
    [SerializeField] private GameObject _higlightPileObj;
    [SerializeField] private GameObject[] _samplesInBucket;
    
    [SerializeField] private GameObject _stockPileHotspotItem;
    [SerializeField] private GameObject _activateStock;
    [SerializeField] private GameObject _wheelLoader;
    [SerializeField] private GameObject _PileMound;
    [SerializeField] private GameObject _left,_right,_top,_bottom;

    public AudioSource engineSfx;

    [Header("Animator")]
    [SerializeField] private Animator _gamePlayAnim;
    [SerializeField] private Animator _shovelAnim;


    private int _bucketSamplesCount = 0;
    #endregion
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void OnValidate()
    {
        if (_samplesInBucket == null || _samplesInBucket.Length == 0)
            Debug.LogWarning("[UiController] No bucket samples hooked up.");
    }
    #endif

    #region Unity Methods

    private void Awake()
    {
        GameManager.Instance?.DoFadeIn();
    }
    private void Start()
    {
        RegisterButtonEvents();
    }

    #endregion

    #region Register Button Events

    private void RegisterButtonEvents()
    {
        _startBtn.onClick.RemoveAllListeners();
        _settingsBtn.onClick.RemoveAllListeners();
        _resumeBtn.onClick.RemoveAllListeners();
        _openInstructionsBtn.onClick.RemoveAllListeners();
        _closeInstructionsBtn.onClick.RemoveAllListeners();
        _exitBtn.onClick.RemoveAllListeners();
        _retryBtn.onClick.RemoveAllListeners();

        _startBtn.onClick.AddListener(StartBtn_fn);
        _settingsBtn.onClick.AddListener(SettingsBtn_fn);
        _resumeBtn.onClick.AddListener(ResumeBtn_fn);
        _openInstructionsBtn.onClick.AddListener(OpenInstructionsBtn_fn);
        _closeInstructionsBtn.onClick.AddListener(CloseInstructionsBtn_fn);
        _exitBtn.onClick.AddListener(ExitBtn_fn);
        _retryBtn.onClick.AddListener(RetryBtn_fn);

    }

    private void StartBtn_fn()
    {
        _startPromptUI.SetActive(false);
        _mcqPromptUI.SetActive(true);
    }
    private void SettingsBtn_fn()
    {
        Time.timeScale = 0f;
        _settingsUI.SetActive(true);
    }
    private void ResumeBtn_fn()
    {
        Time.timeScale = 1f;
        _settingsUI.SetActive(false);
    }
    private void OpenInstructionsBtn_fn()
    {
        _instructionsUI.SetActive(true);
    }
    private void CloseInstructionsBtn_fn()
    {
        _instructionsUI.SetActive(false);
    }
    private void ExitBtn_fn()
    {
        Time.timeScale = 1f;
        _settingsUI.SetActive(false);
    }

    private void RetryBtn_fn()
    {
        GameManager.Instance?.StopFadeTransitions();
        GameManager.Instance?.DoFadeIn();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    public void ActivateSamplingState()
    {
        _gamePlayAnim.SetBool("CollectSample", false);

        _defaultCamera.SetActive(false);
        _tooLowCamera.SetActive(false);
        _collectedSampleCamera.SetActive(false);
        _samplingCamera.SetActive(true);
        _samplingUI.SetActive(true);

        _scrapedMudPatchObj.SetActive(false);
    }
    public void ActivateTooLowState(Action onCompletedAction)
    {
        GameManager.Instance?.StopFadeTransitions();
        GameManager.Instance?.DoFadeIn();

        _samplingCamera.SetActive(false);
        _collectedSampleCamera.SetActive(false);

        _tooLowCamera.SetActive(true);

        LeanTween.delayedCall(0.5f, () =>
        {
            _scrapedMudPatchObj.SetActive(true);

            onCompletedAction?.Invoke();
        });
    }
    public void ActivateCollectSampleState()
    {
        _vehicleController.engineSFX.volume = 0.3f;
    //    _vehicleController.IsEngineOn = true;
        if (engineSfx != null && !engineSfx.isPlaying)
        {
            engineSfx.Play();
       
        }

        LeanTween.delayedCall(1, () =>
        {
            _gamePlayAnim.SetBool("CollectSample", true);
            StartCoroutine(WaitAndActivateStockPile());
            StartCoroutine(WaitAndActivateStock());
            _gamePlayAnim.speed = 1f; // Resume normal speed
            LeanTween.delayedCall(1.5f, () =>
            {
               // _vehicleController.IsEngineOn = false;

                _samplingCamera.SetActive(false);
                _collectedSampleCamera.SetActive(true);
            });

            LeanTween.delayedCall(5, () => 
            {
                //_vehicleController.IsEngineOn = false;
                _PileMound.SetActive(false);
            });

        });

    }
    private IEnumerator WaitAndActivateStockPile()
    {
        yield return new WaitForSeconds(30f);

        GameController.Instance.GetUiControllerRef.ActivateStockPileState(_stockPileHotspotItem.GetComponent<StockPileHotspotItem>());
    }
    private IEnumerator WaitAndActivateStock()
    {
        yield return new WaitForSeconds(31.2f);

        // _activateStock.gameObject.SetActive(true);
        ShowStockFromBottom();
      
    }
    void ShowStockFromBottom()
    {
        // Set start position (2 units below)
        Vector3 finalPos = _activateStock.transform.position;
        Vector3 startPos = finalPos + new Vector3(0f, -2f, 0f);
        _activateStock.transform.position = startPos;

        // Activate the GameObject
        _activateStock.SetActive(true);

        // Move up
        LeanTween.move(_activateStock, finalPos, 2.5f).setEase(LeanTweenType.easeOutCubic);
    }
    public void ActivateStockPileState(StockPileHotspotItem stockPileHotspot)
    {
        stockPileHotspot.forceHide();
        _collectedSampleCamera.SetActive(false);
        _stockPileCamera.SetActive(true);

        _loaderBucket.MovementInput = 0.5f;

        LeanTween.delayedCall(2.5f, () =>
        {
            _loaderBucket.MovementInput = 0.2f;

          //  _gamePlayAnim.SetBool("CreateStockPile", true);
          //  _gamePlayAnim.SetBool("CollectSample", false);
        });

        LeanTween.delayedCall(5, () => 
        {
            _loaderBucket.MovementInput = 0.25f;

            _stockPileCamera.SetActive(false);
            _flattenSampleCamera.SetActive(true);

         //   _gamePlayAnim.enabled = false;
            _flattenSamplingUI.SetActive(true);
            _wheelLoader.gameObject.SetActive(false);
        //    engineSfx.Stop();
        });
    }
    public void ActivateFlattenState()
    {
        if (engineSfx != null && !engineSfx.isPlaying)
        {
            engineSfx.Play();
        }

        _flattenSampleCamera.SetActive(false);
        _flatStockPileCamera.SetActive(true);

        _gamePlayAnim.enabled = true;

        _gamePlayAnim.SetBool("FlatStockPile", true);
        _gamePlayAnim.speed = 1f; // Resume normal speed
        // _gamePlayAnim.SetBool("CreateStockPile", false);
        LeanTween.scale(_activateStock, new Vector3(1f, 1f, 1f), 5.5f);
      
        LeanTween.delayedCall(5f, () =>
        {
            _flatStockPileCamera.SetActive(false);
            _bucketShovelCamera.SetActive(true);
            _stockPilePatch1Obj.SetActive(true);
          //  LeanTween.scale(_activateStock, new Vector3(1f, 1f, 1f), 5.5f);
            _gamePlayAnim.gameObject.SetActive(false);

        });

        LeanTween.delayedCall(5.7f, () => 
        {
            _stockPilePatch1Obj.SetActive(true);
        });

        LeanTween.delayedCall(7f, () =>
        {
            _activateStock.gameObject.SetActive(false);
            GameManager.Instance?.StopFadeTransitions();
            GameManager.Instance?.DoFadeIn();

            _shovelObj.SetActive(true);
            _bucketObj.SetActive(true);
            _higlightPileObj.SetActive(true);
            engineSfx.Stop();
            //   _activateStock.SetActive(false);


        });
    }


    public void PickPileSample(PilePickerId pilePickerId, PickPile pile)
{
    GameController.Instance.canInteract = false;

    string keyAnim =
        pilePickerId == PilePickerId.Left   ? "LeftSampleCollection"   :
        pilePickerId == PilePickerId.Right  ? "RightSampleCollection"  :
        pilePickerId == PilePickerId.Upper  ? "UpperSampleCollection"  :
                                              "BottomSampleCollection";

    // EXCLUSIVE: clear all, then set one
    _shovelAnim.SetBool("LeftSampleCollection",   false);
    _shovelAnim.SetBool("RightSampleCollection",  false);
    _shovelAnim.SetBool("UpperSampleCollection",  false);
    _shovelAnim.SetBool("BottomSampleCollection", false);
    _shovelAnim.SetBool(keyAnim, true);

    _bucketShovelCamera.SetActive(true);
    _shovelAnim.SetBool("FlatStockPile", false);

    // ⛔ Remove/Comment this: StartCoroutine(WaitAndActivatePile(keyAnim));

    LeanTween.delayedCall(4f, () =>
    {
        _shovelAnim.SetBool(keyAnim, false);

        // Hide ONLY what was actually clicked
        if (pile && pile.transform && pile.transform.parent)
            pile.transform.parent.gameObject.SetActive(false);

        // Bucket fill (bounds-safe)
        if (_bucketSamplesCount >= 0 && _bucketSamplesCount < _samplesInBucket.Length)
        {
            _samplesInBucket[_bucketSamplesCount].SetActive(true);
            _bucketSamplesCount++;
        }

        if (_bucketSamplesCount >= _samplesInBucket.Length)
            _winUI.SetActive(true);
        else
            GameController.Instance.canInteract = true;
    });
}


    private IEnumerator WaitAndActivatePile(string keyAnim)
    {
        yield return new WaitForSeconds(3.2f);

        if (keyAnim == "LeftSampleCollection")
        {
            _left.gameObject.SetActive(false);
        }
        else if (keyAnim == "RightSampleCollection")
        {
            _right.gameObject.SetActive(false);
        }
        else if (keyAnim == "UpperSampleCollection")
        {
            _top.gameObject.SetActive(false);
        }
        else
        {
            _bottom.gameObject.SetActive(false);
        }

    }

    public void ActiveErrorSampleUI()
    {
        _errorSamplingUI.SetActive(true);
    }
    public void DeactivateErrorSampleUI()
    {
        _errorSamplingUI.SetActive(false);
    }

}
