using UnityEngine;

public class ArrowUI : MonoBehaviour
{
    public Transform target;
    public float yOffset = 2.5f;

    void Update()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
