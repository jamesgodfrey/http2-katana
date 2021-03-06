﻿using System.IO;
using Microsoft.Http2.Protocol.Framing;
using System;
using System.Threading;

namespace Microsoft.Http2.Protocol.IO
{
    // Queue up frames to send, including headers, body, flush, pings, etc.
    internal sealed class WriteQueue : IDisposable
    {
        private readonly IQueue _messageQueue;
        private readonly Stream _stream;
        private bool _disposed;
        private readonly object _writeLock = new object();
        private readonly ActiveStreams _streams;

        public bool IsPriorityTurnedOn { get; private set; }

        public WriteQueue(Stream stream, ActiveStreams streams, bool isPriorityTurnedOn)
        {
            if (stream == null)
                throw new ArgumentNullException("io stream is null");

            if (streams == null)
                throw new ArgumentNullException("streams collection is null");

            //Priorities are turned on for debugging
            IsPriorityTurnedOn = isPriorityTurnedOn;
            _streams = streams;

            if (IsPriorityTurnedOn)
            {
                _messageQueue = new PriorityQueue();
            }
            else
            {
                _messageQueue = new QueueWrapper();
            }
            _stream = stream;
            _disposed = false;
        }

        // Queue up a fully rendered frame to send
        public void WriteFrame(Frame frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame is null");

            //Do not write to already closed stream
            if (frame.FrameType != FrameType.Settings
                && frame.FrameType != FrameType.GoAway
                && frame.FrameType != FrameType.Ping
                && _streams[frame.StreamId] == null)
            {
                return;
            }

            var priority = frame.StreamId != 0 ? _streams[frame.StreamId].Priority : Constants.DefaultStreamPriority;

            IQueueItem entry;

            if (IsPriorityTurnedOn)
            {
                entry = new PriorityQueueEntry(frame, priority);
            }
            else
            {
                entry = new QueueEntry(frame);
            }

            _messageQueue.Enqueue(entry);
        }

        public void PumpToStream(CancellationToken cancel)
        {
            if (cancel == null)
                throw new ArgumentNullException("cancellation token is null");

            while (!_disposed)
            {
                if (cancel.IsCancellationRequested)
                    cancel.ThrowIfCancellationRequested();

                //Send one at a time
                lock (_writeLock)
                {
                    if (_messageQueue.Count > 0)
                    {
                        var entry = _messageQueue.Dequeue();
                        if (entry != null)
                        {
                            _stream.Write(entry.Buffer, 0, entry.Buffer.Length);
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }

                _stream.Flush();
            }
        }

        public void Flush()
        {
            while (_messageQueue.Count > 0 && !_disposed)
            {
                var entry = _messageQueue.Dequeue();
                if (entry != null)
                {
                    _stream.Write(entry.Buffer, 0, entry.Buffer.Length);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }
    }
}
