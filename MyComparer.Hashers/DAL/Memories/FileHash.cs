namespace MyComparer.Hashers.DAL.Memories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using MyListDAL = MyList.DAL;

    public class FileHash : MyListDAL.Memory<Entities.FileHash>, 
        IEnumerable<FileInfo>, 
        IEnumerable<MyListDAL.Entities.File>, 
        IFileHash
    {
        protected FileHash(Guid processId, IEnumerable<Entities.FileHash> fileHashes)
            : base(processId, fileHashes)
        { }

        public FileHash(IEnumerable<Entities.FileHash> fileHashes)
            : base(Guid.NewGuid(), fileHashes)
        { }

        public FileHash()
        { }

        public FileHash(Guid processId)
            : base(processId)
        { }

        public void Add(
            Action<Action<Entities.FileHash>, Action<Exception>> list, 
            Action<Exception> enqueueException, 
            out FileHash newFileHashes)
        {
            Add(list, enqueueException, out IEnumerable<Entities.FileHash> fileHashesEnumerable);
            newFileHashes = new FileHash(fileHashesEnumerable);
        }

        public IEnumerable<MyList.DAL.Entities.File> GetFiles()
            => ((IEnumerable<Entities.FileHash>)this).Select(f => (MyList.DAL.Entities.File)f);

        public IEnumerable<Entities.FileHash> Equals(Entities.FileHash fileHash)
            => _items.Where(hash => hash == fileHash);

        public IEnumerable<Entities.FileHash> Equals(MyListDAL.Entities.File file, IHasher hahserInstance)
        {
            var fileHash = Entities.FileHash.Cast(file, hahserInstance);
            return Equals(fileHash);
        }

        public IFileHash Duplicates()
        {
            var enumerable = _items
                .GroupBy(fHash => fHash)
                .Select(g => g.Key);

            return new FileHash(enumerable);
        }

        public IFileHash Distincts()
        {
            var enumerable = _items.Distinct();

            return new FileHash(enumerable);
        }

        public FileHash ToMemory()
        {
            return this;
        }

        public IEnumerable<Entities.FileHash> Where(Func<Entities.FileHash, int, bool> predicate)
            => _items.Where(predicate);

        public IEnumerable<Entities.FileHash> Where(Func<Entities.FileHash, bool> predicate)
            => _items.Where(predicate);

        public IEnumerable<IGrouping<TKey, Entities.FileHash>> GroupBy<TKey>(Func<Entities.FileHash, TKey> keySelector)
            => _items.GroupBy(keySelector);

        IEnumerator<FileInfo> IEnumerable<FileInfo>.GetEnumerator()
            => ((IEnumerable<FileInfo>)_items).GetEnumerator();

        IEnumerator<MyListDAL.Entities.File> IEnumerable<MyListDAL.Entities.File>.GetEnumerator()
        {
            foreach (var file in this)
                yield return file;
        }

        IFileHash IFileHash.Equals(MyListDAL.Entities.File file, IHasher hasherInstance)
        {
            var fileHash = Entities.FileHash.Cast(file, hasherInstance);
            var enumerable = ((IEnumerable<Entities.FileHash>)this).Where(fH => fH == fileHash);

            return new FileHash(enumerable);
        }
    }
}
