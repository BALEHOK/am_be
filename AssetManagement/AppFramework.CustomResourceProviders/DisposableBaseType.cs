// Copyright 2004-2006, IDesign
// www.idesign.net

using System;
using System.Collections.Generic;
using System.Text;

namespace AppFramework.CustomResourceProviders
{
    public class DisposableBaseType: IDisposable
    {
        private bool m_Disposed;
        protected bool Disposed
        {
           get
           {
                lock(this)
                {
                    return m_Disposed;
                }
            }
        }

    #region IDisposable Members

        public void Dispose()
        {
            lock (this)
            {
                if (m_Disposed == false)
                {
                    Cleanup();
                    m_Disposed = true;

                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        protected virtual void Cleanup()
        {
            // override to provide cleanup
        } 

        ~DisposableBaseType()   
        {
            Cleanup();
        } 

    }

}
