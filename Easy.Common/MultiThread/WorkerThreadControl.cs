using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Easy.Common.MultiThread
{
    public class WorkerThreadControl
    {
        private int m_Proceed = 0;

        public bool ShouldProceed
        {
            get { return Interlocked.CompareExchange(ref m_Proceed, 0, 0) == 0; }
        }

        public void SetToProceed()
        {
            Interlocked.Exchange(ref m_Proceed, 0);
        }

        public void SetToStop()
        {
            Interlocked.Exchange(ref m_Proceed, -1);
        }
    }
}
