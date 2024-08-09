using BombRushMP.Plugin;
using CommonAPI.Phone;
using Reptile;
using System.Collections.Generic;
using System.Linq;

public class AppMultiplayerBaseStages : StageListApp
{
    private static bool LastSortedByPlayerCount = false;
    private static Dictionary<string, Stage> StageMap = new Dictionary<string, Stage>
    {
        { "Hideout", Stage.hideout },
        { "Versum Hill", Stage.downhill },
        { "Millenium Square", Stage.square },
        { "Brink Terminal", Stage.tower },
        { "Millenium Mall", Stage.Mall },
        { "Mataan", Stage.osaka },
        { "Pyramid Island", Stage.pyramid },
        { "Police Station", Stage.Prelude }
    };
    public override bool Available => false;
    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayerBaseStages>("base stages");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Base Stages", 70f);
        ScrollView = PhoneScrollView.Create(this);
    }

    public override void OnAppEnable()
    {
        base.OnAppEnable();
        PopulateButtons(LastSortedByPlayerCount);
    }

    private void PopulateButtons(bool sortByPlayerCount = false)
    {
        LastSortedByPlayerCount = sortByPlayerCount;

        ScrollView.RemoveAllButtons();

        var button = PhoneUIUtility.CreateSimpleButton("Sort: Player Count");
        button.OnConfirm += () =>
        {
            PopulateButtons(true);
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Sort: Default");
        button.OnConfirm += () =>
        {
            PopulateButtons(false);
        };
        ScrollView.AddButton(button);

        var stageMap = new Dictionary<string, Stage>(StageMap);

        if (sortByPlayerCount)
            stageMap = StageMap.OrderByDescending(x => ClientController.GetPlayerCountForStage(x.Value)).ToDictionary(x => x.Key, x => x.Value);

        foreach(var stage in stageMap)
            CreateStageButton(stage.Key, stage.Value);
    }
}
