using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Frictionless;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using Solana.Unity.Wallet;
using WebSocket = NativeWebSocket.WebSocket;
using WebSocketState = NativeWebSocket.WebSocketState;

namespace SolPlay.Scripts.Services
{
    public class SolPlayWebSocketService : MonoBehaviour
    {
        IWebSocket websocket;

        private Dictionary<PublicKey, SocketSubscription> subscriptions =
            new Dictionary<PublicKey, SocketSubscription>();

        private int reconnectTries;
        private int subcriptionCounter;
        public string socketUrl;

        private class SocketSubscription
        {
            public long Id;
            public long SubscriptionId;
            public Action<MethodResult> ResultCallback;
            public PublicKey PublicKey;
        }

        [Serializable]
        public class WebSocketErrorResponse
        {
            public string jsonrpc;
            public string error;
            public string data;
        }

        [Serializable]
        public class WebSocketResponse
        {
            public string jsonrpc;
            public long result;
            public long id;
        }

        [Serializable]
        public class WebSocketMethodResponse
        {
            public string jsonrpc;
            public string method;
            public MethodResult @params;
        }

        [Serializable]
        public class MethodResult
        {
            public AccountInfo result;
            public long subscription;
        }

        [Serializable]
        public class AccountInfo
        {
            public Context context;
            public Value value;
        }

        [Serializable]
        public class Context
        {
            public int slot;
        }

        [Serializable]
        public class Value
        {
            public long lamports;
            public List<string> data;
            public string owner;
            public bool executable;
            public BigInteger rentEpoch;
        }

        private void Awake()
        {
            ServiceFactory.RegisterSingleton(this);
        }

        private void OnApplicationFocus(bool focus)
        {
            Debug.Log("Has focus" + focus);
        }

        public WebSocketState GetState()
        {
            if (websocket == null)
            {
                return WebSocketState.Closed;
            }

            return websocket.State;
        }

        private void SetSocketUrl(string rpcUrl)
        {
            socketUrl = rpcUrl.Replace("https://", "wss://");
            if (socketUrl.Contains("localhost"))
            {
                socketUrl = "ws://localhost:8900";
            }
            Debug.Log("Socket url: " + socketUrl);
        }

        public void Connect(string rpcUrl)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
            
            if (websocket != null)
            {
                websocket.OnOpen -= websocketOnOnOpen();
                websocket.OnError -= websocketOnOnError();
                websocket.OnClose -= OnWebSocketClosed;
                websocket.OnMessage -= websocketOnOnMessage();
                websocket.Close();
            }

            SetSocketUrl(rpcUrl);
            Debug.Log("Connect Socket: " + socketUrl);
            
#if UNITY_WEBGL 
            websocket = new WebSocket(socketUrl);
#else 
            websocket = new SharpWebSockets(socketUrl);
#endif

            websocket.OnOpen += websocketOnOnOpen();
            websocket.OnError += websocketOnOnError();
            websocket.OnClose += OnWebSocketClosed;
            websocket.OnMessage += websocketOnOnMessage();
            websocket.Connect();
            
            Debug.Log("Socket connect done");
        }

