using BombRushMP.Plugin;
using BombRushMP.Plugin.Phone;
using CommonAPI.Phone;
using Reptile;

public abstract class StageListApp : CustomApp
{
    protected ClientController ClientController => ClientController.Instance;
    protected SimplePhoneButton CreateStageButton(string label, Stage stage)
    {
        var button = PhoneUIUtility.CreateSimpleButton($"(0) {label}");
        button.OnConfirm += () =>
        {
            Core.Instance.BaseModule.StageManager.ExitCurrentStage(stage);
        };
        var stageButton = button.gameObject.AddComponent<StageButton>();
        stageButton.Stage = stage;
        stageButton.StageName = label;
        ScrollView.AddButton(button);
        return button;
    }
}
