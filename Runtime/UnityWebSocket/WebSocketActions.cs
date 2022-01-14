using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WebSocketActions
{
    public static Action<bool> WebSocketAccountSubscriptionAction;
    public static Action CloseWebSocketConnectionAction;
    ///// <summary>
    ///// param int is subscription id
    ///// </summary>
    //public static Action<int> WebSocketAccountSubscriptionAction;

    ///// <summary>
    ///// param int subscription id
    ///// </summary>
    //public static Action<int> WebsocketAccountUnsubscriptionAction;
}
