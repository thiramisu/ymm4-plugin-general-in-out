using System.Collections.Immutable;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Shape;

namespace GeneralInOutPlugin.PropertyEditor;

public class ShapeComboBoxItem(IShapePlugin ShapePlugin)
{
    public static ImmutableArray<ShapeComboBoxItem> DefaultItems { get; } = [.. PluginLoader.ShapePlugins.Select((shapePlugin) => new ShapeComboBoxItem(shapePlugin))];
    public static readonly ShapeComboBoxItem DefaultItem;

    static ShapeComboBoxItem()
    {
        DefaultItem = DefaultItems.First((item) => item.ShapePlugin is QuadrilateralShapePlugin);
    }

    public static ShapeComboBoxItem[] GetItemsNotOfType<T>() where T : IShapePlugin => [.. DefaultItems.Where((shapePlugin) => shapePlugin.ShapePlugin is not T)];

    public IShapePlugin ShapePlugin { get; } = ShapePlugin;
    public string Name => ShapePlugin.Name;
}
