using PackageSplitter.Model.Parser;
using PackageSplitter.ViewModel.Convertrs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;

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
                OnPropertyChanged("BackGround");
                if (value == eParseStatus.InProgress)
                    LoadTime = TimeSpan.Zero;
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

        public SolidColorBrush BackGround
        {
            get
            {
                SolidColorBrush brush;
                switch (_Status)
                {
                    case eParseStatus.InProgress:
                        brush = "cCellYellow".FindResource<SolidColorBrush>();
                        break;
                    case eParseStatus.Done:
                        brush = "cCellAdd".FindResource<SolidColorBrush>();
                        break;
                    case eParseStatus.Fail:
                        brush = "cCellDelete".FindResource<SolidColorBrush>();
                        break;
                    default:
                        brush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        break;
                }
                return brush;
            }
        }

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
