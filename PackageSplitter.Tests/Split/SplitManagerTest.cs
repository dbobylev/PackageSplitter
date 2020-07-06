using Antlr4.Runtime.Atn;
using DataBaseRepository.Model;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
using PackageSplitter.Tests.Split.Cases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace PackageSplitter.Tests.Split
{
    class SplitManagerTest
    {
        [SetUp]
        public static void BeforeRun()
        {
            string MAIN_SETTINGS_FILENAME = "PackageSplitterSettings.Json";
            var configuration = new ConfigurationBuilder().AddJsonFile(MAIN_SETTINGS_FILENAME).Build();
            Seri.InitConfig(configuration);
            Config.InitConfig(configuration, MAIN_SETTINGS_FILENAME);
        }

        [Test]
        public static void SplitTest()
        {
            SplitPackageCaseBase splitCase = new SplitCase1();

            RepositoryPackage NewRepositoryPackage = new RepositoryPackage("NEW_" + splitCase.RepositoryPackage.Name, splitCase.RepositoryPackage.Owner);

            Config.Instanse().NewPackageOwner = NewRepositoryPackage.Owner;
            Config.Instanse().NewPackageName = NewRepositoryPackage.Name;

            // Сортируем по убыванию, так как мы сначала должны создать новый пакет, затем обновить старый
            foreach(eSplitterObjectType splitterObjectType in Enum.GetValues(typeof(eSplitterObjectType)).Cast<eSplitterObjectType>().OrderByDescending(x=>x))
            {
                if (splitCase.ExceptedPart.ContainsKey(splitterObjectType))
                {
                    Seri.Log.Information($"Begin split: {splitterObjectType}");

                    // Если мы дошли до OldSpec, и до этого генерировали OldBody, то OldSpec не должно повторно запускаться.
                    if (splitterObjectType != eSplitterObjectType.OldSpec || !splitCase.ExceptedPart.ContainsKey(eSplitterObjectType.OldBody))
                        splitCase.RunSplit(splitterObjectType, eSplitParam.GenerateHeader | eSplitParam.DirectlyUpdateRep);

                    var ExceptedText = splitCase.ExceptedPart[splitterObjectType];
                    Seri.Log.Information($"ExceptedText: {ExceptedText}");
                    var ActualText = GetText(splitterObjectType, splitCase.RepositoryPackage, NewRepositoryPackage);
                    Seri.Log.Information($"ActualText: {ActualText}");

                    Assert.AreEqual(ExceptedText, ActualText);
                    Seri.Log.Information($"Split Done");
                }
            }
        }

        private static string GetText(eSplitterObjectType splitterObjectType, RepositoryPackage oldRepositoryPackage, RepositoryPackage newRepositoryPackage)
        {
            var repositoryPackage = splitterObjectType.IsNew() ? newRepositoryPackage : oldRepositoryPackage;
            var path = splitterObjectType.IsSpec() ? repositoryPackage.SpecRepFullPath : repositoryPackage.BodyRepFullPath;
            return File.ReadAllText(path);
        }
    }
}
