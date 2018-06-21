// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// In CoreCLR String.Format(ref, ref) is small and readily inlined.
// The inline introduces a System.Parms GC struct local which is
// untracked and must be zero initialized in the prolog. When the
// inlined callsite is in a cold path, the inline hurts performance.
//
// There are two test methods below, one of which calls String.Format
// on a cold path and the other which has similar structure but
// does not call String.Format. Expectation is that they will have
// similar performance.
//
// See https://github.com/dotnet/coreclr/issues/7569 for context.

using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Benchmarks;

namespace Inlining
{
[BenchmarkCategory(Categories.CoreCLR, Categories.Inlining)]
public class InlineGCStruct
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int FastFunctionNotCallingStringFormat(int param)
    {
        if (param < 0)
        {
            throw new Exception(String.Format("We do not like the value {0:N0}.", param));
        }

        if (param == int.MaxValue)
        {
            throw new Exception(String.Format("{0:N0} is maxed out.", param));
        }

        if (param > int.MaxValue / 2)
        {
            throw new Exception(String.Format("We do not like the value {0:N0} either.", param));
        }

        return param * 2;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int FastFunctionNotHavingStringFormat(int param)
    {
        if (param < 0)
        {
            throw new ArgumentOutOfRangeException("param", "We do not like this value.");
        }

        if (param == int.MaxValue)
        {
            throw new ArgumentOutOfRangeException("param", "Maxed out.");
        }

        if (param > int.MaxValue / 2)
        {
            throw new ArgumentOutOfRangeException("param", "We do not like this value either.");
        }

        return param * 2;
    }

    [Benchmark]
    public int WithFormat() => FastFunctionNotCallingStringFormat(11);

    [Benchmark]
    public int WithoutFormat() => FastFunctionNotHavingStringFormat(11);
}
}

