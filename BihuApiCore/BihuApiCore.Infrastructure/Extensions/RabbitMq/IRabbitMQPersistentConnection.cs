using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BihuApiCore.Infrastructure.Extensions.RabbitMq
{
    /// <summary>
    /// RabbitMQ持久化连接
    /// </summary>
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        IModel CreateModel();
        bool TryConnect();
    }
}
