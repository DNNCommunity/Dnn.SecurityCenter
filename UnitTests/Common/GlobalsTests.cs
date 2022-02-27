using Dnn.Modules.SecurityCenter.Common;
using System;
using Xunit;

namespace UnitTests.Common
{
    public class GlobalsTests
    {
        [Fact]
        public void ModulePrefixIsValid()
        {
            var modulePrefix = Globals.ModulePrefix;
            Assert.Equal(expected: "DNN_SecurityCenter_", modulePrefix);
        }
    }
}
