using UnityEngine;

public class TopMarble : Marble
{
    public Marble bottomMarble;
    private Vector3 offset;
    private bool isAttachedToBottomMarble = false;

    new void Start()
    {
        base.Start();

        if (bottomMarble != null)
        {
            CalculateOffset();
            AttachToBottomMarble();
            bottomMarble.OnDestroyed += DetachFromBottomMarble;
        }
    }

    void Update()
    {
        if (isAttachedToBottomMarble)
        {
            FollowBottomMarble();
        }
    }

    void CalculateOffset()
    {
        offset = transform.position - bottomMarble.transform.position;
    }

    void FollowBottomMarble()
    {
        if (bottomMarble == null)
        {
            isAttachedToBottomMarble = false;
        }
        else
        {
            transform.position = bottomMarble.transform.position + offset;
        }
    }

    public void AttachToBottomMarble()
    {
        if (bottomMarble != null)
        {
            isAttachedToBottomMarble = true;
        }
    }

    public void DetachFromBottomMarble()
    {
        isAttachedToBottomMarble = false;

        if (bottomMarble != null)
        {
            bottomMarble.OnDestroyed -= DetachFromBottomMarble;
        }
    }

    public void OnDestroy()
    {
        if (bottomMarble != null)
        {
            bottomMarble.OnDestroyed -= DetachFromBottomMarble;
        }
    }
}