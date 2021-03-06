﻿using Owin;

namespace Microsoft.Http2.Owin.Server.Service
{
    public class Startup
    {
        /// <summary>
        /// This class is used for building katana stack in the Http2ServerService.
        /// </summary>
        /// <param name="builder">This object is used for building katana stack</param>
        public void Configuration(IAppBuilder builder)
        {
            builder.UseHttp2();
            builder.UseStaticFiles("root");
        }

    }
}
