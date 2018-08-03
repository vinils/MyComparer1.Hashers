namespace MyComparer.Hashers.DAL.Clouds
{
    using MyList;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using MyListDAL = MyList.DAL;

    public class FileHash : MyListDAL.Cloud<Entities.FileHash>, IFileHash
    {
        public IEnumerable<MyListDAL.Entities.File> GetFiles()
            => ((IEnumerable<Entities.FileHash>)this).Select(f => (MyListDAL.Entities.File)f);

        public FileHash()
        { }

        public FileHash(Guid processId)
            : base(processId)
        { }

        public override void Add(Entities.FileHash file)
        {
            void localAdd(SqlTransaction trans)
                => ExecuteNonQuery(ProcessId, Connection, file, trans);

            Add(localAdd);
        }

        protected void Add(MyListDAL.Entities.File file, IHasher hasherInstance)
        {
            void localAdd(SqlTransaction trans)
                => ExecuteNonQuery(
                    ProcessId, 
                    Connection, 
                    Entities.FileHash.Cast(file, hasherInstance), 
                    trans);

            Add(localAdd);
        }

        public void Add(
            IEnumerable<MyListDAL.Entities.File> files, 
            IHasher hasherInstance, 
            Action<Exception> enqueueException)
        {
            void add(MyListDAL.Entities.File file) => Add(file, hasherInstance);
            files.List(add, enqueueException);
        }

        public override void AddParallel(IEnumerable<Entities.FileHash> files, Action<Exception> enqueueException)
        {
            void addTransaction(Entities.FileHash file, SqlTransaction trans)
                => ExecuteNonQuery(ProcessId, Connection, file, trans);

            AddParallel(files, addTransaction, enqueueException);
        }

        public void AddParallel(MyListDAL.IFile files, IHasher hasherInstance, Action<Exception> enqueueException)
        {
            void addTransactionCasting(MyListDAL.Entities.File file, SqlTransaction trans, IHasher computeHash)
                => ExecuteNonQuery(
                    ProcessId,
                    Connection,
                    Entities.FileHash.Cast(file, computeHash),
                    trans);

            void localAdd(MyListDAL.Entities.File file, SqlTransaction trans) 
                => addTransactionCasting(file, trans, hasherInstance);

            base.AddParallel(files, localAdd, enqueueException);
        }

        public int ExecuteNonQuery(
            Guid processId, 
            SqlConnection connection, 
            Entities.FileHash fileHash, 
            SqlTransaction transaction)
        {
            var query1 = "INSERT INTO [MyCompare1].[dbo].[FilesHash] (ProcessId, FullName, Hash) VALUES (@ProcessId, @FullName, @Hash)";
            using (var command = new SqlCommand(query1, connection, transaction))
            {
                command.Parameters.AddWithValue("@ProcessId", processId);
                command.Parameters.AddWithValue("@FullName", fileHash.File.ToString());
                command.Parameters.AddWithValue("@Hash", fileHash.Hash);

                return command.ExecuteNonQuery();
            }
        }

        public override IEnumerator<Entities.FileHash> GetEnumerator()
        {
            var da = new SqlDataAdapter();
            var cmd = Connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM FilesHash where ProcessId = '" + ProcessId + "'";
            da.SelectCommand = cmd;
            var ds = new System.Data.DataTable();
            da.Fill(ds);

            for (var x = 0; x <= ds.Rows.Count -1; x++)
            {
                //var processId = ds.Rows[x]["ProcessId"].ToString();
                var fullName = ds.Rows[x]["FullName"].ToString();
                var hash = ds.Rows[x]["Hash"].ToString();
                var file = MyListDAL.Entities.File.Cast(fullName);

                yield return new Entities.FileHash(file, hash);
            }
        }

        IEnumerator<MyListDAL.Entities.File> IEnumerable<MyListDAL.Entities.File>.GetEnumerator()
        {
            foreach (var fileHash in this)
                yield return fileHash.File;
        }

        public IFileHash Duplicates()
        {
            throw new NotImplementedException();
        }

        public IFileHash Distincts()
        {
            throw new NotImplementedException();
        }

        public IFileHash Equals(MyListDAL.Entities.File file, IHasher hasherInstance)
        {
            throw new NotImplementedException();
            //var fileHash = Entities.FileHash.Cast(file, hasherInstance);
            //var enumerable = ((IEnumerable<Entities.FileHash>)this).Where(fH => fH == fileHash);

            //return new FileHash(enumerable);
        }
    }
}
