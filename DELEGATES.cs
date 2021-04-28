using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NerveGas
{
    class DELEGATES
    {
        /*https://docs.microsoft.com/en-us/windows/win32/devnotes/etweventwritefull*/
        /*
         *EtwEventWriteFull(
           __in REGHANDLE RegHandle,
           __in PCEVENT_DESCRIPTOR EventDescriptor,
           __in USHORT EventProperty,
           __in_opt LPCGUID ActivityId,
           __in_opt LPCGUID RelatedActivityId,
           __in ULONG UserDataCount,
           __in_ecount_opt(UserDataCount) PEVENT_DATA_DESCRIPTOR UserData
           );
         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate STRUCTS.NtStatus EtwEventWriteFull(ulong regHandle, IntPtr eventDescriptorPointer, ushort eventProperty, IntPtr activityId, IntPtr relatedActivityId, uint userDataCount, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate STRUCTS.NtStatus EtwEventWrite(ulong regHandle, IntPtr eventDescriptor, uint userDataCount, IntPtr userData);
    }
}
