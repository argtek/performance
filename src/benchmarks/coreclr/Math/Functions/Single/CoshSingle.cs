﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Functions
{
    public partial class MathTests
    {
        // Tests MathF.Cosh(float) over 5000 iterations for the domain -1, +1

        private const float coshSingleDelta = 0.0004f;
        private const float coshSingleExpectedResult = 5876.02588f;

        [Benchmark]
        public void CoshSingleBenchmark() => CoshSingleTest();

        public static void CoshSingleTest()
        {
            var result = 0.0f; var value = -1.0f;

            for (var iteration = 0; iteration < iterations; iteration++)
            {
                value += coshSingleDelta;
                result += MathF.Cosh(value);
            }

            var diff = MathF.Abs(coshSingleExpectedResult - result);

            if (diff > singleEpsilon)
            {
                throw new Exception($"Expected Result {coshSingleExpectedResult,10:g9}; Actual Result {result,10:g9}");
            }
        }
    }
}
