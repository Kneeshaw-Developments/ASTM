using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public GameObject StartPanel;
    public GameObject McqPanel;
    public GameObject AdjustHightPanel;
    public GameObject AdjustPositionPanel;
    public GameObject GmeWinPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickStart()
    {
        StartPanel.gameObject.SetActive(false);
    }

}
