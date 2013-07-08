﻿using BitSharp.Common;
using BitSharp.Script;
using BitSharp.WireProtocol;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace BitSharp.BlockHelper
{
    public static class BlockJson
    {
        public static Block GetBlockFromJson(string blockJson)
        {
            var block = Json.Decode(blockJson);
            return new Block
            (
                Header: new BlockHeader
                (
                    Version: Convert.ToUInt32(block.ver),
                    PreviousBlock: UInt256.Parse(block.prev_block, NumberStyles.HexNumber),
                    MerkleRoot: UInt256.Parse(block.mrkl_root, NumberStyles.HexNumber),
                    Time: Convert.ToUInt32(block.time),
                    Bits: Convert.ToUInt32(block.bits),
                    Nonce: Convert.ToUInt32(block.nonce)
                ),
                Transactions: ReadTransactions(block.tx)
            );
        }

        public static ImmutableArray<Transaction> ReadTransactions(dynamic transactions)
        {
            return
                Enumerable.Range(0, (int)transactions.Length)
                .Select(i => (Transaction)ReadTransaction(transactions[i]))
                .ToImmutableArray();
        }

        public static Transaction ReadTransaction(dynamic transaction)
        {
            return new Transaction
            (
                Version: Convert.ToUInt32(transaction.ver),
                Inputs: ReadInputs(transaction.@in),
                Outputs: ReadOutputs(transaction.@out),
                LockTime: Convert.ToUInt32(transaction.lock_time)
            );
        }

        public static ImmutableArray<TransactionIn> ReadInputs(dynamic inputs)
        {
            return
                Enumerable.Range(0, (int)inputs.Length)
                .Select(i => (TransactionIn)ReadInput(inputs[i]))
                .ToImmutableArray();
        }

        public static ImmutableArray<TransactionOut> ReadOutputs(dynamic outputs)
        {
            return
                Enumerable.Range(0, (int)outputs.Length)
                .Select(i => (TransactionOut)ReadOutput(outputs[i]))
                .ToImmutableArray();
        }

        public static TransactionIn ReadInput(dynamic input)
        {
            return new TransactionIn
            (
                PreviousTransactionHash: UInt256.Parse(input.prev_out.hash, NumberStyles.HexNumber),
                PreviousTransactionIndex: Convert.ToUInt32(input.prev_out.n),
                ScriptSignature: input.scriptSig != null ? ReadScript(input.scriptSig) : ReadCoinbase(input.coinbase),
                Sequence: input.sequence != null ? Convert.ToUInt32(input.sequence) : 0xFFFFFFFF
            );
        }

        public static TransactionOut ReadOutput(dynamic output)
        {
            return new TransactionOut
            (
                Value: Convert.ToUInt64(((string)output.value).Replace(".", "")), //TODO cleaner decimal replace
                ScriptPublicKey: ReadScript(output.scriptPubKey)
            );
        }

        public static ImmutableArray<byte> ReadCoinbase(string data)
        {
            return data != null ? HexStringToByteArray(data) : ImmutableArray.Create<byte>();
        }

        public static ImmutableArray<byte> ReadScript(string data)
        {
            if (data == null)
                return ImmutableArray.Create<byte>();

            var bytes = new List<byte>();
            foreach (var x in data.Split(' '))
            {
                if (x.StartsWith("OP_"))
                {
                    bytes.Add((byte)(int)Enum.Parse(typeof(ScriptOp), x));
                }
                else
                {
                    var pushBytes = HexStringToByteArray(x);
                    if (pushBytes.Length >= (int)ScriptOp.OP_PUSHBYTES1 && pushBytes.Length <= (int)ScriptOp.OP_PUSHBYTES75)
                    {
                        bytes.Add((byte)pushBytes.Length);
                        bytes.AddRange(pushBytes);
                    }
                    else
                    {
                        throw new Exception("data is too long");
                    }
                }
            }

            return bytes.ToImmutableArray();
        }

        //TODO not actually an extension method...
        private static ImmutableArray<byte> HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToImmutableArray();
        }
    }
}
