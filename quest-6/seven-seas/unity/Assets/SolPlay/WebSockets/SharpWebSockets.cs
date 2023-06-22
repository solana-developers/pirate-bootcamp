using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace NativeWebSocket
{
    public class SharpWebSockets : IWebSocket
    {
        public event WebSocketOpenEventHandler OnOpen;
        public event WebSocketMessageEventHandler OnMessage;
        public event WebSocketErrorEventHandler OnError;
        public event WebSocketCloseEventHandler OnClose;

        private WebSocketSharp.WebSocket sharpWebSocket;
        private string websocketUrl;
        private List<byte[]> m_MessageList = new List<byte[]>();
        private readonly object IncomingMessageLock = new object();

        public SharpWebSockets(string uri)
        {
            websocketUrl = uri;
        }
        
        public WebSocketState State
        {
            get
            {
                switch (sharpWebSocket.ReadyState)
                {
                    case WebSocketSharp.WebSocketState.New:
                        return WebSocketState.Connecting;
                    case WebSocketSharp.WebSocketState.Connecting:
                        return WebSocketState.Connecting;
                    case WebSocketSharp.WebSocketState.Open:
                        return WebSocketState.Open;
                    case WebSocketSharp.WebSocketState.Closing:
                        return WebSocketState.Closing;
                    case WebSocketSharp.WebSocketState.Closed:
                        return WebSocketState.Closed;
                    default:
                        return WebSocketState.Closed;
                }
            }
        }

        public Task Connect()
        {
            sharpWebSocket = new WebSocketSharp.WebSocket(websocketUrl);
            
            sharpWebSocket.OnOpen += (sender, args) =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(OnOpenMainThread);
                Debug.Log("message");
            };
            sharpWebSocket.OnMessage += (sender, args) =>
            {
                m_MessageList.Add(args.RawData);
            };
            sharpWebSocket.OnClose += (sender, args) =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(OnCloseMainThread);
            };
            sharpWebSocket.OnError += (sender, args) =>
            {
                OnError?.Invoke(args.Message);
                Debug.LogError(args.Message);
            };

            sharpWebSocket.ConnectAsync();
            return Task.CompletedTask;
        }

        private void OnCloseMainThread()
        {
            OnClose?.Invoke(WebSocketCloseCode.Abnormal);
        }

        private void OnOpenMainThread()
        {
            OnOpen?.Invoke();
        }

        public Task Close()
        {
            sharpWebSocket.Close();
            return Task.CompletedTask;
        }

        public Task Send(byte[] bytes)
        {
            sharpWebSocket.Send(bytes);
            return Task.CompletedTask;
        }

        public void DispatchMessageQueue()
        {
            if (m_MessageList.Count == 0)
            {
                return;
            }

            List<byte[]> messageListCopy;

            lock (IncomingMessageLock)
            {
                messageListCopy = new List<byte[]>(m_MessageList);
                m_MessageList.Clear();
            }

            var len = messageListCopy.Count;
            for (int i = 0; i < len; i++)
            {
                OnMessage?.Invoke(messageListCopy[i]);
            }
        }
    }
}