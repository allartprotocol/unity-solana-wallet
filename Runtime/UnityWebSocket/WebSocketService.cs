using Newtonsoft.Json;
using UnityEngine;
using UnityWebSocket;
using System;

public enum SubscriptionType { NONE, accountSubscribe, accountUnsubscribe };
public class WebSocketService : MonoBehaviour
{
    public SubscriptionType _subscriptionTypeReference;

    private SubscriptionModel _subscriptionModel;
    private UnsubsciptionModel _unsubscriptionModel;
    private IWebSocket _socket;

    public IWebSocket Socket => _socket;
    public void StartConnection(string address)
    {
        _socket = new WebSocket(address);
        _socket.OnOpen += OnOpen;
        _socket.OnMessage += OnMessage;
        _socket.OnClose += OnClose;
        _socket.OnError += OnError;
        _socket.ConnectAsync();
    }

    public void CloseConnection()
    {
        if (_socket == null) return;

        _socket.CloseAsync();
    }

    public void SubscribeToWalletAccountEvents(string pubKey)
    {
        if (_socket is null) return;

        _subscriptionTypeReference = SubscriptionType.accountSubscribe;
        SendParameter(ReturnSubscribeParameter(pubKey));
    }

    public void UnSubscribeToWalletAccountEvents()
    {
        if (_socket is null) return;
        if (_subscriptionModel is null) return;

        _subscriptionTypeReference = SubscriptionType.accountUnsubscribe;
        SendParameter(ReturnUnsubscribeParameter());
        _subscriptionModel = null;
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        //todo
    }

    private void OnOpen(object sender, OpenEventArgs e)
    {
        //todo
    }

    private void OnClose(object sender, CloseEventArgs e)
    {
        _subscriptionModel = null;
        _socket = null;
        _subscriptionTypeReference = SubscriptionType.NONE;
        MainThreadDispatcher.Instance().Enqueue(() => { WebSocketActions.CloseWebSocketConnectionAction?.Invoke(); });      
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        switch (_subscriptionTypeReference)
        {
            case SubscriptionType.accountSubscribe:
                try
                {
                    _subscriptionModel = JsonConvert.DeserializeObject<SubscriptionModel>(e.Data);
                    MainThreadDispatcher.Instance().Enqueue(() => { WebSocketActions.WebSocketAccountSubscriptionAction.Invoke(true); });
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                break;
            case SubscriptionType.accountUnsubscribe:
                try
                {
                    _unsubscriptionModel = JsonConvert.DeserializeObject<UnsubsciptionModel>(e.Data);
                    MainThreadDispatcher.Instance().Enqueue(() => { WebSocketActions.WebSocketAccountSubscriptionAction.Invoke(false); });
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                break;
        }
    }

    private void SendParameter(string parameter)
    {
        if (_socket == null) return;

        _socket.SendAsync(parameter);
    }

    private string ReturnSubscribeParameter(string pubkey)
    {
        if (_socket is null) return null;

        string parameterToSend = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"accountSubscribe\",\"params\":[\"" + pubkey + "\",{\"encoding\":\"jsonParsed\"}]}";
        return parameterToSend;
    }

    private string ReturnUnsubscribeParameter()
    {
        if (_socket is null) return null;

        string unsubscribeParameter = "{\"jsonrpc\":\"2.0\", \"id\":1, \"method\":\"accountUnsubscribe\", \"params\":[" + _subscriptionModel.result + "]}";
        return unsubscribeParameter;
    }
}

public class SubscriptionModel
{
    public string jsonrpc { get; set; }
    public int result { get; set; }
    public int id { get; set; }
}
public class UnsubsciptionModel
{
    public string jsonrpc { get; set; }
    public bool result { get; set; }
    public int id { get; set; }
}