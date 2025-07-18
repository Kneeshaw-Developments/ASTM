using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
public class LoaderUpDown : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator _truckAnimator;

    [Header("Animation State Name")]
    [SerializeField] private string _animationStateName = "Adjustloader"; // your animation clip name

    private float _savedTime = 0f;
    private bool _isReversing = false;
    private float _playbackTime = 0.1f;


    private void Start()
    {
        // Play and pause immediately at the start
        _truckAnimator.Play(_animationStateName, 0, 0f);
        _truckAnimator.speed = 0f;
    }
    private void Update()
    {
        Debug.Log(_playbackTime);
    }

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
            _playbackTime += Time.deltaTime / _truckAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
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
          
            _playbackTime -= Time.deltaTime / _truckAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
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

