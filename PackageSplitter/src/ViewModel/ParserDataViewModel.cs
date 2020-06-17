using PackageSplitter.Model.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PackageSplitter.ViewModel
{
    public class ParserDataViewModel :PropertyChangedBase
    {
        public string FileName { get; private set; }

        public long FileSizeKB { get; private set; }

        private eParseStatus _Status;
        public eParseStatus Status 
        { 
            get => _Status;
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
                if (value == eParseStatus.InProgress)
                {
                    LoadTime = TimeSpan.Zero;
                } 
            }
        }

        private TimeSpan _LoadTime;
        public TimeSpan LoadTime
        {
            get => _LoadTime;
            set
            {
                _LoadTime = value;
                OnPropertyChanged();
            }
        }

        public bool InProgress => _Status == eParseStatus.InProgress;

        public ParserDataViewModel(string path)
        {
            var fileInfo = new FileInfo(path);

            FileName = fileInfo.Name;
            FileSizeKB = fileInfo.Length / 1024;
            Status = eParseStatus.Wait;
            LoadTime = TimeSpan.Zero;
        }

        public ParserDataViewModel()
        {

        }
    }
}
