﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSharp.Common.ExtensionMethods;
using System.IO;
using System.Reflection;
using System.Data.Common;
using BitSharp.Storage;
using System.Threading;
using System.Diagnostics;
using System.Data.SQLite;
using BitSharp.Common;

namespace BitSharp.Storage.SQLite
{
    public abstract class SqlDataStorage : IDisposable
    {
        private static readonly string dbFolderPath;
        private static readonly string dbPath;
        private static readonly string connString;

        private static readonly ReaderWriterLockSlim dbLock;
        private static SQLiteConnection conn;
        private static int connCount;
        private static readonly SemaphoreSlim connSemaphore;

        private readonly SQLiteStorageContext _storageContext;
        private bool disposed;

        static SqlDataStorage()
        {
            dbFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BitSharp");
            dbPath = Path.Combine(dbFolderPath, "BitSharp.sqlite");
            connString = @"Data Source=""{0}""; Journal Mode=WAL; PRAGMA cache_size=-100000".Format2(dbPath);

            dbLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            connSemaphore = new SemaphoreSlim(1);
        }

        public SqlDataStorage(SQLiteStorageContext storageContext)
        {
            this._storageContext = storageContext;

            connSemaphore.Do(() =>
            {
                if (Interlocked.Increment(ref connCount) == 1)
                {
                    conn = OpenConnection();
                    CreateDatabase();
                }
            });
        }

        public SQLiteStorageContext StorageContext { get { return this._storageContext; } }

        public void Dispose()
        {
            if (!this.disposed)
            {
                connSemaphore.Do(() =>
                {
                    if (!this.disposed)
                    {
                        this.disposed = true;

                        if (Interlocked.Decrement(ref connCount) == 0)
                        {
                            conn.Dispose();
                            conn = null;
                        }
                    }
                });
            }
        }

        protected ReadConnection OpenReadConnection()
        {
            return new ReadConnection(conn, dbLock);
        }

        protected WriteConnection OpenWriteConnection()
        {
            //return new WriteConnection(conn, dbLock);
            return new WriteConnection(OpenConnection(), dbLock);
        }

        private SQLiteConnection OpenConnection()
        {
            // create db folder
            if (!Directory.Exists(dbFolderPath))
                Directory.CreateDirectory(dbFolderPath);

            var connection = new SQLiteConnection(connString);
            try
            {
                connection.Open();
                return connection;
            }
            catch (Exception)
            {
                connection.Dispose();
                throw;
            }
        }

        private void CreateDatabase()
        {
            dbLock.EnterWriteLock();
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var cmd = conn.CreateCommand())
                using (var stream = assembly.GetManifestResourceStream("BitSharp.Storage.SQLite.Sql.CreateDatabase.sql"))
                using (var reader = new StreamReader(stream))
                {
                    cmd.CommandText = reader.ReadToEnd();

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Database create failed: {0}".Format2(e.Message));
                Debugger.Break();
                throw;
            }
            finally
            {
                dbLock.ExitWriteLock();
            }
        }

        public class ReadConnection : IDisposable
        {
            private readonly SQLiteConnection conn;
            private readonly ReaderWriterLockSlim dbLock;
            private bool disposed;

            public ReadConnection(SQLiteConnection conn, ReaderWriterLockSlim dbLock)
            {
                this.conn = conn;
                this.dbLock = dbLock;
                this.disposed = false;
                //this.dbLock.EnterReadLock();
            }

            public SQLiteCommand CreateCommand()
            {
                return this.conn.CreateCommand();
            }

            public void Dispose()
            {
                if (!this.disposed)
                {
                    //this.dbLock.ExitReadLock();
                    this.disposed = true;
                }
            }
        }

        public class WriteConnection : IDisposable
        {
            private static DateTime lastLockTime = DateTime.MinValue;

            private readonly SQLiteConnection conn;
            private readonly SQLiteTransaction trans;
            private readonly ReaderWriterLockSlim dbLock;
            private bool disposed;

            public WriteConnection(SQLiteConnection conn, ReaderWriterLockSlim dbLock)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                //var lastLockTime = WriteConnection.lastLockTime;
                //var now = DateTime.UtcNow;
                //var delta = now - lastLockTime;
                //var minWait = TimeSpan.FromMilliseconds(50);
                //if (delta < minWait)
                //    Thread.Sleep(minWait - delta);

                this.conn = conn;
                this.dbLock = dbLock;
                this.disposed = false;
                //this.dbLock.EnterWriteLock();
                try
                {
                    stopwatch.Stop();
                    //Debug.WriteLine("waited: {0:#,##0.000000}s".Format2(stopwatch.ElapsedSecondsFloat()));

                    this.trans = this.conn.BeginTransaction();
                }
                catch (Exception)
                {
                    //this.dbLock.ExitWriteLock();
                    throw;
                }
            }

            public SQLiteCommand CreateCommand()
            {
                var cmd = this.conn.CreateCommand();
                try
                {
                    cmd.Transaction = this.trans;
                    return cmd;
                }
                catch (Exception)
                {
                    cmd.Dispose();
                    throw;
                }
            }

            public void Commit()
            {
                this.trans.Commit();
            }

            public void Rollback()
            {
                this.trans.Rollback();
            }

            public void Dispose()
            {
                if (!this.disposed)
                {
                    try
                    {
                        this.trans.Dispose();
                        this.conn.Dispose();
                    }
                    finally
                    {
                        lastLockTime = DateTime.UtcNow;
                        //this.dbLock.ExitWriteLock();
                        this.disposed = true;
                    }
                }
            }
        }
    }
}
