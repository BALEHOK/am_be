using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Tasks.Runners;
using AppFramework.Core.UnitTests.Fixtures;
using AppFramework.DataProxy;
using AppFramework.Tasks;
using AppFramework.UnitTests.Common.Fixtures;
using Xunit;
using Xunit.Extensions;

namespace AppFramework.Core.UnitTests
{
    public class TaskRunnerFactoryTests
    {
        [Theory, AutoDomainData]
        public void TaskRunnerFactory_ReturnsCorrectInstance(
             long userId,
             TaskRunnerFactory sut)
        {
            // Arrange
             var taskStub = new Entities.Task
             {
                 FunctionType = (int)Tasks.Enumerations.TaskFunctionType.ExecuteSqlServerAgentJob
             };
             // Act
             var result = sut.GetRunner(taskStub, userId, null);
             // Assert
             Assert.NotNull(result);
             Assert.IsAssignableFrom<ITaskRunner>(result);
        }
    }
}
