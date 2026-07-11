using BombRushMP.Plugin;
using CommonAPI.Phone;

public class AppMultiplayerThemes : CustomApp
{
    public override bool Available => false;
    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayerThemes>("themes");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Themes");
        ScrollView = PhoneScrollView.Create(this);
        PopulateButtons();
    }

    private void PopulateButtons()
    {
        ScrollView.RemoveAllButtons();

        foreach(var themeName in MPPlugin.ThemePaths.Keys)
        {
            var button = PhoneUIUtility.CreateSimpleButton(themeName);
            button.OnConfirm += () =>
            {
                MPSettings.Instance.Theme = themeName;
                MyPhone.CloseCurrentApp();
            };
            ScrollView.AddButton(button);
        }
    }
}
