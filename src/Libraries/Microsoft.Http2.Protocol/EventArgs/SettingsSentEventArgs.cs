﻿using Microsoft.Http2.Protocol.Framing;

namespace Microsoft.Http2.Protocol.EventArgs
{
    /// <summary>
    /// This class is designed for future usage
    /// </summary>
    public class SettingsSentEventArgs : System.EventArgs
    {
        public SettingsFrame SettingsFrame { get; private set; }
        
        public SettingsSentEventArgs(SettingsFrame frame)
        {
            SettingsFrame = frame;
        }
    }
}
