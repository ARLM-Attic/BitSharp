﻿using BitSharp.Common;
using BitSharp.Common.ExtensionMethods;
using BitSharp.Storage;
using BitSharp.Storage.SQLite.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitSharp.Network;
using System.Data.SqlClient;
using System.Data.Common;
using BitSharp.Blockchain;
using System.Data;
using System.Globalization;
using System.Collections.Immutable;
using BitSharp.Data;

namespace BitSharp.Storage.SQLite
{
    public class TxKeyStorage : SqlDataStorage, ITxKeyStorage
    {
        public TxKeyStorage(SQLiteStorageContext storageContext)
            : base(storageContext)
        { }

        public bool TryReadValue(UInt256 txHash, out TxKey txKey)
        {
            using (var conn = this.OpenReadConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT BlockHash, TransactionIndex
                    FROM TransactionLocators
                    WHERE TransactionHash = @txHash";

                cmd.Parameters.SetValue("@txHash", DbType.Binary, 32).Value = txHash.ToDbByteArray();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var blockHash = reader.GetUInt256(0);
                        var txIndex = reader.GetUInt32(1);

                        txKey = new TxKey(txHash, blockHash, txIndex);
                        return true;
                    }
                    else
                    {
                        txKey = default(TxKey);
                        return false;
                    }
                }
            }
        }

        public bool TryWriteValues(IEnumerable<KeyValuePair<UInt256, WriteValue<TxKey>>> txKeys)
        {
            throw new NotSupportedException();
        }
    }
}
