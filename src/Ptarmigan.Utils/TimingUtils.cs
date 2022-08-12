﻿using System;
using System.Diagnostics;

namespace Ptarmigan.Utils
{
    public static class TimingUtils
    {

        public static void TimeIt(this Action action, string label = "")
            => TimeIt<bool>(action.ToFunction(), label);

        public static Disposer TimeIt(string label = "")
        {
            Console.WriteLine($"Starting timing {label}");
            var sw = Stopwatch.StartNew();
            return new Disposer(() => sw.OutputTimeElapsed(label));
        }

        public static string PrettyPrintTimeElapsed(this Stopwatch sw)
            => $"{sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}.{sw.Elapsed.Milliseconds}";

        public static void OutputTimeElapsed(this Stopwatch sw, string label)
            => Console.WriteLine($"{label}: time elapsed {sw.PrettyPrintTimeElapsed()}");

        public static string MSecToSecondsString(long msec)
            => $"{msec / 1000}.{msec % 1000}";

        public static T TimeIt<T>(this Func<T> function, string label = "")
        {
            var sw = Stopwatch.StartNew();
            var r = function();
            sw.OutputTimeElapsed(label);
            return r;
        }


        public static DateTime JanFirst1970 = new DateTime(1970, 1, 1);

        public static DateTime InitializedTime = DateTime.Now.ToUniversalTime();

        public static double NowInMSec()
            => (DateTime.Now.ToUniversalTime() - JanFirst1970).TotalMilliseconds;

        public static double ElapsedMSec()
            => (InitializedTime - JanFirst1970).TotalMilliseconds;


        public static long GetMSecElapsed(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            return sw.ElapsedMilliseconds;
        }
    }
}