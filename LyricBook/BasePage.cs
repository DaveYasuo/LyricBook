using LyricBook.Core;

namespace LyricBook;

public class BasePage : ContentPage
{
    public BasePage()
    {
        NavigationPage.SetBackButtonTitle(this, "Back");
        if (DeviceInfo.Idiom == DeviceIdiom.Watch)
            NavigationPage.SetHasNavigationBar(this, false);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        SetupBinding(BindingContext);
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        TearDownBinding(BindingContext);
    }

    protected void SetupBinding(object bindingContext)
    {
        if (bindingContext is not BaseViewModel vm) return;
        vm.DoDisplayAlert += OnDisplayAlert;
        vm.DoNavigate += OnNavigate;
        vm.OnAppearing();
    }

    protected void TearDownBinding(object bindingContext)
    {
        if (bindingContext is not BaseViewModel vm) return;
        vm.OnDisappearing();
        vm.DoDisplayAlert -= OnDisplayAlert;
        vm.DoNavigate -= OnNavigate;
    }

    private Task OnDisplayAlert(string message)
    {
        return DisplayAlert(Title, message, "OK");
    }

    private Task OnNavigate(BaseViewModel vm, bool showModal)
    {
        var name = vm.GetType().Name;
        name = name.Replace("ViewModel", "Page", StringComparison.Ordinal);

        var ns = GetType().Namespace;
        var pageType = Type.GetType($"{ns}.{name}");

        var page = (BasePage)Activator.CreateInstance(pageType);
        page.BindingContext = vm;

        return showModal
            ? Navigation.PushModalAsync(page)
            : Navigation.PushAsync(page);
    }
}