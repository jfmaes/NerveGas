using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MinHook;

namespace NerveGas
{
    class Program
    {
        [DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern STRUCTS.NtStatus EtwEventWrite(ulong regHandle, IntPtr eventDescriptor, uint userDataCount, IntPtr userData);
        public static DELEGATES.EtwEventWriteFull ogEtwEventWriteFull;
        public static DELEGATES.EtwEventWrite ogEtwEventWrite;

        public static STRUCTS.NtStatus EtwEventWriteFull_Hook(ulong regHandle, IntPtr eventDescriptorPointer, ushort eventProperty, IntPtr activityId, IntPtr relatedActivityId, uint userDataCount, IntPtr userData)
        {
            Console.WriteLine("ETWEventWriteFull CALLED");
            return (STRUCTS.NtStatus)ogEtwEventWriteFull(regHandle, eventDescriptorPointer, eventProperty, activityId, relatedActivityId, userDataCount, userData);
        }
        public static STRUCTS.NtStatus EtwEventWrite_Hook(ulong regHandle, IntPtr eventDescriptor, uint userDataCount, IntPtr userData)
        {
            Console.WriteLine("ETWEventWrite CALLED");
            return (STRUCTS.NtStatus)ogEtwEventWrite(regHandle, eventDescriptor, userDataCount, userData);
        }

        public static void enableETWHook()
        {
            using (HookEngine engine = new HookEngine())
            {
                ogEtwEventWriteFull = engine.CreateHook("ntdll.dll", "EtwEventWriteFull", new DELEGATES.EtwEventWriteFull(EtwEventWriteFull_Hook));
                ogEtwEventWrite = engine.CreateHook("ntdll.dll", "EtwEventWrite", new DELEGATES.EtwEventWrite(EtwEventWrite_Hook));
                engine.EnableHooks();
            }
        }

        public static void sayHello()
        {
            Console.WriteLine("hello");
        }
        static void Main(string[] args)
        {
            enableETWHook();
            Console.WriteLine("hook got placed");
            EtwEventWrite(0, IntPtr.Zero, 0, IntPtr.Zero);
            sayHello();
            byte[] assemblyBytes = File.ReadAllBytes(@"C:\Users\Jean\source\repos\ReflectionDemoApp\bin\Release\ReflectionDemoApp.exe");
            Assembly.Load(assemblyBytes);
            Console.ReadKey();
        }
    }
}
