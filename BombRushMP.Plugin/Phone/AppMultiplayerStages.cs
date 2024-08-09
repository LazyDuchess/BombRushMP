using BombRushMP.Plugin;
using CommonAPI.Phone;

public class AppMultiplayerStages : CustomApp
{
    public override bool Available => false;
    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayerStages>("stages");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Stages");
        ScrollView = PhoneScrollView.Create(this);
        PopulateButtons();
    }

    private void PopulateButtons()
    {
        ScrollView.RemoveAllButtons();

        var button = PhoneUIUtility.CreateSimpleButton("Base Stages");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppMultiplayerBaseStages));
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Custom Stages");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppMultiplayerCustomStages));
        };
        ScrollView.AddButton(button);
    }
}
