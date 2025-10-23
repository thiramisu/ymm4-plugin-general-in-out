using GeneralInOutPlugin.PropertyEditor.InOutAnimationParameter;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.ItemEditor.CustomVisibilityAttributes;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace GeneralInOutPlugin.VideoEffect.Video.GeneralEffectInOut;

public class VideoEffectWithInOutAnimation(IVideoEffect effect) : VideoEffectBase, IInOutAnimationParameter
{
    [Display(GroupName = "アニメーション設定", Name = "個別設定", Description = "個別設定する")]
    [ToggleSlider]
    public bool IsCustom { get => isCustom; set => Set(ref isCustom, value); }
    bool isCustom = false;

    // [ShowPropertyEditorWhen]での制御を行いたいが、再帰的には適用されないようなので仕方なくコピペ
    [ShowPropertyEditorWhen(nameof(IsCustom), true)]
    [Display(GroupName = "アニメーション設定", Name = "登場所要", Description = "登場アニメーションが再生される秒数。\n0にすると瞬間移動になります。これと登場前遅延の両方を0にすると登場アニメーションが無効になります")]
    [TextBoxSlider("F2", "秒", 0d, 4d)]
    [DefaultValue(0.3d)]
    [Range(0d, YMM4Constants.VeryLargeValue)]
    public double DurationSecondsIn { get => durationSecondsIn; set => Set(ref durationSecondsIn, value); }
    double durationSecondsIn = 0.3d;

    [ShowPropertyEditorWhen(nameof(IsCustom), true)]
    [Display(GroupName = "アニメーション設定", Name = "退場所要", Description = "退場アニメーションが再生される秒数。\n0にすると瞬間移動になります。これと退場後遅延の両方を0にすると退場アニメーションが無効になります")]
    [TextBoxSlider("F2", "秒", 0d, 4d)]
    [DefaultValue(0.3d)]
    [Range(0d, YMM4Constants.VeryLargeValue)]
    public double DurationSecondsOut { get => durationSecondsOut; set => Set(ref durationSecondsOut, value); }
    double durationSecondsOut = 0.3d;

    [ShowPropertyEditorWhen(nameof(IsCustom), true)]
    [Display(GroupName = "アニメーション設定", Name = "登場前遅延", Description = "アイテム表示から登場アニメーション開始までの秒数")]
    [TextBoxSlider("F2", "秒", -4d, 4d)]
    [DefaultValue(0d)]
    [Range(YMM4Constants.VerySmallValue, YMM4Constants.VeryLargeValue)]
    public double DelaySecondsBeforeIn { get => delaySecondsBeforeIn; set => Set(ref delaySecondsBeforeIn, value); }
    double delaySecondsBeforeIn = 0d;

    [ShowPropertyEditorWhen(nameof(IsCustom), true)]
    [Display(GroupName = "アニメーション設定", Name = "退場後遅延", Description = "退場アニメーション終了からアイテム非表示までの秒数")]
    [TextBoxSlider("F2", "秒", -4d, 4d)]
    [DefaultValue(0d)]
    [Range(YMM4Constants.VerySmallValue, YMM4Constants.VeryLargeValue)]
    public double DelaySecondsAfterOut { get => delaySecondsAfterOut; set => Set(ref delaySecondsAfterOut, value); }
    double delaySecondsAfterOut = 0d;

    [Display(AutoGenerateField = true)]
    public IVideoEffect Effect { get; } = effect;

    public override string Label => !isCustom
                ? Effect.Label
                : $"[個] {Effect.Label} / {(DelaySecondsBeforeIn + DurationSecondsIn != 0d ? FormatDuration("登場", delaySecondsBeforeIn, DurationSecondsIn, false) : string.Empty)}{(DelaySecondsAfterOut + DurationSecondsOut != 0d ? FormatDuration("退場", delaySecondsAfterOut, DurationSecondsOut, true) : string.Empty)}{(DelaySecondsBeforeIn + DurationSecondsIn == 0d && DelaySecondsAfterOut + DurationSecondsOut == 0d ? "[なし]" : string.Empty)}";

    static string FormatDuration(string label, double delay, double duration, bool isReversed)
    {
        var endSeconds = string.Format("{0:F2}", delay + duration);
        if (delay == 0d)
        {
            return $"[{label}:{endSeconds}秒]";
        }
        var startSeconds = string.Format("{0:F2}", delay);
        if (isReversed)
        {
            (startSeconds, endSeconds) = (endSeconds, startSeconds);
        }

        return $"[{label}:{startSeconds}{(isReversed ? "←" : "→")}{endSeconds}秒]";
    }

    protected override bool Set<T>(ref T storage, T value, [CallerMemberName] string name = "", params string[] etcChangedPropertyNames)
    {
        if (base.Set(ref storage, value, name, etcChangedPropertyNames))
        {
            OnPropertyChanged(nameof(Label));
            return true;
        }
        return false;
    }

    public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription) => throw new NotImplementedException();

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices) => Effect.CreateVideoEffect(devices);

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Effect];
}