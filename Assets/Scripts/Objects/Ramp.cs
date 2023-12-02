using UnityEngine;

public class Ramp : MonoBehaviour
{
    public GameObject hitPrefab;

    public AudioClip boardHitSound;

    void Update()
    {
        if (GameManager.Instance.Lives <= 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                Destroy(Instantiate(hitPrefab, hit.point, Quaternion.identity), 1);

                AudioSource.PlayClipAtPoint(boardHitSound, hit.point);
            }
        }
    }
}
