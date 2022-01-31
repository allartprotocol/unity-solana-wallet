using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WebSocketActions
{
    /// <summary>
    /// <param bool> if bool is true then subscribe otherwise unsubscribe</param>
    /// </summary>
    public static Action<bool> WebSocketAccountSubscriptionAction;
    public static Action CloseWebSocketConnectionAction;
}
