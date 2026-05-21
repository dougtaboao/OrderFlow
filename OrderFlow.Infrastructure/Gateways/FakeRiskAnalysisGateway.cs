using Microsoft.Extensions.Logging;
using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace OrderFlow.Infrastructure.Gateways
{
    public class FakeRiskAnalysisGateway : IRiskAnalysisGateway
    {
        private readonly ILogger<FakeRiskAnalysisGateway> _logger;
        private readonly AsyncRetryPolicy<RiskAnalysisResult> _retryPolicy;
        private readonly AsyncTimeoutPolicy<RiskAnalysisResult> _timeoutPolicy;
        private readonly AsyncCircuitBreakerPolicy<RiskAnalysisResult> _circuitBreakerPolicy;

        public FakeRiskAnalysisGateway(ILogger<FakeRiskAnalysisGateway> logger)
        {
            _logger = logger;

            _timeoutPolicy = Policy.TimeoutAsync<RiskAnalysisResult>(
                TimeSpan.FromSeconds(3));

            _retryPolicy = Policy<RiskAnalysisResult>
                .Handle<Exception>()
                .OrResult(result => !result.Approved && result.Reason == "Transient risk error.")
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.LogWarning(
                            "Retry risk analysis. Attempt {RetryAttempt}, Delay {DelaySeconds}s",
                            retryAttempt,
                            timespan.TotalSeconds);
                    });

            _circuitBreakerPolicy = Policy<RiskAnalysisResult>
                .Handle<Exception>()
                .OrResult(result => !result.Approved && result.Reason == "Transient risk error.")
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, breakDelay) =>
                    {
                        _logger.LogError(
                            outcome.Exception,
                            "Circuit breaker opened for risk analysis. Duration {DurationSeconds}s",
                            breakDelay.TotalSeconds);
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit breaker reset for risk analysis.");
                    },
                    onHalfOpen: () =>
                    {
                        _logger.LogInformation("Circuit breaker half-open for risk analysis.");
                    });
        }

        public async Task<RiskAnalysisResult> AnalyzeAsync(
            Order order,
            CancellationToken cancellationToken = default)
        {
            var policy = Policy.WrapAsync(
                _circuitBreakerPolicy,
                _retryPolicy,
                _timeoutPolicy);

            return await policy.ExecuteAsync(async ct =>
            {
                _logger.LogInformation(
                    "Calling fake risk analysis gateway. OrderId {OrderId}, Amount {Amount}",
                    order.Id,
                    order.Amount);

                return await SimulateRiskAnalysisAsync(order, ct);
            }, cancellationToken);
        }

        private static async Task<RiskAnalysisResult> SimulateRiskAnalysisAsync(
            Order order,
            CancellationToken cancellationToken)
        {
            if (order.ExternalReference.Contains("TIMEOUT", StringComparison.OrdinalIgnoreCase))
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }

            if (order.ExternalReference.Contains("TRANSIENT", StringComparison.OrdinalIgnoreCase))
            {
                throw new TimeoutException("Transient risk gateway failure.");
            }

            if (order.Amount > 10000)
            {
                return RiskAnalysisResult.Reject("Order amount exceeds risk limit.");
            }

            await Task.Delay(300, cancellationToken);

            return RiskAnalysisResult.Approve();
        }
    }
}