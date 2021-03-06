﻿//-----------------------------------------------------------------------
// <copyright file="SslTlsHandshakeMonitor.cs" company="Microsoft Open Technologies, Inc.">
//Copyright © 2002-2007, The Mentalis.org Team
//Portions Copyright © Microsoft Open Technologies, Inc.
//All rights reserved.
//http://www.mentalis.org/ 
//Redistribution and use in source and binary forms, with or without modification, 
//are permitted provided that the following conditions are met:
//- Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 
//- Neither the name of the Mentalis.org Team, 
//nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, 
//THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
//INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; 
//OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
//-----------------------------------------------------------------------

using System;

using Org.Mentalis.Security.Ssl;
using Org.Mentalis.Security.Ssl.Shared;

namespace Org.Mentalis
{
	internal class SslTlsHandshakeMonitor : ISocketMonitor, IDisposable
	{
        internal HandshakeLayer Layer { get; private set; }
        public SecureSocket Socket { get; private set; }

        internal SslTlsHandshakeMonitor()
        {
        }

        public void Attach(SecureSocket socket)
        {
            this.Socket = socket;
        }

        internal void Attach(SecureSocket socket, ClientHandshakeLayer layer)
        {
            this.Socket = socket;
            this.Layer = layer;

            layer.OnHandshakeFinished += this.Socket.HandshakeFinishedHandler;
        }

        internal void Attach(SecureSocket socket, ServerHandshakeLayer layer)
        {
            this.Socket = socket;
            this.Layer = layer;

            layer.OnHandshakeFinished += this.Socket.HandshakeFinishedHandler;
        }

        public void Dispose()
        {
            if (this.Layer is ServerHandshakeLayer)
                (this.Layer as ServerHandshakeLayer).OnHandshakeFinished -= this.Socket.HandshakeFinishedHandler;
            else
                (this.Layer as ClientHandshakeLayer).OnHandshakeFinished -= this.Socket.HandshakeFinishedHandler;
        }
	}
}
