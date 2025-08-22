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
        var cam = mainCamera != null ? mainCamera : Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, hoverLayer, QueryTriggerInteraction.Collide))
        {
            GameController.Instance.canPickPile = true;

            // Use the ACTUAL hit point for gating, not the redCircle's (which you offset later)
            Vector3 p = hit.point;
            bool inside = Vector3.Distance(p, midPoint.position) <= stockPileAreaDistance;
            GameController.Instance.inStockPileArea = !inside;  // inside = OK → false (no error); outside = error → true

            if (redCircle != null)
            {
                redCircle.SetActive(true);
                redCircle.transform.position = p;
                redCircle.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);

                // ⛔ REMOVE the offset/euler hacks — they desync gating vs where the cursor hit
                // redCircle.transform.localPosition += new Vector3(-0.2f, 0, 0.2f);
                // redCircle.transform.localEulerAngles = new Vector3(0, redCircle.transform.localEulerAngles.y, redCircle.transform.localEulerAngles.z);
            }
        }
        else
        {
            GameController.Instance.canPickPile = false;
            if (redCircle != null) redCircle.SetActive(false);
        }
    }

}
