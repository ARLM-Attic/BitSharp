﻿using BitSharp.Common;
using BitSharp.Common.ExtensionMethods;
using BitSharp.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitSharp.Storage
{
    public class TxKeyCache : UnboundedCache<UInt256, TxKey>
    {
        private readonly CacheContext _cacheContext;

        public TxKeyCache(CacheContext cacheContext, long maxFlushMemorySize, long maxCacheMemorySize)
            : base("TxKeyCache", cacheContext.StorageContext.TxKeyStorage, maxFlushMemorySize, maxCacheMemorySize, txKey => 70)
        {
            this._cacheContext = cacheContext;
        }

        public CacheContext CacheContext { get { return this._cacheContext; } }

        public IStorageContext StorageContext { get { return this.CacheContext.StorageContext; } }

        internal void CacheBlock(Block block)
        {
            for (var txIndex = 0; txIndex < block.Transactions.Length; txIndex++)
            {
                var tx = block.Transactions[txIndex];
                CacheValue(tx.Hash, new TxKey(tx.Hash, block.Hash, (UInt32)txIndex));
            }
        }
    }
}
