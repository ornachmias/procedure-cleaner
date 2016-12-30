using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PC.Common;
using PC.DataAccess;
using PC.Scanner;

namespace PC.Tests
{
    [TestClass]
    public class CodeScannerTest
    {
        [TestMethod]
        public void GetUnusedStoredProcedures_SingleFile_SingleThread_PartialOcurranceFound()
        {
            // Arrange
            var storedProcedureRepository = new Mock<IStoredProceduresRepository>();
            var codeRepository = new Mock<ICodeRepository>();

            storedProcedureRepository.Setup(x => x.GetStoreProceduresNames(It.IsAny<string>()))
                .Returns(new List<string> { "First_Stored_Procedure", "Second_Stored_Procedure" });
            codeRepository.Setup(x => x.GetCodeFilesPaths(It.IsAny<string>()))
                .Returns(new List<string> { "FirstCodeFile" });
            var scanResult = new ScanResult
            {
                Id = "1",
                Line = "Dummy line containing one of the search patterns",
                LineNumber = 5,
                SearchPattern = "First_Stored_Procedure"
            };

            codeRepository.Setup(x => x.SearchFile(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(new List<ScanResult> { scanResult });
            var codeScanner =
                new CodeScanner(codeRepository.Object, storedProcedureRepository.Object);

            // Act
            var result = codeScanner.GetUnusedStoredProcedures("dummyPath", "dummyPath", 1).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Second_Stored_Procedure", result.First());
        }

        [TestMethod]
        public void GetUnusedStoredProcedures_SingleFile_SingleThread_OcurranceNotFound()
        {
            // Arrange
            var storedProcedureRepository = new Mock<IStoredProceduresRepository>();
            var codeRepository = new Mock<ICodeRepository>();

            storedProcedureRepository.Setup(x => x.GetStoreProceduresNames(It.IsAny<string>()))
                .Returns(new List<string> { "First_Stored_Procedure", "Second_Stored_Procedure" });
            codeRepository.Setup(x => x.GetCodeFilesPaths(It.IsAny<string>()))
                .Returns(new List<string> { "FirstCodeFile" });
            codeRepository.Setup(x => x.SearchFile(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(new List<ScanResult>());

            var codeScanner =
                new CodeScanner(codeRepository.Object, storedProcedureRepository.Object);

            // Act
            var result = codeScanner.GetUnusedStoredProcedures("dummyPath", "dummyPath", 1).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains("First_Stored_Procedure"));
            Assert.IsTrue(result.Contains("Second_Stored_Procedure"));
        }

        [TestMethod]
        public void GetUnusedStoredProcedures_MultipleFile_MultipleThreads_PartialOcurrancesFound()
        {
            // Arrange
            var storedProcedureRepository = new Mock<IStoredProceduresRepository>();
            var codeRepository = new Mock<ICodeRepository>();

            storedProcedureRepository.Setup(x => x.GetStoreProceduresNames(It.IsAny<string>()))
                .Returns(new List<string> { "First_Stored_Procedure", "Second_Stored_Procedure" });
            codeRepository.Setup(x => x.GetCodeFilesPaths(It.IsAny<string>()))
                .Returns(new List<string> { "FirstCodeFile" });
            var scanResult = new ScanResult
            {
                Id = "1",
                Line = "Dummy line containing one of the search patterns",
                LineNumber = 5,
                SearchPattern = "First_Stored_Procedure"
            };

            codeRepository.Setup(x => x.SearchFile(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(new List<ScanResult> { scanResult });
            var codeScanner =
                new CodeScanner(codeRepository.Object, storedProcedureRepository.Object);

            // Act
            var result = codeScanner.GetUnusedStoredProcedures("dummyPath", "dummyPath", 4).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Second_Stored_Procedure", result.First());
        }

        [TestMethod]
        public void GetUnusedStoredProcedures_MultipleFiles_MultipleThreads_FilesAccessedOnce()
        {
            // Arrange
            var storedProcedureRepository = new Mock<IStoredProceduresRepository>();
            var codeRepository = new Mock<ICodeRepository>();

            storedProcedureRepository.Setup(x => x.GetStoreProceduresNames(It.IsAny<string>()))
                .Returns(new List<string> { "First_Stored_Procedure", "Second_Stored_Procedure" });
            codeRepository.Setup(x => x.GetCodeFilesPaths(It.IsAny<string>()))
                .Returns(new List<string> { "FirstCodeFile", "SecondCodeFile" });
            var scanResult1 = new ScanResult
            {
                Id = "1",
                Line = "Dummy line containing one of the search patterns",
                LineNumber = 5,
                SearchPattern = "First_Stored_Procedure"
            };

            var scanResult2 = new ScanResult
            {
                Id = "2",
                Line = "Dummy line containing one of the search patterns",
                LineNumber = 7,
                SearchPattern = "Second_Stored_Procedure"
            };

            codeRepository.Setup(x => x.SearchFile("FirstCodeFile", It.IsAny<IEnumerable<string>>()))
                .Returns(new List<ScanResult> { scanResult1 });
            codeRepository.Setup(x => x.SearchFile("SecondCodeFile", It.IsAny<IEnumerable<string>>()))
                .Returns(new List<ScanResult> { scanResult2 });

            var codeScanner =
                new CodeScanner(codeRepository.Object, storedProcedureRepository.Object);

            // Act
            var result = codeScanner.GetUnusedStoredProcedures("dummyPath", "dummyPath", 1).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
            codeRepository.Verify(x => x.SearchFile("FirstCodeFile", It.IsAny<IEnumerable<string>>()), Times.Once);
            codeRepository.Verify(x => x.SearchFile("SecondCodeFile", It.IsAny<IEnumerable<string>>()), Times.Once);
        }
    }
}
