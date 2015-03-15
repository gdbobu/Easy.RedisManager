using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStringSerializer
    {
        To DeserializeFromString<To>(string serializedText);
        object DeserializeFromString(string serializedText, Type type);
        string SerializeToString<TFrom>(TFrom from);
    }
}
