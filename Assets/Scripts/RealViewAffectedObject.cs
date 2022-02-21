using UnityEngine;

public class RealViewAffectedObject : MonoBehaviour
{
    [SerializeField] private Material notRealMaterial;
    [SerializeField] private Material realMaterial;

    protected virtual void OnEnable()
    {
        Game.GetInstance().OnRealViewToggle += Player_OnRealViewToggle;
    }

    protected virtual void OnDisable()
    {
        Game.GetInstance().OnRealViewToggle -= Player_OnRealViewToggle;
    }

    private void Player_OnRealViewToggle(object sender, Game.RealViewEventArgs e)
    {
        ShowRealMaterial(e.isRealViewActive);
    }

    public void ShowRealMaterial(bool showReal = true)
    {
        transform.GetComponent<MeshRenderer>().material = showReal ? realMaterial : notRealMaterial;
    }
}
