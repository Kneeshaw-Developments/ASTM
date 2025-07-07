using UnityEngine;

public class PickPile : MonoBehaviour
{

    [SerializeField] private PilePickerId _iId = PilePickerId.Left;

    private void OnMouseDown()
    {
        if (!GameController.Instance.canInteract) return;

        if (!GameController.Instance.canPickPile) return;

        if (GameController.Instance.inStockPileArea)
        {

            GameController.Instance.canInteract = false;

            GameController.Instance.GetUiControllerRef.ActiveErrorSampleUI();

            LeanTween.delayedCall(3, () =>
            {
                GameController.Instance.GetUiControllerRef.DeactivateErrorSampleUI();
                GameController.Instance.canInteract = true;
            });
        }
        else
        {
            GameController.Instance.canInteract = false;

            GameController.Instance.GetUiControllerRef.PickPileSample(_iId, this);
        }
    }
}
