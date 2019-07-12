using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;


namespace BihuApiCore.Infrastructure.Extensions.RabbitMq
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;

		private readonly ILogger<RabbitMQPersistentConnection> _logger;

		private readonly int _retryCount;

		private IConnection _connection;

		private bool _disposed;

		private object sync_root = new object();

		public bool IsConnected
		{
			get
			{
				return this._connection != null && this._connection.IsOpen && !this._disposed;
			}
		}

		public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<RabbitMQPersistentConnection> logger, int retryCount = 5)
		{
			if (connectionFactory == null)
			{
				throw new ArgumentNullException("connectionFactory");
			}
			this._connectionFactory = connectionFactory;
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			this._logger = logger;
			this._retryCount = retryCount;
		}

		public IModel CreateModel()
		{
			if (!this.IsConnected)
			{
				throw new InvalidOperationException("redis没有可用连接可以执行创建model的任务");
			}
			return this._connection.CreateModel();
		}

		public void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			this._disposed = true;
			try
			{
				this._connection.Dispose();
			}
			catch (IOException ex)
			{
				LoggerExtensions.LogCritical(this._logger, ex.ToString(), Array.Empty<object>());
			}
		}

		public bool TryConnect()
		{
			LoggerExtensions.LogInformation(this._logger, "RabbitMQ客户端正在尝试连接", Array.Empty<object>());
			object obj = this.sync_root;
			bool result;
			lock (obj)
			{
				
			}
			return true;
		}

		private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
		{
			if (this._disposed)
			{
				return;
			}
			LoggerExtensions.LogWarning(this._logger, "A RabbitMQ connection is shutdown. Trying to re-connect...", Array.Empty<object>());
			this.TryConnect();
		}

		private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
		{
			if (this._disposed)
			{
				return;
			}
			LoggerExtensions.LogWarning(this._logger, "A RabbitMQ connection throw exception. Trying to re-connect...", Array.Empty<object>());
			this.TryConnect();
		}

		private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
		{
			if (this._disposed)
			{
				return;
			}
			LoggerExtensions.LogWarning(this._logger, "A RabbitMQ connection is on shutdown. Trying to re-connect...", Array.Empty<object>());
			this.TryConnect();
		}
    }
}
