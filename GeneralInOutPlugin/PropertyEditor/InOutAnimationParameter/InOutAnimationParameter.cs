using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace GeneralInOutPlugin.PropertyEditor.InOutAnimationParameter;

public class InOutAnimationParameter : Animatable, IInOutAnimationParameter
{

    [Display(Name = "登場所要", Description = "登場アニメーションが再生される秒数。\n0にすると瞬間移動になります。これと登場前遅延の両方を0にすると登場アニメーションが無効になります")]
    [TextBoxSlider("F2", "秒", 0d, 4d)]
    [DefaultValue(0.3d)]
    [Range(0d, YMM4Constants.VeryLargeValue)]
    public double DurationSecondsIn { get => durationSecondsIn; set => Set(ref durationSecondsIn, value); }
    double durationSecondsIn = 0.3d;

    [Display(Name = "退場所要", Description = "退場アニメーションが再生される秒数。\n0にすると瞬間移動になります。これと退場後遅延の両方を0にすると退場アニメーションが無効になります")]
    [TextBoxSlider("F2", "秒", 0d, 4d)]
    [DefaultValue(0.3d)]
    [Range(0d, YMM4Constants.VeryLargeValue)]
    public double DurationSecondsOut { get => durationSecondsOut; set => Set(ref durationSecondsOut, value); }
    double durationSecondsOut = 0.3d;

    [Display(Name = "登場前遅延", Description = "アイテム表示から登場アニメーション開始までの秒数")]
    [TextBoxSlider("F2", "秒", -4d, 4d)]
    [DefaultValue(0d)]
    [Range(YMM4Constants.VerySmallValue, YMM4Constants.VeryLargeValue)]
    public double DelaySecondsBeforeIn { get => delayInSeconds; set => Set(ref delayInSeconds, value); }
    double delayInSeconds = 0d;

    [Display(Name = "退場後遅延", Description = "退場アニメーション終了からアイテム非表示までの秒数")]
    [TextBoxSlider("F2", "秒", -4d, 4d)]
    [DefaultValue(0d)]
    [Range(YMM4Constants.VerySmallValue, YMM4Constants.VeryLargeValue)]
    public double DelaySecondsAfterOut { get => delayOutSeconds; set => Set(ref delayOutSeconds, value); }
    double delayOutSeconds = 0d;

    [Display(Name = "精度を高める", Description = "（実験的機能）アニメーションの最中の誤差を少なくするか。\nアイテムに中間点を設定する場合や、何か問題が発生した場合は、Offにしてください")]
    [ToggleSlider]
    public bool IsHighPrecision { get => isHighPrecision; set => Set(ref isHighPrecision, value); }
    bool isHighPrecision = false;

    protected override IEnumerable<IAnimatable> GetAnimatables() => [];

    public void CopyFrom(InOutAnimationParameter parameter)
    {
        DurationSecondsIn = parameter.DurationSecondsIn;
        DurationSecondsOut = parameter.DurationSecondsOut;
        DelaySecondsBeforeIn = parameter.DelaySecondsBeforeIn;
        DelaySecondsAfterOut = parameter.DelaySecondsAfterOut;
        IsHighPrecision = parameter.IsHighPrecision;
    }
}
