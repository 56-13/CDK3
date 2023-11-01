using System;
using System.Xml;
using System.IO;

namespace CDK.Assets.Animations.Components
{
    public struct AnimationLoop : IEquatable<AnimationLoop>
    {
        public int Count;
        public bool RoundTrip;
        public bool Finish;

        public bool IsDefault => Count == 0 && !RoundTrip && !Finish;

        public AnimationLoop(int count, bool roundTrip, bool finish)
            : this()
        {
            Count = count;
            RoundTrip = roundTrip;
            Finish = finish;
        }

        internal AnimationLoop(XmlNode node, string name) 
            : this()
        {
            var str = node.ReadAttributeString(name);

            if (str != null)
            {
                var ps = str.Split(',');
                Count = int.Parse(ps[0]);
                RoundTrip = bool.Parse(ps[1]);
                Finish = bool.Parse(ps[2]);
            }
        }

        internal void Save(XmlWriter writer, string name)
        {
            if (Count != 0 || RoundTrip || Finish) writer.WriteAttributeString(name, $"{Count},{RoundTrip},{Finish}");
        }

        internal float GetProgress(float progress, out int randomSeq0, out int randomSeq1)
        {
            var seq = (int)progress;

            if (Count != 0 && seq >= Count)
            {
                seq = Count - 1;
                progress = RoundTrip && (Count & 1) == 0 ? 0 : 1;
            }
            else
            {
                progress -= seq;

                if (RoundTrip && (seq & 1) == 1)
                {
                    progress = 1 - progress;
                }
            }

            if (RoundTrip)
            {
                randomSeq0 = (seq + 1) & 0x7ffffffe;
                randomSeq1 = seq ^ 1 | 1;
            }
            else
            {
                randomSeq0 = randomSeq1 = seq;
            }
            return progress;
        }

        internal float GetProgress(float progress) => GetProgress(progress, out _, out _);

        internal void Build(BinaryWriter writer)
        {
            writer.Write((ushort)Count);
            writer.Write(RoundTrip);
            writer.Write(Finish);
        }

        public static bool operator ==(AnimationLoop a, AnimationLoop b) => a.Equals(b);
        public static bool operator !=(AnimationLoop a, AnimationLoop b) => !a.Equals(b);
        public bool Equals(AnimationLoop other) => Count == other.Count && RoundTrip == other.RoundTrip && Finish == other.Finish;
        public override bool Equals(object obj) => obj is AnimationLoop loop && Equals(loop);
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Count.GetHashCode());
            hash.Combine(RoundTrip.GetHashCode());
            hash.Combine(Finish.GetHashCode());
            return hash;
        }
    }
}
