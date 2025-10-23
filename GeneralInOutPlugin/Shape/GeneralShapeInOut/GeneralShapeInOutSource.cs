using System.ComponentModel;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace GeneralInOutPlugin.Shape.GeneralShapeInOut;

internal class GeneralShapeInOutSource : IShapeSource
{
    readonly private IGraphicsDevicesAndContext devices;
    readonly private GeneralShapeInOutParameter generalShapeInOutParameter;
    readonly DisposeCollector disposer = new();

    bool shouldUpdateShapeSource = true;

    IShapeSource? shapeSource;

    /// <summary>
    /// 描画結果
    /// </summary>
    public ID2D1Image Output => shapeSource?.Output ?? throw new Exception($"{nameof(shapeSource)}がnullです。事前にUpdateを呼び出す必要があります。");

    public GeneralShapeInOutSource(IGraphicsDevicesAndContext devices, GeneralShapeInOutParameter generalShapeInOutParameter)
    {
        this.devices = devices;

        this.generalShapeInOutParameter = generalShapeInOutParameter;

        generalShapeInOutParameter.PropertyChanged += OnParameterChanged;
    }

    private void OnParameterChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(generalShapeInOutParameter.ShapeParameter):
                shouldUpdateShapeSource = true;
                break;
        }
    }

    /// <summary>
    /// 図形を更新する
    /// </summary>
    /// <param name="timelineItemSourceDescription"></param>
    public void Update(TimelineItemSourceDescription timelineItemSourceDescription)
    {
        //var fps = timelineItemSourceDescription.FPS;
        //var frame = timelineItemSourceDescription.ItemPosition.Frame;
        //var length = timelineItemSourceDescription.ItemDuration.Frame;

        if (shapeSource is null || shouldUpdateShapeSource)
        {
            if (shapeSource is not null)
            {
                disposer.RemoveAndDispose(ref shapeSource);
            }
            shapeSource = generalShapeInOutParameter.ShapeParameter.CreateShapeSource(devices);
            disposer.Collect(shapeSource);
        }

        var isHighPrecision = generalShapeInOutParameter.InOutAnimationParameter.IsHighPrecision;

        var (_, _, fps, overridingItemDuration, valueRate) = TimeOverrideParameters.Calc(timelineItemSourceDescription, generalShapeInOutParameter.InOutAnimationParameter, isHighPrecision);

        shapeSource.Update(timelineItemSourceDescription with
        {
            //ItemPosition = TimeOverrideParameters.GetItemPosition(valueRate, fps),
            ItemPosition = TimeOverrideParameters.GetItemPosition(timelineItemSourceDescription, valueRate, fps, isHighPrecision),
            ItemDuration = overridingItemDuration,
        });
    }


    #region IDisposable
    private bool disposedValue;


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // マネージド状態を破棄します (マネージド オブジェクト)
                generalShapeInOutParameter.PropertyChanged -= OnParameterChanged;
                disposer.DisposeAndClear();
            }

            // アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
            // 大きなフィールドを null に設定します
            disposedValue = true;
        }
    }

    // // 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
    // ~SampleShapeSource()
    // {
    //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}