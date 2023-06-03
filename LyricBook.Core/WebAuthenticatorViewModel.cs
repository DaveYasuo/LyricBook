using System.Windows.Input;

namespace LyricBook.Core;

public class WebAuthenticatorViewModel : BaseViewModel
{
    private const string AuthenticationUrl = "https://xamarin-essentials-auth-sample.azurewebsites.net/mobileauth/";

    private string _accessToken = string.Empty;

    public WebAuthenticatorViewModel()
    {
        MicrosoftCommand = new Command(() => OnAuthenticate("Microsoft"));
        GoogleCommand = new Command(() => OnAuthenticate("Google"));
        FacebookCommand = new Command(() => OnAuthenticate("Facebook"));
        AppleCommand = new Command(() => OnAuthenticate("Apple"));
    }

    public ICommand MicrosoftCommand { get; }

    public ICommand GoogleCommand { get; }

    public ICommand FacebookCommand { get; }

    public ICommand AppleCommand { get; }

    public string AuthToken
    {
        get => _accessToken;
        set => SetProperty(ref _accessToken, value);
    }

    private async void OnAuthenticate(string scheme)
    {
        try
        {
            WebAuthenticatorResult r;

            if (scheme.Equals("Apple", StringComparison.Ordinal)
                && DeviceInfo.Platform == DevicePlatform.iOS
                && DeviceInfo.Version.Major >= 13)
            {
                // Make sure to enable Apple Sign In in both the
                // entitlements and the provisioning profile.
                var options = new AppleSignInAuthenticator.Options
                {
                    IncludeEmailScope = true,
                    IncludeFullNameScope = true
                };
                r = await AppleSignInAuthenticator.AuthenticateAsync(options);
            }
            else
            {
                var authUrl = new Uri(AuthenticationUrl + scheme);
                var callbackUrl = new Uri("xamarinessentials://");

                r = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
            }

            AuthToken = string.Empty;
            if (r.Properties.TryGetValue("name", out var name) && !string.IsNullOrEmpty(name))
                AuthToken += $"Name: {name}{Environment.NewLine}";
            if (r.Properties.TryGetValue("email", out var email) && !string.IsNullOrEmpty(email))
                AuthToken += $"Email: {email}{Environment.NewLine}";
            AuthToken += r.AccessToken ?? r.IdToken;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Login canceled.");

            AuthToken = string.Empty;
            await DisplayAlertAsync("Login canceled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");

            AuthToken = string.Empty;
            await DisplayAlertAsync($"Failed: {ex.Message}");
        }
    }
}