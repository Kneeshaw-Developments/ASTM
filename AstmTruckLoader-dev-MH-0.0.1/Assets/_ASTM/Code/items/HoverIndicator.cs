using UnityEngine;

public class HoverIndicator : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject redCircle; 
    public LayerMask hoverLayer; 
    public Transform midPoint;
    public float stockPileAreaDistance;

    void Start()
    {
        if (redCircle != null)
            redCircle.SetActive(false); // Hide on start
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, hoverLayer))
        {
            GameController.Instance.canPickPile = true;

            redCircle.SetActive(true);
            redCircle.transform.position = hit.point;
            redCircle.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            redCircle.transform.localPosition += new Vector3(-0.2f, 0, 0.2f);
            redCircle.transform.localEulerAngles = new Vector3(0, redCircle.transform.localEulerAngles.y, redCircle.transform.localEulerAngles.z);

            if(Vector3.Distance(redCircle.transform.position, midPoint.transform.position) <= stockPileAreaDistance)
            {
                GameController.Instance.inStockPileArea = false;
            }
            else
            {
                GameController.Instance.inStockPileArea = true;

            }
        }
        else
        {
            GameController.Instance.canPickPile = false;
            redCircle.SetActive(false);
        }
    }
}
