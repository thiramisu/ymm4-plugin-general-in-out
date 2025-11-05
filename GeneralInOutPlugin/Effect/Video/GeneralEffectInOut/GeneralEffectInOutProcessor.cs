using GeneralInOutPlugin.PropertyEditor.InOutAnimationParameter;
using System.ComponentModel;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

#pragma warning disable IDE0130 // Namespace がフォルダー構造と一致しません
namespace GeneralInOutPlugin.VideoEffect.Video.GeneralEffectInOut;
#pragma warning restore IDE0130 // Namespace がフォルダー構造と一致しません

internal class GeneralEffectInOutProcessor : IVideoEffectProcessor
{
    ID2D1Image? input;
    ID2D1Image? output;

    bool shouldUpdateEffectProcessors = true;
    (IVideoEffectProcessor videoEffectProcessor, VideoEffectWithInOutAnimation videoEffect)[] cachedProcessorEffectPair = [];

    private readonly GeneralEffectInOut item;
    private readonly IGraphicsDevicesAndContext devices;
    private readonly DisposeCollector disposer = new();

    public GeneralEffectInOutProcessor(GeneralEffectInOut item, IGraphicsDevicesAndContext devices)
    {
        this.item = item;
        this.item.PropertyChanged += OnPropertyChanged;

        this.devices = devices;

        disposer.Collect(input);
        disposer.Collect(output);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(GeneralEffectInOut.Effects):
                shouldUpdateEffectProcessors = true;
                break;
        }
    }

    public ID2D1Image Output => output ?? input ?? throw new NullReferenceException("No valid output image");

    public DrawDescription Update(EffectDescription effectDescription)
    {
        UpdateProcessorCacheIfNeeded();

        if (cachedProcessorEffectPair.Length == 0)
        {
            return effectDescription.DrawDescription;
        }

        var drawDesc = effectDescription.DrawDescription;
        var isHighPrecision = item.InOutAnimationParameter.IsHighPrecision;
        var (currentTime, remainingTime, fps, overridingItemDuration, inheritedValueRate) = TimeOverrideParameters.Calc(effectDescription, item.InOutAnimationParameter, isHighPrecision);

        var effectDesc = effectDescription with
        {
            ItemDuration = overridingItemDuration,
        };

        output = input;

        foreach (var (processor, effect) in cachedProcessorEffectPair)
        {
            if (!effect.IsEnabled)
            {
                continue;
            }

            var valueRate = effect.IsCustom ? ((IInOutAnimationParameter)effect).GetEffectValueRate(currentTime, remainingTime) : inheritedValueRate;

            effectDesc = effectDesc with
            {
                DrawDescription = drawDesc,
                //ItemPosition = TimeOverrideParameters.GetItemPosition(valueRate, fps),
                ItemPosition = TimeOverrideParameters.GetItemPosition(effectDesc, valueRate, fps, isHighPrecision),
            };
            processor.SetInput(output);
            drawDesc = processor.Update(effectDesc);
            output = processor.Output;
        }
        return drawDesc;
    }


    void UpdateProcessorCacheIfNeeded()
    {
        if (!shouldUpdateEffectProcessors)
        {
            return;
        }

        shouldUpdateEffectProcessors = false;
        cachedProcessorEffectPair = [.. item.GetEffectsAsVideoEffectWithAnimation()
                .Select(effect => (effect.CreateVideoEffect(devices), effect))];
    }

    public void ClearInput()
    {
        foreach (var (processor, _) in cachedProcessorEffectPair)
        {
            processor.ClearInput();
        }
        input = null;
    }

    public void SetInput(ID2D1Image? input) => this.input = input;

    public void Dispose()
    {
        foreach (var (processor, _) in cachedProcessorEffectPair)
        {
            processor.Dispose();
        }
    }
}