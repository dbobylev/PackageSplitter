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

            ExceptedPart[eSplitterObjectType.NewSpec] = @"create or replace package TESTREPOSITORY.NEW_PACKAGE1 is

  -- Процедура запускает что-то в цикле
  procedure ImportData(pID in number);

end NEW_PACKAGE1;
/
";

            ExceptedPart[eSplitterObjectType.NewBody] = @"create or replace package body TESTREPOSITORY.NEW_PACKAGE1 is

  -- Процедура запускает что-то в цикле
  procedure ImportData(pID in number) is
  begin
    for i in (select SomeProperty from testtable where id = pid)
    loop
      runSomething(i.SomeProperty);
    end loop;
  end ImportData;

end NEW_PACKAGE1;
/
";

            ExceptedPart[eSplitterObjectType.OldBody] = @"create or replace package body  Package1 
is

  -- Процедура запускает что-то в цикле
  procedure ImportData(pID in number) is
  begin
    new_package1.ImportData(pID => pID);
  end ImportData;
end Package1;
/
";
        }
    }
}
