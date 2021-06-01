using System;
using System.Collections.Generic;
using Xunit;
using CommandAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using AutoMapper;
using CommandAPI.Profiles;
using CommandAPI.Data;
using CommandAPI.Models;
using CommandAPI.Dtos;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        Mock<ICommandAPIRepo> mockRepo;
        CommandsProfile realProfile;
        MapperConfiguration configuration;
        IMapper mapper;

        public CommandsControllerTests()
        {
            // initialized for each test
            mockRepo = new Mock<ICommandAPIRepo>();
            realProfile = new CommandsProfile();
            configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            mapper = new Mapper(configuration);
        }

        public void Dispose()
        {
            mockRepo = null;
            mapper = null;
            configuration = null;
            realProfile = null;
        }

        private List<Command> GetCommands(int num)
        {
            var commands = new List<Command>();
            if (num > 0)
            {
                commands.Add(new Command
                {
                    Id = 0,
                    HowTo = "How to generate a migration",
                    CommandLine = "dotnet ef migrations add <Name of Migration>",
                    Platform = ".NET Core EF"
                });
            }
            return commands;
        }

        [Fact]
        public void GetCommandItems_Returns200OK_WhenDBIsEmpty()
        {
            // Arrange
            // The Setup method is used to establish behavior
            // by passing in an expression that calls a specific method,
            // and adding the returned value of the expression.
            mockRepo.Setup(repo => repo.GetAllCommands())
                .Returns(GetCommands(0)); // with 0 as the parameter, returns empty collection

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetCommandItems_ReturnsOneItem_WhenDBHasOneResource()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();
            var okResult = result.Result as OkObjectResult;
            var commands = okResult.Value as List<CommandReadDto>;
            Assert.Single(commands);
        }

        [Fact]
        public void GetCommandItems_Returns200OK_WhenDBHasOneResource()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();
            
            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetCommandItems_ReturnsCorrectType_WhenDBHasOneResource()
        {
            // Valuable unit test since it verifies that the returned data is
            // is of the specific type.

            // Arrange
            mockRepo.Setup(repo => repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<CommandReadDto>>>(result);
        }

        [Fact]
        public void GetCommandByID_Returns404NotFound_WhenNonExistentIDProvided()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetCommandById(0);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        
        [Fact]
        public void GetCommandByID_Returns200OK_WhenValidIDProvided()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command {
                Id = 1,
                HowTo = "Mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetCommandById(1);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetCommandByID_ReturnsCorrectType_WhenValidIDProvided()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command {
                Id = 1,
                HowTo = "Mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetCommandById(1);

            // Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void CreateCommand_ReturnsCorrectResourceType_WhenValidObjectSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command {
                Id = 1,
                HowTo = "Mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.CreateCommand(new CommandCreateDto {  });

            // Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void CreateCommand_Returns201Created_WhenValidObjectSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command {
                Id = 1,
                HowTo = "Mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.CreateCommand(new CommandCreateDto {  });

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }

        
        [Fact]
        public void UpdateCommand_Returns204NoContent_WhenValidObjectSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command {
                Id = 1,
                HowTo = "Mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.UpdateCommand(1, new CommandUpdateDto {  });

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

                
        [Fact]
        public void UpdateCommand_Returns404NotFound_WhenNonExistentResourceIDSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.UpdateCommand(0, new CommandUpdateDto {  });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void PartialCommandUpdate_Returns404NotFound_WhenNonExistentResourceIDSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.PartialCommandUpdate(0, new Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<CommandUpdateDto> { });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteCommand_Returns204NoContent_WhenValidResourceIDSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command {
                Id = 1,
                HowTo = "Mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.DeleteCommand(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteCommand_Returns404NotFound_WhenNonExistingResourceIDSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.DeleteCommand(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}