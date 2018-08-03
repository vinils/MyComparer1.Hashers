namespace MyComparer.Hashers.DAL.Entities
{
    using System;
    using System.IO;
    using MyListDAL = MyList.DAL;

    public sealed class FileHash
    {
        private static FileHash Cast(MyListDAL.Entities.File file, byte[] hash)
        {
            var strHash = BitConverter
                .ToString(hash)
                .Replace("-", "")
                .ToLowerInvariant();

            return new FileHash(file, strHash);
        }

        private static FileHash Cast(FileInfo file, Func<Stream, byte[]> hahser)
        {
            using (var stream = file.OpenRead())
            {
                var hash = hahser(stream);
                return Cast(file, hash);
            }
        }

        public static FileHash Cast(FileInfo file, IHasher hasherInstance)
            => Cast(file, hasherInstance.ComputeHash);

        public static FileHash Cast(MyListDAL.Entities.File file, IHasher hasherInstance)
            => Cast((FileInfo)file, hasherInstance);

        internal MyListDAL.Entities.File File { get; private set; }

        public string Hash { get; private set; }

        public FileHash(MyListDAL.Entities.File file, string hash)
        {
            File = file;
            Hash = hash;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }

        public static implicit operator MyListDAL.Entities.File(FileHash fileHash)
            => fileHash.File;
    }
}
