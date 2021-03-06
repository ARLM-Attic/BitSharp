USE BitSharp_Blockchains
GO

IF OBJECT_ID('BlockchainMetadata') IS NULL
CREATE TABLE BlockchainMetadata
(
    Guid BINARY(16) NOT NULL,
    RootBlockHash BINARY(32) NOT NULL,
    TotalWork BINARY(64) NOT NULL,
    IsComplete INTEGER NOT NULL,
	CONSTRAINT PK_BlockchainMetaData PRIMARY KEY
	(
        Guid
	)
);

IF OBJECT_ID('ChainedBlocks') IS NULL
CREATE TABLE ChainedBlocks
(
    Guid BINARY(16) NOT NULL,
    RootBlockHash BINARY(32) NOT NULL,
	BlockHash BINARY(32) NOT NULL,
	PreviousBlockHash BINARY(32) NOT NULL,
	Height INTEGER NOT NULL,
	TotalWork BINARY(64) NOT NULL,
	CONSTRAINT PK_ChainedBlocks PRIMARY KEY NONCLUSTERED
	(
        Guid,
        RootBlockHash,
		BlockHash
	)
);

IF NOT EXISTS(SELECT * FROM sysindexes WHERE name = 'IX_ChainedBlocks_Guid_RootHash')
CREATE INDEX IX_ChainedBlocks_Guid_RootHash ON ChainedBlocks ( Guid, RootBlockHash );

IF OBJECT_ID('UtxoData') IS NULL
CREATE TABLE UtxoData
(
	Guid BINARY(16) NOT NULL,
	RootBlockHash BINARY(32) NOT NULL,
	UtxoChunkBytes VARBINARY(MAX) NOT NULL
);

IF NOT EXISTS(SELECT * FROM sysindexes WHERE name = 'IX_UtxoData_Guid_RootBlockHash')
CREATE INDEX IX_UtxoData_Guid_RootBlockHash ON UtxoData ( Guid, RootBlockHash );
