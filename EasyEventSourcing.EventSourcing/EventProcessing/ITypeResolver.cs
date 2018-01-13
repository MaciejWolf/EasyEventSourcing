using System;

namespace EasyEventSourcing.EventSourcing.EventProcessing
{
    public interface ITypeResolver
    {
        Type Lookup(string typeName);
    }
}
