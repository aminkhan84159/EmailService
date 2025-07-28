using MeshComm.Core.ServiceFramework.Enums;
using MeshComm.Core.ServiceFramework.Messages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Transactions;
using Serilog;

namespace MeshComm.Core.ServiceFramework.Handlers
{
    public class HandlerBase<T, U>
        where T : RequestBase
        where U : ResponseBase, new()
    {
        private readonly ILogger _logger;
        private readonly DbContext _dbContext;

        public T Request { get; private set; } = null!;
        public U Response { get; private set; } = null!;
        public bool RethrowExceptions { get; private set; }

        public HandlerBase(
            ILogger logger,
            DbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public U Handle(T request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Request = request;
            Response = new U();

            try
            {
                var transactionOptions = new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.Snapshot
                };

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Logic to be implemented by child classes occurs here.
                    var success = HandleCore();

                    if (success)
                    {
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Exception = ex.ToString();
                _logger.Error($"Error in Handle method in HandlerBase. Message: {ex.Message} --- Inner exception: {ex.InnerException}");

                if (RethrowExceptions)
                {
                    throw;
                }
            }
            finally
            {
                stopwatch.Stop();
                Response.ExecutionTimeSeconds = stopwatch.Elapsed.TotalSeconds;

                LogRequest();
            }

            return Response;
        }

        /// </summary>
        /// <param name="request">The Request object.</param>
        /// <returns>The Response object.</returns>
        /// <exception cref="ArgumentNullException" />
        public async Task<U> HandleAsync(T request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Request = request;
            Response = new U();

            try
            {
                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    var transactionOptions = new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.Snapshot
                    };

                    using (var transaction = new TransactionScope(
                        TransactionScopeOption.Required,
                        transactionOptions,
                        TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var success = await HandleCoreAsync();

                        if (success)
                        {
                            transaction.Complete();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Response.Exception = ex.ToString();
                _logger.Error($"Error in HandleAsync method in HandlerBase. Message: {ex.Message} --- Inner exception: {ex.InnerException}");

                if (RethrowExceptions)
                {
                    throw;
                }
            }
            finally
            {
                stopwatch.Stop();
                Response.ExecutionTimeSeconds = stopwatch.Elapsed.TotalSeconds;

                LogRequest();
            }
            return Response;
        }

        private void LogRequest()
        {
            var request = JsonSerializer.Serialize(Request);
            var response = JsonSerializer.Serialize(Response);

            // Use Serilog to log request information to FlexClaimRequestLog
            _logger.Information("{Request}{Response}{ResponseStatus}{ResponseExecutionTimeSeconds}{Exception}{IsActive}{CreatedBy}{CreatedOn}",
                request, response, Response.ResponseStatusType.ToString(), Convert.ToInt32(Response.ExecutionTimeSeconds), Response.Exception, true, "System", DateTime.UtcNow);
        }

        protected bool Conflict(string? message = null)
        {
            _logger.Error($"Handler Error - Conflict. message: {message}");

            return SetStatus(false, message, ResponseStatusTypeEnum.Conflict);
        }

        protected bool Forbidden(string? message = null)
        {
            _logger.Error($"Handler Error - Forbidden. message: {message}");

            return SetStatus(false, message, ResponseStatusTypeEnum.Forbidden);
        }

        protected bool BadRequest(string? message = null)
        {
            _logger.Error($"Handler Error - BadRequest. message: {message}");

            return SetStatus(false, message, ResponseStatusTypeEnum.BadRequest);
        }

        protected bool BadRequest(List<string> validationErrors)
        {
            _logger.Error($"Handler Error - BadRequest. message: {validationErrors}");

            return SetStatus(false, ResponseStatusTypeEnum.BadRequest, validationErrors);
        }

        protected bool InternalServerError(string? message = null)
        {
            _logger.Error($"Handler Error - InternalServerError. message: {message}");

            return SetStatus(false, message, ResponseStatusTypeEnum.InternalServerError);
        }

        protected bool NotFound(string? message = null)
        {
            _logger.Error($"Handler Error - NotFound. message: {message}");

            return SetStatus(false, message, ResponseStatusTypeEnum.NotFound);
        }

        protected bool Created(string? message = null)
        {
            return SetStatus(true, message, ResponseStatusTypeEnum.Created);
        }

        protected bool Success(string? message = null)
        {
            return SetStatus(true, message, ResponseStatusTypeEnum.Ok);
        }

        private bool SetStatus(bool isSuccess, string? message, ResponseStatusTypeEnum responseStatusType)
        {
            Response.IsSuccess = isSuccess;
            Response.Message = message;
            Response.ResponseStatusType = responseStatusType;

            return Response.IsSuccess;
        }

        private bool SetStatus(bool isSuccess, ResponseStatusTypeEnum responseStatusType, List<string> validationErrors)
        {
            SetStatus(isSuccess, "Validation Errors", responseStatusType);
            Response.ValidationErrors = validationErrors;

            return Response.IsSuccess;
        }

        /// <summary>
        /// Core message handling functionality to be overrridden by child class implementations.
        /// </summary>
        protected virtual bool HandleCore()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Async core message handling functionality to be overrridden by child class implementations.
        /// </summary>
        protected virtual Task<bool> HandleCoreAsync()
        {
            throw new NotImplementedException();
        }
    }
}
