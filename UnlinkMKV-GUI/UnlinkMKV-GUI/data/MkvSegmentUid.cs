using System;

namespace UnlinkMKV_GUI.data
{
    public class MkvSegmentUid
    {
        private string _hexString;

        public MkvSegmentUid(string hexString)
        {
            this._hexString = hexString.Substring(hexString.IndexOf("0x", StringComparison.Ordinal));
        }

        public bool IsSame(MkvSegmentUid segment)
        {
            if (segment == null)
            {
                return false;
            }
            return segment._hexString == this._hexString;
        }

        public override string ToString()
        {
            return this._hexString;
        }
    }
}