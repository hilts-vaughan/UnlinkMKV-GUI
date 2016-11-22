using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using static System.Char;

namespace UnlinkMKV_GUI.data.xml
{
    public class XmlMkvInfoSummaryMapper : IMkvInfoSummaryMapper
    {
        public XDocument DecodeStringIntoDocument(string sourceString)
        {
            var resultDocument = new XDocument();
            var root = new XElement("MKVInfo");
            resultDocument.Add(root);

            var nodeList = sourceString.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            for (var index = 0; index < nodeList.Count; index++)
            {
                var node = nodeList[index];
                if (node.Contains("|")) continue;
                var text = node.Replace("+", "").Trim();
                text = CreateFriendlyName(text);
                var rootNode = new XElement(text);
                root.Add(rootNode);
                PopulateChildrenFromNode(nodeList.Skip(index + 1).ToList(), node, rootNode, 1);
            }

            return resultDocument;
        }

        private void PopulateChildrenFromNode(List<string> followingBuffer, string thisNode, XContainer root,
            int depthSeek)
        {
            var prev = root;

            for (var index = 0; index < followingBuffer.Count; index++)
            {
                var childBufferNode = followingBuffer[index];
                var indexOfPlusSign = childBufferNode.IndexOf("+", StringComparison.Ordinal);

                if (indexOfPlusSign == depthSeek)
                {
                    var nodeData = GetNodeKeyPairValue(childBufferNode);
                    var newNode = new XElement(CreateFriendlyName(nodeData.Item1)) {Value = nodeData.Item2};
                    root.Add(newNode);

                    // Update the previous pointer
                    prev = newNode;
                }
                else if (indexOfPlusSign == depthSeek + 1)
                {
                    // you're a child, recurse down
                    var newBuffer = followingBuffer.Skip(index).ToList();


                    // Your buffer should only contain things that are not at or below your level...
                    var i = 0;
                    for (i = 0; i < newBuffer.Count; i++)
                    {
                        var s = newBuffer[i];
                        if (s.IndexOf("+", StringComparison.Ordinal) < indexOfPlusSign)
                        {
                            break;
                        }
                    }

                    newBuffer = newBuffer.Take(i).ToList();

                    PopulateChildrenFromNode(newBuffer, childBufferNode, prev, depthSeek + 1);

                    // Seek in based on the run length...
                    var offset = GetPlusSignRunLengthOfDepth(newBuffer, depthSeek + 1);
                    index += offset - 1;

                }
                else if(indexOfPlusSign == depthSeek - 1)
                {
                    // you're leaving your parent, it's time to move on with your life :)
                    break;
                }
            }
        }

        private Tuple<string, string> GetNodeKeyPairValue(string node)
        {
            var splits = node.Split(":".ToCharArray(), 2);
            var key = splits[0];

            var value = "";
            if (splits.Length > 1)
            {
                value = splits[1];
            }

            return new Tuple<string, string>(key.Trim(), value.Trim());
        }

        private string CreateFriendlyName(string input)
        {
            var pascal = input.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int index = 0; index < pascal.Count; index++)
            {
                var str = pascal[index];
                pascal[index] = ToUpper(str[0]) + str.Substring(1);
            }

            var newInput = string.Join("", pascal);

            return Regex.Replace(newInput,"[^A-Za-z _]","").Replace(" ", "_");
        }

        private int GetPlusSignRunLengthOfDepth(IEnumerable<string> input, int target)
        {
            var count = 0;
            foreach (var x in input)
            {
                if (x.IndexOf("+", StringComparison.Ordinal) >= target)
                {
                    count++;
                }
                else
                {
                    return count;
                }
            }

            return count;
        }

    }
}