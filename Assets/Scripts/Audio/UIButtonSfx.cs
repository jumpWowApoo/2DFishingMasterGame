using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIButtonSfx : MonoBehaviour
{
    public void Ui_Click()         => AudioHub.I?.PlayUi(UiSfx.ButtonClick);
    public void Ui_Hover()         => AudioHub.I?.PlayUi(UiSfx.ButtonHover);
    public void Ui_OpenWindow()    => AudioHub.I?.PlayUi(UiSfx.OpenWindow);
    public void Ui_CloseWindow()   => AudioHub.I?.PlayUi(UiSfx.CloseWindow);
    public void Ui_OpenCrate()     => AudioHub.I?.PlayUi(UiSfx.OpenCrate);
    public void Ui_PutInCrate()    => AudioHub.I?.PlayUi(UiSfx.PutInCrate);
    public void Ui_TakeOutCrate()  => AudioHub.I?.PlayUi(UiSfx.TakeOutCrate);
    public void Ui_BuyConfirm()    => AudioHub.I?.PlayUi(UiSfx.BuyConfirm);
    public void Ui_Error()         => AudioHub.I?.PlayUi(UiSfx.Error);
    public void Ui_Notification()  => AudioHub.I?.PlayUi(UiSfx.Notification);
}