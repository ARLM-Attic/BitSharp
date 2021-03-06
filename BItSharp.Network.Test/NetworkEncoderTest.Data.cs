﻿using BitSharp.Common;
using BitSharp.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSharp.Network.Test
{
    public partial class NetworkEncoderTest
    {
        public static readonly NetworkAddress NETWORK_ADDRESS_1 = new NetworkAddress
        (
            Services: 0x01,
            IPv6Address: ImmutableArray.Create<byte>(0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f),
            Port: 0x02
        );

        public static readonly ImmutableArray<byte> NETWORK_ADDRESS_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x02);

        public static readonly NetworkAddressWithTime NETWORK_ADDRESS_WITH_TIME_1 = new NetworkAddressWithTime
        (
            Time: 0x01,
            NetworkAddress: NETWORK_ADDRESS_1
        );

        public static readonly ImmutableArray<byte> NETWORK_ADDRESS_WITH_TIME_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x02);

        public static readonly AddressPayload ADDRESS_PAYLOAD_1 = new AddressPayload
        (
            NetworkAddresses: ImmutableArray.Create(NETWORK_ADDRESS_WITH_TIME_1)
        );

        public static readonly ImmutableArray<byte> ADDRESS_PAYLOAD_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x02);

        public static readonly AlertPayload ALERT_PAYLOAD_1 = new AlertPayload
        (
            Payload: "Payload",
            Signature: "Signature"
        );

        public static readonly ImmutableArray<byte> ALERT_PAYLOAD_1_BYTES = ImmutableArray.Create<byte>(0x07, 0x50, 0x61, 0x79, 0x6c, 0x6f, 0x61, 0x64, 0x09, 0x53, 0x69, 0x67, 0x6e, 0x61, 0x74, 0x75, 0x72, 0x65);

        public static readonly TxInput TRANSACTION_INPUT_1 = new TxInput
        (
            previousTxOutputKey: new TxOutputKey
            (
                txHash: UInt256.Parse("00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff", NumberStyles.HexNumber),
                txOutputIndex: 0x01
            ),
            scriptSignature: ImmutableArray.Create<byte>(0x00, 0x01, 0x02, 0x03, 0x04),
            sequence: 0x02
        );

        public static readonly ImmutableArray<byte> TRANSACTION_INPUT_1_BYTES = ImmutableArray.Create<byte>(0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0x01, 0x00, 0x00, 0x00, 0x05, 0x00, 0x01, 0x02, 0x03, 0x04, 0x02, 0x00, 0x00, 0x00);

        public static readonly TxOutput TRANSACTION_OUTPUT_1 = new TxOutput
        (
            value: 0x01,
            scriptPublicKey: ImmutableArray.Create<byte>(0x00, 0x01, 0x02, 0x03, 0x04)
        );

        public static readonly ImmutableArray<byte> TRANSACTION_OUTPUT_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x01, 0x02, 0x03, 0x04);

        public static readonly Transaction TRANSACTION_1 = new Transaction
        (
            version: 0x01,
            inputs: ImmutableArray.Create(TRANSACTION_INPUT_1),
            outputs: ImmutableArray.Create(TRANSACTION_OUTPUT_1),
            lockTime: 0x02
        );

        public static readonly ImmutableArray<byte> TRANSACTION_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x00, 0x00, 0x00, 0x01, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0x01, 0x00, 0x00, 0x00, 0x05, 0x00, 0x01, 0x02, 0x03, 0x04, 0x02, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x01, 0x02, 0x03, 0x04, 0x02, 0x00, 0x00, 0x00);

        public static readonly BlockHeader BLOCK_HEADER_1 = new BlockHeader
        (
            version: 0x01,
            previousBlock: UInt256.Parse("00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff", NumberStyles.HexNumber),
            merkleRoot: UInt256.Parse("ffeeddccbbaa99887766554433221100ffeeddccbbaa99887766554433221100", NumberStyles.HexNumber),
            time: 0x02,
            bits: 0x03,
            nonce: 0x04
        );

        public static readonly ImmutableArray<byte> BLOCK_HEADER_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x00, 0x00, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff, 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00);

        public static readonly Block BLOCK_1 = new Block
        (
            header: BLOCK_HEADER_1,
            transactions: ImmutableArray.Create(TRANSACTION_1)
        );

        public static readonly ImmutableArray<byte> BLOCK_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x00, 0x00, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff, 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x01, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0x01, 0x00, 0x00, 0x00, 0x05, 0x00, 0x01, 0x02, 0x03, 0x04, 0x02, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x01, 0x02, 0x03, 0x04, 0x02, 0x00, 0x00, 0x00);

        public static readonly GetBlocksPayload GET_BLOCKS_PAYLOAD_1 = new GetBlocksPayload
        (
            Version: 0x01,
            BlockLocatorHashes: ImmutableArray.Create(UInt256.Parse("00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff", NumberStyles.HexNumber), UInt256.Parse("ffeeddccbbaa99887766554433221100ffeeddccbbaa99887766554433221100", NumberStyles.HexNumber)),
            HashStop: UInt256.Parse("8899aabbccddeeff00112233445566778899aabbccddeeff0011223344556677", NumberStyles.HexNumber)
        );

        public static readonly ImmutableArray<byte> GET_BLOCKS_PAYLOAD_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x00, 0x00, 0x00, 0x02, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88);

        public static readonly InventoryVector INVENTORY_VECTOR_1 = new InventoryVector
        (
            Type: 0x01,
            Hash: UInt256.Parse("00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff", NumberStyles.HexNumber)
        );

        public static readonly ImmutableArray<byte> INVENTORY_VECTOR_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x00, 0x00, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00);

        public static readonly InventoryPayload INVENTORY_PAYLOAD_1 = new InventoryPayload
        (
            InventoryVectors: ImmutableArray.Create(INVENTORY_VECTOR_1)
        );

        public static readonly ImmutableArray<byte> INVENTORY_PAYLOAD_1_BYTES = ImmutableArray.Create<byte>(0x01, 0x01, 0x00, 0x00, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0x00);

        public static readonly Message MESSAGE_1 = new Message
        (
            Magic: 0xD9B4BEF9,
            Command: "command",
            PayloadSize: 5,
            PayloadChecksum: 0xC1078A76,
            Payload: ImmutableArray.Create<byte>(0, 1, 2, 3, 4)
        );

        public static readonly ImmutableArray<byte> MESSAGE_1_BYTES = ImmutableArray.Create<byte>(0xf9, 0xbe, 0xb4, 0xd9, 0x63, 0x6f, 0x6d, 0x6d, 0x61, 0x6e, 0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x76, 0x8a, 0x07, 0xc1, 0x00, 0x01, 0x02, 0x03, 0x04);

        public static readonly VersionPayload VERSION_PAYLOAD_1_NO_RELAY = new VersionPayload
        (
            ProtocolVersion: 0x01,
            ServicesBitfield: 0x02,
            UnixTime: 0x03,
            RemoteAddress: NETWORK_ADDRESS_1,
            LocalAddress: NETWORK_ADDRESS_1,
            Nonce: 0x04,
            UserAgent: "UserAgent",
            StartBlockHeight: 0x05,
            Relay: false
        );

        public static readonly ImmutableArray<byte> VERSION_PAYLOAD_1_NO_RELAY_BYTES = ImmutableArray.Create<byte>(0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x02, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x55, 0x73, 0x65, 0x72, 0x41, 0x67, 0x65, 0x6e, 0x74, 0x05, 0x00, 0x00, 0x00);

        public static readonly VersionPayload VERSION_PAYLOAD_2_RELAY = new VersionPayload
        (
            ProtocolVersion: VersionPayload.RELAY_VERSION,
            ServicesBitfield: 0x02,
            UnixTime: 0x03,
            RemoteAddress: NETWORK_ADDRESS_1,
            LocalAddress: NETWORK_ADDRESS_1,
            Nonce: 0x04,
            UserAgent: "UserAgent",
            StartBlockHeight: 0x05,
            Relay: true
        );

        public static readonly ImmutableArray<byte> VERSION_PAYLOAD_2_RELAY_BYTES = ImmutableArray.Create<byte>(0x71, 0x11, 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x02, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x55, 0x73, 0x65, 0x72, 0x41, 0x67, 0x65, 0x6e, 0x74, 0x05, 0x00, 0x00, 0x00, 0x01);

    }
}
