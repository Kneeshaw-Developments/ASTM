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
            option.IBtn.onClick.RemoveAllListeners();
            option.IBtn.onClick.AddListener(() => { OptionBtn_fn(option); });
        }
    }
    private void OptionBtn_fn(Option option)
    {
        bool correct = option.ICorrect;

        _correctObj.SetActive(correct);
        _wrongObj.SetActive(!correct);

        LeanTween.delayedCall(3, () =>
        {
            _correctObj.SetActive(false);
            _wrongObj.SetActive(false);

            if (correct)
                MoveToSamplingSequence();
        });
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
    public Button IBtn;
    public bool ICorrect = false;
}
