using GeneralInOutPlugin.PropertyEditor.InOutAnimationParameter;
using GeneralInOutPlugin.PropertyEditor.PluginInfoButton;
using GeneralInOutPlugin.PropertyEditor.PluginInfoFile;
using GeneralInOutPlugin.PropertyEditor.PluginInfoLink;
using GeneralInOutPlugin.PropertyEditor.PluginInfoRepo;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

#pragma warning disable IDE0130 // Namespace がフォルダー構造と一致しません
namespace GeneralInOutPlugin.VideoEffect.Video.GeneralEffectInOut;
#pragma warning restore IDE0130 // Namespace がフォルダー構造と一致しません

// 映像エフェクト
// 映像エフェクトには必ず[VideoEffect]属性を設定してください。
[VideoEffect("登場退場（汎用）", ["登場退場"], [], true, false)]
[PluginDetails(AuthorName = PluginInfo.AuthorName)]
[PluginInfoLink(PluginInfo.DownloadLinkLabel, PluginInfo.DownloadLinkUrl)]
[PluginInfoLink(PluginInfo.ContactLinkLabel, PluginInfo.ContactLinkUrl)]
[PluginInfoFile(PluginInfo.ReadMePath)]
[PluginInfoFile(PluginInfo.AboutHighPrecisionPath, PluginInfo.AboutHighPrecisionLabel)]
[PluginInfoFile(PluginInfo.LicensePath)]
[PluginInfoRepo(PluginInfo.GitHubAutorName, PluginInfo.GitHubRepoName)]
public class GeneralEffectInOut : VideoEffectBase
{
    /// <summary>
    /// エフェクトの名前
    /// </summary>
    public override string Label => "登場退場（汎用）";

    [Display(GroupName = "登場退場（汎用） / 全体設定")]
    [PluginInfoButton(PropertyEditorSize = PropertyEditorSize.FullWidth)]
    [JsonIgnore]
    [Obsolete("コントロール生成用ダミープロパティ")]
    public static bool PluginInfoButtonDummy => false;

    [Display(GroupName = "登場退場（汎用） / 全体設定", AutoGenerateField = true)]
    public InOutAnimationParameter InOutAnimationParameter { get; } = new();

    [Display(GroupName = "登場退場（汎用） / エフェクト定義", Name = "エフェクト", Description = "アニメーション1回分の変化")]
    [VideoEffectSelector]
    public ImmutableList<IVideoEffect> Effects
    {
        get => effects;
        set
        {
            // 自動的にラップする
            var wrapped = value
                .Select(e => e is VideoEffectWithInOutAnimation
                    ? e // すでにラップされている場合
                    : new VideoEffectWithInOutAnimation(e))
                .ToImmutableList();

            _ = Set(ref effects, wrapped);
        }
    }
    ImmutableList<IVideoEffect> effects = [];
    public IEnumerable<VideoEffectWithInOutAnimation> GetEffectsAsVideoEffectWithAnimation() => effects.Cast<VideoEffectWithInOutAnimation>();

    public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription) => [];

    /// <summary>
    /// 映像エフェクトを作成する
    /// </summary>
    /// <param name="devices">デバイス</param>
    /// <returns>映像エフェクト</returns>
    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices) => new GeneralEffectInOutProcessor(this, devices);

    /// <summary>
    /// クラス内のIAnimatableを列挙する。
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<IAnimatable> GetAnimatables() => [InOutAnimationParameter, .. Effects];
}