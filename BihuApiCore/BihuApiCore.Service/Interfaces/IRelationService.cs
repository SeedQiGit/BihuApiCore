using System;
using System.Collections.Generic;
using System.Text;

namespace BihuApiCore.Service.Interfaces
{
    public interface IRelationService
    {

        Int64 UserId { get; set; }

        void ReceiveAndWork(long userId);

    }
}
