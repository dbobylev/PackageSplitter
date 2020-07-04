using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Tests.Split.Source
{
    class TestPackage1 :PackageText
    {
        public const string METHOD1_NAME = "ImportData";

        public TestPackage1()
        {
            PackageName = "Package1";

            SpecText += HelperPackageParts.GetHeader(PackageName);
            SpecText += HelperPackageParts.GetVariableDeclaration("LogLevel", eVariableType.number, "-- настройка", "3");
            SpecText += HelperPackageParts.GetVariableDeclaration("LogLevel2", eVariableType.number, "-- настройка", "5");
            SpecText += HelperPackageParts.GetSpecMethod1(METHOD1_NAME);
            SpecText += HelperPackageParts.GetBottom(PackageName);

            BodyText += HelperPackageParts.GetHeader(PackageName, true);
            BodyText += HelperPackageParts.GetBodyMethod1(METHOD1_NAME);
            BodyText += HelperPackageParts.GetBottom(PackageName);
        }
    }
}
