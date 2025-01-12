using Microsoft.Extensions.Logging;
using Moq;
using PhoBloberWebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoBloberWebAPIxUnitTest
{
    public class WeatherForecastControllerTests
    {
        private readonly Mock<ILogger<WeatherForecastController>> _loggerMock;
        private readonly WeatherForecastController _controller;

        public WeatherForecastControllerTests()
        {
            _loggerMock = new Mock<ILogger<WeatherForecastController>>();
            _controller = new WeatherForecastController(_loggerMock.Object);
        }

        [Fact]
        public void Get_ShouldReturnWeatherForecasts()
        {
            // Act
            var results = _controller.Get().ToList();

            // Assert
            Assert.Equal(5, results.Count);
            Assert.All(results, item =>
            {
                Assert.InRange(item.TemperatureC, -20, 55);
                Assert.Contains(item.Summary, WeatherForecastController.Summaries);
            });

            // Since this controller doesn't log in its Get method, we don't have to verify logger here.
            // But if there were logging, you could use:
            //_loggerMock.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Once);
        }

    }
}
