using BombRushMP.MapStation;
using BombRushMP.Plugin;
using CommonAPI.Phone;
using Reptile;
using System.Collections.Generic;
using System.Linq;
using BombRushMP.Common;

public class AppMultiplayerCustomStages : StageListApp
{
    private static bool LastSortedByPlayerCount = false;
    public override bool Available => false;
    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayerCustomStages>("custom stages");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Custom Stages", 70f);
        ScrollView = PhoneScrollView.Create(this);
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

        button = PhoneUIUtility.CreateSimpleButton("Sort: Alphabetically");
        button.OnConfirm += () =>
        {
            PopulateButtons(false);
        };
        ScrollView.AddButton(button);

        var stageList = new List<MPStage>(MapStationSupport.Stages);
        stageList = stageList.OrderBy(x => x.DisplayName).ToList();

        if (sortByPlayerCount)
            stageList = stageList.OrderByDescending(x => ClientController.GetPlayerCountForStage((Stage)x.Id)).ToList();

        foreach (var stage in stageList)
            CreateStageButton(stage.DisplayName, (Stage)stage.Id);
    }
}
