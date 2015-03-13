using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Common
{
    public interface ITracer
    {
        void WriteDebug(string error);
        void WriteDebug(string format, params object[] args);
        void WriteWarning(string warning);
        void WriteWarning(string format, params object[] args);

        void WriteError(Exception ex);
        void WriteError(string error);
        void WriteError(string format, params object[] args);
    }
}
