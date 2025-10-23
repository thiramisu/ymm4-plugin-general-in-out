namespace GeneralInOutPlugin.PropertyEditor.InOutAnimationParameter;

internal interface IInOutAnimationParameter
{
    double DurationSecondsIn { get; set; }
    double DurationSecondsOut { get; set; }
    double DelaySecondsBeforeIn { get; set; }
    double DelaySecondsAfterOut { get; set; }

    public double GetEffectValueRate(TimeSpan currentTime, TimeSpan remainingTime)
    {
        var startTime = TimeSpan.FromSeconds(DelaySecondsBeforeIn);
        var endTime = TimeSpan.FromSeconds(DelaySecondsAfterOut);
        var durationTimeIn = TimeSpan.FromSeconds(DurationSecondsIn);
        var durationTimeOut = TimeSpan.FromSeconds(DurationSecondsOut);
        var valueRateOnIn = DurationSecondsIn == 0d ? (currentTime < startTime ? 0d : 1d) : Math.Clamp((currentTime - startTime) / durationTimeIn, 0d, 1d);
        var valueRateOnOut = DurationSecondsOut == 0d ? (remainingTime < endTime ? -1d : 0d) : Math.Clamp((remainingTime - endTime) / durationTimeOut, 0d, 1d) - 1d;
        return Math.Clamp(valueRateOnIn + valueRateOnOut, 0d, 1d);
    }
}