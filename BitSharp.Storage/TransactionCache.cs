﻿using BitSharp.Common;
using BitSharp.Common.ExtensionMethods;
using BitSharp.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSharp.Storage
{
    public class TransactionCache : UnboundedCache<UInt256, Transaction>
    {
        private readonly CacheContext _cacheContext;

        public TransactionCache(CacheContext cacheContext, long maxCacheMemorySize)
            : base("TransactionCache", new TransactionStorage(cacheContext), 0, maxCacheMemorySize, Transaction.SizeEstimator)
        { }

        public CacheContext CacheContext { get { return this._cacheContext; } }

        public IStorageContext StorageContext { get { return this.CacheContext.StorageContext; } }
    }

    public class TransactionStorage : IUnboundedStorage<UInt256, Transaction>
    {
        private readonly CacheContext _cacheContext;

        public TransactionStorage(CacheContext cacheContext)
        {
            this._cacheContext = cacheContext;
        }

        public CacheContext CacheContext { get { return this._cacheContext; } }

        public IStorageContext StorageContext { get { return this.CacheContext.StorageContext; } }

        public void Dispose()
        {
        }

        public bool TryReadValue(UInt256 txHash, out Transaction value)
        {
            HashSet<TxKey> txKeySet;
            if (this.CacheContext.TxKeyCache.TryGetValue(txHash, out txKeySet))
            {
                foreach (var txKey in txKeySet)
                {
                    Block block;
                    if (this.CacheContext.BlockCache.TryGetValue(txKey.BlockHash, out block))
                    {
                        if (txKey.TxIndex >= block.Transactions.Length)
                            throw new MissingDataException(DataType.Transaction, txKey.TxHash); //TODO should be invalid data, not missing data

                        value = block.Transactions[txKey.TxIndex.ToIntChecked()];
                        return true;
                    }
                }
            }

            value = default(Transaction);
            return false;
        }

        public bool TryWriteValues(IEnumerable<KeyValuePair<UInt256, WriteValue<Transaction>>> values)
        {
            throw new NotSupportedException();
        }
    }
}
