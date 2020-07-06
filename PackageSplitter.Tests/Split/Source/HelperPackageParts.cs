using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Tests.Split.Source
{
    [Flags]
    enum eVariableType { varchar2, date, number }


    static class HelperPackageParts
    {
        public static string GetHeader(string name, bool IsPackageBody = false) => $"create or replace package {(IsPackageBody ? "body " : string.Empty)} {name} \r\nis\r\n\r\n";

        public static string GetBottom(string name) => $"end {name};\r\n/";

        public static string GetVariableDeclaration(string name, eVariableType variabletype, string InitComment = null, string defaultValue = null)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(InitComment))
                sb.AppendLine($"  {InitComment}");
            sb.Append($"  {name} {variabletype}");
            if (variabletype == eVariableType.varchar2)
                sb.Append("(100)");
            if (!string.IsNullOrEmpty(defaultValue))
                sb.Append($" default {defaultValue}");
            sb.Append(";\r\n");
            return sb.ToString();
        }

        public static string GetBodyMethod1(string name)
        {
            var s = @$"  -- Процедура запускает что-то в цикле
  procedure {name}(pID in number) is
  begin
    for i in (select SomeProperty from testtable where id = pid)
    loop
      runSomething(i.SomeProperty);
    end loop;
  end {name};
";
            return s;
        }

        public static string GetSpecMethod1(string name)
        {
            var s = @$"  -- Процедура запускает что-то в цикле
  procedure {name}(pID in number);
";
            return s;
        }
    }
}
