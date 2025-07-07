using UnityEngine;

public class GameController : Singleton<GameController>
{
    private UiController _uiControllerRef;
    public UiController GetUiControllerRef
    {
        get
        {
            if(_uiControllerRef == null)
            {
                _uiControllerRef = FindFirstObjectByType<UiController>();
            }

            return _uiControllerRef;
        }
    }


    public bool inStockPileArea = false;
    public bool canPickPile = false;
    public bool canInteract = true;

}
