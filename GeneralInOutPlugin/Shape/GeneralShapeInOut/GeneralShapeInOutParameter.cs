using GeneralInOutPlugin.PropertyEditor;
using GeneralInOutPlugin.PropertyEditor.InOutAnimationParameter;
using GeneralInOutPlugin.PropertyEditor.PluginInfoButton;
using GeneralInOutPlugin.PropertyEditor.PluginInfoDetails;
using GeneralInOutPlugin.PropertyEditor.PluginInfoFile;
using GeneralInOutPlugin.PropertyEditor.PluginInfoLink;
using GeneralInOutPlugin.PropertyEditor.PluginInfoRepo;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace GeneralInOutPlugin.Shape.GeneralShapeInOut;

[PluginDetails(AuthorName = PluginInfo.AuthorName)]
[PluginInfoDetails(GeneralShapeInOutPlugin.DefaultName)]
[PluginInfoLink(PluginInfo.DownloadLinkLabel, PluginInfo.DownloadLinkUrl)]
[PluginInfoLink(PluginInfo.ContactLinkLabel, PluginInfo.ContactLinkUrl)]
[PluginInfoFile(PluginInfo.ReadMePath)]
[PluginInfoFile(PluginInfo.AboutHighPrecisionPath, PluginInfo.AboutHighPrecisionLabel)]
[PluginInfoFile(PluginInfo.LicensePath)]
[PluginInfoRepo(PluginInfo.GitHubAutorName, PluginInfo.GitHubRepoName)]
class GeneralShapeInOutParameter : ShapeParameterBase
{
    [Display()]
    [PluginInfoButton]
    [JsonIgnore]
    [Obsolete("コントロール生成用ダミープロパティ")]
    public static bool PluginInfoButtonDummy => false;

    public static ShapeComboBoxItem[] ShapeComboBoxItems => ShapeComboBoxItem.GetItemsNotOfType<GeneralShapeInOutPlugin>();

    [Display(Name = "種類2")]
    [CommonComboBox(nameof(ShapeComboBoxItem.Name), nameof(ShapeComboBoxItem.ShapePlugin), nameof(ShapeComboBoxItems))]
    public IShapePlugin ShapePlugin
    {
        get => shapePlugin;
        set
        {
            // インスタンスが異なるとComboBoxの選択中が空になってしまうため、代入しなおす
            // GetType()だけだとバージョン変更時にたぶん正しく判定されないので、FullNameで比較する
            var valueTypeFullName = value.GetType().FullName;
            if (Set(ref shapePlugin, ShapeComboBoxItems.First((item) => item.ShapePlugin.GetType().FullName == valueTypeFullName).ShapePlugin))
            {
                var newParameter = value.CreateShapeParameter(GetSharedData());
                if (ShapeParameter.GetType() == newParameter.GetType())
                {
                    // UndoRedo時にCreateすると履歴が壊れるので、それを避ける
                    return;
                }
                // 親と子で共通のstoreを使用
                ShapeParameter = newParameter;
            }
        }
    }
    IShapePlugin shapePlugin = ShapeComboBoxItem.DefaultItem.ShapePlugin;

    [Display(AutoGenerateField = true)]
    public IShapeParameter ShapeParameter
    {
        get => shapeParameter;
        set
        {
            if (shapeParameter == value)
            {
                return;
            }
            UnSubscribeChildUndoRedoable(ShapeParameter);
            _ = Set(ref shapeParameter, value);
            SubscribeChildUndoRedoable(ShapeParameter);
        }
    }
    IShapeParameter shapeParameter;

    [Display(GroupName = "登場退場アニメーション", AutoGenerateField = true)]
    public InOutAnimationParameter InOutAnimationParameter { get; } = new();

    public GeneralShapeInOutParameter(SharedDataStore? sharedData) : base(sharedData)
    {
        // 親と子で共通のstoreを使用
        shapeParameter = shapePlugin.CreateShapeParameter(sharedData);
        SubscribeChildUndoRedoable(ShapeParameter);
    }

    public GeneralShapeInOutParameter() : this(null)
    {
    }

    public override IEnumerable<string> CreateMaskExoFilter(int keyFrameIndex, ExoOutputDescription desc, ShapeMaskExoOutputDescription shapeMaskParameters) => [];

    public override IEnumerable<string> CreateShapeItemExoFilter(int keyFrameIndex, ExoOutputDescription desc) => [];

    public override IShapeSource CreateShapeSource(IGraphicsDevicesAndContext devices) => new GeneralShapeInOutSource(devices, this);

    protected override IEnumerable<IAnimatable> GetAnimatables() => [InOutAnimationParameter, ShapeParameter];

    protected override void LoadSharedData(SharedDataStore store)
    {
        //var data = store.Load<SharedData>();
        //if (data is null)
        //    return;
        //data.CopyTo(this);
    }

    protected override void SaveSharedData(SharedDataStore store) =>
        // 2つのプラグイン間で互いに呼び出しあう可能性を考えると、もしセーブデータを参照してしまうと無限ループになるので ShapePlugin は保存しない
        //store.Save(new SharedData(this));
        // shapeParameter の内部実装のSaveを呼び出す
        shapeParameter.GetSharedData();
}
