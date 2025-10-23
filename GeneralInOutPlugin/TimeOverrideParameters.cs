using GeneralInOutPlugin.PropertyEditor.InOutAnimationParameter;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace GeneralInOutPlugin;

internal static class TimeOverrideParameters
{
    /// <summary>
    /// 比を維持しつつ精度を上げるために掛け算する倍率
    /// </summary>
    const int PrecisionScale = YMM4Constants.VeryLargeValue;

    public static (TimeSpan currentTime, TimeSpan remainingTime, int fps, YukkuriMovieMaker.Player.Video.FrameTime overridingItemDuration, double valueRate) Calc<T>(T timelineItemSourceDescription, IInOutAnimationParameter inOutAnimationParameter, bool isHighPrecision)
        where T : TimelineItemSourceDescription
    {
        var currentTime = timelineItemSourceDescription.ItemPosition.Time;
        var itemDuration = timelineItemSourceDescription.ItemDuration.Time;
        var remainingTime = itemDuration - currentTime;
        var fps = timelineItemSourceDescription.FPS;

        var overridingItemDuration = GetItemPosition(timelineItemSourceDescription, 1d, fps, isHighPrecision);
        var valueRate = inOutAnimationParameter.GetEffectValueRate(currentTime, remainingTime);
        return (currentTime, remainingTime, fps, overridingItemDuration, valueRate);
    }

    public static YukkuriMovieMaker.Player.Video.FrameTime GetItemPosition<T>(T timelineItemSourceDescription, double valueRate, int fps, bool isHighPrecision) where T : TimelineItemSourceDescription
        => isHighPrecision ? new((int)(PrecisionScale * valueRate), fps) : new(timelineItemSourceDescription.ItemDuration.Time * valueRate, fps);
}
