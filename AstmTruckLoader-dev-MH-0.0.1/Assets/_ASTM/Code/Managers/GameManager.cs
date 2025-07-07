using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Setters/Private Variables

    [Header("Fade In/Out Objects")]
    [SerializeField] private float _fadeDuration = 1;
    [SerializeField] private GameObject _fadeInTransition;
    [SerializeField] private GameObject _fadeOutTransition;

    #endregion

    #region Getters/Public Variables

    public float GetTransitionDuration => _fadeDuration;

    #endregion

    public void DoFadeIn()
    {
        _fadeOutTransition.SetActive(false);
        _fadeInTransition.SetActive(true);
    }
    public void DoFadeOut()
    {
        _fadeOutTransition.SetActive(true);
        _fadeInTransition.SetActive(false);
    }
    public void StopFadeTransitions()
    {
        _fadeOutTransition.SetActive(false);
        _fadeInTransition.SetActive(false);
    }

}

#region Enums

public enum Scenes
{
    Splash,
    Game,
}

public enum TruckPosState
{
    Perfect,
    Correct,
    TooFarIncorrect,
    TooOnToIncorrect
}

public enum PilePickerId
{
    Left,
    Upper,
    Right,
    Bottom
}
#endregion
