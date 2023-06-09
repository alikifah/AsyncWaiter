# AsyncWaiter

A state machine class, that allows waiting for callback methods in async / await context. This state machine acts also similar to AutoResetEvent but in async style(awaitable AutoResetEvent).


## Example of use

```c#

public static class program
{
    // max. time to wait for the response
    const int MAX_TIME_TO_WAIT_MS = 2000;

    // create a waiter, that waits max. 2 seconds for the response
    static AsyncWaiter<int> waiter = new AsyncWaiter<int>(MAX_TIME_TO_WAIT_MS);

    // this method mimics sending a request, waiting for response and invoking a callback when response arrives
    public static void RequestValue(Action<int> onResulCallback)
    {
        Random random = new Random();

        // random delay before sending the response back
        Thread.Sleep(random.Next(3000));

        // response comes 
        onResulCallback?.Invoke(200);
    }


    // call back to be invoked, when response comes
    static void onvalueReceived(int result)
    {
        // we must call this method and supply it with response
        waiter.Receive(result);
    }

    public static async Task Main()
    {
        // first, we call the method supplied with the callback asynchronously in order not to block the main thread
        _ = Task.Run( ()=>  RequestValue(onvalueReceived));

        // then we wait for the "waiter.Receive" to be called or for MAX_TIME_TO_WAIT_MS to run out
        var result = await waiter.Wait();

        // desplay result
        if (!result.IsTimeOut)
            Console.WriteLine(result.Data);
        else
            Console.WriteLine("Time out!");

    }
}
```
