namespace Identification
{
    using System;

    public interface IBatchIdentificator
    {
        IObservable<Identification> Identify(Uri origin, CaptureOptions options);
    }
}