        private WebSocketMessageEventHandler websocketOnOnMessage()
        {
            return (bytes) =>
            {
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                //Debug.Log("SocketMessage:" + message);
                WebSocketErrorResponse errorResponse = JsonConvert.DeserializeObject<WebSocketErrorResponse>(message);
                if (!string.IsNullOrEmpty(errorResponse.error))
                {
                    Debug.LogError(errorResponse.error);
                    return;
                }

                if (message.Contains("method"))
                {
                    WebSocketMethodResponse methodResponse =
                        JsonConvert.DeserializeObject<WebSocketMethodResponse>(message);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.Value.SubscriptionId == methodResponse.@params.subscription)
                        {
                            subscription.Value.ResultCallback(methodResponse.@params);
                        }
                    }
                }
                else
                {
                    WebSocketResponse response = JsonConvert.DeserializeObject<WebSocketResponse>(message);

                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.Value.Id == response.id)
                        {
                            subscription.Value.SubscriptionId = response.result;
                        }
                    }
                }
            };
        }

        private static WebSocketErrorEventHandler websocketOnOnError()
        {
            return (e) =>
            {
                Debug.LogError("Socket Error! " + e + " maybe you need to use a different RPC node. For example helius or quicknode");
            };
        }

        private WebSocketOpenEventHandler websocketOnOnOpen()
        {
            return () =>
            {
                reconnectTries = 0;
                foreach (var subscription in subscriptions)
                {
                    SubscribeToPubKeyData(subscription.Value.PublicKey, subscription.Value.ResultCallback);
                }

                Debug.Log("Socket Connection open!");
                MessageRouter.RaiseMessage(new SocketServerConnectedMessage());
            };
        }

        private void OnWebSocketClosed(WebSocketCloseCode closecode)
        {
            Debug.Log("Socket disconnect: " + closecode);
            if (this)
            {
                StartCoroutine(Reconnect());
            }
        }

        public IEnumerator Reconnect()
        {
            while (true)
            {
                yield return new WaitForSeconds(reconnectTries * 0.5f + 0.1f);
                reconnectTries++;
                Debug.Log("Reconnect Socket");
                Connect(socketUrl);
                while (websocket == null || websocket.State == WebSocketState.Closed)
                {
                    yield return null;
                }
                while (websocket.State == WebSocketState.Connecting)
                {
                    yield return null;
                }
                while (websocket.State == WebSocketState.Closed || websocket.State == WebSocketState.Closing)
                {
                    yield break;
                }

                if (websocket.State == WebSocketState.Open)
                {
                    yield break;
                }
            }
        }

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                websocket.DispatchMessageQueue();
            }
#endif
        }

        public async void SubscribeToBlocks()
        {
            if (websocket.State == WebSocketState.Open)
            {
                string accountSubscribeParams ="{ \"jsonrpc\": \"2.0\", \"id\": \"22\", \"method\": \"blockSubscribe\", \"params\": [\"all\"] }";
                await websocket.Send(System.Text.Encoding.UTF8.GetBytes(accountSubscribeParams));
            }
        }

        public async void SubscribeToPubKeyData(PublicKey pubkey, Action<MethodResult> onMethodResult)
        {
            var subscriptionsCount = subcriptionCounter++;
            var socketSubscription = new SocketSubscription()
            {
                Id = subscriptionsCount,
                SubscriptionId = 0,
                ResultCallback = onMethodResult,
                PublicKey = pubkey
            };
            if (subscriptions.ContainsKey(pubkey))
            {
                subscriptions[pubkey].Id = subscriptionsCount;
            }
            else
            {
                subscriptions.Add(pubkey, socketSubscription);
            }
            
            if (websocket.State == WebSocketState.Open)
            {
                string accountSubscribeParams =
                    "{\"jsonrpc\":\"2.0\",\"id\":" + subscriptionsCount +
                    ",\"method\":\"accountSubscribe\",\"params\":[\"" + pubkey.Key +
                    "\",{\"encoding\":\"jsonParsed\",\"commitment\":\"confirmed\"}]}";
                await websocket.Send(System.Text.Encoding.UTF8.GetBytes(accountSubscribeParams));
            }
        }

        async void UnSubscribeFromPubKeyData(PublicKey pubkey, long id)
        {
            if (websocket.State == WebSocketState.Open)
            {
                string unsubscribeParameter = "{\"jsonrpc\":\"2.0\", \"id\":" + id +
                                              ", \"method\":\"accountUnsubscribe\", \"params\":[" + pubkey.Key + "]}";
                await websocket.Send(System.Text.Encoding.UTF8.GetBytes(unsubscribeParameter));
            }
        }

        private async void OnApplicationQuit()
        {
            if (websocket == null)
            {
                return;
            }

            await websocket.Close();
        }
    }

    public class SocketServerConnectedMessage
    {
    }
}