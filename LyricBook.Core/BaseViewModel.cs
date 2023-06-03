namespace LyricBook.Core;

public class BaseViewModel : ObservableObject
{
    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value, onChanged: () => OnPropertyChanged(nameof(IsNotBusy)));
    }

    public bool IsNotBusy => !IsBusy;

    public virtual void OnAppearing()
    {
    }

    public virtual void OnDisappearing()
    {
    }

    public event Func<string, Task> DoDisplayAlert;

    public event Func<BaseViewModel, bool, Task> DoNavigate;

    public Task DisplayAlertAsync(string message)
    {
        return DoDisplayAlert?.Invoke(message) ?? Task.CompletedTask;
    }

    public Task NavigateAsync(BaseViewModel vm, bool showModal = false)
    {
        return DoNavigate?.Invoke(vm, showModal) ?? Task.CompletedTask;
    }
}