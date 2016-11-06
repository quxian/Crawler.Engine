using Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    public abstract class AbstractPipeline<TPreviousResult, UResult> : IPipeline<TPreviousResult, UResult> {
        public event Action OnDispose;
        public event Action<UResult> OnResult;

        public abstract void Extract(TPreviousResult previousResult);

        public IPipeline<TPreviousResult, UResult> NextPipeline<VNextResult>(IPipeline<UResult, VNextResult> nextPipeline) {
            OnResult += nextPipeline.Extract;
            OnDispose += nextPipeline.Dispose;

            return this;
        }

        protected virtual void _onResult(UResult result) {
            OnResult?.Invoke(result);
        }

        public virtual void SavingState() { }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    OnDispose?.Invoke();

                    SavingState();
                }

                OnDispose = null;
                OnResult = null;
                disposedValue = true;
            }
        }

        ~AbstractPipeline() {
            Dispose(true);
        }

        public void Dispose() {
            Dispose(true);
        }
        #endregion
    }
}
