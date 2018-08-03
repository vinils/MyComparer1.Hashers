namespace MyComparer
{
    using System;
    using System.IO;

    public interface IHasher : IDisposable
    {
        Func<Stream, byte[]> ComputeHash { get; }
    }
}
