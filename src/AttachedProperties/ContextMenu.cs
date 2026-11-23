using System.Windows.Input;

namespace The49.Maui.ContextMenu;

public static partial class ContextMenu
{
    public static readonly BindableProperty ClickCommandProperty
        = BindableProperty.CreateAttached(
            "ClickCommand",
            typeof(ICommand),
            typeof(VisualElement),
            null,
            propertyChanged: ClickCommandChanged);

    public static readonly BindableProperty ClickCommandParameterProperty
        = BindableProperty.CreateAttached(
            "ClickCommandParameter",
            typeof(object),
            typeof(VisualElement),
            null);

    public static readonly BindableProperty MenuProperty
        = BindableProperty.CreateAttached(
            "Menu",
            typeof(DataTemplate),
            typeof(VisualElement),
            null,
            propertyChanged: MenuChanged);

    public static readonly BindableProperty PreviewProperty
       = BindableProperty.CreateAttached(
           "Preview",
           typeof(Preview),
           typeof(VisualElement),
           null);

    public static readonly BindableProperty ShowMenuOnClickProperty
       = BindableProperty.CreateAttached(
           "ShowMenuOnClick",
           typeof(bool),
           typeof(VisualElement),
           false,
           propertyChanged: ShowMenuOnClickChanged);

    static void MenuChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement visualElement)
        {
            return;
        }

        // MAUI can replace a view's handler (and its native view) at any time, e.g. when a page or the
        // map is rebuilt: follow every handler change, not only the first one, or the menu is lost.
        visualElement.HandlerChanged -= MenuHostHandlerChanged;
        if (newValue != null)
        {
            visualElement.HandlerChanged += MenuHostHandlerChanged;
        }

        if (visualElement.Handler == null)
        {
            // set up by MenuHostHandlerChanged once the native view exists
            return;
        }

        if (oldValue != null)
        {
            DisposeMenu(visualElement);
        }
        if (newValue != null)
        {
            SetupMenu(visualElement);
        }
    }

    static void MenuHostHandlerChanged(object sender, EventArgs e)
    {
        var visualElement = (VisualElement)sender;

        // the native view is gone or new: state kept for the previous one no longer applies
        ResetMenuPlatformState(visualElement);

        if (visualElement.Handler != null && GetMenu(visualElement) != null)
        {
            SetupMenu(visualElement);
        }
    }

    static void ClickCommandChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement visualElement)
        {
            return;
        }

        visualElement.HandlerChanged -= ClickHostHandlerChanged;
        if (newValue != null)
        {
            visualElement.HandlerChanged += ClickHostHandlerChanged;
        }

        if (visualElement.Handler == null)
        {
            return;
        }

        if (oldValue == null && newValue != null)
        {
            SetupClickCommand(visualElement);
        }
        if (oldValue != null && newValue == null)
        {
            DisposeClickCommand(visualElement);
        }
    }

    static void ClickHostHandlerChanged(object sender, EventArgs e)
    {
        var visualElement = (VisualElement)sender;

        ResetClickPlatformState(visualElement);

        if (visualElement.Handler != null && GetClickCommand(visualElement) != null)
        {
            SetupClickCommand(visualElement);
        }
    }

    static void ShowMenuOnClickChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var menu = GetMenu(bindable);
        if (menu != null && (bindable as VisualElement)?.Handler != null)
        {
            DisposeMenu(bindable);
            SetupMenu(bindable);
        }
    }

    static partial void SetupMenu(BindableObject bindable);
    static partial void ResetMenuPlatformState(VisualElement visualElement);
    static partial void ResetClickPlatformState(VisualElement visualElement);
    static partial void DisposeMenu(BindableObject bindable);
    static partial void SetupClickCommand(BindableObject bindable);
    static partial void DisposeClickCommand(BindableObject bindable);


    public static ICommand GetClickCommand(BindableObject view)
    {
        return (ICommand)view.GetValue(ClickCommandProperty);
    }

    public static void SetClickCommand(BindableObject view, ICommand value)
    {
        view.SetValue(ClickCommandProperty, value);
    }

    public static DataTemplate GetMenu(BindableObject view)
    {
        return (DataTemplate)view.GetValue(MenuProperty);
    }

    public static void SetMenu(BindableObject view, DataTemplate value)
    {
        view.SetValue(MenuProperty, value);
    }
    public static Preview GetPreview(BindableObject view)
    {
        return (Preview)view.GetValue(PreviewProperty);
    }

    public static void SetPreview(BindableObject view, Preview value)
    {
        view.SetValue(PreviewProperty, value);
    }

    public static object GetClickCommandParameter(BindableObject view)
    {
        return view.GetValue(ClickCommandParameterProperty);
    }

    public static void SetClickCommandParameter(BindableObject view, object value)
    {
        view.SetValue(ClickCommandParameterProperty, value);
    }

    public static bool GetShowMenuOnClick(BindableObject view)
    {
        return (bool)view.GetValue(ShowMenuOnClickProperty);
    }

    public static void SetShowMenuOnClick(BindableObject view, bool value)
    {
        view.SetValue(ShowMenuOnClickProperty, value);
    }

    public static void ExecuteClickCommand(BindableObject bindable, object defaultValue)
    {
        var command = GetClickCommand(bindable);
        var commandParameter = GetClickCommandParameter(bindable);

        command?.Execute(commandParameter ?? defaultValue);
    }
}
