namespace MyComparer.Hashers.DAL
{
    using MyList;
    using System;
    using System.Collections.Generic;
    using MyListDAL = MyList.DAL;

    public static class MyExtension
    {
        public static void List(this 
            IEnumerable<MyListDAL.Entities.File> files, 
            Action<Entities.FileHash> action, 
            IHasher hasherInstance)
        {
            void actionCasting(MyListDAL.Entities.File file) 
                => action(Entities.FileHash.Cast(file, hasherInstance));

            files.List(actionCasting);
        }

        public static void List(this 
            IEnumerable<MyListDAL.Entities.File> files, 
            Action<Entities.FileHash> action, 
            Action<Exception> enqueueException, 
            IHasher hasherInstance)
        {
            void actionCasting(MyListDAL.Entities.File file) 
                => action(Entities.FileHash.Cast(file, hasherInstance));

            files.List(actionCasting, enqueueException);
        }

        public static void ListAggregatingExceptions(
            this IEnumerable<MyListDAL.Entities.File> files, 
            Action<Entities.FileHash> action, 
            IHasher hasherInstance)
        {
            void actionCasting(MyListDAL.Entities.File file) 
                => action(Entities.FileHash.Cast(file, hasherInstance));

            files.ListAggregatingExceptions(actionCasting);
        }

        public static void ListParallel(
            this IEnumerable<MyListDAL.Entities.File> files, 
            Action<Entities.FileHash> action, 
            Action<Exception> enqueueException, 
            IHasher hasherInstance)
        {
            void actionCasting(MyListDAL.Entities.File file) 
                => action(Entities.FileHash.Cast(file, hasherInstance));

            files.ListParallel(actionCasting, enqueueException);
        }

        public static void ListParallelAggregatingExceptions(
            this IEnumerable<MyListDAL.Entities.File> files, 
            Action<Entities.FileHash> action, 
            IHasher hasherInstance)
        {
            void actionCasting(MyListDAL.Entities.File file) 
                => action(Entities.FileHash.Cast(file, hasherInstance));

            files.ListParallelAggregatingExceptions(actionCasting);
        }

        public static void Add(this IFileHash fileHashes, MyListDAL.Entities.File file, IHasher hasherInstance)
        {
            var fileHash = Entities.FileHash.Cast(file, hasherInstance);

            fileHashes.Add(fileHash);
        }

        public static void Add(
            this IFileHash fileHashes, 
            Action<Action<Entities.FileHash>, Action<Exception>, IHasher> list,
            IHasher hasherInstance,
            Action<Exception> enqueueException, 
            out Memories.FileHash newFileHashes)
        {
            var newFileHashesOut = new Memories.FileHash();

            void add(Entities.FileHash localFileHashes)
            {
                fileHashes.Add(localFileHashes);
                newFileHashesOut.Add(localFileHashes);
            }

            list(add, enqueueException, hasherInstance);

            newFileHashes = newFileHashesOut;
        }
    }
}
