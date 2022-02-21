using UnityEngine;

public class RealViewAffectedObject : MonoBehaviour
{
    [Header("Not Real View Sprites")]
    [SerializeField] private Sprite notRealSprite;
    [SerializeField] private Sprite realSprite;

    protected virtual void OnEnable()
    {
        Game.GetInstance().OnRealViewToggle += Player_OnRealViewToggle;
    }

    //protected virtual void OnDisable()
    //{
    //    Game.GetInstance().OnRealViewToggle -= Player_OnRealViewToggle;
    //}

    private void Player_OnRealViewToggle(object sender, Game.RealViewEventArgs e)
    {
        ShowRealSprite(e.isRealViewActive);
    }

    public void ShowRealSprite(bool showReal = true)
    {
        transform.GetComponent<SpriteRenderer>().sprite = showReal ? realSprite : notRealSprite;
    }
}
