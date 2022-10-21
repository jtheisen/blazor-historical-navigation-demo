using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace BlazorApp6;

public class HistoricalNavigator : INotifyPropertyChanged
{
    private readonly IJSRuntime js;
    private readonly NavigationManager navigationManager;

    Int32 currentIndex = 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Int32 CurrentPageIndex => currentIndex;

    public HistoricalNavigator(IJSRuntime js, NavigationManager navigationManager)
    {
        this.js = js;
        this.navigationManager = navigationManager;

        navigationManager.RegisterLocationChangingHandler(HandleLocationChanging);
        navigationManager.LocationChanged += HandleLocationChanged;
    }

    private ValueTask HandleLocationChanging(LocationChangingContext ctx)
    {
        if (Int32.TryParse(ctx.HistoryEntryState, out currentIndex))
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        return ValueTask.CompletedTask;
    }

    private void HandleLocationChanged(Object? _, LocationChangedEventArgs e)
    {
        if (Int32.TryParse(e.HistoryEntryState, out currentIndex))
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }

    [JSInvokable]
    public void Navigate(String url, Boolean replace)
    {
        var options = new NavigationOptions
        {
            HistoryEntryState = (replace ? currentIndex : currentIndex + 1).ToString(),
            ReplaceHistoryEntry = replace
        };
        navigationManager.NavigateTo(url, options);
    }
}
