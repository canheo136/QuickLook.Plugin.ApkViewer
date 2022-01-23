using System;
using System.IO;
using System.Linq;
using AAPTForNet;

namespace StartupDebuging {
    class Program {

        static readonly string[] APK_FILES = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "apks"), "*.apk");

        public static void Main(string[] args) {

            if (APK_FILES == null || APK_FILES.Length == 0)
                throw new Exception("There are no apk files to run AAPT");

            var apks = APK_FILES.Take(20).Select(f => {

                try {
                    var apk = AAPTool.Decompile(f);
                    return apk;
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                    Console.WriteLine("===========================================");
                    return null;
                }

            }).ToList();

#pragma warning disable CS0219
            var breakPoint = "Break me";
        }
    }
}
