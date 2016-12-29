using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PC.Common;
using PC.DataAccess;
using PC.Scanner;

namespace PC.Tests
{
    [TestClass]
    public class CodeScannerTests
    {
        [TestMethod]
        public void ScanCode_SingleFile_SingleOccurrence_SingleThread_OccurrenceFound()
        {
            // Arrange
            var storedProcedureRepository = new Mock<IStoredProceduresRepository>();
            var codeRepository = new Mock<ICodeRepository>();

            storedProcedureRepository.Setup(x => x.GetStoreProceduresNames(It.IsAny<string>()))
                .Returns(new List<string> {"First_Stored_Procedure", "Second_Stored_Procedure" });
            codeRepository.Setup(x => x.GetCodeFilesPaths(It.IsAny<string>()))
                .Returns(new List<string> {"FirstCodeFile"});
            var scanResult = new ScanResult
            {
                Id = "1",
                Line = "Dummy line containing one of the search patterns",
                LineNumber = 5,
                SearchPattern = "First_Stored_Procedure"
            };

            codeRepository.Setup(x => x.SearchFile(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(new List<ScanResult> {scanResult});
            var resultQueue = new ConcurrentQueue<ScanResult>();
            var codeScanner =
                new CodeScanner(codeRepository.Object, storedProcedureRepository.Object);

            // Act
            codeScanner.ScanCode("dummyPath", "dummyPath", resultQueue);

            // Assert
            Assert.AreEqual(1, resultQueue.Count);
        }

        [TestMethod]
        public void ScanCode_SingleFile_NoOccurrence_SingleThread_NoOccurrenceFound()
        {
        }

        [TestMethod]
        public void ScanCode_SingleFile_MultipleOccurrence_MultipleThreads_MultipleOccurrenceFound()
        {
        }

        [TestMethod]
        public void ScanCode_MultipleFiles_MultipleThreads_EachFileCheckedOnce()
        {
        }

        [TestMethod]
        public void ScanCode_FileContainsFileNamePattern_NoOccurrenceFound()
        {
        }
    }
}
