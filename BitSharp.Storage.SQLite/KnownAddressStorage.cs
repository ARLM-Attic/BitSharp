﻿using BitSharp.Common;
using BitSharp.Storage.SQLite;
using BitSharp.Storage.SQLite.ExtensionMethods;
using BitSharp.Storage;
using BitSharp.WireProtocol;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BitSharp.Storage.SQLite
{
    public struct KnownAddressKey
    {
        public readonly ImmutableArray<byte> IPv6Address;
        public readonly UInt16 Port;
        private readonly int _hashCode;

        public KnownAddressKey(ImmutableArray<byte> IPv6Address, UInt16 Port)
        {
            this.IPv6Address = IPv6Address;
            this.Port = Port;

            this._hashCode = Port.GetHashCode() ^ new BigInteger(IPv6Address.ToArray()).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is KnownAddressKey))
                return false;

            var other = (KnownAddressKey)obj;
            return other.IPv6Address.SequenceEqual(this.IPv6Address) && other.Port == this.Port;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }
    }

    public class KnownAddressStorage : SqlDataStorage, IBoundedStorage<KnownAddressKey, NetworkAddressWithTime>
    {
        public KnownAddressStorage(SQLiteStorageContext storageContext)
            : base(storageContext)
        { }

        public IEnumerable<KnownAddressKey> ReadAllKeys()
        {
            using (var conn = this.OpenReadConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT IPAddress, Port
                    FROM KnownAddresses";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ipAddress = reader.GetBytes(0).ToImmutableArray();
                        var port = reader.GetUInt16(1);
                        yield return new KnownAddressKey(ipAddress, port);
                    }
                }
            }
        }

        public IEnumerable<KeyValuePair<KnownAddressKey, NetworkAddressWithTime>> ReadAllValues()
        {
            using (var conn = this.OpenReadConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT IPAddress, Port, Services, Time
                    FROM KnownAddresses";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ipAddress = reader.GetBytes(0).ToImmutableArray();
                        var port = reader.GetUInt16(1);
                        var services = reader.GetUInt64(2);
                        var time = reader.GetUInt32(3);

                        var key = new KnownAddressKey(ipAddress, port);
                        var knownAddress = new NetworkAddressWithTime(time, new NetworkAddress(services, ipAddress, port));

                        yield return new KeyValuePair<KnownAddressKey, NetworkAddressWithTime>(key, knownAddress);
                    }
                }
            }
        }

        public bool TryReadValue(KnownAddressKey key, out NetworkAddressWithTime knownAddress)
        {
            using (var conn = this.OpenReadConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT IPAddress, Port, Services, Time
                    FROM KnownAddresses";

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var ipAddress = reader.GetBytes(0).ToImmutableArray();
                        var port = reader.GetUInt16(1);
                        var services = reader.GetUInt64(2);
                        var time = reader.GetUInt32(3);

                        knownAddress = new NetworkAddressWithTime(time, new NetworkAddress(services, ipAddress, port));

                        return true;
                    }
                    else
                    {
                        knownAddress = default(NetworkAddressWithTime);
                        return false;
                    }
                }
            }
        }

        public bool TryWriteValues(IEnumerable<KeyValuePair<KnownAddressKey, WriteValue<NetworkAddressWithTime>>> values)
        {
            using (var conn = this.OpenWriteConnection())
            using (var cmd = conn.CreateCommand())
            {
                foreach (var keyPair in values)
                {
                    cmd.CommandText = keyPair.Value.IsCreate ? CREATE_QUERY : UPDATE_QUERY;

                    var knownAddress = keyPair.Value.Value;

                    cmd.Parameters.SetValue("@ipAddress", System.Data.DbType.Binary, 16).Value = knownAddress.NetworkAddress.IPv6Address.ToArray();
                    cmd.Parameters.SetValue("@port", System.Data.DbType.Binary, 2).Value = knownAddress.NetworkAddress.Port.ToDbByteArray();
                    cmd.Parameters.SetValue("@services", System.Data.DbType.Binary, 8).Value = knownAddress.NetworkAddress.Services.ToDbByteArray();
                    cmd.Parameters.SetValue("@time", System.Data.DbType.Binary, 4).Value = knownAddress.Time.ToDbByteArray();

                    cmd.ExecuteNonQuery();
                }

                conn.Commit();
                return true;
            }
        }

        public void Truncate()
        {
            using (var conn = this.OpenWriteConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    DELETE FROM KnownAddresses";

                cmd.ExecuteNonQuery();

                conn.Commit();
            }
        }

        private const string CREATE_QUERY = @"
            INSERT OR IGNORE
            INTO KnownAddresses (IPAddress, Port, Services, Time)
	        VALUES (@ipAddress, @port, @services, @time)";

        private const string UPDATE_QUERY = @"
            INSERT OR REPLACE
            INTO KnownAddresses (IPAddress, Port, Services, Time)
	        VALUES (@ipAddress, @port, @services, @time)";
    }
}
