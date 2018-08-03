namespace MyComparer.Hashers.DAL
{
    using System.Collections.Generic;
    using MyListDAL = MyList.DAL;

    public interface IFileHash: MyListDAL.IDao<Entities.FileHash>, IEnumerable<MyListDAL.Entities.File>
    {
        IFileHash Duplicates();
        IFileHash Distincts();
        IFileHash Equals(MyListDAL.Entities.File file, IHasher hasherInstance);
    }
}