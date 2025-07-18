using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameInit : MonoBehaviour
{
    #region Setters/Private Variables

    [Header("Loading UI")]
    [SerializeField] private Image _loadingFiller;

    #endregion

    #region Unity Methods

    private void Start()
    {
        StartCoroutine(LoadGameScene());
    }

    #endregion

    IEnumerator LoadGameScene()
    {
        GameManager.Instance.DoFadeIn();

        float transitionDuration = GameManager.Instance.GetTransitionDuration;

        yield return new WaitForSeconds(transitionDuration);

        GameManager.Instance.StopFadeTransitions();

        _loadingFiller.fillAmount = 0;
        float progress = 0;

        AsyncOperation operation = SceneManager.LoadSceneAsync(Scenes.Game.ToString());
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            progress = Mathf.MoveTowards(progress, operation.progress, 0.4f * Time.deltaTime);

            _loadingFiller.fillAmount = progress / 0.9f;

            if (_loadingFiller.fillAmount > 0.99f)
            {
                _loadingFiller.fillAmount = 1;

                GameManager.Instance.DoFadeOut();
                yield return new WaitForSeconds(transitionDuration);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }

    }
}
