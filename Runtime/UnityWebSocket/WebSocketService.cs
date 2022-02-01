using Newtonsoft.Json;
using UnityEngine;
using UnityWebSocket;
using System;

public enum SubscriptionType { NONE, accountSubscribe, accountUnsubscribe };
public class WebSocketService
{
    public SubscriptionType _subscriptionTypeReference;

    private SubscriptionModel _subscriptionModel;
    private UnsubsciptionModel _unsubscriptionModel;
    private IWebSocket _socket;

    public IWebSocket Socket => _socket;

    /// <summary>
    /// Starts a websocket connection to the forwarded address
    /// </summary>
    /// <param name="address">The address to which we will start the connection</param>
    public void StartConnection(string address)
    {
        _socket = new WebSocket(address);
        _socket.OnOpen += OnOpen;
        _socket.OnMessage += OnMessage;
        _socket.OnClose += OnClose;
        _socket.OnError += OnError;
        _socket.ConnectAsync();
    }

    /// <summary>
    /// Close opened connection
    /// </summary>
    public void CloseConnection()
    {
        if (_socket == null) return;

        _socket.CloseAsync();
    }

    /// <summary>
    /// Subscribes wallet to websocket events
    /// </summary>
    /// <param name="pubKey">Pub key of the wallet which want to subscribe to websocket wvents</param>
    public void SubscribeToWalletAccountEvents(string pubKey)
    {
        if (_socket is null) return;

        _subscriptionTypeReference = SubscriptionType.accountSubscribe;
        SendParameter(ReturnSubscribeParameter(pubKey));
    }

    /// <summary>
    /// Unsubscribes wallet from websocket events
    /// </summary>
    public void UnSubscribeToWalletAccountEvents()
    {
        if (_socket is null) return;
        if (_subscriptionModel is null) return;

        _subscriptionTypeReference = SubscriptionType.accountUnsubscribe;
        SendParameter(ReturnUnsubscribeParameter());
        _subscriptionModel = null;
    }

    /// <summary>
    /// Returns error if it happens
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Received message arguments</param>
    private void OnError(object sender, ErrorEventArgs e)
    {
        //todo
    }

    /// <summary>
    /// Returns message that connection is opened
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Received message arguments</param>
    private void OnOpen(object sender, OpenEventArgs e)
    {
        //todo
    }

    /// <summary>
    /// Returns message that connection is closed
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Received message arguments</param>
    private void OnClose(object sender, CloseEventArgs e)
    {
        _subscriptionModel = null;
        _socket = null;
        _subscriptionTypeReference = SubscriptionType.NONE;
        MainThreadDispatcher.Instance().Enqueue(() => { WebSocketActions.CloseWebSocketConnectionAction?.Invoke(); });
    }

    /// <summary>
    /// Function that is called when a message is received from a websocket.
    /// In our case it was done only for account subscription and unsubscription.function needs to be expanded
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Received message arguments</param>
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

    /// <summary>
    /// Function by which we async send a parameter to the websocket
    /// </summary>
    /// <param name="parameter">Parameter to send to websocket</param>
    private void SendParameter(string parameter)
    {
        if (_socket == null) return;

        _socket.SendAsync(parameter);
    }

    /// <summary>
    /// Returns JSONRPC message for account subscription
    /// </summary>
    /// <param name="pubkey">Pub key of account with which we want to subscribe to the websocket</param>
    /// <returns></returns>
    private string ReturnSubscribeParameter(string pubkey)
    {
        if (_socket is null) return null;

        string parameterToSend = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"accountSubscribe\",\"params\":[\"" + pubkey + "\",{\"encoding\":\"jsonParsed\"}]}";
        return parameterToSend;
    }

    /// <summary>
    /// Returns JSONRPC message for account unsubscription
    /// </summary>
    /// <param name="pubkey">Pub key of account with which we want to unsubscribe from the websocket</param>
    /// <returns></returns>
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