// src/Wc3.MapDocument/MpqHeader.cs
namespace Wc3.Model;

public static class MpqHeader
{
    // MPQ archive header magic: 'M','P','Q', 0x1A
    private static readonly byte[] ArchiveMagic = { 0x4D, 0x50, 0x51, 0x1A };

    /// <summary>Offset where the MPQ archive begins. WC3 maps prefix a 512-byte
    /// header (starts with "HM3W"); the archive is aligned to a 512-byte boundary.</summary>
    public static int FindArchiveOffset(byte[] fileBytes)
    {
        for (int offset = 0; offset + 4 <= fileBytes.Length; offset += 0x200)
        {
            if (fileBytes[offset] == ArchiveMagic[0]
                && fileBytes[offset + 1] == ArchiveMagic[1]
                && fileBytes[offset + 2] == ArchiveMagic[2]
                && fileBytes[offset + 3] == ArchiveMagic[3])
            {
                return offset;
            }
        }
        return -1;
    }
}
