using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace GeneralInOutPlugin.Shape.GeneralShapeInOut;

[PluginDetails(AuthorName = PluginInfo.AuthorName)]
public class GeneralShapeInOutPlugin : IShapePlugin
{
    public bool IsExoShapeSupported => false;

    public bool IsExoMaskSupported => false;

    public string Name => "登場退場アニメーション図形";

    public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData) => new GeneralShapeInOutParameter(sharedData);
}