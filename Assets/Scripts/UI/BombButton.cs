using UnityEngine;

public class BombButton : MonoBehaviour
{
    public MarbleSpawner marbleSpawner;

    private float destructionDelay = 0.1f;
    private SaveObject savedData;
    private bool isPressed;

    void Awake()
    {
        savedData = SaveManager.Load();
        if (!savedData.SelectedPerks.SelectedSpecial.Contains(PerkEnum.Bomb))
        {
            gameObject.SetActive(false);
        }
    }

    public void OnButtonPressed()
    {
        if (isPressed)
        {
            return;
        }

        isPressed = true;

        if (marbleSpawner != null)
        {
            marbleSpawner.DestroyAll(onlyBad: true);
        }

        Destroy(gameObject, destructionDelay);
    }
}