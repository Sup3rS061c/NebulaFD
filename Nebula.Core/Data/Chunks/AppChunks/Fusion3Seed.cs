using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    /// <summary>
    /// Fusion 3 Seed Chunk (0x7EEE / 32494)
    /// CTFAK2.0: Marks Fusion 3 seeded (encrypted) data format.
    /// When this chunk appears, the game uses a different encryption seed (0x7EEE).
    /// </summary>
    public class Fusion3Seed : Chunk
    {
        public Fusion3Seed()
        {
            ChunkName = "Fusion3Seed";
            ChunkID = 0x7EEE;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            // CTFAK2.0: When chunk 32494 is encountered and Settings.F3 is true,
            // set Fusion3Seed flag to indicate special decryption mode
            NebulaCore.Seeded = true;
            this.Log("Fusion 3 Seed detected - game uses F3 encryption mode", color: ConsoleColor.Yellow);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
