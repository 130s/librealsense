﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Intel.RealSense
{
    public class FrameQueue : IDisposable
    {
        internal HandleRef m_instance;

        public FrameQueue(int capacity = 1)
        {
            object error;
            Capacity = capacity;
            m_instance = new HandleRef(this, NativeMethods.rs2_create_frame_queue(capacity, out error));
        }

        public readonly int Capacity;
         
        public bool PollForFrame<T>(out T frame) where T : Frame
        {
            object error;
            IntPtr ptr;
            if (NativeMethods.rs2_poll_for_frame(m_instance.Handle, out ptr, out error) > 0)
            {
                frame = Frame.Create<T>(ptr);
                return true;
            }
            frame = null;
            return false;
        }

        public Frame WaitForFrame(uint timeout_ms = 5000)
        {
            return WaitForFrame<Frame>(timeout_ms);
        }

        public T WaitForFrame<T>(uint timeout_ms = 5000) where T : Frame
        {
            object error;
            var ptr = NativeMethods.rs2_wait_for_frame(m_instance.Handle, timeout_ms, out error);
            return Frame.Create<T>(ptr);
        }

        public bool TryWaitForFrame<T>(out T frame, uint timeout_ms = 5000) where T : Frame
        {
            object error;
            IntPtr ptr;
            bool res = NativeMethods.rs2_try_wait_for_frame(m_instance.Handle, timeout_ms, out ptr, out error) > 0;
            frame = res ? Frame.Create<T>(ptr) : null;
            return res;
        }

        public FrameSet WaitForFrames(uint timeout_ms = 5000)
        {
            object error;
            var ptr = NativeMethods.rs2_wait_for_frame(m_instance.Handle, timeout_ms, out error);
            return FrameSet.Create(ptr);
        }

        public void Enqueue(Frame f)
        {
            object error;
            NativeMethods.rs2_frame_add_ref(f.m_instance.Handle, out error);
            NativeMethods.rs2_enqueue_frame(f.m_instance.Handle, m_instance.Handle);
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                NativeMethods.rs2_delete_frame_queue(m_instance.Handle);

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~FrameQueue()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
