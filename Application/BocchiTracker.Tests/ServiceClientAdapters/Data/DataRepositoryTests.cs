﻿using BocchiTracker.ProjectConfig;
using BocchiTracker.ServiceClientAdapters.Clients;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientAdapters;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.ServiceClientAdapters.Data
{
    public class DataRepositoryTests
    {
        private readonly Mock<IServiceClientAdapterFactory> _serviceClientAdapterFactoryMock;
        private readonly Mock<ICacheProvider> _cacheProviderMock;
        private readonly DataRepository _dataRepository;

        public DataRepositoryTests()
        {
            _serviceClientAdapterFactoryMock = new Mock<IServiceClientAdapterFactory>();
            _cacheProviderMock = new Mock<ICacheProvider>();
            _dataRepository = new DataRepository(_serviceClientAdapterFactoryMock.Object, _cacheProviderMock.Object);
        }

        [Fact]
        public async Task Test_GetLabels()
        {
            // Arrange
            var serviceType = ServiceDefinitions.Github;
            var expectedLabels = new List<IdentifierData> 
            { 
                new IdentifierData { Id = "1", Name = "Label1" }, 
                new IdentifierData { Id = "2", Name = "Label2" } 
            };

            _cacheProviderMock
                .Setup(x => x.IsExpired(It.IsAny<string>()))
                .Returns(true);
            _serviceClientAdapterFactoryMock
                .Setup(x => x.CreateServiceClientAdapter(serviceType))
                .Returns(Mock.Of<IServiceClientAdapter>(c => c.GetLabels() == Task.FromResult(expectedLabels)));

            // Act
            var result = await _dataRepository.GetLabels(serviceType);

            // Assert
            Assert.Equal(expectedLabels, result);
        }

        [Fact]
        public async Task Test_GetPriorities()
        {
            // Arrange
            var serviceType = ServiceDefinitions.Github;
            var expectedLabels = new List<IdentifierData>
            {
                new IdentifierData { Id = "1", Name = "Middle" },
                new IdentifierData { Id = "2", Name = "High" }, 
                new IdentifierData { Id = "3", Name = "Low" }
            };

            _cacheProviderMock
                .Setup(x => x.IsExpired(It.IsAny<string>()))
                .Returns(true);
            _serviceClientAdapterFactoryMock
                .Setup(x => x.CreateServiceClientAdapter(serviceType))
                .Returns(Mock.Of<IServiceClientAdapter>(c => c.GetPriorities() == Task.FromResult(expectedLabels)));

            // Act
            var result = await _dataRepository.GetPriorities(serviceType);

            // Assert
            Assert.Equal(expectedLabels, result);
        }

        [Fact]
        public async Task Test_GetTicketTypes()
        {
            // Arrange
            var serviceType = ServiceDefinitions.Github;
            var expectedLabels = new List<IdentifierData>
            {
                new IdentifierData { Id = "1", Name = "バグ" },
                new IdentifierData { Id = "2", Name = "タスク" }
            };

            _cacheProviderMock
                .Setup(x => x.IsExpired(It.IsAny<string>()))
                .Returns(true);
            _serviceClientAdapterFactoryMock
                .Setup(x => x.CreateServiceClientAdapter(serviceType))
                .Returns(Mock.Of<IServiceClientAdapter>(c => c.GetTicketTypes() == Task.FromResult(expectedLabels)));

            // Act
            var result = await _dataRepository.GetTicketTypes(serviceType);

            // Assert
            Assert.Equal(expectedLabels, result);
        }

        [Fact]
        public async Task Test_GetUsers()
        {
            // Arrange
            var serviceType = ServiceDefinitions.Github;
            var expectedLabels = new List<UserData>
            {
                new UserData { Id = "1", Name = "User1" },
                new UserData { Id = "2", Name = "User2" }
            };

            _cacheProviderMock
                .Setup(x => x.IsExpired(It.IsAny<string>()))
                .Returns(true);
            _serviceClientAdapterFactoryMock
                .Setup(x => x.CreateServiceClientAdapter(serviceType))
                .Returns(Mock.Of<IServiceClientAdapter>(c => c.GetUsers() == Task.FromResult(expectedLabels)));

            // Act
            var result = await _dataRepository.GetUsers(serviceType);

            // Assert
            Assert.Equal(expectedLabels, result);
        }
    }
}
