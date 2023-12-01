using UnityEngine;

public class Ramp : MonoBehaviour
{
    public GameObject hitPrefab;

    void Update()
    {
        // Check for mouse click or touch input
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                // Use hit.point to instantiate the prefab at the clicked location
                Destroy(Instantiate(hitPrefab, hit.point, Quaternion.identity), 1);
            }
        }
    }
}
