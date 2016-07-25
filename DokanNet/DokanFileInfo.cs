﻿using DokanNet.Native;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using static DokanNet.FormatProviders;

#pragma warning disable 649,169

namespace DokanNet
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public sealed class DokanFileInfo
    {
        private ulong _context;
        private readonly ulong _dokanContext;
        private readonly IntPtr _dokanOptions;
        private readonly uint _processId;

        [MarshalAs(UnmanagedType.U1)]
        private bool _isDirectory;

        [MarshalAs(UnmanagedType.U1)]
        private bool _deleteOnClose;

        [MarshalAs(UnmanagedType.U1)]
        private bool _pagingIo;

        [MarshalAs(UnmanagedType.U1)]
        private bool _synchronousIo;

        [MarshalAs(UnmanagedType.U1)]
        private bool _nocache;

        [MarshalAs(UnmanagedType.U1)]
        private bool _writeToEndOfFile;

        public object Context
        {
            get { return _context != 0 ? ((GCHandle)((IntPtr)_context)).Target : null; }
            set
            {
                if (_context != 0)
                {
                    ((GCHandle)((IntPtr)_context)).Free();
                    _context = 0;
                }
                if (value != null)
                {
                    _context = (ulong)(IntPtr)GCHandle.Alloc(value);
                }
            }
        }

        public int ProcessId => (int)_processId;

        public bool IsDirectory
        {
            get { return _isDirectory; }
            set { _isDirectory = value; }
        }

        public bool DeleteOnClose
        {
            get { return _deleteOnClose; }
            set { _deleteOnClose = value; }
        }

        public bool PagingIo => _pagingIo;

        public bool SynchronousIo => _synchronousIo;

        public bool NoCache => _nocache;

        public bool WriteToEndOfFile => _writeToEndOfFile;

        public WindowsIdentity GetRequestor()
        {
            try
            {
                return new WindowsIdentity(NativeMethods.DokanOpenRequestorToken(this));
            }
            catch
            {
                return null;
            }
        }

        public bool TryResetTimeout(int milliseconds)
        {
            return NativeMethods.DokanResetTimeout((uint)milliseconds, this);
        }

        private DokanFileInfo()
        {
        }

        public override string ToString()
        {
            return DokanFormat($"{{{Context}, {DeleteOnClose}, {IsDirectory}, {NoCache}, {PagingIo}, #{ProcessId}, {SynchronousIo}, {WriteToEndOfFile}}}");
        }
    }
}

#pragma warning restore 649, 169