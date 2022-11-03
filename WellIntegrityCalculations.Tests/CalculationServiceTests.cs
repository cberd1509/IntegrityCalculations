using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellIntegrityCalculations.Controllers;
using WellIntegrityCalculations.Services;

namespace WellIntegrityCalculations.Tests
{
    public class CalculationServiceTests
    {
        private readonly IntegrityCalculationsController _controller;
        private readonly ICalculationService _calculationService;

        public CalculationServiceTests()
        {
            _calculationService = Substitute.For<ICalculationService>();
        }

    }
}
