using System;
using System.IO;
using UnlinkMKV_GUI.data;

namespace UnlinkMKV_GUI.merge.extract
{
    public abstract class BaseMatroskaExtractor : IDisposable
    {
        private string _oldPath;

        /// <summary>
        /// Performs the request extract of elements on the given file.
        /// </summary>
        public abstract void PerformExtraction(MkvInfo segment, string file);


        protected string CreateAndChangeTo(string request)
        {
            // OK, change directory to make sure things are outputted there
            _oldPath = Environment.CurrentDirectory;

            var newPath = Path.Combine(_oldPath, request);
            Directory.CreateDirectory(newPath);
            Environment.CurrentDirectory = newPath;

            return this._oldPath;
        }

        public void Dispose()
        {
            // Make sure we restore once we're done with this
            Environment.CurrentDirectory = _oldPath;
        }

    }
}