﻿namespace PC.Common
{
    public class ScanResult
    {
        public string Id { get; set; }
        public int LineNumber { get; set; }
        public string Line { get; set; }
        public string SearchPattern { get; set; }
        public string FilePath { get; set; }
    }
}
