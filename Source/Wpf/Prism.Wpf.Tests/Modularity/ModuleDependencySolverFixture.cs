

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    [TestClass]
    public class ModuleDependencySolverFixture
    {
        private ModuleDependencySolver solver;

        [TestInitialize]
        public void Setup()
        {
            solver = new ModuleDependencySolver();
        }

        [TestMethod]
        public void ModuleDependencySolverIsAvailable()
        {
            Assert.IsNotNull(solver);
        }

        [TestMethod]
        public void CanAddModuleName()
        {
            solver.AddModule("ModuleA");
            Assert.AreEqual(1, solver.ModuleCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotAddNullModuleName()
        {
            solver.AddModule(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotAddEmptyModuleName()
        {
            solver.AddModule(String.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotAddDependencyWithoutAddingModule()
        {
            solver.AddDependency("ModuleA", "ModuleB");
        }

        [TestMethod]
        public void CanAddModuleDepedency()
        {
            solver.AddModule("ModuleA");
            solver.AddModule("ModuleB");
            solver.AddDependency("ModuleB", "ModuleA");
            Assert.AreEqual(2, solver.ModuleCount);
        }

        [TestMethod]
        public void CanSolveAcyclicDependencies()
        {
            solver.AddModule("ModuleA");
            solver.AddModule("ModuleB");
            solver.AddDependency("ModuleB", "ModuleA");
            string[] result = solver.Solve();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("ModuleA", result[0]);
            Assert.AreEqual("ModuleB", result[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(CyclicDependencyFoundException))]
        public void FailsWithSimpleCycle()
        {
            solver.AddModule("ModuleB");
            solver.AddDependency("ModuleB", "ModuleB");
            string[] result = solver.Solve();
        }

        [TestMethod]
        public void CanSolveForest()
        {
            solver.AddModule("ModuleA");
            solver.AddModule("ModuleB");
            solver.AddModule("ModuleC");
            solver.AddModule("ModuleD");
            solver.AddModule("ModuleE");
            solver.AddModule("ModuleF");
            solver.AddDependency("ModuleC", "ModuleB");
            solver.AddDependency("ModuleB", "ModuleA");
            solver.AddDependency("ModuleE", "ModuleD");
            string[] result = solver.Solve();
            Assert.AreEqual(6, result.Length);
            List<string> test = new List<string>(result);
            Assert.IsTrue(test.IndexOf("ModuleA") < test.IndexOf("ModuleB"));
            Assert.IsTrue(test.IndexOf("ModuleB") < test.IndexOf("ModuleC"));
            Assert.IsTrue(test.IndexOf("ModuleD") < test.IndexOf("ModuleE"));
        }

        [TestMethod]
        [ExpectedException(typeof(CyclicDependencyFoundException))]
        public void FailsWithComplexCycle()
        {
            solver.AddModule("ModuleA");
            solver.AddModule("ModuleB");
            solver.AddModule("ModuleC");
            solver.AddModule("ModuleD");
            solver.AddModule("ModuleE");
            solver.AddModule("ModuleF");
            solver.AddDependency("ModuleC", "ModuleB");
            solver.AddDependency("ModuleB", "ModuleA");
            solver.AddDependency("ModuleE", "ModuleD");
            solver.AddDependency("ModuleE", "ModuleC");
            solver.AddDependency("ModuleF", "ModuleE");
            solver.AddDependency("ModuleD", "ModuleF");
            solver.AddDependency("ModuleB", "ModuleD");
            solver.Solve();
        }

        [TestMethod]
        [ExpectedException(typeof(ModularityException))]
        public void FailsWithMissingModule()
        {
            solver.AddModule("ModuleA");
            solver.AddDependency("ModuleA", "ModuleB");
            solver.Solve();
        }
    }
}