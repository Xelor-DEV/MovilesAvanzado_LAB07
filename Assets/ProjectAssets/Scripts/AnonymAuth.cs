using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;

public class AnonymAuth : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync(); // inicializar servicios
        Debug.Log(UnityServices.State);
        SetupEvents();

        await SignInAnonymousAsync();
    }

    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Player ID: " + AuthenticationService.Instance.PlayerId);
            Debug.Log("Acess Token: " + AuthenticationService.Instance.AccessToken);
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.Log(err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player Log Out");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player Session Expired");
        };
    }

    private async Task SignInAnonymousAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            /*
            Debug.Log("Sign in Anon Succeded");
            Debug.Log("Player ID: " + AuthenticationService.Instance.PlayerId);
            */
        }
        catch (AuthenticationException err)
        {
            Debug.LogError(err);
        }
        catch (RequestFailedException err)
        {
            Debug.LogError(err);
        }
    }
}
