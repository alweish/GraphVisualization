﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using Common;
using log4net;
using GraphDataService.Query.Contract;

namespace GraphBusinessService.ShortestPath.Tests
{
    [TestClass]
    public class GraphBusinessServiceShortestPathTests
    {
        Mock<ILoggerFactory> loggerFactory;
        Mock<ILog> log;
        Mock<IGraphDataServiceQueryClientFactory> clientFactory;
        Mock<IGraphDataServiceQueryClient> client;

        [TestInitialize]
        public void SetUpMocks()
        {
            log = new Mock<ILog>();
            loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(f => f.GetLogger(It.IsAny<Type>())).Returns(log.Object);
            client = new Mock<IGraphDataServiceQueryClient>();
            clientFactory = new Mock<IGraphDataServiceQueryClientFactory>();
            clientFactory.Setup(f => f.CreateClient()).Returns(client.Object);
        }

        [TestMethod]
        public void WhenQueryingForShortestPathBetweenTheSameVertexReturnAnEmptyResult()
        {
            client.Setup(c => c.GetEdges()).Returns(new List<Edge>
            {
                new Edge { Vertex1 = 1, Vertex2 = 2 },
                new Edge { Vertex1 = 1, Vertex2 = 1 },
                new Edge { Vertex1 = 2, Vertex2 = 1 }
            });

            var sut = new GraphBusinessServiceShortestPath(loggerFactory.Object, clientFactory.Object);
            var path  = sut.GetShortestPath(1, 1);

            Assert.AreEqual(0, path.Count);
        }

        [TestMethod]
        public void WhenQueryingForShortestPathBetweenTwoEndsOfAPathReturnThePath()
        {
            client.Setup(c => c.GetEdges()).Returns(new List<Edge>
            {
                new Edge { Vertex1 = 1, Vertex2 = 2 },
                new Edge { Vertex1 = 2, Vertex2 = 3 },
                new Edge { Vertex1 = 3, Vertex2 = 4 }
            });

            var sut = new GraphBusinessServiceShortestPath(loggerFactory.Object, clientFactory.Object);
            var path = sut.GetShortestPath(1, 4);

            Assert.AreEqual(3, path.Count);
        }

        [TestMethod]
        public void WhenQueryingForShortestPathBetweenTwoOfThreeInterconnectedVerticesReturnTheEdgeBetweenThem()
        {
            client.Setup(c => c.GetEdges()).Returns(new List<Edge>
            {
                new Edge { Vertex1 = 1, Vertex2 = 2 },
                new Edge { Vertex1 = 2, Vertex2 = 1 },
                new Edge { Vertex1 = 2, Vertex2 = 3 },
                new Edge { Vertex1 = 3, Vertex2 = 2 },
                new Edge { Vertex1 = 3, Vertex2 = 1 },
                new Edge { Vertex1 = 1, Vertex2 = 3 }
            });

            var sut = new GraphBusinessServiceShortestPath(loggerFactory.Object, clientFactory.Object);
            var path = sut.GetShortestPath(1, 3);

            Assert.AreEqual(1, path.Count);
            Assert.AreEqual(1, path[0].Vertex1);
            Assert.AreEqual(3, path[0].Vertex2);
        }
    }
}