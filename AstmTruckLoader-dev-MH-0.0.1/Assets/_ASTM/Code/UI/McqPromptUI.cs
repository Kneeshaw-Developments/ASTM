using UnityEngine;
using UnityEngine.UI;

public class McqPromptUI : MonoBehaviour
{
    #region Setters/Private Variables

    [Header("MCQ Option")]
    [SerializeField] private Option[] _options;

    [Header("Objects")]
    [SerializeField] private GameObject _correctObj;
    [SerializeField] private GameObject _wrongObj;

    #endregion

    #region Unity Methods

    private void Start()
    {
        RegisterButtonEvents();
    }

    #endregion

    #region Register Button Events

    private void RegisterButtonEvents()
    {
        foreach (Option option in _options)
        {
            option.IToggle.onValueChanged.RemoveAllListeners();
            option.IToggle.onValueChanged.AddListener((value) => { OptionBtn_fn(option, value); });
        }
    }
    private void OptionBtn_fn(Option option, bool value)
    {
        _wrongObj.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
        _wrongObj.SetActive(false);
        _correctObj.SetActive(false);

        if (!value)
        {
            return;
        }

        bool correct = option.ICorrect;

        _correctObj.SetActive(correct);
        _wrongObj.SetActive(!correct);

        if (correct)
        {
            _correctObj.GetComponent<Button>().onClick.AddListener(MoveToSamplingSequence);
            LeanTween.delayedCall(3, () =>
            {
                MoveToSamplingSequence();
            });
        }
    }

    #endregion

    private void MoveToSamplingSequence()
    {
        gameObject.SetActive(false);
        GameController.Instance.GetUiControllerRef.ActivateSamplingState();
    }
}

[System.Serializable]
public class Option
{
    public Toggle IToggle;
    public bool ICorrect = false;
}
