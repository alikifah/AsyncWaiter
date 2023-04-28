// =======================================================================================
//    Author: Al-Khafaji, Ali Kifah
//    Date:   20.4.2023
//    Description: A state machine that allows waiting for callbacks in await / async style 
// =======================================================================================

using System;
using System.Threading;
using System.Threading.Tasks;

    public class WaiterResponse<T>
    {
        public bool IsTimeOut { get; set; }
        public T Data { get; set; }
    }

    public class AsyncWaiter<T>
    {
        private int timeOut;
        private bool isReceived = false;
        private CancellationToken ct;
        private CancellationTokenSource cts;
        private T response;

        public AsyncWaiter(int timeOut)
        {
            this.timeOut = timeOut;
        }
        public async Task<WaiterResponse<T>> Wait()
        {
            cts = new CancellationTokenSource();
            ct = cts.Token;
            try
            {
                await Task.Delay(timeOut, ct);
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
                cts.Dispose();
            }
        }
        public void Receive(T response)
        {
            if (isReceived) return;
            isReceived = true;

            this.response = response;
            cts.Cancel();
        }

    }
