using Aoc;
using Aoc.NUnit;
using static _2024.Day09;

namespace _2024;

public sealed class Day09 : Puzzle<Disk>
{
    protected override Disk GetInput(Stream stream)
    {
        return Disk.Open(stream);
    }

    [Solution(1928)]
    public static long Part1(Disk disk)
    {
        disk.Defrag(new DefragPartialFiles());
        return disk.ComputeCrc();
    }

    [Solution(2858)]
    public static long Part2(Disk disk)
    {
        disk.Defrag(new DefragCompleteFiles());
        return disk.ComputeCrc();
    }

    public sealed record Disk
    {
        public const int FreeSpaceMarker = int.MaxValue;

        private Disk(List<FileDescriptor> fileDescriptors)
        {
            FileDescriptors = fileDescriptors;
            Blocks = MapFiles(FileDescriptors).ToArray();
        }

        public List<FileDescriptor> FileDescriptors { get; }

        internal int[] Blocks { get; }

        public long ComputeCrc()
        {
            return Blocks
                .Select((v, idx) => v == FreeSpaceMarker
                    ? (long)0
                    : v * idx)
                .Sum();
        }

        private static IEnumerable<int> MapFiles(IEnumerable<FileDescriptor> fileSystem)
        {
            var prev = new FileDescriptor(FreeSpaceMarker, 0, 0);
            foreach (FileDescriptor fd in fileSystem)
            {
                int? emptySpaceOffset = fd.Offset - (prev.Offset + prev.Size);
                if (emptySpaceOffset > 0)
                {
                    for (int i = 0; i < emptySpaceOffset; i++)
                    {
                        yield return FreeSpaceMarker;
                    }
                }

                for (int i = 0; i < fd.Size; i++)
                {
                    yield return fd.Id;
                }

                prev = fd;
            }
        }

        public static Disk Open(Stream stream)
        {
            return new Disk(ReadDiskMap().ToList());

            IEnumerable<FileDescriptor> ReadDiskMap()
            {
                using var br = new BinaryReader(stream);
                int fileOffset = 0;
                int fileId = 0;
                int headPos = 0;
                while (true)
                {
                    int v = br.Read();
                    if (v == -1)
                    {
                        yield break;
                    }

                    if (v is < '0' or > '9') // Ignore non-digits
                    {
                        continue;
                    }

                    bool isFile = headPos % 2 == 0;
                    int sz = v - '0';
                    if (isFile)
                    {
                        yield return new FileDescriptor(fileId++, fileOffset, sz);
                    }

                    fileOffset += sz;
                    headPos++;
                }
            }
        }

        public void Defrag(IDefragMethod defragMethod)
        {
            defragMethod.Execute(this);
        }
    }

    public readonly record struct FileDescriptor(int Id, int Offset, int Size)
    {
        public void MoveFile(Span<int> diskSpan, int offset)
        {
            // Move file.
            diskSpan
                .Slice(Offset, Size)
                .CopyTo(diskSpan.Slice(offset, Size));
            diskSpan
                .Slice(Offset, Size)
                .Fill(Disk.FreeSpaceMarker);
        }
    };

    public interface IDefragMethod
    {
        public void Execute(Disk disk);
    }

    private sealed class DefragPartialFiles : IDefragMethod
    {
        public void Execute(Disk disk)
        {
            int[] blocks = disk.Blocks;
            int freeSpaceAt = 0;
            int i = blocks.Length - 1;
            while (i > freeSpaceAt)
            {
                freeSpaceAt = Array.IndexOf(blocks, Disk.FreeSpaceMarker, freeSpaceAt);

                for (; i > freeSpaceAt; i--)
                {
                    if (blocks[i] == Disk.FreeSpaceMarker)
                    {
                        continue;
                    }

                    // Move single block.
                    blocks[freeSpaceAt] = blocks[i];
                    blocks[i--] = Disk.FreeSpaceMarker;
                    break;
                }
            }
        }
    }

    private sealed class DefragCompleteFiles : IDefragMethod
    {
        public void Execute(Disk disk)
        {
            int[] freeSpaceBuffer = new int[9];
            Array.Fill(freeSpaceBuffer, Disk.FreeSpaceMarker);

            Span<int> diskSpan = disk.Blocks.AsSpan();
            foreach (FileDescriptor fd in disk.FileDescriptors.AsEnumerable().Reverse())
            {
                // Find free space for entire file (BEFORE itself) and move it.
                int freeSpaceAt = diskSpan.IndexOf(freeSpaceBuffer.AsSpan()[..fd.Size]);
                if (freeSpaceAt != -1 && freeSpaceAt <= fd.Offset)
                {
                    fd.MoveFile(diskSpan, freeSpaceAt);
                }
            }
        }
    }
}
