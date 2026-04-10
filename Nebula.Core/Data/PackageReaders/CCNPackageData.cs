using Nebula.Core.Data.Chunks;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.PackageReaders
{
    public class CCNPackageData : PackageData
    {
        public override void Read(ByteReader reader)
        {
            this.Log($"Running build '{NebulaCore.GetCommitHash()}'");
            if (NebulaCore.Fusion == 1.1f)
                return;

            Header = reader.ReadAscii(4);
            this.Log("Game Header: " + Header);

            // CTFAK2.0: PAMU=Unicode, PAME=ASCII, CRUF=Fusion3
            if (Header == "PAMU")
                NebulaCore._yunicode = true;
            else if (Header == "PAME")
                NebulaCore._yunicode = false;
            else if (Header == "CRUF")
            {
                // Fusion 3 header - will be handled in ExtendedHeader
                NebulaCore.Seeded = true;
            }

            RuntimeVersion = reader.ReadShort();
            RuntimeSubversion = reader.ReadShort();
            ProductVersion = reader.ReadInt();
            ProductBuild = reader.ReadInt();
            NebulaCore.Build = ProductBuild;
            if (RuntimeVersion != 769)
            {
                if (NebulaCore.Build < 280)
                    NebulaCore.Fusion = 2f + (ProductVersion == 1 ? 0.1f : 0);
                this.Log("Fusion Build: " + ProductBuild + " (Fusion " + NebulaCore.Fusion + ")");
            }
            else
            {
                NebulaCore.Fusion = 1.5f;
                this.Log("Fusion 1.5");
            }

            if (Parameters.ForceUnicode)
                NebulaCore._yunicode = true;

            Frames = new List<Frame>();
            while (reader.HasMemory(8))
            {
                var newChunk = Chunk.InitChunk(reader);
                this.Log($"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                if (newChunk.ChunkID == 32494)
                    NebulaCore.Seeded = true;
                if (newChunk.ChunkID == 8787)
                    NebulaCore.Plus = true;

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader);

                // CTFAK2.0: After reading EditorFilename chunk, generate decryption key
                // Key order depends on Build > 284 (CTFAK2.0 line 194-195)
                if (newChunk.ChunkID == 0x222E && !string.IsNullOrEmpty(NebulaCore.PackageData.EditorFilename))
                {
                    if (NebulaCore.Build > 284)
                        Decryption.MakeKey(NebulaCore.PackageData.AppName, NebulaCore.PackageData.Copyright, NebulaCore.PackageData.EditorFilename);
                    else
                        Decryption.MakeKey(NebulaCore.PackageData.EditorFilename, NebulaCore.PackageData.AppName, NebulaCore.PackageData.Copyright);
                }

                if (!(NebulaCore.Fusion == 1.5f && newChunk.ChunkID >= 0x6666 && newChunk.ChunkID <= 0x6669))
                    newChunk.ChunkData = new byte[0];
            }
            reader.Seek(reader.Size());
        }
    }
}
