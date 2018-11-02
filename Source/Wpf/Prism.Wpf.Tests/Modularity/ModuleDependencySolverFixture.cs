

using System;
using System.Collections.Generic;
using Xunit;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    
    public class ModuleDependencySolverFixture
    {
        private ModuleDependencySolver solver;

        public ModuleDependencySolverFixture()
        {
            solver = new ModuleDependencySolver();
        }

        [Fact]
        public void ModuleDependencySolverIsAvailable()
        {
            Assert.NotNull(solver);
        }

        [Fact]
        public void CanAddModuleName()
        {
            solver.AddModule("ModuleA");
            Assert.Equal(1, solver.ModuleCount);
        }

        [Fact]
        public void CannotAddNullModuleName()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                solver.AddModule(null);
            });
            
        }

        [Fact]
        public void CannotAddEmptyModuleName()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                solver.AddModule(String.Empty);
            });
            
        }

        [Fact]
        public void CannotAddDependencyWithoutAddingModule()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                solver.AddDependency("ModuleA", "ModuleB");
            });
            
        }

        [Fact]
        public void CanAddModuleDepedency()
        {
            solver.AddModule("ModuleA");
            solver.AddModule("ModuleB");
            solver.AddDependency("ModuleB", "ModuleA");
            Assert.Equal(2, solver.ModuleCount);
        }

        [Fact]
        public void CanSolveAcyclicDependencies()
        {
            solver.AddModule("ModuleA");
            solver.AddModule("ModuleB");
            solver.AddDependency("ModuleB", "ModuleA");
            string[] result = solver.Solve();
            Assert.Equal(2, result.Length);
            Assert.Equal("ModuleA", result[0]);
            Assert.Equal("ModuleB", result[1]);
        }

        [Fact]
        public void FailsWithSimpleCycle()
        {
            var ex = Assert.Throws<CyclicDependencyFoundException>(() =>
            {
                solver.AddModule("ModuleB");
                solver.AddDependency("ModuleB", "ModuleB");
                string[] result = solver.Solve();
            });

        }

        [Fact]
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
            Assert.Equal(6, result.Length);
            List<string> test = new List<string>(result);
            Assert.True(test.IndexOf("ModuleA") < test.IndexOf("ModuleB"));
            Assert.True(test.IndexOf("ModuleB") < test.IndexOf("ModuleC"));
            Assert.True(test.IndexOf("ModuleD") < test.IndexOf("ModuleE"));
        }

        [Fact]
        public void FailsWithComplexCycle()
        {
            var ex = Assert.Throws<CyclicDependencyFoundException>(() =>
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
            });

        }

        [Fact]
        public void FailsWithMissingModule()
        {
            var ex = Assert.Throws<ModularityException>(() =>
            {
                solver.AddModule("ModuleA");
                solver.AddDependency("ModuleA", "ModuleB");
                solver.Solve();
            });

        }
    }
}