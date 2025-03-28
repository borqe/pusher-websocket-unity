using System;
using System.Threading.Tasks;
using PusherClient;
using UnityEngine;

public class PusherManager : MonoBehaviour
{
    // A mutation of https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial/writing-game-manager
    public static PusherManager instance = null;
    protected Pusher _pusher;
    protected Channel _channel;
    private const string APP_KEY = "APP_KEY";
    private const string APP_CLUSTER = "APP_CLUSTER";

    async Task Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        await InitialisePusher();
        Console.WriteLine("Starting");
    }

    protected async Task InitialisePusher()
    {
        //Environment.SetEnvironmentVariable("PREFER_DNS_IN_ADVANCE", "true");

        if (_pusher == null && (APP_KEY != "APP_KEY") && (APP_CLUSTER != "APP_CLUSTER"))
        {
            _pusher = new Pusher(APP_KEY, new PusherOptions()
            {
                Cluster = APP_CLUSTER,
                Encrypted = true
            });

            _pusher.Error += OnPusherOnError;
            _pusher.ConnectionStateChanged += PusherOnConnectionStateChanged;
            _pusher.Connected += PusherOnConnected;
            _channel = await _pusher.SubscribeAsync("my-channel");
			_pusher.Subscribed += OnChannelOnSubscribed;
            await _pusher.ConnectAsync();
        }
        else
        {
            Debug.LogError("APP_KEY and APP_CLUSTER must be correctly set. Find how to set it at https://dashboard.pusher.com");
        }
    }

    protected void PusherOnConnected(object sender)
    {
        Debug.Log("Connected");
        _channel.Bind("my-event", (dynamic data) =>
        {
            Debug.Log("my-event received");
        });
    }

    protected void PusherOnConnectionStateChanged(object sender, ConnectionState state)
    {
        Debug.Log("Connection state changed");
    }

    protected void OnPusherOnError(object s, PusherException e)
    {
        Debug.Log("Errored");
    }

    protected void OnChannelOnSubscribed(object s, Channel channel)
    {
        Debug.Log("Subscribed");
    }

    async Task OnApplicationQuit()
    {
        if (_pusher != null)
        {
            await _pusher.DisconnectAsync();
        }
    }
}
