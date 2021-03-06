﻿using System;

namespace Microsoft.Http2.Protocol.Framing
{
    internal class ContinuationFrame : Frame, IEndStreamFrame, IHeadersFrame
    {

        private const int PreambleSizeWithoutPriority = 8;
        private readonly HeadersList _headers = new HeadersList();

        public HeadersList Headers
        {
            get
            {
                return _headers;
            }
        }

        public bool IsEndHeaders
        {
            get
            {
                return (Flags & FrameFlags.EndHeaders) == FrameFlags.EndHeaders;
            }
            set
            {
                if (value)
                {
                    Flags |= FrameFlags.EndHeaders;
                }
            }
        }

        // 8 bits, 24-31
        public bool IsEndStream
        {
            get
            {
                return (Flags & FrameFlags.EndStream) == FrameFlags.EndStream;
            }
            set
            {
                if (value)
                {
                    Flags |= FrameFlags.EndStream;
                }
            }
        }

        public ArraySegment<byte> CompressedHeaders
        {
            get
            {
                const int offset = Constants.FramePreambleSize;
                return new ArraySegment<byte>(Buffer, offset, Buffer.Length - offset);
            }
        }

        //outgoing
        public ContinuationFrame(int streamId, byte[] headerBytes)
        {
            _buffer = new byte[headerBytes.Length + PreambleSizeWithoutPriority];

            StreamId = streamId;
            FrameType = FrameType.Headers;
            FrameLength = Buffer.Length - Constants.FramePreambleSize;

            // Copy in the headers
            System.Buffer.BlockCopy(headerBytes, 0, Buffer, PreambleSizeWithoutPriority, headerBytes.Length);
        }

        //outgoing
        public ContinuationFrame(Frame preamble)
            :base(preamble)
        {

        }
    }
}
