namespace UnlinkMKV_GUI.merge
{
    public class MergeResult
    {
        public MergeResult(string resultFilePath)
        {
            ResultFilePath = resultFilePath;
        }

        public string ResultFilePath { get; private set; }
    }
}