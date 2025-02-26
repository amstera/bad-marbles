using UnityEngine;

public class TrackBarrier : MonoBehaviour
{
    public PointsText pointsTextPrefab;
    public GameObject BigExplosion;
    public StressReceiver stressReceiver;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Marble marble = collision.gameObject.GetComponent<Marble>();
        if (marble != null)
        {
            if (marble.color == MarbleColor.Tier || marble.color == MarbleColor.Life)
            {
                return;
            }

            if (gameManager.Lives > 0)
            {
                var hitPoint = collision.contacts[0].point;
                if (marble.points > 0)
                {
                    int points = marble.points * gameManager.CalculateStreakMultiplier();
                    gameManager.AddScore(points);
                    ShowPoints(points, hitPoint);
                }
                else if (marble.livesLost > 0)
                {
                    gameManager.LoseLives(marble.livesLost);
                    if (marble.color == MarbleColor.Bomb)
                    {
                        stressReceiver?.InduceStress(1f, true);
                        Destroy(Instantiate(BigExplosion, hitPoint, Quaternion.identity), 5);
                    }
                    else
                    {
                        ShowLives(hitPoint, -marble.livesLost);
                    }
                }
            }

            Destroy(marble.gameObject);
        }
    }

    private void ShowPoints(int points, Vector3 position)
    {
        if (points > 0)
        {
            PointsText pointsTextInstance = Instantiate(pointsTextPrefab, position, Quaternion.identity);
            pointsTextInstance.SetPoints(points);
        }
    }

    private void ShowLives(Vector3 position, int livesLost)
    {
        PointsText pointsTextInstance = Instantiate(pointsTextPrefab, position, Quaternion.identity);
        pointsTextInstance.SetLives(livesLost);
    }
}