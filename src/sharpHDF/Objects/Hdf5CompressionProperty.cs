
namespace sharpHDF.Library.Objects
{
    public class Hdf5CompressionProperty
    {
        public Hdf5CompressionProperty(ulong[] chunkDimensions, uint compressionLevel)
        {
            ChunkDimensions = chunkDimensions;
            CompressionLevel = compressionLevel;
        }
        public ulong[] ChunkDimensions { get; }
        public uint CompressionLevel { get; }
    }
}
