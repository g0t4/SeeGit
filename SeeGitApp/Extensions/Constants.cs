﻿using System;
using System.Collections.Generic;

namespace SeeGit
{
    public static class StandardLayoutAlgorithms
    {
        // FYI some were different in GraphSharp (i.e. EfficientSugiyama IIRC)
        // https://github.com/KeRNeLith/GraphShape/blob/master/src/GraphShape/Algorithms/Layout/StandardLayoutAlgorithmFactory.cs#L22-L51
        // BTW, WhyTF are these hardcoded in the library and private?! it's a contract that consumers have to pass, no reason to be private, unless I am doing something wrong and I shouldn't be picking this myself OR I should use some other intermediary to use it?!
        public const string Circular = "Circular";
        public const string Tree = "Tree";
        public const string FR = "FR";
        public const string BoundedFR = "BoundedFR";
        public const string KK = "KK";
        public const string ISOM = "ISOM";
        public const string LinLog = "LinLog";
        public const string Sugiyama = "Sugiyama";
        public const string CompoundFDP = "CompoundFDP";
        public const string Random = "Random";
    }

    public static class StandardOverlapRemovalAlgorithms
    {
        // https://github.com/KeRNeLith/GraphShape/blob/master/src/GraphShape/Algorithms/OverlapRemoval/StandardOverlapRemovalAlgorithmFactory.cs#L15-L21
        public const string FSA = "FSA";
        public const string OneWayFSA = "OneWayFSA";
    }

    public static class Extensions
    {
        public static string AtMost(this string s, int characterCount)
        {
            if (s == null) return null;
            if (s.Length <= characterCount)
            {
                return s;
            }
            return s.Substring(0, characterCount);
        }

        public static string StringJoin<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}