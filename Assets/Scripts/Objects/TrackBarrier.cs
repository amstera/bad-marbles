using UnityEngine;

public class TrackBarrier : MonoBehaviour
{
    public PointsText pointsTextPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        Marble marble = collision.gameObject.GetComponent<Marble>();
        if (marble != null)
        {
            if (marble.color == MarbleColor.Tier)
            {
                return;
            }

            if (GameManager.Instance.Lives > 0)
            {
                if (marble.points > 0)
                {
                    GameManager.Instance.AddScore(marble.points);
                    ShowPoints(marble.points, collision.contacts[0].point);
                }
                else if (marble.livesLost > 0)
                {
                    GameManager.Instance.LoseLives(marble.livesLost);
                }
            }

            Destroy(collision.gameObject);
        }
    }

    private void ShowPoints(int points, Vector3 position)
    {
        if (points > 0)
        {
            points *= GameManager.Instance.CalculateMultiplier();
            PointsText pointsTextInstance = Instantiate(pointsTextPrefab, position, Quaternion.identity);
            pointsTextInstance.SetPoints(points);
        }
    }
}