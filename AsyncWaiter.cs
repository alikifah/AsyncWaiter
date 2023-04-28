// =====================================================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   24.4.2023
//    Description: A state machine, that allows waiting for callback methods in async / await context
// ======================================================================================================


using System;
using System.Threading;
using System.Threading.Tasks;

// the result of the waiting operation
 public class WaiterResponse<T>
    {
        public bool IsTimeOut { get; set; }
        public T Data { get; set; }
    }


    public class AsyncWaiter<T>
    {
        #region private members
        private int timeout_ms;
        private bool isReceived = false;
        private CancellationToken ct;
        private CancellationTokenSource cts;
        private T response;
        #endregion

        #region constructor and destructor
        public AsyncWaiter(int timeout_ms)
        {
            this.timeout_ms = timeout_ms;
            cts = new CancellationTokenSource();
            ct = cts.Token;
        }
        ~AsyncWaiter(){
            cts?.Dispose();
        }
        #endregion

        #region public methods
        // reset the waiter to be reused
        public void Reset()
        {
            if (cts != null) return;
            cts = new CancellationTokenSource();
            ct = cts.Token;
            isReceived = false;
        }

        //  wait for the "Receive" method to be called or for timeout_ms to run out
        public async Task<WaiterResponse<T>> Wait()
        {
            try
            {
                await Task.Delay(timeout_ms, ct);
                // time out
                return new WaiterResponse<T>
                {
                    IsTimeOut = true
                };
            }
            catch (OperationCanceledException)
            {
                return new WaiterResponse<T>
                {
                    Data = response
                };
            }
            finally
            {
                cts?.Dispose();
                cts = null;
            }
        }
        // receive the waited object
        public void Receive(T response)
        {
            if (isReceived) return;
            isReceived = true;

            this.response = response;
            cts?.Cancel();
        }
        #endregion

    }
