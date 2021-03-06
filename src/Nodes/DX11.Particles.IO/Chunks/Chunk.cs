﻿#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;
#endregion usings

namespace DX11.Particles.IO
{
    #region Chunk
    public class Chunk
    {
        #region Fields
        private const int DefaultBufferSize = 4096;
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
        
        public int Id;
        public string FileName;

        public int ElementCount; // number of elements in this chunk
        public int BytesPerElement; // one element consists of sizeof(single) * xyzrgb(a) bytes

        public bool finishedLoading = false;

        public MemoryComStream MemoryStream;
        public BinaryWriter BinaryWriter;
        #endregion Fields

        public Chunk(int id, string fileName, int bytesPerElement)
        {
            Id = id;
            FileName = fileName;
            BytesPerElement = bytesPerElement;
            MemoryStream = new MemoryComStream();
            BinaryWriter = new BinaryWriter(this.MemoryStream);
        }

        public void UpdateElementCount()
        {
            ElementCount = Convert.ToInt32(MemoryStream.Length / BytesPerElement);
        }
        
    }
    #endregion Chunk

    #region ChunkAccessData
    public class ChunkAccessData
    {
        public int chunkId;

        public int streamPosition = 0;
        public int streamLimit = -1;
        public int streamDownsampling = 1;
        public int streamDownsamplingOffset = 0;
        public bool streamFinished = false;
        public double streamLocktime = 0;

        public bool isLocked = false;
        System.Timers.Timer timer;

        public ChunkAccessData(int chunkId, int streamLimit, int streamDownsampling, int streamDownsamplingOffset, double streamLocktime)
        {
            this.chunkId = chunkId;
            this.streamLimit = streamLimit;
            this.streamDownsampling = streamDownsampling;
            this.streamDownsamplingOffset = streamDownsamplingOffset;
            this.streamLocktime = streamLocktime;
        }

        public void StartLock()
        {
            if (streamLocktime > 0)
            {
                timer = new System.Timers.Timer();
                timer.Interval = streamLocktime;
                timer.Elapsed += Unlock;
                timer.Enabled = true;
                this.isLocked = true;
            }
        }

        private void Unlock(object sender, ElapsedEventArgs e)
        {
            this.isLocked = false;
        }

    }
    #endregion ChunkAccessData
}
