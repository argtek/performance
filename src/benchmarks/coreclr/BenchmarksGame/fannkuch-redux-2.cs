// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Adapted from fannkuch-redux C# .NET Core #2 program
// http://benchmarksgame.alioth.debian.org/u64q/program.php?test=fannkuchredux&lang=csharpcore&id=2
// aka (as of 2017-09-01) rev 1.2 of https://alioth.debian.org/scm/viewvc.php/benchmarksgame/bench/fannkuchredux/fannkuchredux.csharp-2.csharp?root=benchmarksgame&view=log
// Best-scoring single-threaded C# .NET Core version as of 2017-09-01

/* The Computer Language Benchmarks Game
   http://benchmarksgame.alioth.debian.org/

   contributed by Isaac Gouy, transliterated from Mike Pall's Lua program 
*/

using BenchmarkDotNet.Attributes;
using Benchmarks;

namespace BenchmarksGame
{
    [BenchmarkCategory(Categories.CoreCLR, Categories.BenchmarksGame)]
    public class FannkuchRedux_2
    {
        [Benchmark(Description = nameof(FannkuchRedux_2))]
        [Arguments(10, 73196)]
        public int[] RunBench(int n, int expectedSum) => fannkuch(n); // expectedSum argument needs to remain to keep old benchmark id in BenchView, do NOT remove it

        public int[] fannkuch(int n)
        {
            int[] p = new int[n], q = new int[n], s = new int[n];
            int sign = 1, maxflips = 0, sum = 0, m = n - 1;
            for (int i = 0; i < n; i++) { p[i] = i; q[i] = i; s[i] = i; }
            do
            {
                // Copy and flip.
                var q0 = p[0];                                     // Cache 0th element.
                if (q0 != 0)
                {
                    for (int i = 1; i < n; i++) q[i] = p[i];             // Work on a copy.
                    var flips = 1;
                    do
                    {
                        var qq = q[q0];
                        if (qq == 0)
                        {                                // ... until 0th element is 0.
                            sum += sign * flips;
                            if (flips > maxflips) maxflips = flips;   // New maximum?
                            break;
                        }
                        q[q0] = q0;
                        if (q0 >= 3)
                        {
                            int i = 1, j = q0 - 1, t;
                            do { t = q[i]; q[i] = q[j]; q[j] = t; i++; j--; } while (i < j);
                        }
                        q0 = qq; flips++;
                    } while (true);
                }
                // Permute.
                if (sign == 1)
                {
                    var t = p[1]; p[1] = p[0]; p[0] = t; sign = -1; // Rotate 0<-1.
                }
                else
                {
                    var t = p[1]; p[1] = p[2]; p[2] = t; sign = 1;  // Rotate 0<-1 and 0<-1<-2.
                    for (int i = 2; i < n; i++)
                    {
                        var sx = s[i];
                        if (sx != 0) { s[i] = sx - 1; break; }
                        if (i == m) return new int[] { sum, maxflips };  // Out of permutations.
                        s[i] = i;
                        // Rotate 0<-...<-i+1.
                        t = p[0]; for (int j = 0; j <= i; j++) { p[j] = p[j + 1]; }
                        p[i + 1] = t;
                    }
                }
            } while (true);
        }
    }
}
