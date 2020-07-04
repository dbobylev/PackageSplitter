using DataBaseRepository.Model;
using PackageSplitter.Tests.Split.Source;
using PackageSplitter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackageSplitter.Tests.Split.Cases
{
    class SplitCase1 : SplitPackageCaseBase
    {
        public SplitCase1() : base (new TestPackage1())
        {
            var element = _splitter.Elements.First(x => x.PackageElementName == TestPackage1.METHOD1_NAME);
            element.NewBody = eElementStateType.Add;
            element.NewSpec = eElementStateType.Add;
            element.OldBody = eElementStateType.CreateLink;

            ExceptedNewBody = "123";
            ExceptedNewSpec = "123";
            ExceptedOldBody = "123";
        }
    }
}
