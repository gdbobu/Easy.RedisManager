using Easy.Common.MultiThread.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Easy.Common.MultiThread
{
    /// <summary>
    /// 工作线程定义
    /// </summary>
    public class WorkerThread
    {
        #region Delegate Define
        public delegate void ExceptionHandler(Exception e);
        public delegate void AfterLoopHandler();
        #endregion

        #region Event Define
        private ExceptionHandler m_OnExceptionHandler = null;
        #endregion

        #region Variable
        private Thread m_Thread;
        // 将初始状态设置为非终止
        private AutoResetEvent m_AutoResetEvent = new AutoResetEvent(false);
        private WorkerThreadControl m_WorkerThreadControl = new WorkerThreadControl();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onExceptionHandler"></param>
        public WorkerThread(ExceptionHandler onExceptionHandler = null)
        {
            this.m_OnExceptionHandler = onExceptionHandler;
        }
        #endregion

        #region Private Method

        /// <summary>
        /// Exception Handle
        /// </summary>
        /// <param name="e"></param>
        private void DoExceptionHandler(Exception e)
        {
            if (m_OnExceptionHandler != null)
            {
                try
                {
                    m_OnExceptionHandler(e);
                }
                catch { }
            }
        }

        /// <summary>
        /// Stop
        /// </summary>
        private void Stop()
        {
            m_WorkerThreadControl.SetToStop();
            m_AutoResetEvent.Set();
            CloseThread(3000, ref m_Thread);
        }
        #endregion

        #region Public Method

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="threadStart"></param>
        /// <param name="onAfterLoopHandler"></param>
        /// <param name="delayInMs"></param>
        /// <param name="synchStart"></param>
        /// <returns></returns>
        public bool Start(ThreadStart threadStart, AfterLoopHandler onAfterLoopHandler = null,
            int delayInMs = Timeout.Infinite, bool synchStart = false)
        {
            bool blnReturn = false;
            if (threadStart != null && (m_Thread == null || !m_Thread.IsAlive))
            {
                AutoResetEvent evSynch = null;
                if (synchStart)
                    evSynch = new AutoResetEvent(false);

                m_Thread = new Thread(new ThreadStart(() =>
                {
                    if (evSynch != null)
                            evSynch.Set();

                        while (true)
                        {
                            if (delayInMs == Timeout.Infinite || delayInMs > 0)
                                m_AutoResetEvent.WaitOne(delayInMs);

                            if (m_WorkerThreadControl.ShouldProceed)
                            {
                                try
                                {
                                    threadStart();
                                }
                                catch (StopThreadException)
                                {
                                    m_WorkerThreadControl.SetToStop();
                                }
                                catch (Exception e)
                                {
                                    DoExceptionHandler(e);
                                }
                            }
                            else
                                break;
                        }

                        if (onAfterLoopHandler != null)
                        {
                            try
                            {
                                onAfterLoopHandler();
                            }
                            catch (Exception e)
                            {
                                DoExceptionHandler(e);
                            }
                        }
                }));

                m_Thread.Start();

                if (evSynch != null)
                    evSynch.WaitOne();

                blnReturn = true;
            }
            return blnReturn;
        }


        /// <summary>
        /// Stop
        /// </summary>
        /// <param name="workerThread"></param>
        public static void Stop(ref WorkerThread workerThread)
        {
            if (workerThread != null)
            {
                workerThread.Stop();
                workerThread = null;
            }
        }

        /// <summary>
        /// CloseThread
        /// </summary>
        /// <param name="timeoutInMs"></param>
        /// <param name="thread"></param>
        public static void CloseThread(int timeoutInMs, ref Thread thread)
        {
            try
            {
                if (thread != null)
                    if (!thread.Join(timeoutInMs))
                        thread.Abort();
            }
            catch
            {
            }
            finally
            {
                thread = null;
            }
        }

        /// <summary>
        /// SetEvent
        /// </summary>
        public void SetEvent()
        {
            m_AutoResetEvent.Set();
        }

        /// <summary>
        /// SetToStop
        /// </summary>
        public void SetToStop()
        {
            m_WorkerThreadControl.SetToStop();
        }
        /// <summary>
        /// Check the thread is Alive
        /// </summary>
        public bool IsThreadActive
        {
            get
            {
                return m_Thread != null &&
                            (m_Thread.ThreadState == ThreadState.Background ||
                             m_Thread.ThreadState == ThreadState.Running ||
                             m_Thread.ThreadState == ThreadState.WaitSleepJoin);
            }
        }

        #endregion
    }
}
