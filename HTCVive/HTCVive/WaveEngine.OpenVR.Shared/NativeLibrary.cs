#region File Description
//-----------------------------------------------------------------------------
// NativeLibraries
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using WaveEngine.Common;
#endregion

namespace WaveEngine.OpenVR
{
    /// <summary>
    /// This static class is used to load the native libraries.
    /// </summary>
    public static class NativeLibraries
    {
        [SuppressUnmanagedCodeSecurity, DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        static NativeLibraries()
        {
            LoadNativeLibrary("openvr_api.dll");
        }

        /// <summary>
        /// Load a native library embedded in the current assembly.
        /// </summary>
        /// <param name="fileName">Library name.</param>
        private static void LoadNativeLibrary(string fileName)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);

            if (!File.Exists(path) || (LoadLibrary(path) == IntPtr.Zero))
            {
                path = Path.Combine(Path.GetTempPath(), fileName);

                try
                {
                    Type type = typeof(NativeLibraries);

                    string name;

                    if (Environment.Is64BitProcess)
                    {
                        name = type.Namespace + ".Libs.x64." + fileName;
                    }
                    else
                    {
                        name = type.Namespace + ".Libs.x86." + fileName;
                    }

                    using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        Assembly.GetExecutingAssembly().GetManifestResourceStream(name).CopyTo(stream);
                    }
                }
                catch
                {
                }

                LoadLibrary(path);
            }
        }
    }
}
