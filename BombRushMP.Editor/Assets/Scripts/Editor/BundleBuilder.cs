using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class BundleBuilder : MonoBehaviour
{
    [MenuItem("BombRushMP/Build Asset Bundle (Release)")]
    private static void BuildAssetBundleRelease()
    {
        BuildAssets(true);
    }

    [MenuItem("BombRushMP/Build Asset Bundle (Debug)")]
    private static void BuildAssetBundleDebug()
    {
        BuildAssets(false);
    }

    private const string BundleName = "assets";
    private const string OutputPath = "Build";

    private static void BuildAssets(bool compressed)
    {
        Directory.CreateDirectory(OutputPath);
        var assets = AssetDatabase.GetAssetPathsFromAssetBundle(BundleName);
        var deps = AssetDatabase.GetDependencies(assets, true);
        deps = deps.Where(dep => AssetDatabase.GetImplicitAssetBundleName(dep) == string.Empty && !dep.EndsWith(".cs")).ToArray();
        var finalAssets = assets.Concat(deps).Distinct().ToArray();
        var build = new AssetBundleBuild
        {
            assetBundleName = BundleName,
            assetNames = finalAssets
        };
        BuildPipeline.BuildAssetBundles(OutputPath, new[] { build }, compressed ? BuildAssetBundleOptions.None : BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
    }
}