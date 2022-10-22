using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.JSInterop;
using System.ComponentModel;
using RouteData = Microsoft.AspNetCore.Components.RouteData;

namespace BlazorApp6;

public struct HistoryState
{
    public RouteData routeData;
}

public class HistoricalNavigator : INotifyPropertyChanged
{
    private readonly IJSRuntime js;
    private readonly NavigationManager navigationManager;

    List<RouteData?> history = new List<RouteData?>();

    public event PropertyChangedEventHandler? PropertyChanged;

    public Int32 CurrentHistoryIndex
    {
        get
        {
            if (Int32.TryParse(navigationManager.HistoryEntryState, out var index))
            {
                return index;
            }
            else
            {
                return 0;
            }
        }
    }

    public void EnsureHistorySlot(Int32 i)
    {
        while (history.Count <= i)
        {
            history.Add(default);
        }
    }

    public RouteData SetCurrentRouteData(RouteData? routeData)
    {
        if (routeData == null) return null;

        var i = CurrentHistoryIndex;

        EnsureHistorySlot(i);

        history[i] = routeData;

        return routeData;
    }

    public RouteData? GetPreviousRoutData()
    {
        var i = CurrentHistoryIndex - 1;

        if (i < 0 || i >= history.Count) return null;

        return history[i];
    }

    public HistoricalNavigator(IJSRuntime js, NavigationManager navigationManager)
    {
        this.js = js;
        this.navigationManager = navigationManager;
    }

    [JSInvokable]
    public void Navigate(String url, Boolean replace)
    {
        var i = CurrentHistoryIndex;

        var options = new NavigationOptions
        {
            HistoryEntryState = (replace ? i : i + 1).ToString(),
            ReplaceHistoryEntry = replace
        };
        navigationManager.NavigateTo(url, options);
    }
}
