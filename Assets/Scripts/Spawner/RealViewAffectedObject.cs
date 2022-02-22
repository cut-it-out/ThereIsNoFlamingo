using UnityEngine;

public class RealViewAffectedObject : MonoBehaviour
{
    [Header("Not Real View Sprites")]
    [SerializeField] private Sprite notRealSprite;
    [SerializeField] private Sprite realSprite;
    [Header("Not Real View Mesh")]
    [SerializeField] private Transform objMeshWithSpriteRenderer;

    public bool IsRealObject { get; private set; } = false;

    protected virtual void OnEnable()
    {
        Game.GetInstance().OnRealViewToggle += Object_OnRealViewToggle;
    }

    //protected virtual void OnDisable()
    //{
    //    Game.GetInstance().OnRealViewToggle -= Player_OnRealViewToggle;
    //}

    private void Object_OnRealViewToggle(object sender, Game.RealViewEventArgs e)
    {
        ShowRealSprite(e.isRealViewActive);
    }

    public void ShowRealSprite(bool showReal = true)
    {
        objMeshWithSpriteRenderer.GetComponent<SpriteRenderer>().sprite = showReal ? 
            (IsRealObject == true ? realSprite : null ) : notRealSprite;
    }

    public void SetIsReal(bool value)
    {
        IsRealObject = value;
    }

    public virtual void DestroySelf()
    {
        Game.GetInstance().OnRealViewToggle -= Object_OnRealViewToggle;
        Destroy(gameObject);
    }
}
